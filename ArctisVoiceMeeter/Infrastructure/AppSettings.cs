using System;
using System.IO;
using System.Text.Json;
using ArctisVoiceMeeter.Model;

namespace ArctisVoiceMeeter.Infrastructure;

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
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        return settings ?? CreateDefaultAppSettings();
    }

    public void Save(string settingsFile = "settings.json")
    {
        var settingsJson = JsonSerializer.Serialize(this, new JsonSerializerOptions()
        {
            WriteIndented = true
        });
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