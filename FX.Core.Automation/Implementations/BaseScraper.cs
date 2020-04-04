using FX.Core.Automation.Abstract;
using FX.Core.Automation.Models;
using FX.Core.Automation.Settings;
using FX.Core.Storage.Serialization.Abstract;
using NLog;
using PuppeteerSharp;
using PuppeteerSharp.Input;
using System;
using System.Linq;
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
        protected CookieParam[] CurrentPageCookies;

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
        public virtual void Initialize(P parameters)
        {
            // initialize settings
            SettingsManager.LoadSettings();
            var appSettings = SettingsManager.Settings;

            if (appSettings == null)
                throw new Exception("Cannot instantiate scraper without settings loaded.");

            // initialize parameters
            Parameters = parameters;

            // initialize browser

            // prepare launch options
            LaunchOptions = new LaunchOptions() { Headless = appSettings.Headless };
            ViewPortOptions = new ViewPortOptions() { Width = appSettings.WindowWidth, Height = appSettings.WindowHeight };
            TypeOptions = new TypeOptions() { Delay = 50 };

            // get chromium reference (not async, downloads it, blocks everything while downloading)
            var revision = new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision).Result;
            Browser = Puppeteer.LaunchAsync(LaunchOptions).Result;
            CurrentPage = Browser.NewPageAsync().Result;
            CurrentPage.SetViewportAsync(ViewPortOptions).Wait();
        }



        /// <summary>
        /// 1. Checks for locally stored cookies. If found, runs the mechanism that checks if the login (usually tries the homepage)
        /// 2. If Step 1 fails, 
        /// 3. Run again step 1. If failed, stops and throws error. Cannot connect
        /// </summary>
        /// <returns></returns>
        public virtual async Task DoLoginWorkflowAsync()
        {
            Logger.Info("Testing authentication.");
            CurrentPageCookies = await DataSerializer.GetData<CookieParam[]>(Parameters.CookiePath);

            if (await IsLoggedIn())
            {
                Logger.Info("Already logged in.");
                return;
            }

            Logger.Info("Local login not found. Trying to login on site.");
            await LoginAsync();

            if (!(await IsLoggedIn()))
                throw new Exception("Cannot login.");

            Logger.Info("Login success.");
        }

        /// <summary>
        /// Executes the basic automated login steps (click login, input data, click submit)
        /// </summary>
        /// <returns></returns>
        public virtual async Task LoginAsync()
        {
            var appSettings = SettingsManager.Settings;

            if (CurrentPage.Url != appSettings.AppUrl)
                await CurrentPage.GoToAsync(appSettings.AppUrl);

            // go to login and take refs
            await CurrentPage.ClickAsync(appSettings.SIGNIN_SELECTOR);
            var emailInput = await CurrentPage.WaitForSelectorAsync(appSettings.LOGIN_USER_SELECTOR);
            var passInput = await CurrentPage.WaitForSelectorAsync(appSettings.LOGIN_PASS_SELECTOR);
            var buttonLogin = await CurrentPage.WaitForSelectorAsync(appSettings.LOGIN_SUBMIT_SELECTOR);

            // input data, with waiting time, and click
            await emailInput.TypeAsync(Parameters.Username, TypeOptions);
            await CurrentPage.WaitForTimeoutAsync(appSettings.TIME_TO_WAIT);
            await passInput.TypeAsync(Parameters.Password, TypeOptions);
            await CurrentPage.WaitForTimeoutAsync(appSettings.TIME_TO_WAIT);
            await buttonLogin.ClickAsync();
            await CurrentPage.WaitForTimeoutAsync(2 * appSettings.TIME_TO_WAIT);

            // save cookies
            CurrentPageCookies = await CurrentPage.GetCookiesAsync();
            if (CurrentPageCookies != null && CurrentPageCookies.Any())
            {
                await DataSerializer.SaveDataAsync(CurrentPageCookies, Parameters.CookiePath);
            }
        }

        /// <summary>
        /// User is logged in if it has cookies, and when they're set in GS page, the sign-in button does not appear
        /// </summary>
        /// <returns></returns>
        public virtual async Task<bool> IsLoggedIn()
        {
            if (CurrentPageCookies == null || !CurrentPageCookies.Any())
                return false;

            if (CurrentPage == null)
                return false;

            var appSettings = SettingsManager.Settings;
            await CurrentPage.SetCookieAsync(CurrentPageCookies);
            await CurrentPage.GoToAsync(appSettings.AppUrl);
            var matched = await CurrentPage.QuerySelectorAllAsync(appSettings.SIGNIN_SELECTOR);
            return !matched.Any();
        }



        public virtual async Task CloseBrowser()
        {
            await Browser.CloseAsync();
            Browser = null;
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
            CloseBrowser().Wait();
        }
    }
}
