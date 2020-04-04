using FX.Core.Automation.Settings;
using FX.Core.Config.Settings.Abstract;

namespace FX.Core.Automation.Abstract
{
    public interface IScraperSettingsManager<T>: IBasicSettings<T>
        where T: CoreScrapperSettings
    {
    }
}
