using ArctisVoiceMeeter.Model;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ArctisVoiceMeeter.ViewModels;

public partial class ChannelBindingViewModel : ObservableObject
{
    public ChannelBindingViewModel(ArctisVoiceMeeterChannelBinding channelBinding)
    {
        ChannelBinding = channelBinding;
    }
    [ObservableProperty] private string _name = "";

    public ArctisVoiceMeeterChannelBinding ChannelBinding { get; }

}