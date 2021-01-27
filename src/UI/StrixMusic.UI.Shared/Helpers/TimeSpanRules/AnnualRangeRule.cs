using System;

namespace StrixMusic.Helpers.TimeSpanRules
{
    /// <summary>
    /// A rule for if day is with in a certain time range.
    /// </summary>
    public class AnnualRangeRule : ITimeSpanRule
    {
        private int _startDay;
        private int _endDay;
        private bool _wrapped = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="DailyRangeRule"/> class.
        /// </summary>
        /// <param name="start">The time of day to start as a DateTime.</param>
        /// <param name="end">The time of day to start as a DateTime.</param>
        public AnnualRangeRule(DateTime start, DateTime end)
        {
            _startDay = start.DayOfYear;
            _endDay = end.DayOfYear;

            if (_endDay < _startDay)
            {
                int cache = _endDay;
                _endDay = _startDay;
                _startDay = cache + 366;
                _wrapped = true;
            }
        }

        /// <inheritdoc/>
        public bool IsNow(DateTime now)
        {
            int currentDay = now.DayOfYear;
            if (_wrapped && currentDay < 366)
                currentDay += 366;

            return currentDay < _endDay && currentDay > _startDay;
        }
    }
}
