using CommunityToolkit.Mvvm.ComponentModel;

namespace ArctisVoiceMeeter.Model;

public partial class HeadsetChannelBinding : ObservableObject
{
    private bool _isEnabled;

    public HeadsetChannelBinding(int index, ArctisChannel boundChannel)
    {
        Index = index;
        BoundChannel = boundChannel;
    }

    [ObservableProperty] private int _index;
    [ObservableProperty] private ArctisChannel _boundChannel;

    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            if (value)
                Enable();
            else
                Disable();
            SetProperty(ref _isEnabled, value);
        }
    }

    public void Enable()
    {

    }

    public void Disable()
    {

    }
}