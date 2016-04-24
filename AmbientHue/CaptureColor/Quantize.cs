namespace AmbientHue.CaptureColor
{
    using System.Drawing;

    using AForge.Imaging.ColorReduction;

    public class Quantize : ICaptureColor
    {
        public Color Capture(Bitmap bitmap, Rectangle bounds)
        {
            // create the color quantization algorithm
            IColorQuantizer quantizer = new MedianCutQuantizer();
            var imageQuantizer = new ColorImageQuantizer(quantizer);
            const int NrOfColors = 4;
            Color[] palette = imageQuantizer.CalculatePalette(bitmap, NrOfColors);

            for (int i = 0; i < NrOfColors; i++)
            {
                var color = palette[i];
                if ((color.R < 240 && color.G < 240 && color.B < 240 &&
                     color.R > 15 && color.G > 15 && color.B > 15) ||
                    i == NrOfColors - 1)
                {
                    return color;
                }
            }

            return Color.Black;
        }

    }
}