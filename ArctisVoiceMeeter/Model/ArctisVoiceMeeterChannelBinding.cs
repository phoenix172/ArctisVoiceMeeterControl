using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace ArctisVoiceMeeter.Model;

public class ArctisVoiceMeeterChannelBinding : INotifyPropertyChanged, IDisposable
{
    private CancellationTokenSource? _tokenSource;
    private Task? _bindingTask;
    private readonly ArctisClient _arctis;
    private readonly VoiceMeeterClient _voiceMeeter;
    private uint _arctisRefreshRate = 60;
    private uint _arctisGameVolume;
    private uint _arctisChatVolume;
    private float _voiceMeeterMinVolume;
    private float _voiceMeeterMaxVolume;
    private ArctisChannel _boundChannel;
    private uint _boundStrip;
    private float _voiceMeeterVolume;

    public ArctisVoiceMeeterChannelBinding(ArctisClient arctis, VoiceMeeterClient voiceMeeter)
    {
        _arctis = arctis;
        _voiceMeeter = voiceMeeter;
    }

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

    public void Bind()
    {
        if (_bindingTask != null)
            return;

        _tokenSource = new CancellationTokenSource();
        _bindingTask = Task.Factory.StartNew(async () =>
        {
            try
            {
                while (!_tokenSource.IsCancellationRequested)
                {
                    await PollOnce();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Unbind();
            }
        }, _tokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
    }

    public void Unbind()
    {
        if (_bindingTask == null || _tokenSource == null)
            return;

        _tokenSource.Cancel();
        _tokenSource.Dispose();
        _bindingTask.Dispose();
        _tokenSource = null;
        _bindingTask = null;
    }

    private async Task PollOnce()
    {
        var status = _arctis.GetStatus();

        ArctisChatVolume = status.ChatVolume;
        ArctisGameVolume = status.GameVolume;

        VoiceMeeterVolume = GetScaledChannelVolume(BoundChannel);
        _voiceMeeter.TrySetGain(BoundStrip, VoiceMeeterVolume);

        await Task.Delay(1000 / (int)ArctisRefreshRate);
    }

    private uint GetArctisVolume(ArctisChannel channel)
    {
        return channel == ArctisChannel.Chat ? ArctisChatVolume : ArctisGameVolume;
    }

    private float GetScaledChannelVolume(ArctisChannel channel)
    {
        var volume = GetArctisVolume(channel);
        return MathHelper.Scale(volume, 0, 100, VoiceMeeterMinVolume, VoiceMeeterMaxVolume);
    }

    public void Dispose()
    {
        Unbind();
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