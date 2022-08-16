﻿using NLog;
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
            var creationLog = new NLog.Targets.FileTarget("Instances creation") { FileName = Path.Combine(InitialData.WorkingDirectory, "creation.log") };
            
            // Rules for mapping loggers to targets            
            logConfig.AddRule(LogLevel.Info, LogLevel.Fatal, creationLog);

            // Apply config           
            NLog.LogManager.Configuration = logConfig;
        }

        public static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
    }
}
