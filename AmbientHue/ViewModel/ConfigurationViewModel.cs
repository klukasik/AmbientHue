namespace AmbientHue.ViewModel
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Input;

    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Command;

    using Q42.HueApi;
    using Q42.HueApi.Interfaces;
    using Q42.HueApi.NET;

    public class ConfigurationViewModel : ViewModelBase
    {
        private readonly IHueConfiguration hueConfiguration = new HueConfiguration();

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

                LoadLights();
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
            }
        }

        public ConfigurationViewModel()
        {
            var defaultBridge = this.hueConfiguration.IP;
            if (this.bridges.Contains(defaultBridge) == false)
            {
                this.bridges.Add(defaultBridge);
            }
            this.selectedBridge = defaultBridge;

            this.AppKey = this.hueConfiguration.AppKey;

            var defaultLightName = this.hueConfiguration.LightName;
            if (this.lights.Contains(defaultLightName) == false)
            {
                this.lights.Add(defaultLightName);
            }
            this.selectedLight = defaultLightName;
        }

        public ICommand LocateCommand
        {
            get
            {
                return new RelayCommand(
                    async () =>
                        {
                            this.bridges.Clear();

                            IBridgeLocator locator = new SSDPBridgeLocator();
                            (await locator.LocateBridgesAsync(TimeSpan.FromSeconds(10))).ToList()
                                .ForEach(bridge => this.bridges.Add(bridge));

                            if (this.bridges.Contains(this.SelectedBridge) == false)
                            {
                                if (this.bridges.Count > 0)
                                {
                                    this.SelectedBridge = this.bridges[0];
                                }
                                else
                                {
                                    this.SelectedBridge = null;
                                }
                            }

                            this.RaisePropertyChanged();
                        });
            }
        }

        public ICommand RegisterCommand
        {
            get
            {
                return new RelayCommand(
                    async () =>
                        {
                            ILocalHueClient client = new LocalHueClient(this.selectedBridge);
                            this.AppKey = await client.RegisterAsync("AmbientHue", "v0.1");
                        },
                    () => string.IsNullOrEmpty(this.selectedBridge) == false);
            }
        }

        private async void LoadLights()
        {
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
                if (this.lights.Count > 0)
                {
                    this.SelectedLight = this.lights[0];
                }
                else
                {
                    this.SelectedLight = null;
                }
            }

            this.RaisePropertyChanged();
        }
    }
}