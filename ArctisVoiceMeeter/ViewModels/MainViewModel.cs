using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using ArctisVoiceMeeter.Infrastructure;
using CommunityToolkit.Mvvm.ComponentModel;
using NetworkCopy.Utilities;

namespace ArctisVoiceMeeter.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly StartupManager _startupManager;

    public MainViewModel(
        HeadsetStatusListViewModel headsetStatus, 
        ChannelBindingListViewModel channelBindingListViewModel,
        StartupManager startupManager)
    {
        _startupManager = startupManager;
        ChannelBindings = channelBindingListViewModel;
        HeadsetStatus = headsetStatus;
    }

    public bool RunOnStartup
    {
        get => _startupManager.RunOnStartup;
        set
        {
            _startupManager.RunOnStartup = value;
            OnPropertyChanged();
        }
    }

    public ChannelBindingListViewModel ChannelBindings { get; set; }
    public HeadsetStatusListViewModel HeadsetStatus { get; }
}