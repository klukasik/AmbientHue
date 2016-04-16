namespace AmbientHue
{
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
                ConfigurationManager.AppSettings["LightName"] = value;
            }
        }

        public bool IsCapturePossible => string.IsNullOrEmpty(this.IP) == false && string.IsNullOrEmpty(this.AppKey) == false
                                         && string.IsNullOrEmpty(this.LightName) == false;
    }
}