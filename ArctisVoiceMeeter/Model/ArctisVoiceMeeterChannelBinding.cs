using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Security.Authentication.ExtendedProtection;
using ArctisVoiceMeeter.Infrastructure;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ArctisVoiceMeeter.Model;

public partial class ArctisVoiceMeeterChannelBinding : ObservableObject, IDisposable
{
    private readonly VoiceMeeterClient _voiceMeeter;
   
    public ArctisVoiceMeeterChannelBindingOptions Options { get; }

    [NotifyPropertyChangedFor(nameof(BindChatChannel))]
    [NotifyPropertyChangedFor(nameof(BindGameChannel))]
    [ObservableProperty]
    private ArctisChannel _boundChannel;

    [ObservableProperty]
    private string _bindingName;

    public ArctisVoiceMeeterChannelBinding(HeadsetPoller poller, VoiceMeeterClient voiceMeeter, ArctisVoiceMeeterChannelBindingOptions options)
    {
        HeadsetPoller = poller;
        _voiceMeeter = voiceMeeter;
        Options = options;
        BindingName = Options.BindingName;

        HeadsetPoller.ArctisStatusChanged += OnHeadsetStatusChanged;
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
        Options.VoiceMeeterVolume = GetScaledChannelVolume(arctisStatus);
        _voiceMeeter.TrySetGain(Options.BoundStrip, Options.VoiceMeeterVolume);
    }

    private float GetScaledChannelVolume(ArctisStatus status)
    {
        var volume = status.GetArctisVolume(BoundChannel);
        return MathHelper.Scale(volume, 0, 100, Options.VoiceMeeterMinVolume, Options.VoiceMeeterMaxVolume);
    }

    public void Dispose()
    {
        HeadsetPoller.ArctisStatusChanged -= OnHeadsetStatusChanged;
    }
}