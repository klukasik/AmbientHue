﻿namespace AmbientHue
{
    public interface IHueConfiguration
    {
        string IP { get; set; }
        string AppKey { get; set; }
        string LightName { get; set; }
        CaptureMethod CaptureMethod { get; set; }
        bool IsCapturePossible { get; }
    }
}