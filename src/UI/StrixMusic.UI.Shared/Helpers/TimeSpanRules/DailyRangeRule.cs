using System;

namespace StrixMusic.Helpers.TimeSpanRules
{
    /// <summary>
    /// A rule for if it's a certain range of times in the day.
    /// </summary>
    public class DailyRangeRule : ITimeSpanRule
    {
        private TimeSpan _startTime;
        private TimeSpan _endTime;
        private bool _wrapped;

        /// <summary>
        /// Initializes a new instance of the <see cref="DailyRangeRule"/> class.
        /// </summary>
        /// <param name="start">The time of day to start as a TimeSpan.</param>
        /// <param name="end">The time of day to start as a TimeSpan.</param>
        public DailyRangeRule(TimeSpan start, TimeSpan end)
        {
            _startTime = start;
            _endTime = end;

            if (_endTime < _startTime)
            {
                TimeSpan cache = _endTime;
                _endTime = _startTime;
                _startTime = cache.Add(TimeSpan.FromDays(1));
                _wrapped = true;
            }
        }

        /// <inheritdoc/>
        public bool IsNow(DateTime now)
        {
            TimeSpan currentTime = now.TimeOfDay;
            if (_wrapped && currentTime < TimeSpan.FromDays(1))
                currentTime = currentTime.Add(TimeSpan.FromDays(1));

            return currentTime < _endTime && currentTime > _startTime;
        }
    }
}
