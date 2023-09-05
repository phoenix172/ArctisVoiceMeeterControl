using ArctisVoiceMeeter.Model;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ArctisVoiceMeeter.ViewModels
{
    public partial class HeadsetViewModel : ObservableObject
    {
        public HeadsetPoller Poller { get; }

        [ObservableProperty]
        private ArctisStatus _status;

        public HeadsetViewModel(HeadsetPoller poller)
        {
            Poller = poller;

            poller.ArctisStatusChanged += OnHeadsetStatusChanged;
        }

        private void OnHeadsetStatusChanged(object? sender, ArctisStatus status)
        {
            Status = status;
        }
    }
}
