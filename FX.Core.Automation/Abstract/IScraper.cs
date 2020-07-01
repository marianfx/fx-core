using FX.Core.Automation.Models;
using System;
using System.Threading.Tasks;

namespace FX.Core.Automation.Abstract
{
    public interface IScraper<P>: IDisposable
        where P : ScraperParameters
    {
        /// <summary>
        /// Initializes all the settings, instances and parameters that this instance requires.
        /// </summary>
        /// <param name="parameters">Holds various configuration parameters</param>
        Task Initialize(P parameters);
        
        /// <summary>
        /// Closes the open browser (shut down)
        /// </summary>
        /// <returns></returns>
        Task CloseBrowser();

        /// <summary>
        /// Triggers navigation to a page url; If force is specified, it ignores if the page is already there (otherwise, if already on the page, does nothing)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        Task NavigateToPage(string url, bool force = false);

        /// <summary>
        /// Wrapper around click method on the page
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        Task ExecuteClick(string selector);

        /// <summary>
        /// Finds an input by selector and inserts the given text in it (wrapper)
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        Task TypeInInput(string selector, string text);

        /// <summary>
        /// Makes the page wait for no reason
        /// </summary>
        /// <param name="milliseconds"></param>
        /// <returns></returns>
        Task WaitABit(int milliseconds);

        /// <summary>
        /// Returns the Scroll Height of the browser's view port
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        Task<double> GetCurrentScrollHeight(string selector);

        /// <summary>
        /// Gets the current location in the full scroll height, taking into account the full height of the scroll
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="addClientHeight"></param>
        /// <returns></returns>
        Task<double> GetCurrentScroll(string selector, bool addClientHeight = true);

        /// <summary>
        /// Scrolls to a certain location.
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="toHeight"></param>
        /// <param name="aproximate"></param>
        /// <returns></returns>
        Task ScrollTo(string selector, int toHeight, bool aproximate = true);

        /// <summary>
        /// Given a list of selectors identifying items / pages / modals in a page that usually have an X / Close button, tries  to close them all
        /// </summary>
        /// <param name="selectors"></param>
        /// <returns></returns>
        Task CloseAnnoyingElements(string[] selectors);
    }
}
