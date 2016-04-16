using System.Windows;

namespace AmbientHue
{
    using AmbientHue.ViewModel;

    using Q42.HueApi;
    using Q42.HueApi.Interfaces;

    /// <summary>
    /// Interaction logic for ConfigurationWindow.xaml
    /// </summary>
    public partial class ConfigurationWindow : Window
    {
        public ConfigurationWindow()
        {
            InitializeComponent();
            this.DataContext = new ConfigurationViewModel();
        }
    }
}
