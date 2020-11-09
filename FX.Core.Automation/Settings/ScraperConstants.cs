namespace FX.Core.Automation.Settings
{
    public class ScraperConstants
    {
        public const string FUNC_SELECT_ALL = @"(sel) => {
                                                    return document.querySelectorAll(sel){{SPECIAL}};
                                                }";
        public const string FUNC_SELECT_ALL_INDEX = @"(sel) => {
                                                    return document.querySelectorAll(sel)[{{INDEX}}]{{SPECIAL}};
                                                }";
        public const string FUNC_SELECT_SIMPLE = @"(sel) => {
                                                    return document.querySelector(sel){{SPECIAL}}; 
                                                }";
        public const string WINDOW_FUNCTION = @"(sel) => { return window{{SPECIAL}}; }";

        // some default replacements
        public const string FUNC_SELECT_SIMPLE_INNER_TEXT = @"(sel) => {
                                                    return document.querySelector(sel).innerText; 
                                                }";
        public const string FUNC_SELECT_SIMPLE_ATTRIBUTE = @"(sel) => {
                                                    return document.querySelector(sel).attributes['{{SPECIAL}}'].value; 
                                                }";
    }
}
