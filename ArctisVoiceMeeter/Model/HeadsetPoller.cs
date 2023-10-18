using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace ArctisVoiceMeeter.Model;

public class HeadsetPoller : IDisposable
{
    private CancellationTokenSource? _tokenSource;
    private Task? _bindingTask;
    private readonly ArctisClient _arctis;
    //private uint _arctisGameVolume;
    //private uint _arctisChatVolume;

    public event EventHandler<ArctisStatus>? ArctisStatusChanged;

    public HeadsetPoller(ArctisClient arctis)
    {
        _arctis = arctis;
    }


    public uint ArctisRefreshRate { get; set; } = 60;
    //public uint ArctisGameVolume
    //{
    //    get => _arctisGameVolume;
    //    private set => SetField(ref _arctisGameVolume, value);
    //}

    //public uint ArctisChatVolume
    //{
    //    get => _arctisChatVolume;
    //    private set => SetField(ref _arctisChatVolume, value);
    //}


    public void Dispose()
    {
        Unbind();
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
                while (!token.IsCancellationRequested)
                {
                    await PollOnce();
                }
            }
            catch (Exception ex)
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
        var arctisStatus = _arctis.GetStatus();
        ArctisStatusChanged?.Invoke(this, arctisStatus);
        await Task.Delay(1000 / (int)ArctisRefreshRate);
    }
}