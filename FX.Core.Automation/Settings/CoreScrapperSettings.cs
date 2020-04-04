using FX.Core.Config.Settings.Models;

namespace FX.Core.Automation.Settings
{
    public class CoreScrapperSettings : ISettingsObject
    {
        public string AppUrl { get; set; }
        public string ErrorUrl { get; set; }
        public bool Headless { get; set; }
        public int WindowWidth { get; set; }
        public int WindowHeight { get; set; }
        public int TIME_TO_WAIT { get; set; }
        public int TYPE_DELAY { get; set; }

        // selectors
        public string SIGNIN_SELECTOR { get; set; }
        public string LOGIN_FORM_SELECTOR { get; set; }
        public string LOGIN_USER_SELECTOR { get; set; }
        public string LOGIN_PASS_SELECTOR { get; set; }
        public string LOGIN_SUBMIT_SELECTOR { get; set; }

        public virtual bool Validate()
        {
            return true;
        }
    }
}
