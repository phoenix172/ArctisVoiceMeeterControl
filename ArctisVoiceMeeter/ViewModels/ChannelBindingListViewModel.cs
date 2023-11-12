using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using ArctisVoiceMeeter.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ArctisVoiceMeeter.ViewModels;

public partial class ChannelBindingListViewModel : ObservableObject
{
    private readonly ChannelBindingService _bindingService;
    private ObservableCollection<ChannelBindingViewModel> _channelBindingsSource;

    public ChannelBindingListViewModel(ChannelBindingService bindingService)
    {
        _bindingService = bindingService;
        ChannelBindings = CreateChannelBindingsCollectionView();
    }

    public ICollectionView ChannelBindings { get; }

    [RelayCommand]
    public void CreateBinding()
    {
        var binding = _bindingService.AddNewBinding();
        _channelBindingsSource.Add(new ChannelBindingViewModel(binding));
    }

    [RelayCommand]
    public void RemoveBinding(ChannelBindingViewModel binding)
    {
        if(_bindingService.RemoveBinding(binding.BindingName))
            _channelBindingsSource.Remove(binding);
    }

    [RelayCommand]
    public void RenameBinding(ChannelBindingViewModel binding)
    {

    }

    private ICollectionView CreateChannelBindingsCollectionView()
    {
        var viewModels = _bindingService.Bindings.Select(x => new ChannelBindingViewModel(x));
        _channelBindingsSource = new ObservableCollection<ChannelBindingViewModel>(viewModels);
        return CollectionViewSource.GetDefaultView(_channelBindingsSource);
    }
}