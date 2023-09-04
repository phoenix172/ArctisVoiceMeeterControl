using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ArctisVoiceMeeter.Model;

public partial class ArctisVoiceMeeterChannelBinding
{
    private uint _arctisRefreshRate = 60;
    private uint _arctisGameVolume;
    private uint _arctisChatVolume;
    private float _voiceMeeterMinVolume;
    private float _voiceMeeterMaxVolume;
    private ArctisChannel _boundChannel;
    private uint _boundStrip;
    private float _voiceMeeterVolume;

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

    public float VoiceMeeterVolume
    {
        get => _voiceMeeterVolume;
        private set => SetField(ref _voiceMeeterVolume, value);
    }

    public uint ArctisRefreshRate
    {
        get => _arctisRefreshRate;
        set => SetField(ref _arctisRefreshRate, value);
    }

    public uint ArctisGameVolume
    {
        get => _arctisGameVolume;
        private set => SetField(ref _arctisGameVolume, value);
    }

    public uint ArctisChatVolume
    {
        get => _arctisChatVolume;
        private set => SetField(ref _arctisChatVolume, value);
    }

    public float VoiceMeeterMinVolume
    {
        get => _voiceMeeterMinVolume;
        set => SetField(ref _voiceMeeterMinVolume, value);
    }

    public float VoiceMeeterMaxVolume
    {
        get => _voiceMeeterMaxVolume;
        set => SetField(ref _voiceMeeterMaxVolume, value);
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

    public uint BoundStrip
    {
        get => _boundStrip;
        set => SetField(ref _boundStrip, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}