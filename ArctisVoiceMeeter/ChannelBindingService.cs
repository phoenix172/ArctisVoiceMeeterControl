using ArctisVoiceMeeter.Model;
using ArctisVoiceMeeter.ViewModels;
using Awesome.Net.WritableOptions;
using CommunityToolkit.Mvvm.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Authentication.ExtendedProtection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Xml.Linq;
using ChannelBinding = ArctisVoiceMeeter.Model.ChannelBinding;

namespace ArctisVoiceMeeter
{
    public class ChannelBindingService : IDisposable
    {
        public event NotifyCollectionChangedEventHandler BindingsChanged
        {
            add => Bindings.CollectionChanged += value;
            remove => Bindings.CollectionChanged -= value;
        }

        private readonly IWritableOptions<ArctisVoiceMeeterPresets> _presets;
        private readonly HeadsetPoller _headsetPoller;
        private readonly VoiceMeeterClient _voiceMeeterClient;

        public ChannelBindingService(HeadsetPoller headsetPoller, VoiceMeeterClient voiceMeeterClient, IWritableOptions<ArctisVoiceMeeterPresets> presets)
        {
            _presets = presets;
            _headsetPoller = headsetPoller;
            _voiceMeeterClient = voiceMeeterClient;

            _headsetPoller.Bind();

            Bindings = CreateBindingsCollection();
            BindingsChanged += (_, _) => UpdateChannelBindings();
            //TODO: _headsetPoller.HeadsetCountChanged += (_, _) => UpdateChannelBindings()

            UpdateChannelBindings();
        }

        private void UpdateChannelBindings()
        {
            HeadsetBindings = new ObservableCollection<HeadsetChannelBinding>(CalculateHeadsetBindings());
            foreach (var channelBinding in Bindings)
            {
                channelBinding.BoundHeadsets = GetHeadsetBindings(channelBinding.HeadsetIndex).ToArray();
            }
        }

        public ObservableCollection<ChannelBinding> Bindings { get; }
        public ObservableCollection<HeadsetChannelBinding> HeadsetBindings { get; private set; }

        public void SetBinding(ChannelBindingOptions options)
        {
            if (BindingExists(options.BindingName))
                ChangeBinding(options);
            else
                AddBinding(options);
        }

        public ChannelBinding AddNewBinding()
        {
            string bindingName = "Preset " + (Bindings.Count + 1);
            while (BindingExists(bindingName))
                bindingName += "_";

            return AddBinding(new ChannelBindingOptions(bindingName));
        }

        public ChannelBinding AddBinding(ChannelBindingOptions options)
        {
            if (_presets.Value.Any(x => x.BindingName == options.BindingName))
                throw new ArgumentException("A Channel Binding with this name already exists.", nameof(options.BindingName));

            _presets.Update(x => x.Add(options));

            var binding = CreateBinding(options);
            Bindings.Add(binding);
            return binding;
        }

        public void ChangeBinding(ChannelBindingOptions binding)
        {
            _presets.Update(presets =>
            {
                var previous = presets.FirstOrDefault(x => x.BindingName == binding.BindingName);
                if (previous == null)
                    throw new ArgumentException("An existing Channel Binding with this name was not found.",
                        nameof(binding.BindingName));
                previous.CopyFrom(binding);
            });
        }

        public ChannelBinding? GetBinding(string name) => Bindings.FirstOrDefault(x => x.BindingName == name);

        public bool BindingExists(string name) => GetBinding(name) != null;

        public bool RemoveBinding(string name)
        {
            bool result = false;

            _presets.Update(presets =>
            {
                var bindingOption = presets.FirstOrDefault(x => x.BindingName == name);
                if (bindingOption == null) return;
                presets.Remove(bindingOption);
                result = true;
            });

            if (!result) return false;

            var binding = GetBinding(name);
            if (binding != null)
                Bindings.Remove(binding);

            return true;
        }

        public IEnumerable<HeadsetChannelBinding> GetHeadsetBindings(int? headsetIndex)
        {
            var bindings = HeadsetBindings.Where(x => x.Index == headsetIndex);

            return bindings;
        }

        private IEnumerable<HeadsetChannelBinding> CalculateHeadsetBindings()
        {
            var bindingsLookup = Bindings
                .SelectMany(x => x.BoundHeadsets.Select(y => (y.Index, x.BoundChannel, x.BindingName, Binding:x)))
                .ToDictionary(x => (x.Index, x.BoundChannel, x.BindingName), x=>x.Binding);
            var headsetIndices = Enumerable.Range(0, _headsetPoller.GetStatus().Length);
            var headsetChannels = Enum.GetValues<ArctisChannel>();

            var bindings =
                from index in headsetIndices
                from channel in headsetChannels
                from binding in Bindings
                select CreateHeadsetChannelBinding(index, channel, binding);
            return bindings;

            HeadsetChannelBinding CreateHeadsetChannelBinding(int index, ArctisChannel channel, ChannelBinding binding)
            {
                HeadsetChannelBinding result = new HeadsetChannelBinding(index, channel)
                {
                    IsEnabled = bindingsLookup.TryGetValue((index, channel, binding.BindingName), out _)
                };
                result.PropertyChanged += (sender, args) =>
                {
                    var source = sender as HeadsetChannelBinding;
                    binding.BoundChannel = source.BoundChannel;

                };
                return result;
            }
        }

        public void Dispose()
        {
            _headsetPoller.Unbind();

            foreach (var binding in Bindings)
            {
                binding.OptionsChanged -= BindingPropertyChanged;
                binding.Dispose();
            }

            Bindings.Clear();
        }

        private ChannelBinding CreateBinding(ChannelBindingOptions options)
        {
            var binding = new ChannelBinding(_headsetPoller, _voiceMeeterClient, options);
            binding.OptionsChanged += BindingPropertyChanged;
            return binding;
        }

        private ObservableCollection<ChannelBinding> CreateBindingsCollection()
        {
            ArctisVoiceMeeterPresets options = _presets.Value;
            var bindings = options.Select(CreateBinding);

            return new ObservableCollection<ChannelBinding>(bindings);
        }
        private void BindingPropertyChanged(object? sender, ChannelBindingOptions options)
        {
            ChangeBinding(options);
        }
    }
}
