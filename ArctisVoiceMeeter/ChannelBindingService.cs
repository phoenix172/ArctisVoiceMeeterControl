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

        private readonly IServiceProvider _provider;
        private readonly IWritableOptions<ArctisVoiceMeeterPresets> _presets;

        public ChannelBindingService(IServiceProvider provider, IWritableOptions<ArctisVoiceMeeterPresets> presets)
        {
            _provider = provider;
            _presets = presets;
            Bindings = CreateBindingsCollection();
            BindingsChanged += (_, _) => UpdateChannelBindings();

            UpdateChannelBindings();
        }

        private void UpdateChannelBindings()
        {
            foreach (var channelBinding in Bindings)
            {
                channelBinding.BoundHeadsets = GetHeadsetBindings(channelBinding.HeadsetIndex).ToArray();
            }
        }

        public ObservableCollection<ChannelBinding> Bindings { get; }

        public void SetBinding(ChannelBindingOptions options)
        {
            if(BindingExists(options.BindingName))
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

        public ChannelBinding? GetBinding(string name) => Bindings.FirstOrDefault(x=>x.BindingName == name);

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

        public IEnumerable<HeadsetChannelBinding> GetHeadsetBindings(int? headsetIndex = null)
        {
            var bindings = Bindings
                    .SelectMany(x => x.BoundHeadsets)
                    .Select(x => new HeadsetChannelBinding(x.Index, x.BoundChannel));

            if (headsetIndex != null)
                bindings = bindings.Where(x => x.Index == headsetIndex);

            return bindings;
        }

        public void Dispose()
        {
            foreach (var binding in Bindings)
            {
                binding.OptionsChanged -= BindingPropertyChanged;
                binding.Dispose();
            }

            Bindings.Clear();
        }

        private ChannelBinding CreateBinding(ChannelBindingOptions options)
        {
            var voiceMeeterClient = _provider.GetRequiredService<VoiceMeeterClient>();
            var poller = _provider.GetRequiredService<HeadsetPoller>();
            poller.Bind();
            
            var binding = new ChannelBinding(poller, voiceMeeterClient, options);
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
