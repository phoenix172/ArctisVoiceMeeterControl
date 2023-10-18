using ArctisVoiceMeeter.Model;
using ArctisVoiceMeeter.ViewModels;
using Awesome.Net.WritableOptions;
using CommunityToolkit.Mvvm.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Security.Authentication.ExtendedProtection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Xml.Linq;

namespace ArctisVoiceMeeter
{
    public class ChannelBindingService : IDisposable
    {
        private readonly IServiceProvider _provider;
        private readonly IWritableOptions<ArctisVoiceMeeterPresets> _presets;

        public ChannelBindingService(IServiceProvider provider, IWritableOptions<ArctisVoiceMeeterPresets> presets)
        {
            _provider = provider;
            _presets = presets;
            Bindings = CreateBindingsCollection();
        }

        public ObservableCollection<ArctisVoiceMeeterChannelBinding> Bindings { get; }

        public void SetBinding(ArctisVoiceMeeterChannelBindingOptions options)
        {
            if(BindingExists(options.BindingName))
                ChangeBinding(options);
            else
                AddBinding(options);
        }

        public void AddBinding(ArctisVoiceMeeterChannelBindingOptions options)
        {
            if (_presets.Value.Any(x => x.BindingName == options.BindingName))
                throw new ArgumentException("A Channel Binding with this name already exists.", nameof(options.BindingName));

            _presets.Update(x => x.Add(options));
            
            Bindings.Add(CreateBinding(options));
        }

        public void ChangeBinding(ArctisVoiceMeeterChannelBindingOptions binding)
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

        public ArctisVoiceMeeterChannelBinding? GetBinding(string name) => Bindings.FirstOrDefault(x=>x.BindingName == name);

        public bool BindingExists(string name) => GetBinding(name) != null;

        public bool RemoveBinding(string name)
        {
            bool result = false;

            _presets.Update(presets =>
            {
                var bindingOption = presets.FirstOrDefault(x => x.BindingName == name);
                if (bindingOption == null) return;
                presets.Remove(bindingOption);
            });

            if (!result) return false;

            var binding = GetBinding(name);
            if (binding != null)
                Bindings.Remove(binding);

            return true;
        }

        public void Dispose()
        {
            ClearBindings();
        }

        private ObservableCollection<ArctisVoiceMeeterChannelBinding> CreateBindingsCollection()
        {
            ArctisVoiceMeeterPresets options = _presets.Value;
            var bindings = options.Select(CreateBinding);

            return new ObservableCollection<ArctisVoiceMeeterChannelBinding>(bindings);
        }

        private ArctisVoiceMeeterChannelBinding CreateBinding(ArctisVoiceMeeterChannelBindingOptions options)
        {
            var voiceMeeterClient = _provider.GetRequiredService<VoiceMeeterClient>();
            var poller = _provider.GetRequiredService<HeadsetPoller>();
            poller.Bind();
            
            var binding = new ArctisVoiceMeeterChannelBinding(poller, voiceMeeterClient, options);
            binding.OptionsChanged += BindingPropertyChanged;
            return binding;
        }

        private void ClearBindings()
        {
            if (Bindings == null) return;

            foreach (var binding in Bindings)
            {
                binding.OptionsChanged -= BindingPropertyChanged;
                binding.Dispose();
            }

            Bindings.Clear();
        }

        private void BindingPropertyChanged(object? sender, ArctisVoiceMeeterChannelBindingOptions options)
        {
            ChangeBinding(options);
        }
    }
}
