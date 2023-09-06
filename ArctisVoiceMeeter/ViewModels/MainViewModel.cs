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

namespace ArctisVoiceMeeter.ViewModels;

public class MainViewModel
{
    public MainViewModel(HeadsetViewModel headsetViewModel, ChannelBindingService bindingService)
    {
        HeadsetViewModel = headsetViewModel;
        ChannelBindings = bindingService.Bindings.Select(x => new ChannelBindingViewModel(x.Value, x.Key)).ToList();
        //arctis ??= new ArctisClient();
        //_poller ??= new HeadsetPoller(arctis);
        //voiceMeeter ??= new VoiceMeeterClient();

        //HeadsetViewModel = new HeadsetViewModel(_poller);

        //var channelBinding = new ArctisVoiceMeeterChannelBinding(_poller, voiceMeeter)
        //{
        //    BoundStrip = _settings.BoundStrip,
        //    BoundChannel = _settings.BoundChannel,
        //    VoiceMeeterMinVolume = _settings.VoiceMeeterMinVolume,
        //    VoiceMeeterMaxVolume = _settings.VoiceMeeterMaxVolume
        //};
        //channelBinding.HeadsetPoller.Bind();

        //ChannelBindings = new List<ChannelBindingViewModel> { new ChannelBindingViewModel(channelBinding) };
    }

    public List<ChannelBindingViewModel> ChannelBindings { get; }
    public HeadsetViewModel HeadsetViewModel { get; set; }

    public void HandleClose()
    {
        //    var channelBinding = ChannelBindings.First().ChannelBinding;
        //    channelBinding.HeadsetPoller.Unbind();

        //    _settings.BoundStrip = channelBinding.BoundStrip;
        //    _settings.BoundChannel = channelBinding.BoundChannel;
        //    _settings.VoiceMeeterMinVolume = channelBinding.VoiceMeeterMinVolume;
        //    _settings.VoiceMeeterMaxVolume = channelBinding.VoiceMeeterMaxVolume;

        //    _settings.Save();
    }
}