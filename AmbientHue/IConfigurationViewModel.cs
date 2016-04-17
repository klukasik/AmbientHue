namespace AmbientHue
{
    using System.Drawing;
    using System.Windows.Media;

    using Color = System.Drawing.Color;

    public interface IConfigurationViewModel
    {
        Color Color { set; } 
    }
}