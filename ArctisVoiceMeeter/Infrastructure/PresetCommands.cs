using System.Windows.Input;

namespace ArctisVoiceMeeter.Infrastructure;

public static class PresetCommands
{
    public static RoutedCommand DeletePresetCommand = new();
    public static RoutedCommand RenamePresetCommand = new();
    public static RoutedCommand CreatePresetCommand = new();
}