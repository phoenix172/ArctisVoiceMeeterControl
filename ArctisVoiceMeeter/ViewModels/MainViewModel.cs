using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Windows.Data;
using ArctisVoiceMeeter.Infrastructure;
using ArctisVoiceMeeter.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ArctisVoiceMeeter.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly ChannelBindingService _bindingService;
    private ObservableCollection<ChannelBindingViewModel> _channelBindingsSource;


    public MainViewModel(HeadsetViewModel headsetViewModel, ChannelBindingService bindingService)
    {
        _bindingService = bindingService;
        HeadsetViewModel = headsetViewModel;
        ChannelBindings = CreateChannelBindingsCollectionView();
    }

    public ICollectionView ChannelBindings { get; }
    public HeadsetViewModel HeadsetViewModel { get; }

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

    private ICollectionView CreateChannelBindingsCollectionView()
    {
        var viewModels = _bindingService.Bindings.Select(x => new ChannelBindingViewModel(x));
        _channelBindingsSource = new ObservableCollection<ChannelBindingViewModel>(viewModels);
        return CollectionViewSource.GetDefaultView(_channelBindingsSource);
    }
}