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

namespace ArctisVoiceMeeter.ViewModels;

public partial class MainViewModel : ObservableObject
{
    public MainViewModel(
        HeadsetStatusListViewModel headsetStatus, 
        ChannelBindingListViewModel channelBindingListViewModel)
    {
        ChannelBindings = channelBindingListViewModel;
        HeadsetStatus = headsetStatus;
    }

    public ChannelBindingListViewModel ChannelBindings { get; set; }
    public HeadsetStatusListViewModel HeadsetStatus { get; }
}