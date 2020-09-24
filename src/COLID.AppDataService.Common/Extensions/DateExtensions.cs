using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using COLID.AppDataService.Common.Enums;

namespace COLID.AppDataService.Common.Extensions
{
    /// <summary>
    /// Extension for date arithmetik.
    /// </summary>
    public static class DateExtensions
    {
        public static DateTime NextDay(this DateTime date)
        {
            return date.AddDays(1);
        }

        #region Week

        public static DateTime FirstDayOfWeek(this DateTime date)
        {
            DayOfWeek dayOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
            int offset = dayOfWeek - date.DayOfWeek;
            DateTime firstDayOfWeekDate = date.AddDays(offset);
            return firstDayOfWeekDate;
        }

        public static DateTime LastDayOfWeek(this DateTime date)
        {
            DateTime lastDayOfWeekDate = FirstDayOfWeek(date).AddDays(6);
            return lastDayOfWeekDate;
        }

        public static DateTime FirstDayOfNextWeek(this DateTime date)
        {
            return FirstDayOfWeek(date).AddDays(7);
        }

        #endregion Week

        #region Month

        public static DateTime FirstDayOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1, date.Hour, date.Minute, date.Second);
        }

        public static DateTime LastDayOfMonth(this DateTime date)
        {
            var lastDayInMonth = DateTime.DaysInMonth(date.Year, date.Month);
            return new DateTime(date.Year, date.Month, lastDayInMonth, date.Hour, date.Minute, date.Second);
        }

        public static DateTime FirstDayOfNextMonth(this DateTime date)
        {
            return FirstDayOfMonth(date).AddMonths(1);
        }

        public static DateTime LastDayOfNextMonth(this DateTime date)
        {
            return date.AddMonths(1).LastDayOfMonth();
        }

        #endregion Month

        #region Quarter

        public static DateTime FirstDayOfNextQuarter(this DateTime date)
        {
            return LastDayOfQuarter(date).AddDays(1);
        }

        public static DateTime LastDayOfQuarter(this DateTime date)
        {
            var quartalDay = date.AddMonths(3 - (date.Month - 1) % 3);
            return quartalDay.AddDays(-quartalDay.Day);
        }

        public static DateTime LastDayOfNextQuarter(this DateTime date)
        {
            var quartalDay = date.AddMonths(3 - (date.Month - 1) % 3);
            return quartalDay.AddDays(-quartalDay.Day).AddMonths(3);
        }

        #endregion Quarter

        #region Interval-Calculation

        public static DateTime? CalculateByInterval(this DateTime date, SendInterval interval)
        {
            return interval switch
            {
                SendInterval.Immediately => DateTime.Now.Truncate(TimeSpan.FromSeconds(1)),
                SendInterval.Daily => date.NextDay().Truncate(TimeSpan.FromSeconds(1)),
                SendInterval.Weekly => date.FirstDayOfNextWeek().Truncate(TimeSpan.FromSeconds(1)),
                SendInterval.Monthly => date.FirstDayOfNextMonth().Truncate(TimeSpan.FromSeconds(1)),
                _ => null, // also SendInterval.Never
            };
        }

        public static DateTime? CalculateByInterval(this DateTime date, DeleteInterval interval)
        {
            return interval switch
            {
                DeleteInterval.Weekly => date.AddDays(7).Truncate(TimeSpan.FromSeconds(1)),
                DeleteInterval.Monthly => date.AddMonths(1).Truncate(TimeSpan.FromSeconds(1)),
                DeleteInterval.Quarterly => date.AddMonths(3).Truncate(TimeSpan.FromSeconds(1)),
                _ => null,
            };
        }

        public static DateTime? CalculateByInterval(this DateTime date, ExecutionInterval interval)
        {
            return interval switch
            {
                ExecutionInterval.Daily => date.NextDay(),
                ExecutionInterval.Weekly => date.FirstDayOfNextWeek(),
                ExecutionInterval.Monthly => date.FirstDayOfNextMonth(),
                _ => null,
            };
        }

        #endregion Interval-Calculation

        public static DateTime Truncate(this DateTime dateTime, TimeSpan timeSpan)
        {
            if (timeSpan == TimeSpan.Zero) return dateTime; // Or could throw an ArgumentException
            if (dateTime == DateTime.MinValue || dateTime == DateTime.MaxValue) return dateTime; // do not modify "guard" values
            return dateTime.AddTicks(-(dateTime.Ticks % timeSpan.Ticks));
        }
    }
}
