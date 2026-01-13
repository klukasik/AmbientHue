namespace AmbientHue.CaptureColor
{
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Fast capture method that samples pixels from the edges/periphery of the screen.
    /// Useful for ambient lighting that focuses on screen borders rather than content.
    /// </summary>
    public class CaptureEdgeSampling : ICaptureColor
    {
        private const int EdgeThickness = 50; // Pixels from edge to sample
        private const int SampleStride = 4;   // Sample every 4th pixel

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

                byte* srcPointer = (byte*)srcData.Scan0;
                int stride = srcData.Stride;

                // Sample top edge
                for (int i = 0; i < EdgeThickness && i < bounds.Height; i += SampleStride)
                {
                    byte* rowPointer = srcPointer + (i * stride);
                    for (int j = 0; j < bounds.Width; j += SampleStride)
                    {
                        int pixelOffset = j * 4;
                        b += rowPointer[pixelOffset];
                        g += rowPointer[pixelOffset + 1];
                        r += rowPointer[pixelOffset + 2];
                        sampleCount++;
                    }
                }

                // Sample bottom edge (if screen is tall enough)
                if (bounds.Height > EdgeThickness)
                {
                    for (int i = bounds.Height - EdgeThickness; i < bounds.Height; i += SampleStride)
                    {
                        byte* rowPointer = srcPointer + (i * stride);
                        for (int j = 0; j < bounds.Width; j += SampleStride)
                        {
                            int pixelOffset = j * 4;
                            b += rowPointer[pixelOffset];
                            g += rowPointer[pixelOffset + 1];
                            r += rowPointer[pixelOffset + 2];
                            sampleCount++;
                        }
                    }
                }

                // Sample left edge (excluding corners already sampled)
                for (int i = EdgeThickness; i < bounds.Height - EdgeThickness; i += SampleStride)
                {
                    byte* rowPointer = srcPointer + (i * stride);
                    for (int j = 0; j < EdgeThickness && j < bounds.Width; j += SampleStride)
                    {
                        int pixelOffset = j * 4;
                        b += rowPointer[pixelOffset];
                        g += rowPointer[pixelOffset + 1];
                        r += rowPointer[pixelOffset + 2];
                        sampleCount++;
                    }
                }

                // Sample right edge (excluding corners already sampled, if screen is wide enough)
                if (bounds.Width > EdgeThickness)
                {
                    for (int i = EdgeThickness; i < bounds.Height - EdgeThickness; i += SampleStride)
                    {
                        byte* rowPointer = srcPointer + (i * stride);
                        for (int j = bounds.Width - EdgeThickness; j < bounds.Width; j += SampleStride)
                        {
                            int pixelOffset = j * 4;
                            b += rowPointer[pixelOffset];
                            g += rowPointer[pixelOffset + 1];
                            r += rowPointer[pixelOffset + 2];
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

            return Color.FromArgb(255, (int)r, (int)g, (int)b);
        }
    }
}
