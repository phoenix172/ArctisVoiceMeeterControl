using ArctisVoiceMeeter.Model;
using Awesome.Net.WritableOptions;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
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

        //private IReadOnlyDictionary<string, ArctisVoiceMeeterChannelBindingOptions> _bindings;


        public ChannelBindingService(IServiceProvider provider, IWritableOptions<ArctisVoiceMeeterPresets> config)
        {
            _provider = provider;
            _config = config;
        }

        public ArctisVoiceMeeterChannelBinding CreateBinding(string name, ArctisVoiceMeeterChannelBindingOptions options)
        {
            var binding = new ArctisVoiceMeeterChannelBinding(
                _provider.GetRequiredService<HeadsetPoller>(),
                _provider.GetRequiredService<VoiceMeeterClient>(),
                options
                );

            return binding;
        }

        public ArctisVoiceMeeterChannelBinding CreateBindingAndSave(string name, ArctisVoiceMeeterChannelBindingOptions options)
        {
            _config.Update(x => x.Add(name, options));
            return CreateBinding(name, options);
        }

        public void RemoveBinding(string name)
        {
            _config.Update(x => x.Remove(name));
        }
    }
}
