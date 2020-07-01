using FX.Core.Automation.Models;
using FX.Core.Automation.Settings;
using FX.Core.Config.Logging.NLog.Implementations;
using FX.Core.Storage.Serialization.Abstract;
using FX.Core.Storage.Serialization.Implementations;
using FX.Core.Storage.Serialization.Settings;
using Microsoft.Extensions.Options;
using NLog;
using System.IO;

namespace FX.Core.Automation.Implementations.Builders
{
    public abstract class BaseScraperBuilder<P, S, X>
        where P : ScraperParameters
        where S : CoreScrapperSettings
        where X : BaseScraper<P, S>
    {
        public ILogger Logger { get; set; }
        public IDataSerializer DataSaver { get; set; }


        public virtual BaseScraperBuilder<P, S, X> AddConsoleLogger(string nlogConfigFile = "nlog.config")
        {
            var settingsFile = Path.Combine(Directory.GetCurrentDirectory(), nlogConfigFile);
            Logger = LoggerFactory.CreateConsoleLogger(settingsFile);
            return this;
        }

        public virtual BaseScraperBuilder<P, S, X> AddExternalLogger(ILogger logger)
        {
            Logger = logger;
            return this;
        }

        public virtual BaseScraperBuilder<P, S, X> AddDataSaver()
        {
            var dataSaverSettings = new JsonSerializationSettings
            {
                RootPath = Directory.GetCurrentDirectory(),
                UseIdDirectlyAsName = true
            };
            DataSaver = new JsonDataSerializer(Options.Create(dataSaverSettings));
            return this;
        }

        public abstract X BuildScraper();
    }
}
