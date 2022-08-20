// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Sdk.BaseModels
{
    /// <summary>
    /// Represents an item that can be played.
    /// </summary>
    public interface IPlayableBase : IImageCollectionBase, IUrlCollectionBase
    {
        /// <summary>
        /// The ID of the playable item.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Name of the playable item.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Provides comments about the item. This may contain markdown content.
        /// </summary>
        string? Description { get; }

        /// <summary>
        /// The last time the item was played. If never played or unknown, value is null.
        /// </summary>
        DateTime? LastPlayed { get; }

        /// <inheritdoc cref="MediaPlayback.PlaybackState"/>
        PlaybackState PlaybackState { get; }

        /// <summary>
        /// How long the playable item takes to complete playback.
        /// </summary>
        /// <remarks>If not applicable, use <see cref="TimeSpan.Zero"/>.</remarks>
        TimeSpan Duration { get; }

        /// <summary>
        /// If true, <see cref="ChangeNameAsync(string, CancellationToken)"/> can be used.
        /// </summary>
        bool IsChangeNameAsyncAvailable { get; }

        /// <summary>
        /// If true, <see cref="ChangeDescriptionAsync(string, CancellationToken)"/> can be used.
        /// </summary>
        bool IsChangeDescriptionAsyncAvailable { get; }

        /// <summary>
        /// If true, <see cref="ChangeDurationAsync(TimeSpan, CancellationToken)"/> can be used.
        /// </summary>
        bool IsChangeDurationAsyncAvailable { get; }

        /// <summary>
        /// Changes the <see cref="Name"/> of this playable item.
        /// </summary>
        /// <param name="name">The new name to use.</param>
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ChangeNameAsync(string name, CancellationToken cancellationToken = default);

        /// <summary>
        /// Changes the <see cref="Description"/> for this item.
        /// </summary>
        /// <param name="description">The new description for this playable item.</param>
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ChangeDescriptionAsync(string? description, CancellationToken cancellationToken = default);

        /// <summary>
        /// Changes the <see cref="Duration"/> for this item.
        /// </summary>
        /// <param name="duration">The new duration for this playable item.</param>
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ChangeDurationAsync(TimeSpan duration, CancellationToken cancellationToken = default);

        /// <summary>
        /// Raised when <see cref="PlaybackState"/> changes.
        /// </summary>
        event EventHandler<PlaybackState>? PlaybackStateChanged;

        /// <summary>
        /// Raised when <see cref="Name"/> changes.
        /// </summary>
        event EventHandler<string>? NameChanged;

        /// <summary>
        /// Raised when <see cref="Description"/> changes.
        /// </summary>
        event EventHandler<string?>? DescriptionChanged;

        /// <summary>
        /// Raised when <see cref="Duration"/> changes;
        /// </summary>
        event EventHandler<TimeSpan>? DurationChanged;

        /// <summary>
        /// Raised when <see cref="LastPlayed"/> changes.
        /// </summary>
        event EventHandler<DateTime?>? LastPlayedChanged;

        /// <summary>
        /// Raised when <see cref="IsChangeNameAsyncAvailable"/> changes.
        /// </summary>
        event EventHandler<bool>? IsChangeNameAsyncAvailableChanged;

        /// <summary>
        /// Raised when <see cref="IsChangeDescriptionAsyncAvailable"/> changes.
        /// </summary>
        event EventHandler<bool>? IsChangeDescriptionAsyncAvailableChanged;

        /// <summary>
        /// Raised when <see cref="IsChangeDurationAsyncAvailable"/> changes.
        /// </summary>
        event EventHandler<bool>? IsChangeDurationAsyncAvailableChanged;
    }
}
