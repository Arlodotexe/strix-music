using System;

namespace StrixMusic.Helpers.TimeSpanRules
{
    /// <summary>
    /// An interface for checking if a time matches.
    /// </summary>
    public interface ITimeSpanRule
    {
        /// <summary>
        /// Gets if a time fits the rule.
        /// </summary>
        /// <param name="now">The time to check.</param>
        /// <returns>Whether or not the time <paramref name="now"/> fits the rule.</returns>
        public bool IsNow(DateTime now);
    }
}
