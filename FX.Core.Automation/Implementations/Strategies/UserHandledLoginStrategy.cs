using FX.Core.Automation.Abstract;
using FX.Core.Automation.Abstract.Strategies;
using FX.Core.Automation.Models;
using System.Threading.Tasks;

namespace FX.Core.Automation.Implementations.Strategies
{
    public class UserHandledLoginStrategy<P> : ILoginStrategy<P, LoginStrategyParameters>
        where P : ScraperParameters
    {
        public async Task DoLogin(IScraperWithLogin<P> scraper, LoginStrategyParameters parameters)
        {
            await scraper.NavigateToPage(parameters.LoginUrl);

            // wait for user to do login
            await scraper.WaitForElement(parameters.ElementSelectorForLoggedInPage);
            await scraper.WaitABit(1000);
        }
    }
}
