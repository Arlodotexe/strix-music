// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

namespace StrixMusic.Sdk.AppModels
{
    /// <summary>
    /// Used to identify what kind of content an audio stream contains.
    /// </summary>
    public enum TrackType
    {
        /// <summary>
        /// A standard song.
        /// </summary>
        Song,

        /// <summary>
        /// An episode of a podcast.
        /// </summary>
        PodcastEpisode,

        /// <summary>
        /// A spoken book.
        /// </summary>
        Audiobook,

        /// <summary>
        /// A continuous audio stream with no determinate end.
        /// </summary>
        RadioOrStream,
    }
}
