using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using COLID.AppDataService.Common.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace COLID.AppDataService.Tests.Unit.Extensions
{
    public class DateExtensionsTest
    {
        protected readonly ITestOutputHelper _output;

        // All dates in comments will be in the following format:
        // (YYYY-MM-DD HH:MM:SS)
        public DateExtensionsTest(ITestOutputHelper output)
        {
            _output = output;
            CultureInfo de = new CultureInfo("de-DE");
            Thread.CurrentThread.CurrentCulture = de;
        }

        [Fact]
        public void NextDay_ShouldReturn_NextDayDate()
        {
            var initialDate = new DateTime(2020, 2, 1, 13, 37, 45);
            var expectedDate = new DateTime(2020, 2, 2, 13, 37, 45);

            var newDate = initialDate.NextDay();
            PrintBeforeAndAfter(initialDate, newDate);

            Assert.Equal(expectedDate, newDate);
        }

        #region Week

        [Fact]
        public void FirstDayOfWeek_ShouldReturn_FirstDayOfWeekDate()
        {
            var initialDate = new DateTime(2020, 2, 1, 13, 37, 45);
            var expectedDate = new DateTime(2020, 1, 27, 13, 37, 45);

            var newDate = initialDate.FirstDayOfWeek();
            PrintBeforeAndAfter(initialDate, newDate);

            Assert.Equal(expectedDate, newDate);
        }

        [Fact]
        public void LastDayOfWeek_ShouldReturn_LastDayOfWeekDate()
        {
            var initialDate = new DateTime(2020, 6, 17, 13, 37, 45);
            var expectedDate = new DateTime(2020, 6, 21, 13, 37, 45);

            var newDate = initialDate.LastDayOfWeek();
            PrintBeforeAndAfter(initialDate, newDate);

            Assert.Equal(expectedDate, newDate);
        }

        [Fact]
        public void FirstDayOfNextWeek_ShouldReturn_FirstDayOfNextWeekDate()
        {
            var initialDate = new DateTime(2020, 6, 29, 13, 37, 45);
            var expectedDate = new DateTime(2020, 7, 6, 13, 37, 45);

            var newDate = initialDate.FirstDayOfNextWeek();
            PrintBeforeAndAfter(initialDate, newDate);

            Assert.Equal(expectedDate, newDate);
        }

        #endregion Week

        #region Month

        [Fact]
        public void FirstDayOfMonth_ShouldReturn_FirstDayOfMonthDate()
        {
            var initialDate = new DateTime(2020, 4, 29, 13, 37, 45);
            var expectedDate = new DateTime(2020, 4, 1, 13, 37, 45);

            var newDate = initialDate.FirstDayOfMonth();
            PrintBeforeAndAfter(initialDate, newDate);

            Assert.Equal(expectedDate, newDate);
        }

        [Fact]
        public void LastDayOfMonth_ShouldReturn_LasttDayOfMonthDate()
        {
            var initialDate = new DateTime(2020, 2, 4, 13, 37, 45);
            var expectedDate = new DateTime(2020, 2, 29, 13, 37, 45);

            var newDate = initialDate.LastDayOfMonth();
            PrintBeforeAndAfter(initialDate, newDate);

            Assert.Equal(expectedDate, newDate);
        }

        [Fact]
        public void FirstDayOfNextMonth_ShouldReturn_FirstDayOfNextMonthDate()
        {
            var initialDate = new DateTime(2020, 4, 15, 13, 37, 45);
            var expectedDate = new DateTime(2020, 5, 1, 13, 37, 45);

            var newDate = initialDate.FirstDayOfNextMonth();
            PrintBeforeAndAfter(initialDate, newDate);

            Assert.Equal(expectedDate, newDate);
        }

        [Fact]
        public void LastDayOfNextMonth_ShouldReturn_LastDayOfNextMonthDate()
        {
            var initialDate = new DateTime(2020, 4, 15, 13, 37, 45);
            var expectedDate = new DateTime(2020, 5, 31, 13, 37, 45);

            var newDate = initialDate.LastDayOfNextMonth();
            PrintBeforeAndAfter(initialDate, newDate);

            Assert.Equal(expectedDate, newDate);
        }

        #endregion Month

        #region Quarter

        [Fact]
        public void LastDayOfQuarter_ShouldReturn_LastDayOfQuarterDate()
        {
            var initialDate = new DateTime(2020, 5, 5, 13, 37, 45);
            var expectedDate = new DateTime(2020, 6, 30, 13, 37, 45);

            var newDate = initialDate.LastDayOfQuarter();
            PrintBeforeAndAfter(initialDate, newDate);

            Assert.Equal(expectedDate, newDate);
        }

        [Fact]
        public void FirstDayOfNextQuarter_ShouldReturn_FirstDayOfNextQuarterDate()
        {
            var initialDate = new DateTime(2020, 5, 5, 13, 37, 45);
            var expectedDate = new DateTime(2020, 7, 1, 13, 37, 45);

            var newDate = initialDate.FirstDayOfNextQuarter();
            PrintBeforeAndAfter(initialDate, newDate);

            Assert.Equal(expectedDate, newDate);
        }

        #endregion Quarter

        private void PrintBeforeAndAfter(DateTime before, DateTime after)
        {
            _output.WriteLine($"before:\t{before}");
            _output.WriteLine($"after:\t{after}");
        }
    }
}
