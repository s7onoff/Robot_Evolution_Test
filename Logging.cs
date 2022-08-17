using NLog;
using System;
using System.IO;

namespace Robot_Evolution
{
    public static class Logging
    {
        public static void SetLoggingConfiguration()
        {
            var logConfig = new NLog.Config.LoggingConfiguration();

            // Targets where to log to: File and Console
            var creationLog = new NLog.Targets.FileTarget("Instances creation")
            {
                FileName = Path.Combine(InitialData.WorkingDirectory, "creation.log")
            };

            var debugLog = new NLog.Targets.FileTarget("Debug info")
            {
                FileName = Path.Combine(InitialData.WorkingDirectory, "debug.log")
            };

            var logconsole = new NLog.Targets.ConsoleTarget("logconsole");

            // Rules for mapping loggers to targets            
            logConfig.AddRule(LogLevel.Info, LogLevel.Fatal, creationLog);
            logConfig.AddRule(LogLevel.Info, LogLevel.Fatal, logconsole);
            logConfig.AddRule(LogLevel.Debug, LogLevel.Info, debugLog);

            // Apply config           
            NLog.LogManager.Configuration = logConfig;
        }

        public static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
    }
}
