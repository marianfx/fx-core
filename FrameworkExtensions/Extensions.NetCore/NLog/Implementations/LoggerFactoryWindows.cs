using FX.Core.Config.Logging.NLog.Implementations;
using NLog;
using NLog.Config;
using NLog.Targets;
using NLog.Windows.Forms;
using System;
using System.Linq;
using System.Windows.Forms;

namespace FX.Core.Core3Extensions.NLog.Implementations
{
    public class LoggerFactoryWindows: LoggerFactory
    {
        /// <summary>
        /// Creates a logger instance that can write to rich textbox form controls
        /// </summary>
        /// <param name="config"></param>
        /// <param name="control"></param>
        /// <returns></returns>
        public static ILogger CreateRichTextboxLogger(string settingsPath, RichTextBox control)
        {
            Target.Register<RichTextBoxTarget>("RichTextBox");

            var config = LoadConfigFromFile(settingsPath);
            return CreateRichTextboxLogger(config, control);
        }

        /// <summary>
        /// Validates the configuration and the control
        /// </summary>
        /// <param name="config"></param>
        /// <param name="control"></param>
        protected static void Validate(LoggingConfiguration config, RichTextBox control)
        {
            Validate(config);

            if (control == null)
                throw new Exception("The rich text box must be specified");
        }

        /// <summary>
        /// Creates a logger instance that can write to rich textbox form controls
        /// </summary>
        /// <param name="config"></param>
        /// <param name="control"></param>
        /// <returns></returns>
        public static ILogger CreateRichTextboxLogger(LoggingConfiguration config, RichTextBox control)
        {
            Validate(config, control);

            var target = config.AllTargets.FirstOrDefault(x => x.Name == "logTextBox") as RichTextBoxTarget;

            if (target == null) // add target if not configured
            {
                target = new RichTextBoxTarget();
                config.AddTarget("logTextBox", target);

                // set names
                target.Name = "logTextBox";
                target.FormName = control.FindForm().Name;
                target.ControlName = control.Name;
            }

            config.AddRule(LogLevel.Trace, LogLevel.Fatal, target);

            LogManager.Configuration = config;
            return LogManager.GetCurrentClassLogger();
        }
    }
}
