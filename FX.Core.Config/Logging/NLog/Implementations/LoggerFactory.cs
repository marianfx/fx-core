using NLog;
using NLog.Config;
using System.Linq;
using NL = NLog;

namespace FX.Core.Config.Logging.NLog.Implementations
{
    public class LoggerFactory
    {
        /// <summary>
        /// Reads the config file for NLog from an XML file (use if you don't want the default config load).
        /// </summary>
        /// <param name="settingsPath"></param>
        /// <returns></returns>
        public static LoggingConfiguration LoadConfigFromFile(string settingsPath)
        {
            return new XmlLoggingConfiguration(settingsPath);
        }


        /// <summary>
        /// Returns a console logger. Must be initialized with the path to the configuration file .
        /// The configuration can be read:
        /// - manually from the config file (with LoadConfigFromFile)
        /// - initialized programatically 
        /// - provided from LogManager.Configuration if in the application directory there's a nlog.config file
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static ILogger CreateConsoleLogger(string settingsPath)
        {
            var config = LoadConfigFromFile(settingsPath);
            return CreateConsoleLogger(config);
        }

        /// <summary>
        /// Validates and saves the configuration loaded
        /// </summary>
        /// <param name="config"></param>
        protected static void Validate(LoggingConfiguration config)
        {
            if (LogManager.Configuration == null)
                throw new System.Exception("NLog initialized with invalid config");
        }

        /// <summary>
        /// Returns a console logger. Must be initialized with the configuration.
        /// The configuration can be read:
        /// - manually from the config file (with LoadConfigFromFile)
        /// - initialized programatically 
        /// - provided from LogManager.Configuration if in the application directory there's a nlog.config file
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static ILogger CreateConsoleLogger(LoggingConfiguration config)
        {
            Validate(config);

            var target = config.AllTargets.FirstOrDefault(x => x.Name == "logConsole") as NL.Targets.ConsoleTarget;

            if (target == null) // add console target if not configured
            {
                target = new NL.Targets.ConsoleTarget("logConsole");
            }
            config.AddRule(LogLevel.Trace, LogLevel.Fatal, target);

            LogManager.Configuration = config;
            return LogManager.GetCurrentClassLogger();
        } 
    }
}
