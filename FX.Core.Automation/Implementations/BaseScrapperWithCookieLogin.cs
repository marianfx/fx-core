using FX.Core.Automation.Abstract;
using FX.Core.Automation.Abstract.Strategies;
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
    public class BaseScrapperWithCookieLogin<P, S, T> : BaseScrapperWithLogin<P, S, T>, IScraperWithLogin<P>
        where P : ScraperParameters
        where S : CoreScrapperSettings
        where T : LoginStrategyParametersBase
    {
        protected CookieParam[] CurrentPageCookies;

        public BaseScrapperWithCookieLogin(ILogger logger,
            IScraperSettingsManager<S> settingsManager,
            IDataSerializer dataSerializer,
            ILoginStrategy<P, T> loginStrategy) 
            : base(logger, settingsManager, dataSerializer, loginStrategy)
        {
        }

        public override async Task LoadExistingLogin()
        {
            CurrentPageCookies = await DataSerializer.GetData<CookieParam[]>(Parameters.CookiePath);
        }

        /// <summary>
        /// Executes the basic automated login steps (click login, input data, click submit)
        /// </summary>
        /// <returns></returns>
        public override async Task LoginAsync()
        {
            if (LoginStrategy == null)
                throw new ArgumentNullException("There is no login strategy specified");

            // execute login strategy
            await LoginStrategy.DoLogin(this, GetLoginStrategyParameters());

            // save cookies
            CurrentPageCookies = await CurrentPage.GetCookiesAsync();
            if (CurrentPageCookies != null && CurrentPageCookies.Any())
            {
                await DataSerializer.SaveDataAsync(CurrentPageCookies, Parameters.CookiePath);
            }
        }

        public override T GetLoginStrategyParameters()
        {
            var appSettings = SettingsManager.Settings;
            return new LoginStrategyParameters
            {
                LoginUrl = appSettings.AppUrl,
                LoginButtonSelector = appSettings.SIGNIN_SELECTOR,
                InputSelectorUser = appSettings.LOGIN_USER_SELECTOR,
                InputTextUser = Parameters.Username,
                InputSelectorPassword = appSettings.LOGIN_PASS_SELECTOR,
                InputTextPassword = Parameters.Password,
                ButtonSelectorExecuteLogin = appSettings.LOGIN_SUBMIT_SELECTOR
            } as T;
        }

        /// <summary>
        /// User is logged in if it has cookies, and when they're set in GS page, the sign-in button does not appear
        /// </summary>
        /// <returns></returns>
        public override async Task<bool> IsLoggedIn()
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
