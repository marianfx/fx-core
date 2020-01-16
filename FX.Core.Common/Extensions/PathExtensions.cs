using System;
using System.IO;

namespace FX.Core.Common.Extensions
{
    public static class PathExtensions
    {
        /// <summary>
        /// Combines the current path with a guid path, guid comes first.
        /// The result is {guid}/path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string CombineWithNewGuid(this string path) => Path.Combine(Guid.NewGuid().ToString(), path);

        /// <summary>
        /// Combines the current path with the given path according to the system rules
        /// The result is path1/path2
        /// </summary>
        /// <param name="path"></param>
        /// <param name="path2"></param>
        /// <returns></returns>
        public static string CombineWith(this string path, string path2) => Path.Combine(path, path2);
    }
}
