using System;
using System.ComponentModel;
using System.Diagnostics;
using ArctisVoiceMeeter.Infrastructure;

namespace ArctisVoiceMeeter.Model;

public partial class ArctisVoiceMeeterChannelBinding : INotifyPropertyChanged, IDisposable
{
    private readonly VoiceMeeterClient _voiceMeeter;
    private readonly HeadsetPoller _headsetPoller;
    private ArctisChannel _boundChannel;

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


    public ArctisChannel BoundChannel
    {
        get => _boundChannel;
        set
        {
            SetField(ref _boundChannel, value);
            OnPropertyChanged(nameof(BindChatChannel));
            OnPropertyChanged(nameof(BindGameChannel));
        }
    }

    public ArctisVoiceMeeterChannelBinding(HeadsetPoller poller, VoiceMeeterClient voiceMeeter)
    {
        _headsetPoller = poller;
        _voiceMeeter = voiceMeeter;

        HeadsetPoller.ArctisStatusChanged += OnHeadsetStatusChanged;
    }

    private void OnHeadsetStatusChanged(object? sender, ArctisStatus e)
    {
        UpdateVoiceMeeterGain(e);
    }

    public HeadsetPoller HeadsetPoller
    {
        get { return _headsetPoller; }
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
        //_headsetPoller.Dispose();
    }
}