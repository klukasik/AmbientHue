using System.Windows;

namespace AmbientHue
{
    using AmbientHue.ViewModel;

    using GalaSoft.MvvmLight.Ioc;

    using Hardcodet.Wpf.TaskbarNotification;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private TaskbarIcon notifyIcon;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (ServiceLocator.IsLocationProviderSet == false)
            {
                ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

                var hueConfiguration = new HueConfiguration();
                SimpleIoc.Default.Register<IHueConfiguration>(() => hueConfiguration);
                SimpleIoc.Default.Register<ConfigurationViewModel>();
                SimpleIoc.Default.Register<NotifyIconViewModel>();
            }

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
