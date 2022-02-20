// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// Enumeration for sort types of playlists.
    /// </summary>
    public enum PlaylistSortingType
    {
        /// <summary>
        /// Default order of the collection.
        /// </summary>
        Unsorted,

        /// <summary>
        /// Sort playlists by name.
        /// </summary>
        Alphanumerical,

        /// <summary>
        /// Sort playlists by date added to collection.
        /// </summary>
        DateAdded,

        /// <summary>
        /// Sort playlists by duration.
        /// </summary>
        Duration,

        /// <summary>
        /// Sort playlists by last played.
        /// </summary>
        LastPlayed,
    }
}
