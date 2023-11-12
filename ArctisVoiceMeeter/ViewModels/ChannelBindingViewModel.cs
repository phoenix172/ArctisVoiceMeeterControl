using System.ComponentModel;
using ArctisVoiceMeeter.Model;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ArctisVoiceMeeter.ViewModels;

public partial class ChannelBindingViewModel : ObservableObject, IEditableObject
{
    public ChannelBindingViewModel(ChannelBinding channelBinding)
    {
        BindingName = channelBinding.BindingName;
        ChannelBinding = channelBinding;
    }
    [ObservableProperty] private string _bindingName;
    [ObservableProperty] private bool _isRenaming;

    public ChannelBinding ChannelBinding { get; }

    public void BeginEdit()
    {
        IsRenaming = true;
    }

    public void CancelEdit()
    {
        BindingName = ChannelBinding.BindingName;
        IsRenaming = false;
    }

    public void EndEdit()
    {
        ChannelBinding.Rename(BindingName);
        IsRenaming = false;
    }
}