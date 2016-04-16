using System.Windows;

namespace AmbientHue
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Linq;
    using System.Threading;
    using System.Windows.Forms;

    using Q42.HueApi;
    using Q42.HueApi.Interfaces;
    using Q42.HueApi.NET;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void btnLocate_Click(object sender, RoutedEventArgs e)
        {
            this.cmbBridge.Items.Clear();

            IBridgeLocator locator = new SSDPBridgeLocator();
            var bridges = await locator.LocateBridgesAsync(TimeSpan.FromSeconds(30));
            bridges.ToList().ForEach(bridge => this.cmbBridge.Items.Add(bridge));

            if (this.cmbBridge.Items.Count > 0)
            {
                this.cmbBridge.SelectedIndex = 0;
            }
        }

        private static async void Register()
        {
            ILocalHueClient client = new LocalHueClient("192.168.1.8");
            var appKey = await client.RegisterAsync("mypersonalappname", "mydevicename");

            Console.WriteLine(appKey);
        }

        public static async void StartCapture(CancellationTokenSource cancellationToken)
        {
            ILocalHueClient client = new LocalHueClient("192.168.1.8:80");
            string appKey = "";
            client.Initialize(appKey);

            var lights = await client.GetLightsAsync();
            string lightId = lights.First(l => l.Name == "Iris").Id;

            var turnOffCommand = new LightCommand();
            turnOffCommand.TurnOff();
            await client.SendCommandAsync(turnOffCommand);

            Rectangle bounds = Screen.GetBounds(Point.Empty);
            using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    while (cancellationToken.IsCancellationRequested == false)
                    {
                        graphics.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);

                        var command = CalculateAverage(bitmap, bounds);

                        await client.SendCommandAsync(command, new[] { lightId });
                    }
                }
            }
        }

        private static unsafe LightCommand CalculateAverage(Bitmap bitmap, Rectangle bounds)
        {
            BitmapData srcData = null;
            double g = 0;
            double r = 0;
            double b = 0;

            try
            {
                srcData = bitmap.LockBits(bounds, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                unsafe
                {
                    byte* srcPointer = (byte*)srcData.Scan0;

                    for (int i = 0; i < bounds.Height; i++)
                    {
                        for (int j = 0; j < bounds.Width; j++)
                        {
                            b += srcPointer[0]; // Blue
                            g += srcPointer[1]; // Green
                            r += srcPointer[2]; // Red

                            srcPointer += 4;
                        }
                    }
                }
            }
            finally
            {
                if (srcData != null)
                {
                    bitmap.UnlockBits(srcData);
                }
            }

            var command = new LightCommand();
            command.TurnOn();

            double nrOfPixels = bounds.Width * bounds.Height;
            r /= nrOfPixels;
            g /= nrOfPixels;
            b /= nrOfPixels;

            command.Brightness = 255;
            command.SetColor((int)r, (int)g, (int)b);

            Console.WriteLine("{0} {1} {2}", (int)r, (int)g, (int)b);

            return command;
        }
    }
}
