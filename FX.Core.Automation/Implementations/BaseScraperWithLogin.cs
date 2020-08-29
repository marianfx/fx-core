using FX.Core.Automation.Abstract;
using FX.Core.Automation.Abstract.Strategies;
using FX.Core.Automation.Models;
using FX.Core.Automation.Settings;
using FX.Core.Config.Settings.Abstract;
using FX.Core.Storage.Serialization.Abstract;
using NLog;
using System;
using System.Threading.Tasks;

namespace FX.Core.Automation.Implementations
{
    public abstract class BaseScrapperWithLogin<P, S, T> : BaseScraper<P, S>, IScraperWithLogin<P>
        where P : ScraperParameters
        where S : CoreScrapperSettings
        where T : LoginStrategyParametersBase
    {
        public ILoginStrategy<P, T> LoginStrategy { get; set; }

        public BaseScrapperWithLogin() : base() { }

        public BaseScrapperWithLogin(ILogger logger,
            IBasicSettingsManager<S> settingsManager,
            IDataSerializer dataSerializer,
            ILoginStrategy<P, T> loginStrategy)
            : base(logger, settingsManager, dataSerializer)
        {
            LoginStrategy = loginStrategy;
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
            await LoadExistingLogin();
            if (await IsLoggedIn())
            {
                Logger.Info("Already logged in.");
                return;
            }

            Logger.Info("Local login not found. Trying to login on site.");
            await LoginAsync();

            if (!await IsLoggedIn())
                throw new Exception("Cannot login.");

            Logger.Info("Login success.");
        }

        /// <summary>
        /// Loads the existing login data
        /// </summary>
        /// <returns></returns>
        public abstract Task LoadExistingLogin();

        /// <summary>
        /// Executes the basic steps to login this instance / page
        /// </summary>
        /// <returns></returns>
        public abstract Task LoginAsync();

        /// <summary>
        /// Returns the parameters (loaded from this instance) that are then passed to the login strategy.
        /// </summary>
        /// <returns></returns>
        public abstract T GetLoginStrategyParameters();

        /// <summary>
        /// Checks if the user is logged in
        /// </summary>
        /// <returns></returns>
        public abstract Task<bool> IsLoggedIn();
    }
}
