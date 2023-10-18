using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Security.Authentication.ExtendedProtection;
using ArctisVoiceMeeter.Infrastructure;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ArctisVoiceMeeter.Model;

public partial class ArctisVoiceMeeterChannelBinding : ObservableObject, IDisposable
{
    public event EventHandler<ArctisVoiceMeeterChannelBindingOptions> OptionsChanged;

    private readonly VoiceMeeterClient _voiceMeeter;

    [ObservableProperty]
    private ArctisVoiceMeeterChannelBindingOptions _options;

    [NotifyPropertyChangedFor(nameof(BindChatChannel))]
    [NotifyPropertyChangedFor(nameof(BindGameChannel))]
    [ObservableProperty]
    private ArctisChannel _boundChannel;

    [ObservableProperty] 
    private float _voiceMeeterVolume;

    [ObservableProperty]
    private string _bindingName;

    public ArctisVoiceMeeterChannelBinding(HeadsetPoller poller, VoiceMeeterClient voiceMeeter, ArctisVoiceMeeterChannelBindingOptions options)
    {
        HeadsetPoller = poller;
        _voiceMeeter = voiceMeeter;
        Options = options;
        BindingName = Options.BindingName;

        HeadsetPoller.ArctisStatusChanged += OnHeadsetStatusChanged;
        Options.PropertyChanged += OnOptionsPropertyChanged;
    }

    private void OnOptionsPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        OptionsChanged?.Invoke(this, (ArctisVoiceMeeterChannelBindingOptions)sender);
    }

    public HeadsetPoller HeadsetPoller { get; }

    public bool BindChatChannel
    {
        get => BoundChannel == ArctisChannel.Chat;
        set => BoundChannel = value ? ArctisChannel.Chat : ArctisChannel.Game;
    }

    public bool BindGameChannel
    {
        get => BoundChannel == ArctisChannel.Game;
        set => BoundChannel = value ? ArctisChannel.Game : ArctisChannel.Chat;
    }

    private void OnHeadsetStatusChanged(object? sender, ArctisStatus e)
    {
        UpdateVoiceMeeterGain(e);
    }

    private void UpdateVoiceMeeterGain(ArctisStatus arctisStatus)
    {
        VoiceMeeterVolume = GetScaledChannelVolume(arctisStatus);
        _voiceMeeter.TrySetGain(Options.BoundStrip, VoiceMeeterVolume);
    }

    private float GetScaledChannelVolume(ArctisStatus status)
    {
        var volume = status.GetArctisVolume(BoundChannel);
        return MathHelper.Scale(volume, 0, 100, Options.VoiceMeeterMinVolume, Options.VoiceMeeterMaxVolume);
    }

    public void Dispose()
    {
        HeadsetPoller.ArctisStatusChanged -= OnHeadsetStatusChanged;
        Options.PropertyChanged -= OnOptionsPropertyChanged;
    }
}