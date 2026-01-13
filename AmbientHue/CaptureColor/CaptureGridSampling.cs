namespace AmbientHue.CaptureColor
{
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Fast color capture method that samples pixels at regular grid intervals
    /// instead of processing every pixel. Much faster than full-screen methods.
    /// </summary>
    public class CaptureGridSampling : ICaptureColor
    {
        private const int SampleStride = 8; // Sample every 8th pixel for speed

        public unsafe Color Capture(Bitmap bitmap, Rectangle bounds)
        {
            BitmapData srcData = null;
            double g = 0;
            double r = 0;
            double b = 0;
            int sampleCount = 0;

            try
            {
                srcData = bitmap.LockBits(bounds, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                unsafe
                {
                    byte* srcPointer = (byte*)srcData.Scan0;
                    int stride = srcData.Stride;

                    // Sample pixels at grid intervals for much faster processing
                    for (int i = 0; i < bounds.Height; i += SampleStride)
                    {
                        byte* rowPointer = srcPointer + (i * stride);
                        
                        for (int j = 0; j < bounds.Width; j += SampleStride)
                        {
                            int pixelOffset = j * 4;
                            b += rowPointer[pixelOffset];     // Blue
                            g += rowPointer[pixelOffset + 1]; // Green
                            r += rowPointer[pixelOffset + 2]; // Red
                            sampleCount++;
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

            if (sampleCount > 0)
            {
                r /= sampleCount;
                g /= sampleCount;
                b /= sampleCount;
            }

            return Color.FromArgb(0, (int)r, (int)g, (int)b);
        }
    }
}
