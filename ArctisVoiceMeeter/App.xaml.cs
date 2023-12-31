﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ArctisVoiceMeeter.Infrastructure;
using ArctisVoiceMeeter.Model;
using ArctisVoiceMeeter.ViewModels;
using ArctisVoiceMeeter.Views;
using Awesome.Net.WritableOptions;
using Awesome.Net.WritableOptions.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NetworkCopy.Utilities;

namespace ArctisVoiceMeeter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IHost _host;
        private Mutex _singleInstanceMutex;
        private const string SingleInstanceMutexValue = "ArctisVoiceMeeterSingleInstanceMutex";

        public App()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices(ConfigureServices)
                .Build();
        }

        private void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            services.AddSingleton<StartupManager>();

            services.AddSingleton<ArctisClient>();
            services.AddSingleton<HeadsetPoller>();

            services.AddSingleton<VoiceMeeterClient>();

            services.ConfigureWritableOptions<ArctisVoiceMeeterPresets>((IConfigurationRoot)context.Configuration, "Presets");

            services.AddTransient<HeadsetStatusListViewModel>();
            services.AddTransient<ChannelBindingListViewModel>();
            services.AddSingleton<ChannelBindingService>();

            services.AddSingleton<MainViewModel>();
            services.AddSingleton<MainWindow>();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            EnsureSingleInstance();

            using var scope = _host.Services.CreateScope();
            
            var mainWindow = scope.ServiceProvider.GetRequiredService<MainWindow>();
            
            mainWindow.Show();

            if (e.Args.FirstOrDefault() == "--minimized")
                mainWindow.WindowState = WindowState.Minimized;
        }

        private async void App_OnExit(object sender, ExitEventArgs e)
        {
            await _host.StopAsync();
            _singleInstanceMutex.Dispose();
        }

        private void EnsureSingleInstance()
        {
            _singleInstanceMutex = new Mutex(true, SingleInstanceMutexValue, out bool isNewInstance);
            if (!isNewInstance)
            {
                MessageBox.Show("You can only run a single instance of this application");
                Shutdown();
            }
        }
    }
}
