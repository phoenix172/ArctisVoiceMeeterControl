using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ArctisVoiceMeeter.Model;

namespace ArctisVoiceMeeter;

public class AppSettings
{
    public static AppSettings Load(string settingsFile = "settings.json")
    {
        AppSettings? settings = null;

        if (!File.Exists(settingsFile))
            return CreateDefaultAppSettings();

        try
        {
            var settingsJson = File.ReadAllText(settingsFile);
            settings = JsonSerializer.Deserialize<AppSettings>(settingsJson);
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        return settings ?? CreateDefaultAppSettings();
    }

    public void Save(string settingsFile = "settings.json")
    {
        var settingsJson = JsonSerializer.Serialize(this);
        File.WriteAllText(settingsFile, settingsJson);
    }

    private static AppSettings CreateDefaultAppSettings()
    {
        return new()
        {
            BoundStrip = 7,
            BoundChannel = ArctisChannel.Chat,
            VoiceMeeterMinVolume = -7,
            VoiceMeeterMaxVolume = 0
        };
    }

    public float VoiceMeeterMaxVolume { get; set; }

    public float VoiceMeeterMinVolume { get; set; }

    public ArctisChannel BoundChannel { get; set; }

    public uint BoundStrip { get; set; }
}

public class MainViewModel : INotifyPropertyChanged
{
    private readonly AppSettings _settings;

    public MainViewModel(ArctisClient? arctis = null, VoiceMeeterClient? voiceMeeter = null)
    {
        _settings = AppSettings.Load();

        arctis ??= new ArctisClient();
        voiceMeeter ??= new VoiceMeeterClient();

        Binding = new ArctisVoiceMeeterChannelBinding(arctis, voiceMeeter)
        {
            BoundStrip = _settings.BoundStrip,
            BoundChannel = _settings.BoundChannel,
            VoiceMeeterMinVolume = _settings.VoiceMeeterMinVolume,
            VoiceMeeterMaxVolume = _settings.VoiceMeeterMaxVolume
        };
        Binding.Bind();
    }

    public ArctisVoiceMeeterChannelBinding Binding { get; }

    public void HandleClose()
    {
        _settings.Save();
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