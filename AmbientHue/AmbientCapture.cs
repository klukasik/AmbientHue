namespace AmbientHue
{
    using System;
    using System.Drawing;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    using AmbientHue.CaptureColor;

    using Q42.HueApi;
    using Q42.HueApi.ColorConverters;
    using Q42.HueApi.ColorConverters.HSB;
    using Q42.HueApi.Interfaces;

    public class AmbientCapture
    {
        private const int DefaultFrameDelayMs = 100; // ~10 fps by default to reduce CPU load
        private const int DownsampleFactor = 4; // Reduce resolution by 4x for faster processing

        public async void StartCapture(IHueConfiguration hueConfiguration, Action<Color, long> setStatusAction, CancellationTokenSource cancellationToken)
        {
            ILocalHueClient client = new LocalHueClient(hueConfiguration.IP);
            client.Initialize(hueConfiguration.AppKey);

            var lights = await client.GetLightsAsync();
            string lightId = lights.First(l => l.Name == hueConfiguration.LightName).Id;

            Rectangle bounds = Screen.GetBounds(Point.Empty);
            
            // Create downsampled bitmap for faster processing
            // Ensure minimum dimensions of 1x1
            int downsampledWidth = Math.Max(1, bounds.Width / DownsampleFactor);
            int downsampledHeight = Math.Max(1, bounds.Height / DownsampleFactor);
            Rectangle downsampledBounds = new Rectangle(0, 0, downsampledWidth, downsampledHeight);
            
            using (Bitmap fullBitmap = new Bitmap(bounds.Width, bounds.Height))
            using (Bitmap downsampledBitmap = new Bitmap(downsampledWidth, downsampledHeight))
            {
                using (Graphics fullGraphics = Graphics.FromImage(fullBitmap))
                using (Graphics downsampledGraphics = Graphics.FromImage(downsampledBitmap))
                {
                    // Create capture method instance once outside the loop
                    ICaptureColor captureColor = CreateCaptureMethod(hueConfiguration.CaptureMethod);
                    
                    while (cancellationToken.IsCancellationRequested == false)
                    {
                        var watch = System.Diagnostics.Stopwatch.StartNew();
                        
                        // Capture full screen
                        fullGraphics.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
                        
                        // Downsample to smaller bitmap for faster color processing
                        downsampledGraphics.DrawImage(fullBitmap, 0, 0, downsampledWidth, downsampledHeight);

                        // Process downsampled image
                        Color color = captureColor.Capture(downsampledBitmap, downsampledBounds);

                        var rgbColor = new RGBColor(color.R / 255.0, color.G / 255.0, color.B / 255.0);
                        
                        LightCommand command = new LightCommand();
                        command.TurnOn();
                        command.Brightness = (byte)((color.R + color.G + color.B) / 3);
                        command.SetColor(rgbColor);

                        await client.SendCommandAsync(command, new[] { lightId });

                        watch.Stop();
                        var elapsedMs = watch.ElapsedMilliseconds;

                        setStatusAction(color, elapsedMs);
                        
                        // Add delay to prevent 100% CPU usage and limit update rate
                        int delayMs = Math.Max(0, DefaultFrameDelayMs - (int)elapsedMs);
                        if (delayMs > 0)
                        {
                            await Task.Delay(delayMs, cancellationToken.Token);
                        }
                    }
                }
            }
        }
        
        private ICaptureColor CreateCaptureMethod(CaptureMethod method)
        {
            switch (method)
            {
                case CaptureMethod.Quantize:
                    return new Quantize();

                case CaptureMethod.Dominant:
                    return new CaptureDominant();

                case CaptureMethod.Prominent:
                    return new ProminentColor();

                case CaptureMethod.GridSampling:
                    return new CaptureGridSampling();

                case CaptureMethod.CenterWeighted:
                    return new CaptureCenterWeighted();

                case CaptureMethod.EdgeSampling:
                    return new CaptureEdgeSampling();

                default:
                    return new CaptureAverage();
            }
        }
    }
}