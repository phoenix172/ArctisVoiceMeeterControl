using CommunityToolkit.Mvvm.ComponentModel;

namespace ArctisVoiceMeeter.Model;

public partial class ArctisVoiceMeeterChannelBindingOptions : ObservableObject
{
    [ObservableProperty] private string _bindingName;
    [ObservableProperty] private float _voiceMeeterMinVolume;
    [ObservableProperty] private float _voiceMeeterMaxVolume;
    [ObservableProperty] private uint _boundStrip;
    [ObservableProperty] private float _voiceMeeterVolume;
}