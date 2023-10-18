using System.ComponentModel;
using System.Windows;
using ArctisVoiceMeeter.Model;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ArctisVoiceMeeter;

public partial class HeadsetChannelBindingViewModel : ObservableObject
{

    private readonly HeadsetChannelBinding _headsetBinding;
    private bool? _threeState;

    public HeadsetChannelBindingViewModel(HeadsetChannelBinding headsetBinding)
    {
        _headsetBinding = headsetBinding;
        _headsetBinding.PropertyChanged += HeadsetBindingOnPropertyChanged;
    }

    public bool? ThreeState
    {
        get
        {
            (bool IsEnabled, ArctisChannel BoundChannel) switchKey = (_headsetBinding.IsEnabled, _headsetBinding.BoundChannel);
            bool? resultState = switchKey switch
            {
                (IsEnabled: true, BoundChannel: ArctisChannel.Game) => false,
                (IsEnabled: true, BoundChannel: ArctisChannel.Chat) => true,
                _ => null
            };
            return resultState;
        }
        set
        {
            if (value == false)
            {
                _headsetBinding.BoundChannel = ArctisChannel.Game;
                _headsetBinding.IsEnabled = true;
            }
            else if (value == true)
            {
                _headsetBinding.BoundChannel = ArctisChannel.Chat;
                _headsetBinding.IsEnabled = true;
            }
            else
            {
                _headsetBinding.IsEnabled = false;
            }
            
            SetProperty(ref _threeState, value);
        }
    }

    private void HeadsetBindingOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        OnPropertyChanged(nameof(ThreeState));
    }
}