namespace FX.Core.Automation.Settings
{
    public class ScraperConstants
    {
        public const string FUNC_SELECT_ALL = @"(sel) => {
                                                    return document.querySelectorAll(sel){{SPECIAL}};
                                                }";
        public const string FUNC_SELECT_SIMPLE = @"(sel) => {
                                                    return document.querySelector(sel){{SPECIAL}}; 
                                                }";
    }
}
