using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StrixMusic.CoreInterfaces.Enums;

namespace StrixMusic.CoreInterfaces.Interfaces
{
    /// <summary>
    /// Represents an item that can be played.
    /// </summary>
    public interface IPlayable
    {
        /// <summary>
        /// The source core which created the parent.
        /// </summary>
        public ICore SourceCore { get; }

        /// <summary>
        /// The ID of the playable item.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Name of the playable item.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Cover images for the item.
        /// </summary>
        IReadOnlyList<IImage> Images { get; }

        /// <summary>
        /// Provides comments about the item.
        /// </summary>
        string? Description { get; }

        /// <inheritdoc cref="Enums.PlaybackState"/>
        PlaybackState PlaybackState { get; }

        /// <summary>
        /// Attempts to play the item, or resumes playback if already playing.
        /// </summary>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        Task PlayAsync();

        /// <summary>
        /// Attempts to pause the item.
        /// </summary>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        Task PauseAsync();

        /// <summary>
        /// Fires when <see cref="PlaybackState"/> changes.
        /// </summary>
        event EventHandler<PlaybackState>? PlaybackStateChanged;
    }
}
