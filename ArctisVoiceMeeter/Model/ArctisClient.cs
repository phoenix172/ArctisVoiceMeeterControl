using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ArctisVoiceMeeter.Infrastructure;
using HidSharp;

namespace ArctisVoiceMeeter.Model;

public class ArctisClient
{
    private record ArctisRawStatus(HidDevice Device, byte[] Data);

    private const int VendorSteelSeries = 0x1038;
    private const int IdArctis9 = 0x12c2;
    private const byte BatteryMax = 0x9A;
    private const byte BatteryMin = 0x64;

    private const byte ChannelMinVolume = 0x13;
    private const byte ChannelMaxVolume = 0x0;

    private const uint BatteryByteIndex = 4;
    private const uint ChatVolumeByteIndex = 10;
    private const uint GameVolumeByteIndex = 11;

    private HidDevice[] _foundDevices = {};

    public IEnumerable<ArctisStatus> GetStatus()
    {
        var data = GetRawStatus();
        var parsed = data.Select(ParseRawStatus);
        return parsed;
    }

    private ArctisStatus ParseRawStatus(ArctisRawStatus rawStatus)
    {
        var battery = MathHelper.Scale(rawStatus.Data[BatteryByteIndex], BatteryMin, BatteryMax);
        battery = Math.Min(battery, (byte)100);
        var chatVolume = MathHelper.Scale(rawStatus.Data[ChatVolumeByteIndex], ChannelMinVolume, ChannelMaxVolume);
        var gameVolume = MathHelper.Scale(rawStatus.Data[GameVolumeByteIndex], ChannelMinVolume, ChannelMaxVolume);

        var headsetName = rawStatus.Device.GetFriendlyName(); //TODO: Replace with a name that is actually different between same headsets

        return new(battery, chatVolume, gameVolume, headsetName);
    }

    private IEnumerable<ArctisRawStatus> GetRawStatus(int vid = VendorSteelSeries, int pid = IdArctis9)
    {
        var status = ReadHeadsetStatus(_foundDevices).ToList();

        if (status.Count == 0 || status.Count != _foundDevices.Length)
        {
            IEnumerable<HidDevice>? headsets = DeviceList.Local.GetHidDevices(vid, pid);
            status = ReadHeadsetStatus(headsets).ToList();
        }

        if(!status.Any())
            throw new InvalidOperationException("SteelSeries Arctis 9 Wireless headset not found.");

        _foundDevices = status.Select(x => x.Device).ToArray();

        return status;
    }

    private IEnumerable<ArctisRawStatus> ReadHeadsetStatus(IEnumerable<HidDevice> headsets)
    {
        foreach (var headset in headsets)
        {
            if (TryReadHeadsetStatus(headset, out var bytes))
                yield return new(headset, bytes);
        }
    }

    private bool TryReadHeadsetStatus(HidDevice headset, out byte[] bytes)
    {
        bytes = Array.Empty<byte>();
        try
        {
            var data = ReadHeadsetStatus(headset);
            if (data.Any())
            {
                bytes = data;
                return true;
            }
        }
        catch(Exception ex)
        {
            Debug.WriteLine(ex);
            //Ignore exception
        }
        return false;
    }

    private static byte[] ReadHeadsetStatus(HidDevice headset)
    {
        var stream = headset.Open();
        stream.ReadTimeout = 200;
        stream.WriteTimeout = 200;

        stream.Write(new byte[] { 0x0, 0x20 });
        var response = new byte[12];
        stream.Read(response);
        return response;
    }
}