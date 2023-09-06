using ArctisVoiceMeeter.Model;
using ArctisVoiceMeeter.ViewModels;
using Awesome.Net.WritableOptions;
using CommunityToolkit.Mvvm.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Authentication.ExtendedProtection;
using System.Text;
using System.Threading.Tasks;

namespace ArctisVoiceMeeter
{
    public class ChannelBindingService
    {
        private readonly IServiceProvider _provider;
        private readonly IWritableOptions<ArctisVoiceMeeterPresets> _config;

        public ObservableCollection<KeyValuePair<string, ArctisVoiceMeeterChannelBinding>> Bindings { get; }

        //private IReadOnlyDictionary<string, ArctisVoiceMeeterChannelBindingOptions> _bindings;


        public ChannelBindingService(IServiceProvider provider, IWritableOptions<ArctisVoiceMeeterPresets> config)
        {
            _provider = provider;
            _config = config;
            Bindings = new ObservableCollection<KeyValuePair<string, ArctisVoiceMeeterChannelBinding>>(
                config.Value.ToDictionary(x => x.Key, x => CreateBinding(x.Key, x.Value))
            );
        }

        private ArctisVoiceMeeterChannelBinding CreateBinding(string name, ArctisVoiceMeeterChannelBindingOptions options)
        {
            var binding = new ArctisVoiceMeeterChannelBinding(
                _provider.GetRequiredService<HeadsetPoller>(),
                _provider.GetRequiredService<VoiceMeeterClient>(),
                options
                );

            return binding;
        }

        private void AddBinding(string name, ArctisVoiceMeeterChannelBindingOptions options) { 
            Bindings.Add(KeyValuePair.Create(name, CreateBinding(name, options)));
        }

        public void AddBindingAndSave(string name, ArctisVoiceMeeterChannelBindingOptions options)
        {
            _config.Update(x => x.Add(name, options));
            AddBinding(name, options);
        }

        public void RemoveBinding(string name)
        {
            _config.Update(x => x.Remove(name));
        }
    }
}
