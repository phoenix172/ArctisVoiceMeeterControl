using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ArctisVoiceMeeter.Infrastructure;
using ArctisVoiceMeeter.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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

            var presetsConfiguration = context.Configuration.GetSection("Presets");
            ArctisVoiceMeeterPresets? presets = null;
            if (presetsConfiguration.Value == null)
            {
                presets = new ArctisVoiceMeeterPresets();
                services.ConfigureWritable<ArctisVoiceMeeterPresets>(presets);
                services.Configure<>()
            }


            services.ConfigureWritable<ArctisVoiceMeeterPresets>();

            services.AddSingleton<MainWindow>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            using var scope = _host.Services.CreateScope();
            var mainWindow = scope.ServiceProvider.GetService<MainWindow>();

            var presets = scope.ServiceProvider.GetRequiredService<ArctisVoiceMeeterPresets>();

            mainWindow.Show();
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await _host.StopAsync();
            base.OnExit(e);
        }
    }
}
