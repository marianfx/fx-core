using FX.Core.IO.Files.Enums;
using System.Collections.Generic;

namespace FX.Core.IO.Files.Settings
{
    public class FileHandlerSettings
    {
        public Dictionary<FileTypes, string> FileMimeTypes { get; set; } = new Dictionary<FileTypes, string>()
        {
            { FileTypes.Excel, "application/vnd.ms-excel" }
        };
    }
}
