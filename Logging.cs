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

            var generationsLog = new NLog.Targets.FileTarget("Generations info")
            {
                FileName = Path.Combine(InitialData.WorkingDirectory, "generations.log")
            };

            var robotLog = new NLog.Targets.FileTarget("robot logging")
            {
                FileName = Path.Combine(InitialData.WorkingDirectory, "robot.log")
            };

            var debugLog = new NLog.Targets.FileTarget("Debug info")
            {
                FileName = Path.Combine(InitialData.WorkingDirectory, "debug.log")
            };

            var crossoverLog = new NLog.Targets.FileTarget("Crossover info")
            {
                FileName = Path.Combine(InitialData.WorkingDirectory, "crossover.log")
            };

            var logconsole = new NLog.Targets.ConsoleTarget("logconsole");

            // Rules for mapping loggers to targets            
            logConfig.AddRule(LogLevel.Info, LogLevel.Fatal, generationsLog, "Main");
            logConfig.AddRule(LogLevel.Info, LogLevel.Fatal, creationLog, "Main");
            logConfig.AddRule(LogLevel.Info, LogLevel.Fatal, logconsole, "Main");
            //logConfig.AddRule(LogLevel.Debug, LogLevel.Fatal, debugLog, "Main");

            logConfig.AddRule(LogLevel.Info, LogLevel.Fatal, generationsLog, "Generation");
            logConfig.AddRule(LogLevel.Info, LogLevel.Fatal, creationLog, "Generation");
            logConfig.AddRule(LogLevel.Info, LogLevel.Fatal, logconsole, "Generation");
            //logConfig.AddRule(LogLevel.Debug, LogLevel.Fatal, debugLog, "Generation");

            logConfig.AddRule(LogLevel.Info, LogLevel.Fatal, creationLog, "Creation");
            //logConfig.AddRule(LogLevel.Debug, LogLevel.Fatal, debugLog, "Creation");

            logConfig.AddRule(LogLevel.Info, LogLevel.Fatal, crossoverLog, "Crossover");
            //logConfig.AddRule(LogLevel.Debug, LogLevel.Fatal, crossoverLog, "Crossover");
            //logConfig.AddRule(LogLevel.Debug, LogLevel.Fatal, debugLog, "Crossover");

            logConfig.AddRule(LogLevel.Info, LogLevel.Fatal, robotLog, "Robot");
            //logConfig.AddRule(LogLevel.Debug, LogLevel.Fatal, robotLog, "Robot*");
            //logConfig.AddRule(LogLevel.Debug, LogLevel.Fatal, debugLog, "Robot*");

            // Apply config           
            NLog.LogManager.Configuration = logConfig;
        }
    }
}
