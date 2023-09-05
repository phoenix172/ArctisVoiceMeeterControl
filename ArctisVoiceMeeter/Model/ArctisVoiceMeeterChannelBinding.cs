using System;
using System.ComponentModel;
using System.Diagnostics;
using ArctisVoiceMeeter.Infrastructure;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ArctisVoiceMeeter.Model;

public partial class ArctisVoiceMeeterChannelBinding : ObservableObject, IDisposable
{
    private readonly VoiceMeeterClient _voiceMeeter;

    [ObservableProperty] private float _voiceMeeterMinVolume;
    [ObservableProperty] private float _voiceMeeterMaxVolume;
    [ObservableProperty] private uint _boundStrip;
    [ObservableProperty] private float _voiceMeeterVolume;

    [NotifyPropertyChangedFor(nameof(BindChatChannel))]
    [NotifyPropertyChangedFor(nameof(BindGameChannel))]
    [ObservableProperty]
    private ArctisChannel _boundChannel;

    public ArctisVoiceMeeterChannelBinding(HeadsetPoller poller, VoiceMeeterClient voiceMeeter)
    {
        HeadsetPoller = poller;
        _voiceMeeter = voiceMeeter;

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
        VoiceMeeterVolume = GetScaledChannelVolume(arctisStatus);
        _voiceMeeter.TrySetGain(BoundStrip, VoiceMeeterVolume);
    }

    private float GetScaledChannelVolume(ArctisStatus status)
    {
        var volume = status.GetArctisVolume(BoundChannel);
        return MathHelper.Scale(volume, 0, 100, VoiceMeeterMinVolume, VoiceMeeterMaxVolume);
    }

    public void Dispose()
    {
        HeadsetPoller.ArctisStatusChanged -= OnHeadsetStatusChanged;
    }
}