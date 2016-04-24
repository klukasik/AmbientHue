namespace AmbientHue.CaptureColor
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Linq;

    public class ProminentColor : ICaptureColor
    {
        public unsafe Color Capture(Bitmap bitmap, Rectangle bounds)
        {
            Func<byte, byte, byte, double> callback = (r, g, b) => 1;
            //callback = (r, g, b) =>
            //    {
            //        if (r > 245 && g > 245 && b > 245) return 0;
            //        return (r * r + g * g + b * b) / 65535 * 20 + 1;
            //    };
            callback = (r,g,b) => (Math.Abs(r - g) * Math.Abs(r - g) + Math.Abs(r - b) * Math.Abs(r - b) + Math.Abs(g - b) * Math.Abs(g - b)) / 65535 * 50 + 1;

            Dictionary<Color, Pixel> pixels = new Dictionary<Color, Pixel>();
            BitmapData srcData = null;
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
                            byte b = srcPointer[0]; // Blue
                            byte g = srcPointer[1]; // Green
                            byte r = srcPointer[2]; // Red

                            Color color = Color.FromArgb(r, g, b);
                            Pixel pixel;
                            if (pixels.TryGetValue(color, out pixel))
                            {
                                pixel.Count++;
                            }
                            else
                            {
                                double weight = callback(r, g, b);
                                if (weight <= 0)
                                    weight = 1e-10;

                                pixels.Add(color, new Pixel()
                                {
                                    Count = 1,
                                    Weight = weight
                                });
                            }
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

            double maxWeightCount = pixels.Max(el => el.Value.Weight * el.Value.Count);
            return pixels.First(el => el.Value.Weight * el.Value.Count == maxWeightCount).Key;

            Color rgb = Color.Empty;

            rgb = this.GetMostProminentRGB(pixels, 6, rgb);
            rgb = this.GetMostProminentRGB(pixels, 4, rgb);
            rgb = this.GetMostProminentRGB(pixels, 2, rgb);
            rgb = this.GetMostProminentRGB(pixels, 0, rgb);

            return rgb;
        }

        private unsafe Color GetMostProminentRGB(Dictionary<Color, Pixel> pixels, byte degrade, Color rgbMatch)
        {
            Dictionary<Color, Pixel> db = new Dictionary<Color, Pixel>();
            foreach (var pixel in pixels)
            {
                double totalWeight = pixel.Value.Count * pixel.Value.Weight;
                Color dbColor = Color.FromArgb(pixel.Key.R >> degrade, pixel.Key.G >> degrade, pixel.Key.B >> degrade);
                if (this.DoesRgbMatch(rgbMatch, dbColor))
                {
                    Pixel dbPixel;
                    if (db.TryGetValue(dbColor, out dbPixel)) dbPixel.Weight += totalWeight;
                    else db[dbColor] = new Pixel() { Weight = totalWeight };
                }
            }

            if (db.Count == 0)
            {
                return Color.Black;
            }

            double maxWeigth = db.Max(el => el.Value.Weight);
            return db.First(el => el.Value.Weight == maxWeigth).Key;
        }

        private bool DoesRgbMatch(Color rgb, Color newColor)
        {
            if (rgb == Color.Empty)
            {
                return true;
            }

            return rgb.R == newColor.R && rgb.G == newColor.G && rgb.B == newColor.B;
        }
    }

    public struct Pixel
    {
        public int Count;

        public double Weight;
    }
}