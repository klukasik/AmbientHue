namespace AmbientHue.ViewModel
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading;

    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Command;

    using Q42.HueApi;
    using Q42.HueApi.Interfaces;
    using Q42.HueApi.NET;

    public class ConfigurationViewModel : ViewModelBase
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

                this.RaisePropertyChanged(() => this.SelectedBridge);
                this.RegisterCommand.RaiseCanExecuteChanged();
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

                this.RaisePropertyChanged(() => this.AppKey);
                this.RegisterCommand.RaiseCanExecuteChanged();
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

                this.RaisePropertyChanged(() => this.SelectedLight);
            }
        }

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
                    async () =>
                    {
                        this.bridges.Clear();

                        IBridgeLocator locator = new SSDPBridgeLocator();
                        (await locator.LocateBridgesAsync(TimeSpan.FromSeconds(1))).ToList()
                            .ForEach(bridge => this.bridges.Add(bridge));

                        if (this.bridges.Contains(this.SelectedBridge) == false)
                        {
                            this.SelectedBridge = this.bridges.Count > 0 ? this.bridges[0] : null;
                        }
                    });

            this.RegisterCommand = new RelayCommand(
                    async () =>
                        {


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

                            this.LoadLights();
                    },
                    () => string.IsNullOrEmpty(this.selectedBridge) == false && string.IsNullOrEmpty(this.appKey));
        }

        public RelayCommand LocateCommand
        {
            get; set;
        }

        public RelayCommand RegisterCommand { get; set; }

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