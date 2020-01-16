using FX.Core.Common.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FX.Core.Common.Extensions
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Returns a list of all the declared properties of an object (not the inherited ones)
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetOnlyDeclaredProperties(this Type objectType)
        {
            return objectType.GetProperties(BindingFlags.Public
                                            | BindingFlags.Instance
                                            | BindingFlags.DeclaredOnly)
                                            .Select(p => p.Name);
        }

        /// <summary>
        /// Returns a list of all the properties of an object, ordered by: parent first, child second
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetAllPropertiesOrdered(this Type objectType)
        {
            var stack = new Stack<IEnumerable<string>>();
            var thisType = objectType;
            while (objectType != typeof(Object))
            {
                var childProperties = objectType.GetProperties(BindingFlags.Public
                                            | BindingFlags.Instance
                                            | BindingFlags.DeclaredOnly)
                                            .Select(p => p.Name);
                stack.Push(childProperties);
                objectType = objectType.BaseType;
            }

            IEnumerable<string> output = new List<string>();
            while (stack.Count > 0)
                output = output.Concat(stack.Pop());

            return output;
        }

        public static bool IsNumberType(this Type type)
        {
            return Constants.NumericTypes.Contains(type) || Constants.NumericTypes.Contains(Nullable.GetUnderlyingType(type));
        }

        public static bool IsDoubleNumberType(this Type type)
        {
            return Constants.NumericDoubleTypes.Contains(type) || Constants.NumericDoubleTypes.Contains(Nullable.GetUnderlyingType(type));
        }

        public static bool IsDateType(this Type type)
        {
            return Constants.DateTypes.Contains(type) || Constants.DateTypes.Contains(Nullable.GetUnderlyingType(type));
        }

        public static bool IsBoolType(this Type type)
        {
            return type == typeof(bool) || type == typeof(bool?);
        }

        //https://codereview.stackexchange.com/questions/58251/transform-datareader-to-listt-using-reflections
        /// <summary>
        /// Changes the type of an object to the target type, in respect to its nullable state (uses original type)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public static object ChangeType(object value, Type targetType)
        {
            if (value == null || value == DBNull.Value)
                return null;

            return Convert.ChangeType(value, Nullable.GetUnderlyingType(targetType) ?? targetType);
        }
    }
}
