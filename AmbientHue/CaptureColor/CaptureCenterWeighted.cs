namespace AmbientHue.CaptureColor
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Captures color with emphasis on the center of the screen where content is usually focused.
    /// Uses weighted averaging with higher weights for pixels closer to center.
    /// </summary>
    public class CaptureCenterWeighted : ICaptureColor
    {
        private const int SampleStride = 6; // Sample every 6th pixel for performance

        public unsafe Color Capture(Bitmap bitmap, Rectangle bounds)
        {
            BitmapData srcData = null;
            double g = 0;
            double r = 0;
            double b = 0;
            double totalWeight = 0;

            int centerX = bounds.Width / 2;
            int centerY = bounds.Height / 2;
            double maxDistance = Math.Sqrt(centerX * centerX + centerY * centerY);

            try
            {
                srcData = bitmap.LockBits(bounds, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                unsafe
                {
                    byte* srcPointer = (byte*)srcData.Scan0;
                    int stride = srcData.Stride;

                    for (int i = 0; i < bounds.Height; i += SampleStride)
                    {
                        byte* rowPointer = srcPointer + (i * stride);
                        
                        for (int j = 0; j < bounds.Width; j += SampleStride)
                        {
                            // Calculate distance from center
                            double dx = j - centerX;
                            double dy = i - centerY;
                            double distance = Math.Sqrt(dx * dx + dy * dy);
                            
                            // Weight decreases with distance from center (inverse weighting)
                            // Center pixels have weight ~1.0, edges have weight ~0.2
                            double weight = 1.0 - (distance / maxDistance * 0.8);
                            
                            int pixelOffset = j * 4;
                            b += rowPointer[pixelOffset] * weight;     // Blue
                            g += rowPointer[pixelOffset + 1] * weight; // Green
                            r += rowPointer[pixelOffset + 2] * weight; // Red
                            totalWeight += weight;
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

            if (totalWeight > 0)
            {
                r /= totalWeight;
                g /= totalWeight;
                b /= totalWeight;
            }

            return Color.FromArgb(0, (int)r, (int)g, (int)b);
        }
    }
}
