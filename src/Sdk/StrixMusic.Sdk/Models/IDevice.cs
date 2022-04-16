// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using StrixMusic.Sdk.BaseModels;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Sdk.Models
{
    /// <summary>
    /// A device that controls playback of an audio player.
    /// </summary>
    public interface IDevice : IDeviceBase, ISdkMember
    {
        /// <summary>
        /// The core that created this device, if any.
        /// </summary>
        ICore? SourceCore { get; }

        /// <summary>
        /// The original <see cref="ICoreDevice"/> implementation, if any.
        /// </summary>
        ICoreDevice? Source { get; }

        /// <summary>
        /// The complete list of tracks that are queued to play.
        /// </summary>
        ITrackCollection? PlaybackQueue { get; }

        /// <summary>
        /// The currently playing track.
        /// </summary>
        PlaybackItem? NowPlaying { get; }

        /// <summary>
        /// Fires when <see cref="NowPlaying"/> changes.
        /// </summary>
        event EventHandler<PlaybackItem>? NowPlayingChanged;
    }
}
