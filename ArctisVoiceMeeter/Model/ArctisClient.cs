using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ArctisVoiceMeeter.Infrastructure;
using HidSharp;

namespace ArctisVoiceMeeter.Model;

public record ArctisStatus(uint Battery, uint ChatVolume, uint GameVolume);

public class ArctisClient
{
    private const int VendorSteelSeries = 0x1038;
    private const int IdArctis9 = 0x12c2;
    private const byte BatteryMax = 0x9A;
    private const byte BatteryMin = 0x64;

    private const byte ChannelMinVolume = 0x13;
    private const byte ChannelMaxVolume = 0x0;

    private const uint BatteryByteIndex = 4;
    private const uint ChatVolumeByteIndex = 10;
    private const uint GameVolumeByteIndex = 11;

    private HidDevice? _lastSuccessfulDevice;

    public ArctisStatus GetStatus()
    {
        var data = GetRawStatus();
        var parsed = ParseRawStatus(data);
        return parsed;
    }

    private ArctisStatus ParseRawStatus(byte[] data)
    {
        var battery = MathHelper.Scale(data[BatteryByteIndex], BatteryMin, BatteryMax);
        battery = Math.Min(battery, (byte)100);
        var chatVolume = MathHelper.Scale(data[ChatVolumeByteIndex], ChannelMinVolume, ChannelMaxVolume);
        var gameVolume = MathHelper.Scale(data[GameVolumeByteIndex], ChannelMinVolume, ChannelMaxVolume);

        return new(battery, chatVolume, gameVolume);
    }

    private byte[] GetRawStatus(int vid = VendorSteelSeries, int pid = IdArctis9)
    {
        IEnumerable<HidDevice>? headsets = DeviceList.Local.GetHidDevices(vid, pid);
        var status = ReadHeadsetStatus(headsets);
        return status;
    }

    private byte[] ReadHeadsetStatus(IEnumerable<HidDevice> headsets)
    {
        if (_lastSuccessfulDevice != null &&
            TryReadHeadsetStatus(_lastSuccessfulDevice, out var bytes))
            return bytes;

        foreach (var headset in headsets)
        {
            if (TryReadHeadsetStatus(headset, out bytes))
                return bytes;
        }

        throw new InvalidOperationException("SteelSeries Arctis 9 Wireless headset not found.");
    }

    private bool TryReadHeadsetStatus(HidDevice headset, out byte[] bytes)
    {
        bytes = Array.Empty<byte>();
        try
        {
            var data = ReadHeadsetStatus(headset);
            if (data.Any())
            {
                _lastSuccessfulDevice = headset;
                bytes = data;
                return true;
            }
        }
        catch
        {
            //Ignore exception
        }
        return false;
    }

    private static byte[] ReadHeadsetStatus(HidDevice headset)
    {
        var stream = headset.Open();
        stream.Write(new byte[] { 0x0, 0x20 });
        var response = new byte[12];
        stream.Read(response);
        return response;
    }
}