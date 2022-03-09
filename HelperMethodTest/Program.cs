using NLog;
using System;
using System.Collections.Generic;

namespace HelperMethodTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Initialize();

            var payload = new Dictionary<string, object>() {
                { "name", null }
            };
            var name = payload.GetProperty("name", false, false, false);
            Console.WriteLine(name);
        }

        private static void Initialize()
        {
            var config = new NLog.Config.LoggingConfiguration();
            // Targets where to log to: File and Console
            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = "file.txt" };
            var logconsole = new NLog.Targets.ConsoleTarget("logconsole");
            // Rules for mapping loggers to targets
            //config.AddRule(LogLevel.Info, LogLevel.Fatal, logconsole);
            //config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);
            // Apply config
            LogManager.Configuration = config;
        }
    }
}
