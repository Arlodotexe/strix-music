using System;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Models.Base;
using StrixMusic.Sdk.Models.Core;

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