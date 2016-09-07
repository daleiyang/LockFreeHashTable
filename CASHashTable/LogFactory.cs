using System;
using System.IO;
using log4net;
using log4net.Config;

namespace CAS
{
    public static class LogFactory
    {
        public const string ConfigFileName = "log4net.config";

        public static ILog Configure()
        {
            var assemblyDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string path = Path.Combine(assemblyDirectory, ConfigFileName);
            FileInfo configFile = new FileInfo(path);
            XmlConfigurator.ConfigureAndWatch(configFile);
            return LogManager.GetLogger("CASHashTable.Logging");
        }
    }
}
