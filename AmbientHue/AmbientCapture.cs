namespace AmbientHue
{
    using System;
    using System.Drawing;
    using System.Linq;
    using System.Threading;
    using System.Windows.Forms;

    using AmbientHue.CaptureColor;

    using Q42.HueApi;
    using Q42.HueApi.Interfaces;

    public class AmbientCapture
    {
        public async void StartCapture(IHueConfiguration hueConfiguration, Action<Color, long> setStatusAction, CancellationTokenSource cancellationToken)
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
                        var watch = System.Diagnostics.Stopwatch.StartNew();
                        graphics.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);

                        ICaptureColor captureColor;
                        switch (hueConfiguration.CaptureMethod)
                        {
                            case CaptureMethod.Quantize:
                                captureColor = new Quantize();
                                break;

                            case CaptureMethod.Dominant:
                                captureColor = new CaptureDominant();
                                break;

                            case CaptureMethod.Prominent:
                                captureColor = new ProminentColor();
                                break;

                            default:
                                captureColor = new CaptureAverage();
                                break;
                        }

                        Color color = captureColor.Capture(bitmap, bounds);

                        LightCommand command = new LightCommand();
                        command.TurnOn();
                        command.Brightness = (byte)((color.R + color.G + color.B) / 3);
                        command.SetColor(color.R, color.G, color.B);

                        await client.SendCommandAsync(command, new[] { lightId });

                        watch.Stop();
                        var elapsedMs = watch.ElapsedMilliseconds;

                        setStatusAction(color, elapsedMs);
                    }
                }
            }
        }
    }
}