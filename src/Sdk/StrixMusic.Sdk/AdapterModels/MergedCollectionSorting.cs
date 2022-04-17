// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

namespace StrixMusic.Sdk.AdapterModels
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
