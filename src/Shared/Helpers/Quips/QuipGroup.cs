using System;
using System.Text.Json.Serialization;
using StrixMusic.Helpers.Quips.TimeSpanRules;
using StrixMusic.Helpers.TimeSpanRules;

namespace StrixMusic.Helpers.Quips
{
    /// <summary>
    /// A collection of quips following the same rules and weights.
    /// </summary>
    public class QuipGroup
    {
        /// <summary>
        /// Gets or sets the name of the quip group.
        /// </summary>
        [JsonPropertyName("Name")]
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the weight of the quip group.
        /// </summary>
        /// <remarks>
        /// The weight is the relative likely-hood a quip from this group has of appearing.
        /// </remarks>
        [JsonPropertyName("Weight")]
        public int Weight { get; set; }

        /// <summary>
        /// Gets or sets the type of rule constraining the quip group.
        /// </summary>
        [JsonPropertyName("TimeSpanType")]
        public RuleType RuleType { get; set; }

        /// <summary>
        /// Gets or sets the rule constraining the quip group.
        /// </summary>
        [JsonPropertyName("TimeSpanRule")]
        public ITimeSpanRule? Rule { get; set; }

        /// <summary>
        /// Gets or sets the quips 
        /// </summary>
        [JsonPropertyName("Quips")]
        public string[]? Quips { get; set; }

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
    }
}
