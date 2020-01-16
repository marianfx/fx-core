using FX.Core.Common.Settings;
using System;
using System.Globalization;

namespace FX.Core.Common.Extensions
{
    public class DateTimeUtils
    {
        /// <summary>
        /// Extracts a DateTime object from a string, using the provided format.
        /// If the format is not correct does not throw exception but instead returns null.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static DateTime? ExtractDateFromString(string value, string format)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            var dateTemp = new DateTime();
            if (!DateTime.TryParseExact(value, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTemp))
                return null;

            return dateTemp;
        }

        /// <summary>
        /// Extracts a DateTime object from a string, using the format MM/dd/yyyy
        /// If the format is not correct does not throw exception but instead returns null.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static DateTime? ExtractDateStandardFormat(string value)
        {
            return ExtractDateFromString(value, Constants.DefaultDisplayDateFormat);
        }

        /// <summary>
        /// The implementation of the VB.NET DateDiff method, using a custom DateInterval enum
        /// </summary>
        /// <param name="intervalType"></param>
        /// <param name="dateOne"></param>
        /// <param name="dateTwo"></param>
        /// <returns></returns>
        public static long DateDiff(DateInterval intervalType, DateTime dateOne, DateTime dateTwo)
        {
            switch (intervalType)
            {
                case DateInterval.Day:
                case DateInterval.DayOfYear:
                    TimeSpan spanForDays = dateTwo - dateOne;
                    return (long)spanForDays.TotalDays;
                case DateInterval.Hour:
                    TimeSpan spanForHours = dateTwo - dateOne;
                    return (long)spanForHours.TotalHours;
                case DateInterval.Minute:
                    TimeSpan spanForMinutes = dateTwo - dateOne;
                    return (long)spanForMinutes.TotalMinutes;
                case DateInterval.Month:
                    return ((dateTwo.Year - dateOne.Year) * 12) + (dateTwo.Month - dateOne.Month);
                case DateInterval.Quarter:
                    long dateOneQuarter = (long)Math.Ceiling(dateOne.Month / 3.0);
                    long dateTwoQuarter = (long)Math.Ceiling(dateTwo.Month / 3.0);
                    return (4 * (dateTwo.Year - dateOne.Year)) + dateTwoQuarter - dateOneQuarter;
                case DateInterval.Second:
                    TimeSpan spanForSeconds = dateTwo - dateOne;
                    return (long)spanForSeconds.TotalSeconds;
                case DateInterval.Weekday:
                    TimeSpan spanForWeekdays = dateTwo - dateOne;
                    return (long)(spanForWeekdays.TotalDays / 7.0);
                case DateInterval.WeekOfYear:
                    DateTime dateOneModified = dateOne;
                    DateTime dateTwoModified = dateTwo;
                    while (dateTwoModified.DayOfWeek != DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek)
                    {
                        dateTwoModified = dateTwoModified.AddDays(-1);
                    }
                    while (dateOneModified.DayOfWeek != DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek)
                    {
                        dateOneModified = dateOneModified.AddDays(-1);
                    }
                    TimeSpan spanForWeekOfYear = dateTwoModified - dateOneModified;
                    return (long)(spanForWeekOfYear.TotalDays / 7.0);
                case DateInterval.Year:
                    return dateTwo.Year - dateOne.Year;
                default:
                    return 0;
            }
        }
    }
}
