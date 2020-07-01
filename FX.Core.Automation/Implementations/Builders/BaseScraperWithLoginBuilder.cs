using FX.Core.Automation.Abstract.Strategies;
using FX.Core.Automation.Implementations.Strategies;
using FX.Core.Automation.Models;
using FX.Core.Automation.Settings;

namespace FX.Core.Automation.Implementations.Builders
{
    public abstract class BaseScraperWithLoginBuilder<P, S, T, X>: BaseScraperBuilder<P, S, X>
        where P : ScraperParameters
        where S : CoreScrapperSettings
        where T : LoginStrategyParametersBase
        where X : BaseScrapperWithLogin<P, S, T>
    {
        public ILoginStrategy<P, T> LoginStrategy { get; set; }


        public virtual BaseScraperWithLoginBuilder<P, S, T, X> AddLoginStrategy(LoginStrategyTypes loginStrategy)
        {
            switch (loginStrategy)
            {
                case LoginStrategyTypes.AutomatedTyping:
                    LoginStrategy = (ILoginStrategy<P, T>)new AutomatedTypingLoginStrategy<ScraperParameters>();
                    break;
                case LoginStrategyTypes.UserHandled:
                    LoginStrategy = (ILoginStrategy < P, T > )new UserHandledLoginStrategy<ScraperParameters>();
                    break;
                default:
                    break;
            }
            return this;
        }
    }
}
