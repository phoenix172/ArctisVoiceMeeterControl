using ArctisVoiceMeeter.Model;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ArctisVoiceMeeter.ViewModels;

public partial class ChannelBindingViewModel : ObservableObject
{
    public ChannelBindingViewModel(ArctisVoiceMeeterChannelBinding channelBinding)
    {
        BindingName = channelBinding.BindingName;
        ChannelBinding = channelBinding;
    }
    [ObservableProperty] private string _bindingName;

    public ArctisVoiceMeeterChannelBinding ChannelBinding { get; }

}