using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace ArctisVoiceMeeter;

public enum ArctisChannel
{
    Chat,
    Game
}

public class MainViewModel : INotifyPropertyChanged
{
    private readonly ArctisClient _arctis;
    private readonly VoiceMeeterClient _voiceMeeter;

    private float _stripGain = 0f;
    private uint _stripIndex = 7;
    private float _maxVolume = 0;
    private float _minVolume = -7;
    private bool _bindGameChannel = false;
    private bool _bindChatChannel = true;
    private uint _currentChatVolume;
    private uint _currentGameVolume;

    public MainViewModel(ArctisClient? arctis = null, VoiceMeeterClient? voiceMeeter = null)
    {
        arctis ??= new ArctisClient();
        voiceMeeter ??= new VoiceMeeterClient();
        _arctis = arctis;
        _voiceMeeter = voiceMeeter;
        Poll();
    }

    public uint CurrentChatVolume
    {
        get => _currentChatVolume;
        private set => SetField(ref _currentChatVolume, value);
    }

    public uint CurrentGameVolume
    {
        get => _currentGameVolume;
        private set => SetField(ref _currentGameVolume, value);
    }

    public void Poll()
    {
        var pollTask = Task.Run(async () =>
        {
            while (true)
            {
                var status = _arctis.GetStatus();
                CurrentChatVolume = status.ChatVolume;
                CurrentGameVolume = status.GameVolume;

                if (BindChatChannel)
                {
                    float scaledValue = GetScaledChannelVolume(ArctisChannel.Chat);
                    _voiceMeeter.TrySetGain(StripIndex, scaledValue);
                }

                if (BindGameChannel)
                {
                    float scaledValue = GetScaledChannelVolume(ArctisChannel.Game);
                    _voiceMeeter.TrySetGain(StripIndex, scaledValue);
                }

                await Task.Delay(1000 / 60);
            }
        });
    }

    private float GetScaledChannelVolume(ArctisChannel channel)
    {
        var volume = channel == ArctisChannel.Chat ? CurrentChatVolume : CurrentGameVolume;
        return MathHelper.Scale(volume, 0, 100, MinVolume, MaxVolume);
    }

    public uint StripIndex
    {
        get => _stripIndex;
        set => SetField(ref _stripIndex, value);
    }

    public float StripGain
    {
        get => _stripGain;
        set
        {
            SetField(ref _stripGain, value);
            _voiceMeeter.TrySetGain(StripIndex, StripGain);
        }
    }

    public float MinVolume
    {
        get => _minVolume;
        set => SetField(ref _minVolume, value);
    }

    public float MaxVolume
    {
        get => _maxVolume;
        set => SetField(ref _maxVolume, value);
    }

    public bool BindChatChannel
    {
        get => _bindChatChannel;
        set => SetField(ref _bindChatChannel, value);
    }

    public bool BindGameChannel
    {
        get => _bindGameChannel;
        set => SetField(ref _bindGameChannel, value);
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