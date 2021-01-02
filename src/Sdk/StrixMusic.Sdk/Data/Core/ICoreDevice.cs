using System;
using System.Diagnostics.CodeAnalysis;
using StrixMusic.Sdk.Data.Base;

namespace StrixMusic.Sdk.Data.Core
{
    /// <inheritdoc cref="IDeviceBase"/>
    /// <remarks>This interface should be implemented by a core.</remarks>
    [SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity", Justification = "Nullable SourceCore in IDeviceBase is overridden by non-nullable SourceCore in ICoreMember.")]
    public interface ICoreDevice : IDeviceBase, ICoreMember
    {
        /// <summary>
        /// Tracks that have been queued to play next after the current track. Once this queue is exhausted, the next track in the <see cref="IDeviceBase.PlaybackContext"/> will play.
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
