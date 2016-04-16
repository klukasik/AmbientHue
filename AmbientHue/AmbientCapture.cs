namespace AmbientHue
{
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Linq;
    using System.Threading;
    using System.Windows.Forms;

    using Q42.HueApi;
    using Q42.HueApi.Interfaces;

    public class AmbientCapture
    {
        public async void StartCapture(IHueConfiguration hueConfiguration, CancellationTokenSource cancellationToken)
        {
            ILocalHueClient client = new LocalHueClient(hueConfiguration.IP);
            client.Initialize(hueConfiguration.AppKey);

            var lights = await client.GetLightsAsync();
            string lightId = lights.First(l => l.Name == hueConfiguration.LightName).Id;

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

            return command;
        }
    }
}