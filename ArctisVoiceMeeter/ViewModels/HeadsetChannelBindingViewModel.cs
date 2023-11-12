using System.ComponentModel;
using ArctisVoiceMeeter.Model;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ArctisVoiceMeeter.ViewModels;

public partial class HeadsetChannelBindingViewModel : ObservableObject
{

    private readonly HeadsetChannelBinding _headsetBinding;
    private bool? _threeState;

    public HeadsetChannelBindingViewModel(HeadsetChannelBinding headsetBinding)
    {
        _headsetBinding = headsetBinding;
        _headsetBinding.PropertyChanged += HeadsetBindingOnPropertyChanged;
    }

    public string BindingName
    {
        get => _headsetBinding.ChannelBindingName;
        set => _headsetBinding.ChannelBindingName = value;
    }

    public bool? ThreeState
    {
        get => GetThreeState();
        set => SetThreeState(value);
    }

    private void SetThreeState(bool? value)
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

    private bool? GetThreeState()
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

    private void HeadsetBindingOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_headsetBinding.ChannelBindingName))
        {
            OnPropertyChanged(nameof(BindingName));
        }

        OnPropertyChanged(nameof(ThreeState));
    }
}