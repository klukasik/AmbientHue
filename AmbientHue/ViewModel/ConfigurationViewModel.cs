namespace AmbientHue.ViewModel
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;

    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;

    using Q42.HueApi;
    using Q42.HueApi.Interfaces;

    using Color = System.Drawing.Color;

    public partial class ConfigurationViewModel : ObservableObject, IConfigurationViewModel
    {
        private readonly IHueConfiguration hueConfiguration;

        private readonly ObservableCollection<string> bridges = new ObservableCollection<string>();

        public ObservableCollection<string> Bridges => this.bridges;

        private string selectedBridge;

        public string SelectedBridge
        {
            get
            {
                return this.selectedBridge;
            }
            set
            {
                this.hueConfiguration.IP = value;
                this.selectedBridge = value;

                this.AppKey = null;
                this.Lights.Clear();
                this.SelectedLight = null;

                OnPropertyChanged(nameof(SelectedBridge));
                RegisterCommand.NotifyCanExecuteChanged();
            }
        }

        private string appKey;

        public string AppKey
        {
            get
            {
                return this.appKey;
            }
            set
            {
                this.hueConfiguration.AppKey = value;
                this.appKey = value;

                OnPropertyChanged(nameof(AppKey));
                RegisterCommand.NotifyCanExecuteChanged();
            }
        }

        private readonly ObservableCollection<string> lights = new ObservableCollection<string>();
        public ObservableCollection<string> Lights => this.lights;

        private string selectedLight;
        public string SelectedLight
        {
            get
            {
                return this.selectedLight;
            }
            set
            {
                this.hueConfiguration.LightName = value;
                this.selectedLight = value;

                OnPropertyChanged(nameof(SelectedLight));
            }
        }

        private readonly ObservableCollection<string> captureMethods = new ObservableCollection<string>();
        public ObservableCollection<string> CaptureMethods => this.captureMethods;

        private string selectedCaptureMethod;
        public string SelectedCaptureMethod
        {
            get
            {
                return this.selectedCaptureMethod;
            }
            set
            {
                this.hueConfiguration.CaptureMethod = (CaptureMethod)Enum.Parse(typeof(CaptureMethod), value);
                this.selectedCaptureMethod = value;

                OnPropertyChanged(nameof(SelectedCaptureMethod));
            }
        }

        private SolidColorBrush background = new SolidColorBrush();
        public SolidColorBrush Background
        {
            get
            {
                return this.background;
            }
            set
            {
                this.background = value;
                OnPropertyChanged(nameof(Background));
            }
        }

        private Task locateTask;

        public ConfigurationViewModel(IHueConfiguration hueConfiguration)
        {
            this.hueConfiguration = hueConfiguration;

            var defaultBridge = this.hueConfiguration.IP;
            if (this.bridges.Contains(defaultBridge) == false)
            {
                this.bridges.Add(defaultBridge);
            }
            this.selectedBridge = defaultBridge;

            this.appKey = this.hueConfiguration.AppKey;

            this.LoadLights();

            var defaultLightName = this.hueConfiguration.LightName;
            if (this.lights.Contains(defaultLightName) == false)
            {
                this.lights.Add(defaultLightName);
            }
            this.selectedLight = defaultLightName;

            this.LocateCommand = new RelayCommand(
                    () =>
                    {
                        this.bridges.Clear();

                        var uiContext = TaskScheduler.FromCurrentSynchronizationContext();
                        this.locateTask = new Task(
                            async () =>
                                {
                                    IBridgeLocator locator = new HttpBridgeLocator();
                                    var newBridges = (await locator.LocateBridgesAsync(TimeSpan.FromSeconds(1))).Select(b => b.IpAddress).ToList();

                                    await Task.Factory.StartNew(() =>
                                    {
                                        newBridges.ForEach(bridge => this.bridges.Add(bridge));

                                        if (this.bridges.Contains(this.SelectedBridge) == false)
                                        {
                                            this.SelectedBridge = this.bridges.Count > 0 ? this.bridges[0] : null;
                                        }

                                        this.locateTask = null;
                                        this.LocateCommand.NotifyCanExecuteChanged();
                                    }, CancellationToken.None, TaskCreationOptions.None, uiContext);
                                });

                        this.locateTask.Start();
                        this.LocateCommand.NotifyCanExecuteChanged();
                    }, () => this.locateTask == null);

            this.RegisterCommand = new RelayCommand(
                    async () =>
                        {
                            try
                            {
                                this.ShowRegisterMessage = Visibility.Visible;

                                int nrOfTries = 1;
                                while (nrOfTries < 30)
                                {
                                    try
                                    {
                                        ILocalHueClient client = new LocalHueClient(this.selectedBridge);
                                        this.AppKey = await client.RegisterAsync("AmbientHue", "v0.1");
                                        break;
                                    }
                                    catch (Exception)
                                    {
                                        nrOfTries++;
                                        Thread.Sleep(500);
                                    }
                                }
                            }
                            finally
                            {
                                this.ShowRegisterMessage = Visibility.Hidden;
                            }

                            this.LoadLights();
                        },
                    () => string.IsNullOrEmpty(this.selectedBridge) == false && string.IsNullOrEmpty(this.appKey));

            Enum.GetNames(typeof(CaptureMethod))
                .ToList().ForEach(cm => this.captureMethods.Add(cm));

            this.selectedCaptureMethod = this.hueConfiguration.CaptureMethod.ToString();

            this.Color = Color.Blue;
        }

        public RelayCommand LocateCommand
        {
            get; set;
        }

        public RelayCommand RegisterCommand { get; set; }

        public Color Color
        {
            set
            {
                this.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(value.R, value.G, value.B));
            }
        }

        public ICommand HideWindowCommand
        {
            get
            {
                return new RelayCommand(
                    () =>
                    {
                        Application.Current.MainWindow.Hide();
                        Application.Current.MainWindow = null;
                    }
                );
            }
        }

        private string elapsedMsec;

        public string ElapsedMsec
        {
            get
            {
                return this.elapsedMsec;
            }
            set
            {
                this.elapsedMsec = value;
                OnPropertyChanged(nameof(ElapsedMsec));
            }
        }

        private Visibility showRegisterMessage = Visibility.Hidden;
        public Visibility ShowRegisterMessage
        {
            get
            {
                return this.showRegisterMessage;
            }
            set
            {
                this.showRegisterMessage = value;
                OnPropertyChanged(nameof(ShowRegisterMessage));
            }
        }

        private async void LoadLights()
        {
            this.lights.Clear();

            if (string.IsNullOrEmpty(this.selectedBridge) || string.IsNullOrEmpty(this.appKey))
            {
                return;
            }

            ILocalHueClient client = new LocalHueClient(this.selectedBridge);
            client.Initialize(this.appKey);

            (await client.GetLightsAsync()).ToList()
                .ForEach(light => this.lights.Add(light.Name));

            if (this.lights.Contains(this.SelectedLight) == false)
            {
                this.SelectedLight = this.lights.Count > 0 ? this.lights[0] : null;
            }
        }
    }
}