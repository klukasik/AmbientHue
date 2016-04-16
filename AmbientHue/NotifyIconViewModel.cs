using System.Windows;
using System.Windows.Input;

namespace AmbientHue
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides bindable properties and commands for the NotifyIcon. In this sample, the
    /// view model is assigned to the NotifyIcon in XAML. Alternatively, the startup routing
    /// in App.xaml.cs could have created this view model, and assigned it to the NotifyIcon.
    /// </summary>
    public class NotifyIconViewModel
    {
        private CancellationTokenSource cancellationToken;
        private Task captureTask;

        /// <summary>
        /// Shows a window, if none is already open.
        /// </summary>
        public ICommand ShowWindowCommand
        {
            get
            {
                return new DelegateCommand
                {
                    CanExecuteFunc = () => Application.Current.MainWindow == null,
                    CommandAction = () =>
                    {
                        Application.Current.MainWindow = new MainWindow();
                        Application.Current.MainWindow.Show();
                    }
                };
            }
        }

        public ICommand StartAmbientCaptureCommand
        {
            get
            {
                return new DelegateCommand
                {
                    CommandAction = () =>
                        {
                            this.cancellationToken = new CancellationTokenSource();
                            this.captureTask = new Task(() => MainWindow.StartCapture(this.cancellationToken));
                            this.captureTask.Start();
                        },
                    CanExecuteFunc = () => this.captureTask == null
                };
            }
        }

        public ICommand StopAmbientCaptureCommand
        {
            get
            {
                return new DelegateCommand
                {
                    CommandAction = () =>
                        {
                            this.cancellationToken.Cancel();
                            this.cancellationToken = null;
                            this.captureTask = null;
                        },
                    CanExecuteFunc = () => this.captureTask != null
                };
            }
        }

        /// <summary>
        /// Hides the main window. This command is only enabled if a window is open.
        /// </summary>
        public ICommand HideWindowCommand
        {
            get
            {
                return new DelegateCommand
                {
                    CommandAction = () => Application.Current.MainWindow.Close(),
                    CanExecuteFunc = () => Application.Current.MainWindow != null
                };
            }
        }


        /// <summary>
        /// Shuts down the application.
        /// </summary>
        public ICommand ExitApplicationCommand
        {
            get
            {
                return new DelegateCommand { CommandAction = () => Application.Current.Shutdown() };
            }
        }
    }
}
