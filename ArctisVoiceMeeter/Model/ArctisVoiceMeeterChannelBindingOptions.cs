using System.Text.Json.Serialization;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ArctisVoiceMeeter.Model;

public partial class ArctisVoiceMeeterChannelBindingOptions : ObservableObject
{
    [ObservableProperty] private string _bindingName;
    [ObservableProperty] private float _voiceMeeterMinVolume;
    [ObservableProperty] private float _voiceMeeterMaxVolume;
    [ObservableProperty] private uint _boundStrip;
    
    public void CopyFrom(ArctisVoiceMeeterChannelBindingOptions options)
    {
        BindingName = options.BindingName;
        VoiceMeeterMinVolume = options.VoiceMeeterMinVolume;
        VoiceMeeterMaxVolume = options.VoiceMeeterMaxVolume;
        BoundStrip = options.BoundStrip;
    }
}