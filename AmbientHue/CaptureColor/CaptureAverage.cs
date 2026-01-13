namespace AmbientHue.CaptureColor
{
    using System.Drawing;
    using System.Drawing.Imaging;

    using Q42.HueApi;

    class CaptureAverage : ICaptureColor
    {
        public unsafe Color Capture(Bitmap bitmap, Rectangle bounds)
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

            double nrOfPixels = bounds.Width * bounds.Height;
            r /= nrOfPixels;
            g /= nrOfPixels;
            b /= nrOfPixels;

            return Color.FromArgb(255, (int)r, (int)g, (int)b);
        }
    }
}
