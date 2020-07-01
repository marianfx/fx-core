using FX.Core.Automation.Abstract;
using FX.Core.Automation.Models;
using FX.Core.Automation.Settings;
using FX.Core.Storage.Serialization.Abstract;
using NLog;
using PuppeteerSharp;
using PuppeteerSharp.Input;
using System;
using System.Threading.Tasks;

namespace FX.Core.Automation.Implementations
{
    public class BaseScraper<P, S>: IScraper<P>
        where P: ScraperParameters
        where S: CoreScrapperSettings
    {
        protected P Parameters;
        protected ViewPortOptions ViewPortOptions;
        protected LaunchOptions LaunchOptions;
        protected TypeOptions TypeOptions;
        protected Browser Browser;
        protected Page CurrentPage;

        protected readonly ILogger Logger;
        protected readonly IScraperSettingsManager<S> SettingsManager;
        protected readonly IDataSerializer DataSerializer;

        public BaseScraper(ILogger logger,
            IScraperSettingsManager<S> settingsManager,
            IDataSerializer dataSerializer)
        {
            Logger = logger;
            SettingsManager = settingsManager;
            DataSerializer = dataSerializer;
        }

        /// <summary>
        /// Initializes: Settings Manager with settings loaded, saves the parameters and initializes the browser instance
        /// </summary>
        /// <param name="parameters"></param>
        public virtual async Task Initialize(P parameters)
        {
            // initialize settings
            await SettingsManager.LoadSettings();
            var appSettings = SettingsManager.Settings;

            if (appSettings == null)
                throw new Exception("Cannot instantiate scraper without settings loaded.");

            // initialize parameters
            Parameters = parameters;

            // initialize browser
            // prepare launch options
            LaunchOptions = new LaunchOptions() { Headless = appSettings.Headless };
            ViewPortOptions = new ViewPortOptions() { Width = appSettings.WindowWidth, Height = appSettings.WindowHeight };
            TypeOptions = new TypeOptions() { Delay = appSettings.TYPE_DELAY };

            // get chromium reference (not async, downloads it, blocks everything while downloading)
            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);
            Browser = await Puppeteer.LaunchAsync(LaunchOptions);
            CurrentPage = await Browser.NewPageAsync();
            await CurrentPage.SetViewportAsync(ViewPortOptions);
        }

        public virtual async Task CloseBrowser()
        {
            await Browser.CloseAsync();
            Browser = null;
        }

        public virtual async Task NavigateToPage(string url, bool force = false)
        {
            var appSettings = SettingsManager.Settings;
            if (force || CurrentPage.Url != appSettings.AppUrl)
                await CurrentPage.GoToAsync(appSettings.AppUrl);
        }

        public virtual async Task ExecuteClick(string selector)
        {
            await CurrentPage.ClickAsync(selector);
        }

        public virtual async Task TypeInInput(string selector, string text)
        {
            var emailInput = await CurrentPage.WaitForSelectorAsync(selector);
            if (emailInput == null)
                throw new Exception(string.Format("Input with selector {0} not found", selector));

            var appSettings = SettingsManager.Settings;
            await emailInput.TypeAsync(text, TypeOptions);
            await CurrentPage.WaitForTimeoutAsync(appSettings.TIME_TO_WAIT);
        }

        public virtual async Task WaitABit(int milliseconds)
        {
            await CurrentPage.WaitForTimeoutAsync(milliseconds);
        }

        public virtual async Task<double> GetCurrentScrollHeight(string selector)
        {
            var chSelectorFunc = ScraperConstants.FUNC_SELECT_SIMPLE.Replace("{{SPECIAL}}", ".scrollHeight");
            return await CurrentPage.EvaluateFunctionAsync<double>(chSelectorFunc, selector);
        }

        public virtual async Task<double> GetCurrentScroll(string selector, bool addClientHeight = true)
        {
            var chSelectorFunc = ScraperConstants.FUNC_SELECT_SIMPLE.Replace("{{SPECIAL}}", ".scrollTop");
            var scrollTop = await CurrentPage.EvaluateFunctionAsync<double>(chSelectorFunc, selector);

            if (addClientHeight)
            {
                var clHeightFunc = ScraperConstants.FUNC_SELECT_SIMPLE.Replace("{{SPECIAL}}", ".clientHeight");
                var clientHeight = await CurrentPage.EvaluateFunctionAsync<double>(clHeightFunc, selector);
                scrollTop += clientHeight;
            }

            return scrollTop;
        }

        public virtual async Task ScrollTo(string selector, int toHeight, bool aproximate = true)
        {
            var newHeight = toHeight;
            if (aproximate)
                newHeight -= 100;

            var scrollBottom = ScraperConstants.FUNC_SELECT_SIMPLE.Replace("{{SPECIAL}}", ".scrollTo(0, " + newHeight + ")");
            await CurrentPage.EvaluateFunctionAsync(scrollBottom, selector);
        }

        public virtual async Task CloseAnnoyingElements(string[] selectors)
        {
            var stillSearching = true;
            while (stillSearching)
            {
                stillSearching = false;
                foreach (var toCloseSelector in selectors)
                {
                    var toCLose = await CurrentPage.QuerySelectorAsync(toCloseSelector);
                    if (toCLose != null)
                    {
                        await toCLose.ClickAsync();
                        stillSearching = true; // when one closed, others might appear
                    }
                }
            }
        }


        public virtual void Dispose()
        {
            Task.Run(async () => await CloseBrowser())
                .ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}
