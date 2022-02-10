using StrixMusic.Sdk.Services;

namespace StrixMusic.Sdk.Models.Merged
{
    /// <summary>
    /// The different ways that the items in a merged collection are returned from multiple sources.
    /// </summary>
    public enum MergedCollectionSorting
    {
        /// <summary>
        /// The items are ranked by user preference.
        /// </summary>
        /// <seealso cref="SettingsKeys.CoreRanking"/>
        Ranked,

        /// <summary>
        /// Sources are interwoven so that item N and item N+1 aren't from the same source, unless all other sources are exhausted.
        /// </summary>
        Alternating,
    }
}