using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using ArctisVoiceMeeter.Model;

namespace ArctisVoiceMeeter;

public class MainViewModel
{
    private readonly AppSettings _settings;

    public MainViewModel(ArctisClient? arctis = null, VoiceMeeterClient? voiceMeeter = null)
    {
        _settings = AppSettings.Load();

        arctis ??= new ArctisClient();
        voiceMeeter ??= new VoiceMeeterClient();

        Binding = new ArctisVoiceMeeterChannelBinding(arctis, voiceMeeter)
        {
            BoundStrip = _settings.BoundStrip,
            BoundChannel = _settings.BoundChannel,
            VoiceMeeterMinVolume = _settings.VoiceMeeterMinVolume,
            VoiceMeeterMaxVolume = _settings.VoiceMeeterMaxVolume
        };
        Binding.Bind();
    }

    public ArctisVoiceMeeterChannelBinding Binding { get; }

    public void HandleClose()
    {
        Binding.Unbind();

        _settings.BoundStrip = Binding.BoundStrip;
        _settings.BoundChannel = Binding.BoundChannel;
        _settings.VoiceMeeterMinVolume = Binding.VoiceMeeterMinVolume;
        _settings.VoiceMeeterMaxVolume = Binding.VoiceMeeterMaxVolume;

        _settings.Save();
    }
}