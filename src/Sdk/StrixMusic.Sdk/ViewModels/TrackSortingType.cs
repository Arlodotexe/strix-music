// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// Enumeration for sort types of tracks.
    /// </summary>
    [Flags]
    public enum TrackSortingType
    {
        /// <summary>
        /// Default order of the collection.
        /// </summary>
        Unsorted,

        /// <summary>
        /// Sort tracks by name.
        /// </summary>
        Alphanumerical,

        /// <summary>
        /// Sort tracks by track number.
        /// </summary>
        TrackNumber,

        /// <summary>
        /// Sort tracks by date added to collection.
        /// </summary>
        DateAdded,

        /// <summary>
        /// Sort tracks by duration.
        /// </summary>
        Duration,
    }
}