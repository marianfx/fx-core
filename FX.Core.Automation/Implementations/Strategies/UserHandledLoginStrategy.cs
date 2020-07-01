using FX.Core.Automation.Abstract;
using FX.Core.Automation.Abstract.Strategies;
using FX.Core.Automation.Models;
using System.Threading.Tasks;

namespace FX.Core.Automation.Implementations.Strategies
{
    public class UserHandledLoginStrategy<P> : ILoginStrategy<P, LoginStrategyParametersBase>
        where P : ScraperParameters
    {
        public async Task DoLogin(IScraperWithLogin<P> scraper, LoginStrategyParametersBase parameters)
        {
        }
    }
}
