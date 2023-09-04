using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using ArctisVoiceMeeter.Infrastructure;

namespace ArctisVoiceMeeter.Model;

public partial class ArctisVoiceMeeterChannelBinding : INotifyPropertyChanged, IDisposable
{
    private CancellationTokenSource? _tokenSource;
    private Task? _bindingTask;
    private readonly ArctisClient _arctis;
    private readonly VoiceMeeterClient _voiceMeeter;

    public ArctisVoiceMeeterChannelBinding(ArctisClient arctis, VoiceMeeterClient voiceMeeter)
    {
        _arctis = arctis;
        _voiceMeeter = voiceMeeter;
    }

    public void Bind()
    {
        if (_bindingTask != null)
            return;

        _tokenSource = new CancellationTokenSource();
        var token = _tokenSource.Token;
        _bindingTask = Task.Factory.StartNew(async () =>
        {
            try
            {
                while (token.IsCancellationRequested)
                {
                    await PollOnce();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Unbind();
            }
        }, token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
    }

    public void Unbind()
    {
        if (_bindingTask == null || _tokenSource == null)
            return;

        _tokenSource.Cancel();
        _bindingTask.Wait();
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
}