using FX.Core.Automation.Models;
using FX.Core.Automation.Settings;
using FX.Core.Common.Settings.Implementations;
using FX.Core.Config.Logging.NLog.Implementations;
using FX.Core.Config.Settings.Settings;
using FX.Core.Storage.Serialization.Implementations;
using FX.Core.Storage.Serialization.Settings;
using Microsoft.Extensions.Options;
using NLog;
using System.IO;

namespace FX.Core.Automation.Implementations.Builders
{
    public abstract class BaseScraperBuilder<P, S, X>
        where P : ScraperParameters
        where S : CoreScrapperSettings, new()
        where X : BaseScraper<P, S>, new()
    {
        public X Scraper { get; set; }

        public BaseScraperBuilder()
        {
            this.Scraper = new X();
        }


        public virtual BaseScraperBuilder<P, S, X> WithConsoleLogger(string nlogConfigFile = "nlog.config")
        {
            var settingsFile = Path.Combine(Directory.GetCurrentDirectory(), nlogConfigFile);
            var logger = LoggerFactory.CreateConsoleLogger(settingsFile);
            this.Scraper.Logger = logger;
            return this;
        }

        public virtual BaseScraperBuilder<P, S, X> WithExternalLogger(ILogger logger)
        {
            this.Scraper.Logger = logger;
            return this;
        }

        public virtual BaseScraperBuilder<P, S, X> WithDataSaver()
        {
            var dataSaverSettings = new JsonSerializationSettings
            {
                RootPath = Directory.GetCurrentDirectory(),
                UseIdDirectlyAsName = true
            };
            var dataSaver = new JsonDataSerializer(Options.Create(dataSaverSettings));
            this.Scraper.DataSerializer = dataSaver;
            return this;
        }

        public virtual BaseScraperBuilder<P, S, X> WithScraperSettingsManager(string settingsConfigPath = "coresettings.json")
        {
            if (this.Scraper.DataSerializer == null)
                throw new System.Exception("Data Serializer for the scraper must be initialized before initializing settings");

            var settingsConfig = new BasicSettingsConfig(this.Scraper.DataSerializer) { Path = settingsConfigPath };
            this.Scraper.SettingsManager = new BasicSettingsManager<S>(Options.Create(settingsConfig));
            return this;
        }

        public virtual X Build() => this.Scraper;
    }
}
