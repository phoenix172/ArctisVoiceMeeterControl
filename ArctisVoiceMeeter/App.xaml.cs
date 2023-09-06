﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ArctisVoiceMeeter.Infrastructure;
using ArctisVoiceMeeter.Model;
using ArctisVoiceMeeter.ViewModels;
using Awesome.Net.WritableOptions;
using Awesome.Net.WritableOptions.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace ArctisVoiceMeeter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IHost _host;

        public App()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration(Configure)
                .ConfigureServices(ConfigureServices)
                .Build();
        }

        private void Configure(IConfigurationBuilder x)
        {
            x.AddJsonFile("settings.json", optional: true)
                .Build();
        }


        private void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            services.AddSingleton<ArctisClient>();
            services.AddSingleton<HeadsetPoller>();

            services.AddSingleton<VoiceMeeterClient>();

            services.ConfigureWritableOptions<ArctisVoiceMeeterPresets>((IConfigurationRoot) context.Configuration, "Presets");

            services.AddTransient<HeadsetViewModel>();

            services.AddSingleton<MainViewModel>();
            services.AddSingleton<MainWindow>();
        }

        private static void InitializePresets(IServiceScope scope)
        {
            var presets = scope.ServiceProvider.GetService<IWritableOptions<ArctisVoiceMeeterPresets>>();

            if (presets?.Value.Count == 0)
                presets.Update(x => x.Add("Preset", new ArctisVoiceMeeterChannelBindingOptions
                {
                    BoundStrip = 0,
                    VoiceMeeterMaxVolume = 0,
                    VoiceMeeterMinVolume = 0,
                    VoiceMeeterVolume = 0
                }));
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            using var scope = _host.Services.CreateScope();

            InitializePresets(scope);

            var mainWindow = scope.ServiceProvider.GetService<MainWindow>();

            var presets = scope.ServiceProvider.GetRequiredService<IOptions<ArctisVoiceMeeterPresets>>();

            mainWindow.Show();
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await _host.StopAsync();
            base.OnExit(e);
        }
    }
}
