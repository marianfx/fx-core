using FX.Core.Common.Validations.Abstract;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FX.Core.Common.DataModel;
using Microsoft.Extensions.Options;
using FX.Core.Common.Settings;
using FX.Core.Common.Localization;

namespace FX.Core.Common.Validations.Implementations
{
    public class Validator : IValidator
    {
        public List<string> Errors { get; set; } = new List<string>();
        private ValidationSettings _settings;

        public Validator(IOptions<ValidationSettings> settings)
        {
            _settings = settings.Value;
        }

        /// <summary>
        /// Validate an object for not being null.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="valueName">The name to display in default error message. Can be ignored, by default is property name.</param>
        /// <param name="customMessage">Set a custom message to display if the default does not suit you.</param>
        /// <returns></returns>
        public IValidator ValidateObject(object value, string valueName = null, string customMessage = null)
        {
            var message = !string.IsNullOrWhiteSpace(customMessage) ? customMessage
                            : string.Format("{0} {1}.", valueName ?? Resources.Value, Resources.MustBeSpecified);

            if (value == null)
                Errors.Add(message);

            return this;
        }

        /// <summary>
        /// Validates a string checking if it is a valid unsigned long value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="valueName"></param>
        /// <param name="culture"></param>
        /// <param name="customMessage"></param>
        /// <returns></returns>
        public IValidator ValidateUnsignedLong(string value, string valueName = null, CultureInfo culture = null, string customMessage = null)
        {
            culture = culture ?? CultureInfo.CurrentCulture;
            var message = !string.IsNullOrWhiteSpace(customMessage) ? customMessage
                            : string.Format("{0} {1}.", valueName ?? Resources.Value, Resources.MustBeAValidPositiveLongNumber);

            if (string.IsNullOrWhiteSpace(value) || !ulong.TryParse(value, NumberStyles.Float, culture, out ulong result))
                Errors.Add(message);

            return this;
        }

        /// <summary>
        /// Validate a string checking if it is a valid double value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="valueName">The name to display in default error message. Can be ignored, by default is property name.</param>
        /// <param name="customMessage">Set a custom message to display if the default does not suit you.</param>
        /// <returns></returns>
        public IValidator ValidateDouble(string value, string valueName = null, CultureInfo culture = null, string customMessage = null)
        {
            culture = culture ?? CultureInfo.CurrentCulture;
            var message = !string.IsNullOrWhiteSpace(customMessage) ? customMessage
                            : string.Format("{0} {1}.", valueName ?? Resources.Value, Resources.MustBeAValidDoubleNumber);

            if (string.IsNullOrWhiteSpace(value) || !double.TryParse(value, NumberStyles.Float, culture, out double result))
                Errors.Add(message);

            return this;
        }

        /// <summary>
        /// Validates strings. Cannot be null, empty of whitespace.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="valueName">The name to display in default error message. Can be ignored, by default is property name.</param>
        /// <param name="customMessage">Set a custom message to display if the default does not suit you.</param>
        /// <returns></returns>
        public IValidator ValidateString(string value, string valueName = null, string customMessage = null)
        {
            var message = !string.IsNullOrWhiteSpace(customMessage) ? customMessage
                            : string.Format("{0} {1}.", valueName ?? Resources.Value, Resources.MustBeSpecified);

            if (string.IsNullOrWhiteSpace(value))
                Errors.Add(message);

            return this;
        }

        /// <summary>
        /// Validates strings with default validation + checks the string to be in a range of values.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="valueName">The name to display in default error message. Can be ignored, by default is property name.</param>
        /// <param name="customMessage">Set a custom message to display if the default does not suit you.</param>
        /// <returns></returns>
        public IValidator ValidateString(string value, IEnumerable<string> range, bool ignoreCase = true, string valueName = null, string customMessage = null)
        {
            var errors = ValidateString(value, valueName, customMessage);
            if (errors.Errors.Count > 0)
                return errors;

            if (ignoreCase && value != null)
                value = value.ToUpper();
            if (ignoreCase && range != null)
                range = range.Select(r => r.ToUpper());

            var message = !string.IsNullOrWhiteSpace(customMessage) ? customMessage
                            : string.Format("{0} {2} ({1})", valueName ?? Resources.Value, string.Join(", ", range), Resources.MustBeInRange);

            if (range != null && !range.Contains(value))
                Errors.Add(message);

            return this;
        }

        /// <summary>
        /// Validates an Array (IEnumerable). Cannot be null or empty.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="array">The array to validate</param>
        /// <param name="arrayName">The name to display in default error message.</param>
        /// <param name="customMessage">Set a custom message to display if the default does not suit you.</param>
        /// <returns></returns>
        public IValidator ValidateArray<T>(IEnumerable<T> array, string arrayName, string customMessage = null)
        {
            var message = !string.IsNullOrWhiteSpace(customMessage) ? customMessage
                            : string.Format("{0} {1}.", arrayName ?? Resources.Array, Resources.CannotBeNullOrEmpty);

            if (array == null || array.Count() == 0)
                Errors.Add(message);

            return this;
        }

