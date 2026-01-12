namespace AmbientHue.ViewModel
{
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;

    using CommunityToolkit.Mvvm.Input;
    using CommunityToolkit.Mvvm.DependencyInjection;

    /// <summary>
    /// Provides bindable properties and commands for the NotifyIcon. In this sample, the
    /// view model is assigned to the NotifyIcon in XAML. Alternatively, the startup routing
    /// in App.xaml.cs could have created this view model, and assigned it to the NotifyIcon.
    /// </summary>
    public class NotifyIconViewModel
    {
        private readonly IHueConfiguration hueConfiguration;
        private CancellationTokenSource cancellationToken;
        private Task captureTask;

        private readonly ConfigurationViewModel configurationViewModel;

        public NotifyIconViewModel()
        {
            this.hueConfiguration = new HueConfiguration();
            this.configurationViewModel = Ioc.Default.GetService<ConfigurationViewModel>();
        }

        /// <summary>
        /// Shows a window, if none is already open.
        /// </summary>
        public ICommand ShowWindowCommand
        {
            get
            {
                return new RelayCommand(
                    () =>
                    {
                        Application.Current.MainWindow = new ConfigurationWindow();
                        Application.Current.MainWindow.Show();
                    },
                    () => Application.Current.MainWindow == null
                );
            }
        }

        public ICommand StartAmbientCaptureCommand
        {
            get
            {
                return new RelayCommand(
                    () =>
                        {
                            this.cancellationToken = new CancellationTokenSource();
                            var uiContext = TaskScheduler.FromCurrentSynchronizationContext();
                            this.captureTask = new Task(() => new AmbientCapture().StartCapture(this.hueConfiguration,
                                (color, elapsedMsec) =>
                                    {
                                        Task.Factory.StartNew(() =>
                                        {
                                            this.configurationViewModel.Color = color;
                                            this.configurationViewModel.ElapsedMsec = $"{elapsedMsec} msec";
                                        }, CancellationToken.None, TaskCreationOptions.None, uiContext);
                                    }, this.cancellationToken));
                            this.captureTask.Start();
                        },
                    () => this.captureTask == null && this.hueConfiguration.IsCapturePossible
                );
            }
        }

        public ICommand StopAmbientCaptureCommand
        {
            get
            {
                return new RelayCommand(
                    () =>
                        {
                            this.cancellationToken.Cancel();
                            this.cancellationToken = null;
                            this.captureTask = null;
                        },
                    () => this.captureTask != null
                );
            }
        }

        /// <summary>
        /// Hides the main window. This command is only enabled if a window is open.
        /// </summary>
        public ICommand HideWindowCommand
        {
            get
            {
                return new RelayCommand(
                    () =>
                        {
                            Application.Current.MainWindow.Hide();
                            Application.Current.MainWindow = null;
                        },
                    () => Application.Current.MainWindow != null
                );
            }
        }


        /// <summary>
        /// Shuts down the application.
        /// </summary>
        public ICommand ExitApplicationCommand
        {
            get
            {
                return new RelayCommand(() => Application.Current.Shutdown());
            }
        }
    }
}
