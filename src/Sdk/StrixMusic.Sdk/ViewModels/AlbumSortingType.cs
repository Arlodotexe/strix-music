// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// Enumeration for sort types of tracks.
    /// </summary>
    public enum AlbumSortingType
    {
        /// <summary>
        /// Default order of the collection.
        /// </summary>
        Unsorted,

        /// <summary>
        /// Sort albums by name.
        /// </summary>
        Alphanumerical,

        /// <summary>
        /// Sort albums by date added to collection.
        /// </summary>
        DateAdded,

        /// <summary>
        /// Sort albums by duration.
        /// </summary>
        Duration,

        /// <summary>
        /// Sort albums by last played.
        /// </summary>
        LastPlayed,

        /// <summary>
        /// Sort albums by date published.
        /// </summary>
        DatePublished,
    }
}
