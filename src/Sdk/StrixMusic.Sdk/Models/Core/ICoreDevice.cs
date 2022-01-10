using System;
using System.Diagnostics.CodeAnalysis;
using StrixMusic.Sdk.Models.Base;

namespace StrixMusic.Sdk.Models.Core
{
    /// <inheritdoc cref="IDeviceBase"/>
    /// <remarks>This interface should be implemented by a core.</remarks>
    public interface ICoreDevice : IDeviceBase, ICoreMember
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
