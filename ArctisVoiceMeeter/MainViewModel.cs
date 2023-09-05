using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using ArctisVoiceMeeter.Infrastructure;
using ArctisVoiceMeeter.Model;

namespace ArctisVoiceMeeter;

public class MainViewModel
{
    private readonly AppSettings _settings;
    private readonly HeadsetPoller poller;

    public MainViewModel(ArctisClient? arctis = null, VoiceMeeterClient? voiceMeeter = null)
    {
        _settings = AppSettings.Load();

        arctis ??= new ArctisClient();
        poller ??= new HeadsetPoller(arctis);
        voiceMeeter ??= new VoiceMeeterClient();

        HeadsetViewModel = new HeadsetViewModel(poller);

        var channelBinding = new ArctisVoiceMeeterChannelBinding(poller, voiceMeeter)
        {
            BoundStrip = _settings.BoundStrip,
            BoundChannel = _settings.BoundChannel,
            VoiceMeeterMinVolume = _settings.VoiceMeeterMinVolume,
            VoiceMeeterMaxVolume = _settings.VoiceMeeterMaxVolume
        };
        channelBinding.HeadsetPoller.Bind();

        ChannelBindings = new List<ChannelBindingViewModel> { new ChannelBindingViewModel(channelBinding) };
    }

    public List<ChannelBindingViewModel> ChannelBindings { get; }
    public HeadsetViewModel HeadsetViewModel { get; set; }

    public void HandleClose()
    {
        var channelBinding = ChannelBindings.First().ChannelBinding;
        channelBinding.HeadsetPoller.Unbind();

        _settings.BoundStrip = channelBinding.BoundStrip;
        _settings.BoundChannel = channelBinding.BoundChannel;
        _settings.VoiceMeeterMinVolume = channelBinding.VoiceMeeterMinVolume;
        _settings.VoiceMeeterMaxVolume = channelBinding.VoiceMeeterMaxVolume;

        _settings.Save();
    }
}