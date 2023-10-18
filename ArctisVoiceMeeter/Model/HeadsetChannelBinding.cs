using System;
using System.Globalization;
using System.Text.Json.Serialization;
using System.Threading;
using System.Windows.Data;
using System.Windows.Markup;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ArctisVoiceMeeter.Model;

public partial class HeadsetChannelBinding : ObservableObject
{
    [JsonConstructor]
    private HeadsetChannelBinding()
    {

    }

    public HeadsetChannelBinding(int index, ArctisChannel boundChannel, string channelBindingName,
        bool isEnabled = true)
    {
        ChannelBindingName = channelBindingName;
        Index = index;
        BoundChannel = boundChannel;
        IsEnabled = isEnabled;
    }

    [ObservableProperty] private string _channelBindingName;

    [ObservableProperty] private int _index;
    [ObservableProperty] private ArctisChannel _boundChannel;
    [ObservableProperty] private bool _isEnabled;
}