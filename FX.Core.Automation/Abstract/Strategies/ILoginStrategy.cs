using FX.Core.Automation.Models;
using System.Threading.Tasks;

namespace FX.Core.Automation.Abstract.Strategies
{
    public interface ILoginStrategy<P, T>
        where P : ScraperParameters
        where T : LoginStrategyParametersBase
    {
        Task DoLogin(IScraperWithLogin<P> scraper, T parameters);
    }
}
