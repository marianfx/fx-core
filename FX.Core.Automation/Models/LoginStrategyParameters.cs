namespace FX.Core.Automation.Models
{
    public class LoginStrategyParameters: LoginStrategyParametersBase
    {
        public string LoginButtonSelector { get; set; }
        public string InputSelectorUser { get; set; }
        public string InputTextUser { get; set; }
        public string InputSelectorPassword { get; set; }
        public string InputTextPassword { get; set; }
        public string ButtonSelectorExecuteLogin { get; set; }
    }
}
