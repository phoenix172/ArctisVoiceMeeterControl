using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ArctisVoiceMeeter.Model;

public class ChannelBindingViewModel : INotifyPropertyChanged
{
    public ChannelBindingViewModel(ArctisVoiceMeeterChannelBinding channelBinding)
    {
        ChannelBinding = channelBinding;
    }
    private string _name = "Preset";

    public ArctisVoiceMeeterChannelBinding ChannelBinding { get; }

    public string Name
    {
        get => _name;
        set => SetField(ref _name, value);
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