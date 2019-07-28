using System;
using System.Configuration;

namespace ProcessMonitor
{
    public static class Configuration
    {
        public static string ProcessNameToMonitor => ConfigurationManager.AppSettings["ProcessNameToMonitor"];
        public static TimeSpan SamplingInterval => TimeSpan.Parse(ConfigurationManager.AppSettings["SamplingInterval"]);
        public static string AppKey => ConfigurationManager.AppSettings["AppKey"];
    }
}
