using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StrixMusic.Sdk.Enums;
using StrixMusic.Sdk.Events;

namespace StrixMusic.Sdk.Interfaces
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
        /// An external link related to the item.
        /// </summary>
        Uri? Url { get; }

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
        /// How long the playable item takes to complete playback.
        /// </summary>
        TimeSpan Duration { get; }

        /// <summary>
        /// If true, <see cref="PlayAsync()"/> is supported.
        /// </summary>
        bool IsPlayAsyncSupported { get; }

        /// <summary>
        /// If true, <see cref="PauseAsync()"/> is supported.
        /// </summary>
        bool IsPauseAsyncSupported { get; }

        /// <summary>
        /// If true, <see cref="ChangeNameAsync(string)"/> is supported.
        /// </summary>
        bool IsChangeNameAsyncSupported { get; }

        /// <summary>
        /// If true, <see cref="ChangeImagesAsync(IReadOnlyList{IImage})"/> is supported.
        /// </summary>
        bool IsChangeImagesAsyncSupported { get; }

        /// <summary>
        /// If true, <see cref="ChangeDescriptionAsync(string)"/> is supported.
        /// </summary>
        bool IsChangeDescriptionAsyncSupported { get; }

        /// <summary>
        /// If true, <see cref="ChangeDurationAsync(TimeSpan)"/> is supported.
        /// </summary>
        bool IsChangeDurationAsyncSupported { get; }

        /// <summary>
        /// Attempts to play the item, or resumes playback if already playing.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PlayAsync();

        /// <summary>
        /// Attempts to pause the item.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PauseAsync();

        /// <summary>
        /// Changes the <see cref="Name"/> of this playable item.
        /// </summary>
        /// <param name="name">The new name to use.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ChangeNameAsync(string name);

        /// <summary>
        /// Changes the <see cref="Images"/> for this playable item.
        /// </summary>
        /// <param name="images">The new images for this item.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ChangeImagesAsync(IReadOnlyList<IImage> images);

        /// <summary>
        /// Changes the <see cref="Description"/> for this item.
        /// </summary>
        /// <param name="description">The new description for this playable item.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ChangeDescriptionAsync(string? description);

        /// <summary>
        /// Changes the <see cref="Duration"/> for this item.
        /// </summary>
        /// <param name="duration">The new duration for this playable item.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ChangeDurationAsync(TimeSpan duration);

        /// <summary>
        /// Fires when <see cref="PlaybackState"/> changes.
        /// </summary>
        event EventHandler<PlaybackState> PlaybackStateChanged;

        /// <summary>
        /// Fires when <see cref="Name"/> changes.
        /// </summary>
        event EventHandler<string> NameChanged;

        /// <summary>
        /// Fires when <see cref="Description"/> changes.
        /// </summary>
        event EventHandler<string?> DescriptionChanged;

        /// <summary>
        /// Fires when <see cref="Url"/> changes.
        /// </summary>
        event EventHandler<Uri?> UrlChanged;

        /// <summary>
        /// Fires when <see cref="Images"/> changes;
        /// </summary>
        event EventHandler<CollectionChangedEventArgs<IImage>>? ImagesChanged;

        /// <summary>
        /// Fires when <see cref="Duration"/> changes;
        /// </summary>
        event EventHandler<TimeSpan>? DurationChanged;
    }
}
