// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using StrixMusic.Sdk.Models;

namespace StrixMusic.Sdk.MediaPlayback
{
    /// <summary>
    /// Holds data that uniquely identifies an item played from an <see cref="IAudioPlayerService"/>.
    /// </summary>
    public record PlaybackItem
    {
        /// <summary>
        /// The media source to be played.
        /// </summary>
        public IMediaSourceConfig? MediaConfig { get; set; }

        /// <summary>
        /// The track that holds information of from all merged source.
        /// </summary>
        public ITrack? Track { get; set; }
    }
}
