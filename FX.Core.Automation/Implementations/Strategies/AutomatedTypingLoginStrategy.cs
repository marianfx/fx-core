using FX.Core.Automation.Abstract;
using FX.Core.Automation.Abstract.Strategies;
using FX.Core.Automation.Models;
using System.Threading.Tasks;

namespace FX.Core.Automation.Implementations.Strategies
{
    public class AutomatedTypingLoginStrategy<P> : ILoginStrategy<P, LoginStrategyParameters>
        where P : ScraperParameters
    {
        public async Task DoLogin(IScraperWithLogin<P> scraper, LoginStrategyParameters parameters)
        {
            await scraper.NavigateToPage(parameters.LoginUrl);

            // go to login and take refs
            await scraper.ExecuteClick(parameters.LoginButtonSelector);

            // input and click
            await scraper.TypeInInput(parameters.InputSelectorUser, parameters.InputTextUser);
            await scraper.TypeInInput(parameters.InputSelectorPassword, parameters.InputTextPassword);
            await scraper.ExecuteClick(parameters.ButtonSelectorExecuteLogin);
            await scraper.WaitABit(2000);
        }
    }
}
