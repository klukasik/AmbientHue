using System.Windows;
using System.Windows.Input;
using System.Text.RegularExpressions;

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
        }

        /// <summary>
        /// Validates that only numeric input is allowed in the frame delay textbox
        /// </summary>
        private void txtFrameDelay_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Only allow digits
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
