using System;

namespace StrixMusic.Helpers.TimeSpanRules
{
    /// <summary>
    /// A rule for if it's a certain day of the week.
    /// </summary>
    public class WeeklyRule : ITimeSpanRule
    {
        private DayOfWeek _day;

        /// <summary>
        /// Initializes a new instance of the <see cref="DailyRangeRule"/> class.
        /// </summary>
        /// <param name="day">The day of the week.</param>
        public WeeklyRule(DayOfWeek day)
        {
            _day = day;
        }

        /// <inheritdoc/>
        public bool IsNow(DateTime now)
        {
            return now.DayOfWeek == _day;
        }
    }
}
