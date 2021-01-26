using StrixMusic.Helpers.TimeSpanRules;
using System;
using System.Collections.Generic;

namespace StrixMusic.Helpers
{
    /// <summary>
    /// A class for loading localized quips, with time based context.
    /// </summary>
    public class QuipLoader
    {
        /// <summary>
        /// A set of quips for a languauge and time.
        /// </summary>
        private struct QuipGroup
        {
            public QuipGroup(
                string name,
                int count,
                ITimeSpanRule? rule,
                int weight = 1)
            {
                Name = name;
                Count = count;
                Rule = rule;
                Weight = weight;
                Generic = Name == "Generic";
            }

            /// <summary>
            /// Gets whether or not the quips apply at this time.
            /// </summary>
            /// <param name="now">The time to for.</param>
            /// <returns>Whether or not now is in the time range.</returns>
            public bool Applies(DateTime now)
            {
                if (Rule == null)
                    return true;

                return Rule.IsNow(now);
            }

            public string Name { get; }

            public int Count { get; }

            public ITimeSpanRule? Rule { get; }

            public int Weight { get; }

            public bool Generic { get; }
        }

        private static Dictionary<string, QuipGroup[]> _languageSpecialDayMap = new Dictionary<string, QuipGroup[]>
        {
            {
                "en",
                new QuipGroup[]
                {
                    new QuipGroup("Generic", count:10, rule:null),
                    new QuipGroup("Christmas", count:3, rule:new AnnualRangeRule(new DateTime(1, 12, 24), new DateTime(1, 12, 26)), weight:10),
                    new QuipGroup("NewYears", count:2, rule:new AnnualRangeRule(new DateTime(1, 12, 31), new DateTime(1, 1, 2)), weight:10),
                    new QuipGroup("NewYearsEve", count:1, rule:new AnnualRangeRule(new DateTime(1, 12, 31), new DateTime(1, 1, 1))),
                    new QuipGroup("May4th", count:4, rule:new AnnualRangeRule(new DateTime(1, 5, 4), new DateTime(1, 5, 5)), weight:10),
                    new QuipGroup("Dawn", count:1, rule:new DailyRangeRule(TimeSpan.FromHours(4), TimeSpan.FromHours(7)), weight:2),
                    new QuipGroup("Morning", count:1, rule:new DailyRangeRule(TimeSpan.FromHours(7), TimeSpan.FromHours(12)), weight:2),
                    new QuipGroup("Afternoon", count:1, rule:new DailyRangeRule(TimeSpan.FromHours(12), TimeSpan.FromHours(15)), weight:2),
                    new QuipGroup("Evening", count:1, rule:new DailyRangeRule(TimeSpan.FromHours(15), TimeSpan.FromHours(21)), weight:2),
                    new QuipGroup("Night", count:1, rule:new DailyRangeRule(TimeSpan.FromHours(21), TimeSpan.FromHours(0)), weight:2),
                }
            },
            {
                "en-us",
                new QuipGroup[]
                {
                    new QuipGroup("July4th", count:2, rule:new AnnualRangeRule(new DateTime(1, 7, 4), new DateTime(1, 7, 5)), weight:10),
                }
            },
            {
                "he",
                new QuipGroup[]
                {
                    new QuipGroup("Generic", count:3, rule:null),
                    new QuipGroup("Saturday", count:1, rule:new WeeklyRule(DayOfWeek.Saturday)),
                    new QuipGroup("Sunday", count:1, rule:new WeeklyRule(DayOfWeek.Sunday)),
                    new QuipGroup("Morning", count:1, rule:new DailyRangeRule(TimeSpan.FromHours(7), TimeSpan.FromHours(12)), weight:2),
                    new QuipGroup("Afternoon", count:1, rule:new DailyRangeRule(TimeSpan.FromHours(12), TimeSpan.FromHours(15)), weight:2),
                    new QuipGroup("Evening", count:1, rule:new DailyRangeRule(TimeSpan.FromHours(15), TimeSpan.FromHours(21)), weight:2),
                    new QuipGroup("Night", count:1, rule:new DailyRangeRule(TimeSpan.FromHours(21), TimeSpan.FromHours(0)), weight:2),
                }
            },
        };

        private int _sumWeight = 0;

        private List<QuipGroup> _activeQuipGroups;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuipLoader"/> class.
        /// </summary>
        /// <param name="languageCode">The language code for quips to get.</param>
        public QuipLoader(string languageCode)
        {
            // TODO: Region codes
            languageCode = languageCode.Substring(0, 2);
            QuipGroup[] groups = _languageSpecialDayMap[languageCode];

            _activeQuipGroups = new List<QuipGroup>();
            DateTime now = DateTime.Now;

            foreach (var group in groups)
            {
                if (group.Applies(now))
                {
                    _sumWeight += group.Weight * group.Count;
                    _activeQuipGroups.Add(group);
                }
            }
        }

        /// <summary>
        /// Gets the Group and index in the group of a quip to load.
        /// </summary>
        /// <returns>(Group, index) of the group to load.</returns>
        public (string, int) GetGroupIndexQuip()
        {
            Random rand = new Random();
            int i = rand.Next(_sumWeight);

            foreach (var group in _activeQuipGroups)
            {
                if (i <= (group.Weight * group.Count) - 1)
                {
                    i /= group.Weight;
                    return (group.Name, i);
                } else
                {
                    i -= group.Weight * group.Count;
                }
            }

            return ("Generic", 0);
        }
    }
}
