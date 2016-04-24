namespace AmbientHue.CaptureColor
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class CaptureDominant : ICaptureColor
    {
        public unsafe Color Capture(Bitmap bitmap, Rectangle bounds)
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
                            var color = Color.FromArgb(srcPointer[2], // Red
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
                1,//saturations[hueIndex] / hues[hueIndex],
                1//brightnesses[hueIndex] / hues[hueIndex]
                );


            return dominantColor;
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
    }
}