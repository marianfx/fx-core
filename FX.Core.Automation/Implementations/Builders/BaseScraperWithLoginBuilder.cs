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
        public ILoginStrategy<P, T> LoginStrategy { get; set; }
        public IEncryptedSettingsManager<LoginInfoSettings> UserSettingsManager { get; set; }

        public BaseScraperWithLoginBuilder(): base()
        {
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