        /// <summary>
        /// Validates date. Cannot be null, empty of whitespace and must be able to convert to DateTime object using the invariant culture.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="valueName">The name to display in default error message. Can be ignored, by default is property name.</param>
        /// <param name="customMessage">Set a custom message to display if the default does not suit you.</param>
        /// <returns></returns>
        public IValidator ValidateDate(string value, out DateTime date, string valueName = null, string customMessage = null)
        {
            var message = !string.IsNullOrWhiteSpace(customMessage) ? customMessage
                            : string.Format("{0} {1}.", valueName ?? Resources.Value, Resources.MustBeSpecified);

            date = DateTime.Now.Date;
            if (string.IsNullOrWhiteSpace(value) || !DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                Errors.Add(message);

            return this;
        }

        /// <summary>
        /// Validates date. Cannot be null, empty of whitespace, and must be able to convert to DateTime object using the format specified in Global.AcceptedDateFormat
        /// </summary>
        /// <param name="value"></param>
        /// <param name="valueName">The name to display in default error message. Can be ignored, by default is property name.</param>
        /// <param name="customMessage">Set a custom message to display if the default does not suit you.</param>
        /// <returns></returns>
        public IValidator ValidateDateExact(string value, out DateTime date, string valueName = null, string customMessage = null)
        {
            var message = !string.IsNullOrWhiteSpace(customMessage) ? customMessage
                            : string.Format("{0} {1}.", valueName ?? Resources.Value, Resources.MustBeSpecified);

            date = DateTime.Now.Date;
            if (string.IsNullOrWhiteSpace(value) || !DateTime.TryParseExact(value, _settings.AcceptedDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTemp))
                Errors.Add(message);
            else
                date = dateTemp;

            return this;
        }

        /// <summary>
        /// Returns the list of all errors found. Also clears the error list.
        /// </summary>
        /// <returns></returns>
        public List<string> GetErrors()
        {
            var output = new List<string>(Errors);
            Errors = new List<string>();
            return output;
        }

        /// <summary>
        /// Validates the object to be a specific length
        /// </summary>
        /// <param name="value"></param>
        /// <param name="length"></param>
        /// <param name="valueName"></param>
        /// <param name="customMessage"></param>
        /// <returns></returns>
        public IValidator ValidateLength(object value, uint length, string valueName = null, string condition = "!=", string customMessage = null)
        {
            var message = !string.IsNullOrWhiteSpace(customMessage) ? customMessage : $"{valueName ?? value} ";

            switch (condition)
            {
                case "<":
                    if (value == null || value.ToString().Length < length)
                        //Errors.Add(!string.IsNullOrWhiteSpace(customMessage) ? customMessage : $"{value} {Resources.LengthIsLessThan} {length}");
                        Errors.Add(message + Resources.LengthIsLessThan + length);
                    break;
                case "!=":
                    if (value == null || value.ToString().Length != length)
                        //Errors.Add(!string.IsNullOrWhiteSpace(customMessage) ? customMessage : $"{valueName ?? value} {Resources.LengthIsNotEqualTo} {length}");
                        Errors.Add(message + Resources.LengthIsNotEqualTo + length);
                    break;
                case ">":
                    if (value == null || value.ToString().Length > length)
                        //Errors.Add(!string.IsNullOrWhiteSpace(customMessage) ? customMessage : $"{value} {Resources.LengthIsGreaterThan} {length}");
                        Errors.Add(message + Resources.LengthIsGreaterThan + length);
                    break;
            }

            return this;
        }

        /// <summary>
        /// Validates descriptor not to be empty
        /// </summary>
        /// <param name="value"></param>
        /// <param name="valueName">The name to display in default error message. Can be ignored, by default is property name.</param>
        /// <param name="customMessage">Set a custom message to display if the default does not suit you.</param>
        /// <returns></returns>
        public IValidator ValidateDescriptor(Descriptor value, string valueName = null, string customMessage = null)
        {
            var message = !string.IsNullOrWhiteSpace(customMessage) ? customMessage
                            : string.Format("{0} {1}.", valueName ?? Resources.Value, Resources.MustBeSpecified);

            if (value == null || value.IsEmpty())
                Errors.Add(message);

            return this;
        }

        /// <summary>
        /// Validates descriptor not to be empty
        /// </summary>
        /// <param name="value"></param>
        /// <param name="valueName">The name to display in default error message. Can be ignored, by default is property name.</param>
        /// <param name="customMessage">Set a custom message to display if the default does not suit you.</param>
        /// <returns></returns>
        public IValidator ValidateDescriptor(NDescriptor value, string valueName = null, string customMessage = null)
        {
            var message = !string.IsNullOrWhiteSpace(customMessage) ? customMessage
                            : string.Format("{0} {1}.", valueName ?? Resources.Value, Resources.MustBeSpecified);

            if (value == null || value.IsEmpty())
                Errors.Add(message);

            return this;
        }
    }
}
