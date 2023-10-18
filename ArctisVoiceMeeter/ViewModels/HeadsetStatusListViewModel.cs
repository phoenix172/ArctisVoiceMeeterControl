using System.Collections.Specialized;
using System.Linq;
using System.Threading.Channels;
using ArctisVoiceMeeter.Model;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ArctisVoiceMeeter.ViewModels
{
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

    public partial class HeadsetStatusListViewModel : ObservableObject
    {
        private readonly ChannelBindingService _bindingService;
        public HeadsetPoller Poller { get; }

        [ObservableProperty] private HeadsetViewModel[] _headsets = {};

        public HeadsetStatusListViewModel(HeadsetPoller poller, ChannelBindingService bindingService)
        {
            _bindingService = bindingService;
            Poller = poller;
            poller.ArctisStatusChanged += OnHeadsetStatusChanged;
        }

        private void OnHeadsetStatusChanged(object? sender, ArctisStatus[] status)
        {
            SetHeadsetNames(status);

            if (Headsets.Length == status.Length)
            {
                foreach (var (headset, newStatus) in Headsets.Zip(status))
                {
                    headset.UpdateStatus(newStatus);
                }
            }
            else
            {
                Headsets = status.Select((headsetStatus,headsetIndex) => new HeadsetViewModel(headsetIndex,headsetStatus, _bindingService)).ToArray();
            }
        }

        private static void SetHeadsetNames(ArctisStatus[] status)
        {
            for (var i = 0; i < status.Length; i++)
            {
                status[i].HeadsetName = $"Headset {i+1}";
            }
        }
    }
}
