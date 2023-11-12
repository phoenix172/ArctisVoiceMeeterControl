using System.Windows.Input;

namespace ArctisVoiceMeeter.Infrastructure;

public static class PresetCommands
{
    public static RoutedCommand Delete = new();
    public static RoutedCommand Rename = new();
    public static RoutedCommand Commit = new();
    public static RoutedCommand Discard = new();
    public static RoutedCommand Create = new();
}