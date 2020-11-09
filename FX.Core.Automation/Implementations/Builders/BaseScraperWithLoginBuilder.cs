using Fx.Core.Crypto.Abstract;
using Fx.Core.Crypto.Implementations;
using Fx.Core.Crypto.Settings.Settings;
using FX.Core.Automation.Abstract.Strategies;
using FX.Core.Automation.Implementations.Strategies;
using FX.Core.Automation.Models;
using FX.Core.Automation.Settings;
using FX.Core.Crypto.Settings.Abstract;
using FX.Core.Crypto.Settings.Implementations;
using Microsoft.Extensions.Options;

namespace FX.Core.Automation.Implementations.Builders
{
    public abstract class BaseScraperWithLoginBuilder<P, S, T, X>: BaseScraperBuilder<P, S, X>
        where P : ScraperParameters
        where S : CoreScrapperSettings, new()
        where T : LoginStrategyParametersBase
        where X : BaseScrapperWithLogin<P, S, T>, new()
    {
        public IEncryptedSettingsManager<LoginInfoSettings> UserSettingsManager { get; set; }

        public BaseScraperWithLoginBuilder(): base()
        {
        }

        public virtual BaseScraperWithLoginBuilder<P, S, T, X> WithBasicEncryptedUserSettingsManager(IEncryptor encryptor = null, string settingsConfigPath = "appsettings.json")
        {
            if (this.Scraper.DataSerializer == null)
                throw new System.Exception("Data Serializer for the scraper must be initialized before initializing settings");

            encryptor = encryptor ?? new AesHmacEncryptor();

            var settingsConfig = new BasicEncryptedSettingsConfig
            {
                Path = settingsConfigPath,
                DataSerializer = this.Scraper.DataSerializer,
                Encryptor = encryptor
            };

            this.UserSettingsManager = new BasicEncryptedSettingsManager<LoginInfoSettings>(Options.Create(settingsConfig));
            return this;
        }

        public virtual BaseScraperWithLoginBuilder<P, S, T, X> WithSpecializedUserSettingsManager(IEncryptedSettingsManager<LoginInfoSettings> userSettingsManager)
        {
            this.UserSettingsManager = userSettingsManager;
            return this;
        }

        public virtual BaseScraperWithLoginBuilder<P, S, T, X> WithLoginStrategy(LoginStrategyTypes loginStrategy)
        {
            switch (loginStrategy)
            {
                case LoginStrategyTypes.AutomatedTyping:
                    this.Scraper.LoginStrategy = (ILoginStrategy<P, T>)new AutomatedTypingLoginStrategy<ScraperParameters>();
                    break;
                case LoginStrategyTypes.UserHandled:
                    this.Scraper.LoginStrategy = (ILoginStrategy < P, T > )new UserHandledLoginStrategy<ScraperParameters>();
                    break;
                default:
                    throw new System.Exception($"Login strategy {loginStrategy} not found");
            }
            return this;
        }
    }
}
