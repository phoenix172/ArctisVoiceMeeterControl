using System.Linq;
using ArctisVoiceMeeter.Model;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ArctisVoiceMeeter.ViewModels
{
    public partial class HeadsetViewModel : ObservableObject
    {
        public HeadsetPoller Poller { get; }

        [ObservableProperty]
        private ArctisStatus[] _status;

        public HeadsetViewModel(HeadsetPoller poller)
        {
            Poller = poller;
            poller.ArctisStatusChanged += OnHeadsetStatusChanged;
        }

        private void OnHeadsetStatusChanged(object? sender, ArctisStatus[] status)
        {
            SetHeadsetNames(status);

            if (Status?.Length == status.Length)
            {
                foreach (var (left, right) in Status.Zip(status))
                {
                    left.CopyFrom(right);
                }
            }
            else
            {
                Status = status;
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
