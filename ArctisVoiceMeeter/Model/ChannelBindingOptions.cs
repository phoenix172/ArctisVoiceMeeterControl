using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ArctisVoiceMeeter.Model;

public partial class ChannelBindingOptions : ObservableObject
{
    [ObservableProperty] private string _bindingName;
    [ObservableProperty] private float _voiceMeeterMinVolume;
    [ObservableProperty] private float _voiceMeeterMaxVolume;
    [ObservableProperty] private uint _boundStrip;
    [ObservableProperty] private List<HeadsetChannelBinding> _boundHeadsets = new();

    [JsonConstructor]
    private ChannelBindingOptions()
    {
    }

    public ChannelBindingOptions(string bindingName)
    {
        _bindingName = bindingName;
    }

    public void CopyFrom(ChannelBindingOptions options)
    {
        BindingName = options.BindingName;
        VoiceMeeterMinVolume = options.VoiceMeeterMinVolume;
        VoiceMeeterMaxVolume = options.VoiceMeeterMaxVolume;
        BoundStrip = options.BoundStrip;
        BoundHeadsets = options.BoundHeadsets;
    }
}