using System.Linq;
using ArctisVoiceMeeter.Model;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ArctisVoiceMeeter.ViewModels;

public partial class HeadsetViewModel : ObservableObject
{
    [ObservableProperty] private int _index;
    [ObservableProperty] private ArctisStatus _status;
    [ObservableProperty] private HeadsetChannelBindingViewModel[] _channelBindings;

    private readonly ChannelBindingService _bindingService;

    public HeadsetViewModel(int index, ArctisStatus status, ChannelBindingService bindingService)
    {
        _index = index;
        _status = status;
        _bindingService = bindingService;

        _bindingService.BindingsChanged += (_,_) => LoadChannelBindings();            
            
        LoadChannelBindings();
    }

    private void LoadChannelBindings()
    {
        ChannelBindings = _bindingService.GetHeadsetBindings(Index)
            .Select(x=>new HeadsetChannelBindingViewModel(x)).ToArray();
    }

    public string HeadsetName => $"Headset {Index + 1}";

    public void UpdateStatus(ArctisStatus status)
    {
        Status.CopyFrom(status);
    }
}