using System;

namespace StrixMusic.Sdk.Core.Data
{
    /// <inheritdoc cref="IDeviceBase"/>
    /// <remarks>This interface should be implemented by the Sdk.</remarks>
    public interface IDevice : IDeviceBase, ISdkMember
    {
        /// <summary>
        /// Tracks that have been queued to play next after the current track. Once this queue is exhausted, the next track in the <see cref="IDeviceBase.PlaybackContext"/> will play.
        /// </summary>
        ITrackCollection PlaybackQueue { get; }

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