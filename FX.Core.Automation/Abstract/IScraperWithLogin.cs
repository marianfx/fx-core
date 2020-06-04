using FX.Core.Automation.Models;
using System.Threading.Tasks;

namespace FX.Core.Automation.Abstract
{
    public interface IScraperWithLogin<P>: IScraper<P>
        where P : ScraperParameters
    {
        /// <summary>
        /// Does all the possible steps to make sure that the user is logged in.
        /// For example: checks if it is already logged in, runs the login workflow etc.
        /// Throws exception if cannot proceed.
        /// </summary>
        /// <returns></returns>
        Task DoLoginWorkflowAsync();

        /// <summary>
        /// Runs the login steps (from zero, does not care of previous information).
        /// For more complex steps, use 'DoLoginWorkflowAsync'
        /// </summary>
        /// <returns></returns>
        Task LoginAsync();

        /// <summary>
        /// Returns true if the user is already authenticated on the website
        /// </summary>
        /// <returns></returns>
        Task<bool> IsLoggedIn();
    }
}
