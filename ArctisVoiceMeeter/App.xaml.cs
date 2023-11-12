using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
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
                .ConfigureServices(ConfigureServices)
                .Build();
        }

        private void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
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

        protected override async void OnExit(ExitEventArgs e)
        {
            await _host.StopAsync();
            base.OnExit(e);
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            using var scope = _host.Services.CreateScope();
            
            var mainWindow = scope.ServiceProvider.GetService<MainWindow>();
            
            mainWindow.Show();
        }
    }
}
