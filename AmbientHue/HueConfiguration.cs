namespace AmbientHue
{
    using System;
    using System.Configuration;

    public class HueConfiguration : IHueConfiguration
    {
        public string IP
        {
            get
            {
                return ConfigurationManager.AppSettings["IP"];
            }
            set
            {
                this.Save("IP", value);
                ConfigurationManager.AppSettings["IP"] = value;
            }
        }

        public string AppKey
        {
            get
            {
                return ConfigurationManager.AppSettings["AppKey"];
            }
            set
            {
                this.Save("AppKey", value);
                ConfigurationManager.AppSettings["AppKey"] = value;
            }

        }

        public string LightName
        {
            get
            {
                return ConfigurationManager.AppSettings["LightName"];
            }
            set
            {
                this.Save("LightName", value);
                ConfigurationManager.AppSettings["LightName"] = value;
            }
        }

        public CaptureMethod CaptureMethod
        {
            get
            {
                string captureMethod = ConfigurationManager.AppSettings["CaptureMethod"];
                if (string.IsNullOrEmpty(captureMethod))
                {
                    return CaptureMethod.Average;
                }

                return (CaptureMethod)Enum.Parse(typeof(CaptureMethod), captureMethod);
            }
            set
            {
                this.Save("CaptureMethod", value.ToString());
                ConfigurationManager.AppSettings["CaptureMethod"] = value.ToString();
            }
        }

        public int SelectedScreenIndex
        {
            get
            {
                string screenIndex = ConfigurationManager.AppSettings["SelectedScreenIndex"];
                if (string.IsNullOrEmpty(screenIndex) || !int.TryParse(screenIndex, out int index))
                {
                    return 0; // Default to primary screen
                }

                return index;
            }
            set
            {
                this.Save("SelectedScreenIndex", value.ToString());
                ConfigurationManager.AppSettings["SelectedScreenIndex"] = value.ToString();
            }
        }

        public int FrameDelayMs
        {
            get
            {
                string frameDelay = ConfigurationManager.AppSettings["FrameDelayMs"];
                if (string.IsNullOrEmpty(frameDelay) || !int.TryParse(frameDelay, out int delay))
                {
                    return 100; // Default to 100ms (~10 fps)
                }

                return delay;
            }
            set
            {
                this.Save("FrameDelayMs", value.ToString());
                ConfigurationManager.AppSettings["FrameDelayMs"] = value.ToString();
            }
        }

        public bool IsCapturePossible => string.IsNullOrEmpty(this.IP) == false && string.IsNullOrEmpty(this.AppKey) == false
                                         && string.IsNullOrEmpty(this.LightName) == false;

        private void Save(string key, string value)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings[key].Value = value;
            config.Save(ConfigurationSaveMode.Modified);

            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}