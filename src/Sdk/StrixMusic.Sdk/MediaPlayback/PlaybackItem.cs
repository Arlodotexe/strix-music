// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using StrixMusic.Sdk.AppModels;

namespace StrixMusic.Sdk.MediaPlayback
{
    /// <summary>
    /// Holds data that uniquely identifies an item played from an <see cref="IAudioPlayerService"/>.
    /// </summary>
    public record PlaybackItem
    {
        /// <summary>
        /// The relevant media source that can be used to play the <see cref="Track"/>.
        /// </summary>
        public IMediaSourceConfig? MediaConfig { get; set; }

        /// <summary>
        /// The relevant track.
        /// </summary>
        public ITrack? Track { get; set; }
    }
}
