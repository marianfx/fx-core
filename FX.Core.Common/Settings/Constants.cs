using System;
using System.Collections.Generic;

namespace FX.Core.Common.Settings
{
    public class Constants
    {
        public const string DefaultDisplayDateFormat = "MM/dd/yyyy";
        public static readonly string[] DefaultForbiddenChars = { "*", ".", "/", "\\", "[", "]", ":", ";", "|", "=" };

        public static readonly HashSet<Type> NumericTypes = new HashSet<Type>()
        {
            typeof(byte), typeof(sbyte), typeof(UInt16), typeof(UInt32), typeof(UInt64),
            typeof(int), typeof(Int16), typeof(Int32), typeof(Int64),
            typeof(decimal), typeof(double), typeof(Single), typeof(float), typeof(long)
        };

        public static readonly HashSet<Type> NumericDoubleTypes = new HashSet<Type>()
        {
            typeof(decimal), typeof(double), typeof(Single), typeof(float)
        };

        public static readonly HashSet<Type> DateTypes = new HashSet<Type>()
        {
            typeof(DateTime), typeof(TimeSpan)
        };
    }
}
