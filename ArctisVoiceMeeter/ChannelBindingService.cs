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

namespace ArctisVoiceMeeter
{
    public class ChannelBindingService : IDisposable
    {
        private readonly IServiceProvider _provider;
        private readonly IWritableOptions<ArctisVoiceMeeterPresets> _presets;

        public ObservableCollection<ArctisVoiceMeeterChannelBinding> Bindings { get; private set; }

        //private IReadOnlyDictionary<string, ArctisVoiceMeeterChannelBindingOptions> _bindings;


        public ChannelBindingService(IServiceProvider provider, IWritableOptions<ArctisVoiceMeeterPresets> presets)
        {
            _provider = provider;
            _presets = presets;
            LoadBindings();
        }

        private void LoadBindings()
        {
            ArctisVoiceMeeterPresets options = _presets.Value;
            var bindings = options.Select(CreateBinding);

            ClearBindings();
            Bindings = new ObservableCollection<ArctisVoiceMeeterChannelBinding>(bindings);
        }

        private void ClearBindings()
        {
            if (Bindings == null) return;

            foreach (var binding in Bindings)
            {
                binding.PropertyChanged -= BindingPropertyChanged;
                binding.Dispose();
            }

            Bindings.Clear();
        }

        private void BindingPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            _presets.Update(null);
        }

        private ArctisVoiceMeeterChannelBinding CreateBinding(ArctisVoiceMeeterChannelBindingOptions options)
        {
            var voiceMeeterClient = _provider.GetRequiredService<VoiceMeeterClient>();
            var poller = _provider.GetRequiredService<HeadsetPoller>();
            poller.Bind();
            
            var binding = new ArctisVoiceMeeterChannelBinding(poller, voiceMeeterClient, options);
            binding.PropertyChanged += BindingPropertyChanged;
            return binding;
        }

        public void AddBinding(ArctisVoiceMeeterChannelBindingOptions options)
        {
            if (_presets.Value.Any(x => x.BindingName == options.BindingName))
                throw new ArgumentException("A binding with this name already exists.", nameof(options.BindingName));

            _presets.Update(x =>
                x.Add(options));

            LoadBindings();
        }

        public void RemoveBinding(string name)
        {
            var bindingOption = _presets.Value.FirstOrDefault(x => x.BindingName == name);
            if (bindingOption == null) return;

            _presets.Update(x => x.Remove(bindingOption));
            LoadBindings();
        }

        public void Dispose()
        {
            ClearBindings();
        }
    }
}
