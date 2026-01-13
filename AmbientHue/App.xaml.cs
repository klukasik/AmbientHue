using System.Windows;

namespace AmbientHue
{
    using AmbientHue.ViewModel;

    using CommunityToolkit.Mvvm.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection;

    using Hardcodet.Wpf.TaskbarNotification;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private TaskbarIcon notifyIcon;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Configure dependency injection
            var services = new ServiceCollection();
            
            var hueConfiguration = new HueConfiguration();
            services.AddSingleton<IHueConfiguration>(hueConfiguration);
            services.AddSingleton<ConfigurationViewModel>();
            services.AddSingleton<NotifyIconViewModel>();
            
            Ioc.Default.ConfigureServices(services.BuildServiceProvider());

            //create the notifyicon (it's a resource declared in NotifyIconResources.xaml
            notifyIcon = (TaskbarIcon)FindResource("NotifyIcon");
        }

        protected override void OnExit(ExitEventArgs e)
        {
            notifyIcon.Dispose(); //the icon would clean up automatically, but this is cleaner
            base.OnExit(e);
        }
    }
}
