using FX.Core.Common.DataModel;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace FX.Core.Common.Validations.Abstract
{
    public interface IValidator
    {
        List<string> Errors { get; set; }

        /// <summary>
        /// Validate an object for not being null.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="valueName">The name to display in default error message. Can be ignored, by default is property name.</param>
        /// <param name="customMessage">Set a custom message to display if the default does not suit you.</param>
        /// <returns></returns>
        IValidator ValidateObject(object value, string valueName = null, string customMessage = null);

        /// <summary>
        /// Validates a string checking if it is a valid unsigned long value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="valueName"></param>
        /// <param name="culture"></param>
        /// <param name="customMessage"></param>
        /// <returns></returns>
        IValidator ValidateUnsignedLong(string value, string valueName = null, CultureInfo culture = null, string customMessage = null);

        /// <summary>
        /// Validate a string checking if it is a valid double value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="valueName">The name to display in default error message. Can be ignored, by default is property name.</param>
        /// <param name="customMessage">Set a custom message to display if the default does not suit you.</param>
        /// <returns></returns>
        IValidator ValidateDouble(string value, string valueName = null, CultureInfo culture = null, string customMessage = null);

        /// <summary>
        /// Validates strings. Cannot be null, empty of whitespace.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="valueName">The name to display in default error message. Can be ignored, by default is property name.</param>
        /// <param name="customMessage">Set a custom message to display if the default does not suit you.</param>
        /// <returns></returns>
        IValidator ValidateString(string value, string valueName = null, string customMessage = null);

        /// <summary>
        /// Validates strings with default validation + checks the string to be in a range of values.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="valueName">The name to display in default error message. Can be ignored, by default is property name.</param>
        /// <param name="customMessage">Set a custom message to display if the default does not suit you.</param>
        /// <returns></returns>
        IValidator ValidateString(string value, IEnumerable<string> range, bool ignoreCase = true, string valueName = null, string customMessage = null);

        /// <summary>
        /// Validates an Array (IEnumerable). Cannot be null or empty.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="array">The array to validate</param>
        /// <param name="arrayName">The name to display in default error message.</param>
        /// <param name="customMessage">Set a custom message to display if the default does not suit you.</param>
        /// <returns></returns>
        IValidator ValidateArray<T>(IEnumerable<T> array, string arrayName, string customMessage = null);

        /// <summary>
        /// Validates date. Cannot be null, empty of whitespace.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="valueName">The name to display in default error message. Can be ignored, by default is property name.</param>
        /// <param name="customMessage">Set a custom message to display if the default does not suit you.</param>
        /// <returns></returns>
        IValidator ValidateDate(string value, out DateTime date, string valueName = null, string customMessage = null);

        /// <summary>
        /// Validates date. Cannot be null, empty of whitespace, and must be able to convert to DateTime object using the format specified in Global.AcceptedDateFormat
        /// </summary>
        /// <param name="value"></param>
        /// <param name="valueName">The name to display in default error message. Can be ignored, by default is property name.</param>
        /// <param name="customMessage">Set a custom message to display if the default does not suit you.</param>
        /// <returns></returns>
        IValidator ValidateDateExact(string value, out DateTime date, string valueName = null, string customMessage = null);

        /// <summary>
        /// Validates an object length
        /// </summary>
        /// <param name="value"></param>
        /// <param name="valueName"></param>
        /// <param name="customMessage"></param>
        /// <returns></returns>
        IValidator ValidateLength(object value, uint length, string valueName = null, string condition = "!=", string customMessage = null);

        /// <summary>
        /// Validates descriptor not to be empty
        /// </summary>
        /// <param name="value"></param>
        /// <param name="valueName">The name to display in default error message. Can be ignored, by default is property name.</param>
        /// <param name="customMessage">Set a custom message to display if the default does not suit you.</param>
        /// <returns></returns>
        IValidator ValidateDescriptor(Descriptor value, string valueName = null, string customMessage = null);

        /// <summary>
        /// Validates descriptor not to be empty
        /// </summary>
        /// <param name="value"></param>
        /// <param name="valueName">The name to display in default error message. Can be ignored, by default is property name.</param>
        /// <param name="customMessage">Set a custom message to display if the default does not suit you.</param>
        /// <returns></returns>
        IValidator ValidateDescriptor(NDescriptor value, string valueName = null, string customMessage = null);

        /// <summary>
        /// Returns the list of all errors found.
        /// </summary>
        /// <returns></returns>
        List<string> GetErrors();
    }
}
