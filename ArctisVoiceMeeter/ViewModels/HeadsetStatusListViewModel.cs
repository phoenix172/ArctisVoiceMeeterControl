﻿using System.Collections.Specialized;
using System.Linq;
using System.Threading.Channels;
using ArctisVoiceMeeter.Model;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ArctisVoiceMeeter.ViewModels
{
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
