using ArctisVoiceMeeter.Model;

namespace ArctisVoiceMeeter.Infrastructure;

public static class Extensions
{
    public static uint GetArctisVolume(this ArctisStatus status, ArctisChannel channel)
    {
        return channel == ArctisChannel.Chat ? status.ChatVolume : status.GameVolume;
    }
}