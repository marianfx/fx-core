using System;

namespace FX.Core.Common.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Returns the quarter corresponding to the date
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static int GetQuarter(this DateTime d)
        {
            switch (d.Month)
            {
                case 1:
                case 2:
                case 3:
                    return 1;
                case 4:
                case 5:
                case 6:
                    return 2;
                case 7:
                case 8:
                case 9:
                    return 3;
                case 10:
                case 11:
                case 12:
                    return 4;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// The name says a lot about this method
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime GetLastDayOfMonth(this DateTime date) => new DateTime(date.Year, date.Month, 1).AddMonths(1).AddDays(-1);

        /// <summary>
        /// Returns the last date of the last month
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime GetDateEndOfLastMonth(this DateTime date)
        {
            return DateTime.DaysInMonth(date.Year, date.Month) == date.Day ? date : new DateTime(date.AddMonths(-1).Year,
                date.AddMonths(-1).Month, DateTime.DaysInMonth(date.AddMonths(-1).Year, date.AddMonths(-1).Month));
        }

        /// <summary>
        /// Returns true if the current date is the end of the month
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static bool IsEndOfMonth(this DateTime date) => DateTime.DaysInMonth(date.Year, date.Month) == date.Day;
    }
}
