using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FX.Core.Storage.Serialization.Settings
{
    public class JsonSerializationSettings
    {
        /// <summary>
        /// Represents the base path for all saving / loading of files.
        /// Expected structure: RootPath\GUID
        /// </summary>
        public string RootPath { get; set; } = Directory.GetCurrentDirectory();

        /// <summary>
        /// If set to true, it uses the filename directly as the specified ID.
        /// Otherwise, the filename used will be {guid}.json
        /// </summary>
        public bool UseIdDirectlyAsName { get; set; } = false;
    }
}
