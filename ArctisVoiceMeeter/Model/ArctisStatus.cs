using CommunityToolkit.Mvvm.ComponentModel;

namespace ArctisVoiceMeeter.Model;

public partial class ArctisStatus : ObservableObject
{
    [ObservableProperty] private uint _battery;
    [ObservableProperty] private uint _chatVolume;
    [ObservableProperty] private uint _gameVolume;
    [ObservableProperty] private string _headsetName;

    public ArctisStatus(uint battery, uint chatVolume, uint gameVolume, string headsetName)
    {
        Battery = battery;
        ChatVolume = chatVolume;
        GameVolume = gameVolume;
        HeadsetName = headsetName;
    }

    public void CopyFrom(ArctisStatus status)
    {
        Battery = status.Battery;
        ChatVolume = status.ChatVolume;
        GameVolume = status.GameVolume;
        HeadsetName = status.HeadsetName;
    }
}