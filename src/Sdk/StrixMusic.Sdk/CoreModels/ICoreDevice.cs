// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using StrixMusic.Sdk.BaseModels;

namespace StrixMusic.Sdk.CoreModels
{
    /// <summary>
    /// A device that controls playback of an audio player.
    /// </summary>
    /// <remarks>This interface should be implemented by a core.</remarks>
    public interface ICoreDevice : IDeviceBase, ICoreModel
    {
        /// <summary>
        /// A collection of all tracks that have been queued to play, including <see cref="NowPlaying"/>.
        /// </summary>
        ICoreTrackCollection? PlaybackQueue { get; }

        /// <summary>
        /// The currently playing <see cref="ICoreTrack"/>.
        /// </summary>
        ICoreTrack? NowPlaying { get; }

        /// <summary>
        /// Fires when <see cref="NowPlaying"/> changes.
        /// </summary>
        event EventHandler<ICoreTrack>? NowPlayingChanged;
    }
}
