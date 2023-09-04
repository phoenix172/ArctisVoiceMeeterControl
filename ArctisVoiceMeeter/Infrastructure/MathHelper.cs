namespace ArctisVoiceMeeter.Infrastructure;

public class MathHelper
{
    public static float Scale(float val, float inMin, float inMax, float outMin = 0, float outMax = 100)
        => (val - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;

    public static byte Scale(byte val, byte inMin, byte inMax, byte outMin = 0, byte outMax = 100)
        => (byte)((val - inMin) * (outMax - outMin) / (double)(inMax - inMin) + outMin);
}