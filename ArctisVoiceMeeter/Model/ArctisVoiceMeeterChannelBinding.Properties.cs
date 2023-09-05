using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ArctisVoiceMeeter.Model;

public partial class ArctisVoiceMeeterChannelBinding
{
    private float _voiceMeeterMinVolume;
    private float _voiceMeeterMaxVolume;
    private uint _boundStrip;
    private float _voiceMeeterVolume;

    public float VoiceMeeterVolume
    {
        get => _voiceMeeterVolume;
        private set => SetField(ref _voiceMeeterVolume, value);
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