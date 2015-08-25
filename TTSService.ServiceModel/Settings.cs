using System;
using System.Configuration;

namespace TTSService.ServiceModel
{
    public static class Settings
    {
        public static void Init()
        {
            ServiceName = ConfigurationManager.AppSettings["serviceName"];
            ServiceDescription = ConfigurationManager.AppSettings["serviceDescription"];

            UrlService = ConfigurationManager.AppSettings["ServiceUrl"];
            DirectoryFile = ConfigurationManager.AppSettings["DirectoryFile"];

            SonosIp = ConfigurationManager.AppSettings["SonosIp"];
            SonosPort = Convert.ToInt32(ConfigurationManager.AppSettings["SonosPort"]);
        }

        public static String ServiceName { get; set; }
        public static String ServiceDescription { get; set; }
        public static String UrlService { get; set; }
        public static String DirectoryFile { get; set; }

        public static String SonosIp { get; set; }
        public static int SonosPort { get; set; }
    }
}
