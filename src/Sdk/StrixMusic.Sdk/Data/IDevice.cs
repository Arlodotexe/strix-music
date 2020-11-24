using System;
using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Sdk.Data
{
    /// <inheritdoc cref="IDeviceBase"/>
    /// <remarks>This interface should be implemented by the Sdk.</remarks>
    public interface IDevice : IDeviceBase, ISdkMember<ICoreDevice>
    {
        /// <summary>
        /// Tracks that have been queued to play next after the current track. Once this queue is exhausted, the next track in the <see cref="IDeviceBase.PlaybackContext"/> will play.
        /// </summary>
        ITrackCollection? PlaybackQueue { get; }

        /// <summary>
        /// The currently playing <see cref="ITrack"/>.
        /// </summary>
        ITrack? NowPlaying { get; }

        /// <summary>
        /// Fires when <see cref="NowPlaying"/> changes.
        /// </summary>
        event EventHandler<ITrack> NowPlayingChanged;
    }
}