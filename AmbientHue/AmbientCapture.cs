namespace AmbientHue
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Linq;
    using System.Threading;
    using System.Windows.Forms;

    using AForge.Imaging.ColorReduction;

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

                        LightCommand command;
                        switch (hueConfiguration.CaptureMethod)
                        {
                            case CaptureMethod.Quantize:
                                command = Quantize(bitmap, bounds);
                                break;

                            case CaptureMethod.Dominant:
                                command = CalculateDominant(bitmap, bounds);
                                break;

                            default:
                                command = CalculateAverage(bitmap, bounds);
                                break;
                        }

                        await client.SendCommandAsync(command, new[] { lightId });
                    }
                }
            }
        }

        private static unsafe LightCommand CalculateDominant(Bitmap bitmap, Rectangle bounds)
        {
            BitmapData srcData = null;
            const int HueRange = 360;
            int[] hues = new int[HueRange];
            double[] saturations = new double[HueRange];
            double[] brightnesses = new double[HueRange];

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
                            var color = Color.FromArgb(255, srcPointer[2], // Red
                                srcPointer[1], // Green
                                srcPointer[0]); // Blue

                            double hue, saturation, brightness;
                            ColorToHSV(color, out hue, out saturation, out brightness);

                            hues[(int)hue]++;
                            saturations[(int)hue] += saturation;
                            brightnesses[(int)hue] += brightness;

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

            // Find the most common hue.
            int hueIndex = 0;
            int hueCount = hues[hueIndex];
            for (int i = 1; i < hues.Length; i++)
            {
                if (hues[i] > hueCount)
                {
                    hueCount = hues[i];
                    hueIndex = i;
                }
            }

            var dominantColor = ColorFromHSV(
                hueIndex,
                saturations[hueIndex] / hues[hueIndex],
                brightnesses[hueIndex] / hues[hueIndex]);

            var command = new LightCommand();
            command.TurnOn();

            command.Brightness = 255;
            command.SetColor(dominantColor.R, dominantColor.G, dominantColor.B);

            Console.WriteLine("{0} {1} {2}", dominantColor.R, dominantColor.G, dominantColor.B);

            return command;
        }

        public static void ColorToHSV(Color color, out double hue, out double saturation, out double value)
        {
            int max = Math.Max(color.R, Math.Max(color.G, color.B));
            int min = Math.Min(color.R, Math.Min(color.G, color.B));

            hue = color.GetHue();
            saturation = (max == 0) ? 0 : 1d - (1d * min / max);
            value = max / 255d;
        }

        public static Color ColorFromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            int v = Convert.ToInt32(value);
            int p = Convert.ToInt32(value * (1 - saturation));
            int q = Convert.ToInt32(value * (1 - f * saturation));
            int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            if (hi == 0)
                return Color.FromArgb(255, v, t, p);
            else if (hi == 1)
                return Color.FromArgb(255, q, v, p);
            else if (hi == 2)
                return Color.FromArgb(255, p, v, t);
            else if (hi == 3)
                return Color.FromArgb(255, p, q, v);
            else if (hi == 4)
                return Color.FromArgb(255, t, p, v);
            else
                return Color.FromArgb(255, v, p, q);
        }

        public static LightCommand Quantize(Bitmap bitmap, Rectangle bounds)
        {
            // create the color quantization algorithm
            IColorQuantizer quantizer = new MedianCutQuantizer();
            var imageQuantizer = new ColorImageQuantizer(quantizer);
            Color[] palette = imageQuantizer.CalculatePalette(bitmap, 1);

            var command = new LightCommand();
            command.TurnOn();

            command.Brightness = 255;
            command.SetColor(palette[0].R, palette[0].G, palette[0].B);

            return command;
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