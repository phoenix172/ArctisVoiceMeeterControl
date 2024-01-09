using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Security.Authentication.ExtendedProtection;
using ArctisVoiceMeeter.Infrastructure;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ArctisVoiceMeeter.Model;

public class PropertyValueChangedEventArgs : PropertyChangedEventArgs
{
    public object OldValue { get; set; }
    public object NewValue { get; set; }

    public PropertyValueChangedEventArgs(string? propertyName, object? oldValue, object? newValue) : base(propertyName)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}

public partial class ChannelBinding : ObservableObject, IDisposable
{
    public event PropertyChangedEventHandler OptionsChanged;
    public event EventHandler<PropertyValueChangedEventArgs> BindingNameChanged;

    private readonly VoiceMeeterClient _voiceMeeter;

    [ObservableProperty]
    private ChannelBindingOptions _options;

    private HeadsetChannelBinding[] _boundHeadsets;

    [ObservableProperty] 
    private float _voiceMeeterVolume;

    public ChannelBinding(HeadsetPoller poller, VoiceMeeterClient voiceMeeter, ChannelBindingOptions options)
    {
        HeadsetPoller = poller;
        _voiceMeeter = voiceMeeter;
        Options = options;
        BoundHeadsets = Options.BoundHeadsets.ToArray();

        HeadsetPoller.ArctisStatusChanged += OnHeadsetStatusChanged;
        Options.PropertyChanged += OnOptionsPropertyChanged;
    }

    private void OnOptionsPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if(e.PropertyName != nameof(Options.BindingName))
            OptionsChanged?.Invoke(sender, e);
    }

    public HeadsetPoller HeadsetPoller { get; }

    public void Rename(string newName)
    {
        string oldName = Options.BindingName;
        Options.BindingName = newName;
        BindingNameChanged?.Invoke(this, new PropertyValueChangedEventArgs(nameof(BindingName), oldName, newName));
    }

    public HeadsetChannelBinding[] BoundHeadsets
    {
        get => _boundHeadsets;
        set
        {
            SetProperty(ref _boundHeadsets, value);
            Options.BoundHeadsets = value.ToList();
        }
    }

    public string BindingName => Options.BindingName;

    private void OnHeadsetStatusChanged(object? sender, ArctisStatus[] status)
    {
        foreach (var boundHeadset in BoundHeadsets.Where(x=>x.IsEnabled))
        {
            var boundHeadsetStatus = status[boundHeadset.Index];
            UpdateVoiceMeeterGain(boundHeadsetStatus, boundHeadset.BoundChannel);
        }
    }

    private void UpdateVoiceMeeterGain(ArctisStatus arctisStatus, ArctisChannel channel)
    {
        float newVolume = GetScaledChannelVolume(arctisStatus, channel);
        if(newVolume == VoiceMeeterVolume) return;
        VoiceMeeterVolume = newVolume;

        _voiceMeeter.TrySetGain(Options.BoundStrip, VoiceMeeterVolume);
    }

    private float GetScaledChannelVolume(ArctisStatus status, ArctisChannel channel)
    {
        var volume = status.GetArctisVolume(channel);
        return MathHelper.Scale(volume, 0, 100, Options.VoiceMeeterMinVolume, Options.VoiceMeeterMaxVolume);
    }

    public void Dispose()
    {
        HeadsetPoller.ArctisStatusChanged -= OnHeadsetStatusChanged;
        Options.PropertyChanged -= OnOptionsPropertyChanged;
    }
}