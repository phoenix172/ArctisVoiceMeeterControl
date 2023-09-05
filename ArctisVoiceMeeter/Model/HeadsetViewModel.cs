using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ArctisVoiceMeeter.Model
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
