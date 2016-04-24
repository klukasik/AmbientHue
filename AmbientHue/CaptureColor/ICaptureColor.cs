namespace AmbientHue.CaptureColor
{
    using System.Drawing;

    public interface ICaptureColor
    {
        Color Capture(Bitmap bitmap, Rectangle bounds);
    }
}