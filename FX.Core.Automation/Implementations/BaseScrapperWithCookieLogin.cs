using FX.Core.Automation.Abstract;
using FX.Core.Automation.Models;
using FX.Core.Automation.Settings;
using FX.Core.Storage.Serialization.Abstract;
using NLog;
using PuppeteerSharp;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FX.Core.Automation.Implementations
{
    public class BaseScrapperWithCookieLogin<P, S> : BaseScraper<P, S>, IScraperWithLogin<P>
        where P : ScraperParameters
        where S : CoreScrapperSettings
    {
        protected CookieParam[] CurrentPageCookies;

        public BaseScrapperWithCookieLogin(ILogger logger, 
            IScraperSettingsManager<S> settingsManager, 
            IDataSerializer dataSerializer) 
            : base(logger, settingsManager, dataSerializer)
        {
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
    }
}
