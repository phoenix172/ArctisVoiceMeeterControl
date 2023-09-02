using System.Collections.Generic;

namespace ArctisVoiceMeeter;

public class ArctisClient
{
    private const int VENDOR_STEELSERIES = 0x1038;
    private const int ID_ARCTIS_9 = 0x12c2; // 0x12c4;
    private const byte BATTERY_MAX = 0x9A;
    private const byte BATTERY_MIN = 0x64;

    private const byte CHANNEL_MIN_VOLUME = 0x13;
    private const byte CHANNEL_MAX_VOLUME = 0x0;

    public static float Scale(float val, float inMin, float inMax, float outMin = 0, float outMax = 100)
        => (val - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;

    public static byte Scale(byte val, byte inMin, byte inMax, byte outMin = 0, byte outMax = 100)
         => (byte)((val - inMin) * (outMax - outMin) / (double)(inMax - inMin) + outMin);

    public static (int Battery, int ChatVolume, int GameVolume) ParseData(byte[] data)
    {
        byte Battery(int x)
        {
            return Scale(data[x], BATTERY_MIN, BATTERY_MAX, 0, 100);
        }

        var kur = string.Join(" ", data);
        //Console.WriteLine(kur);

        var battery = Battery(4);//miswim si
        var chatVolume = Scale(data[10], CHANNEL_MIN_VOLUME, CHANNEL_MAX_VOLUME);
        var gameVolume = Scale(data[11], CHANNEL_MIN_VOLUME, CHANNEL_MAX_VOLUME);

        return (battery, chatVolume, gameVolume);
    }

    public static byte[] GetRawData(int vid = VENDOR_STEELSERIES, int pid = ID_ARCTIS_9)
    {
        IEnumerable<HidDevice>? headsets = DeviceList.Local.GetHidDevices(vid, pid);
        var headset = DeviceList.Local.GetHidDeviceOrNull(vid, pid);
        var returned = GetDataForHeadset(headsets);

        return returned.FirstOrDefault();
    }

    private static IEnumerable<byte[]> GetDataForHeadset(IEnumerable<HidDevice> headsets)
    {
        foreach (var headset in headsets)
        {
            byte[]? data = null;
            try
            {
                data = GetDataForHeadset(headset);
            }
            catch
            {
            }

            if (data != null)
                yield return data;
        }
    }

    private static byte[] GetDataForHeadset(HidDevice headset)
    {
        var stream = headset.Open();
        stream.Write(new byte[] { 0x0, 0x20 });
        var response = new byte[12];
        stream.Read(response);
        return response;
    }
}