// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OwlCore.Events;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Sdk.Models;
using StrixMusic.Sdk.Models.Base;

namespace StrixMusic.Sdk.MediaPlayback
{
    /// <summary>
    /// Manages an internal queue, handles playback, and delegates playback commands to an <see cref="IAudioPlayerService"/>.
    /// </summary>
    public interface IPlaybackHandlerService : IAudioPlayerBase, IDisposable
    {
        /// <summary>
        /// Associating an <see cref="IAudioPlayerService"/> with a specific <see cref="ICore"/>.
        /// </summary>
        /// <param name="audioPlayer">The player that will be used exclusively by the given core.</param>
        /// <param name="instanceId">A core instance ID to associate this audio player with.</param>
        void RegisterAudioPlayer(IAudioPlayerService audioPlayer, string instanceId);

        /// <summary>
        /// The items that should be played next.
        /// </summary>
        IReadOnlyList<PlaybackItem> NextItems { get; }

        /// <summary>
        /// Items that precede the currently playing item. Used to go to the previous track in the playback context.
        /// </summary>
        IReadOnlyCollection<PlaybackItem> PreviousItems { get; }

        /// <summary>
        /// The currently playing item.
        /// </summary>
        PlaybackItem? CurrentItem { get; }

        /// <summary>
        /// True if the player is using a shuffled track list.
        /// </summary>
        bool ShuffleState { get; }

        /// <inheritdoc cref="MediaPlayback.RepeatState"/>
        RepeatState RepeatState { get; }

        /// <summary>
        /// Loads the given track collection into the play queue and plays the given track.
        /// </summary>
        /// <param name="track">The track to play.</param>
        /// <param name="context">The actual playback context.</param>
        /// <param name="trackCollection">The complete track collection that <paramref name="track"/> belongs to.</param>
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PlayAsync(ITrack track, ITrackCollection trackCollection, IPlayableBase context, CancellationToken cancellationToken = default);

        /// <summary>
        /// Loads the given track collection into the play queue and plays the first track.
        /// </summary>
        /// <param name="context">The actual playback context.</param>
        /// <param name="trackCollection">The tracks to use in the queue.</param>
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PlayAsync(ITrackCollection trackCollection, IPlayableBase context, CancellationToken cancellationToken = default);

        /// <summary>
        /// Loads the given album collection into the play queue and plays the first track of the provided <paramref name="albumCollectionItem"/>.
        /// </summary>
        /// <param name="albumCollectionItem">The album item to play.</param>
        /// <param name="albumCollection">The complete album collection that <paramref name="albumCollectionItem"/> belongs to.</param>
        /// <param name="context">The actual playback context.</param>
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PlayAsync(IAlbumCollectionItem albumCollectionItem, IAlbumCollection albumCollection, IPlayableBase context, CancellationToken cancellationToken = default);

        /// <summary>
        /// Loads the given playlist collection into the play queue and plays the given track.
        /// </summary>
        /// <param name="playlistCollectionItem">The track to play.</param>
        /// <param name="playlistCollection">The complete playlist collection that <paramref name="playlistCollectionItem"/> belongs to.</param>
        /// <param name="context">The actual playback context.</param>
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PlayAsync(IPlaylistCollectionItem playlistCollectionItem, IPlaylistCollection playlistCollection, IPlayableBase context, CancellationToken cancellationToken = default);

        /// <summary>
        /// Loads the given track collection into the play queue and plays the first track.
        /// </summary>
        /// <param name="context">The actual playback context.</param>
        /// <param name="albumCollection">The albums to use in the queue.</param>
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PlayAsync(IAlbumCollection albumCollection, IPlayableBase context, CancellationToken cancellationToken = default);

        /// <summary>
        /// Loads the given artist collection into the play queue and plays the given track.
        /// </summary>
        /// <param name="artistCollection">The album collection to play.</param>
        /// <param name="context">The playback context.</param>
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PlayAsync(IArtistCollection artistCollection, IPlayableBase context, CancellationToken cancellationToken = default);

        /// <summary>
        /// Loads the given playlist collection into the play queue and plays the given track.
        /// </summary>
        /// <param name="playlistCollection">The playlist collection to play.</param>
        /// <param name="context">The actual playback context.</param>
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PlayAsync(IPlaylistCollection playlistCollection, IPlayableBase context, CancellationToken cancellationToken = default);

        /// <summary>
        /// Loads the given track collection into the play queue and plays the first track.
        /// </summary>
        /// <param name="context">The actual playback context.</param>
        /// <param name="artistCollectionItem">The artist item to play.</param>
        /// <param name="artistCollection">The complete artist collection that <paramref name="artistCollectionItem"/> belongs to.</param>
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PlayAsync(IArtistCollectionItem artistCollectionItem, IArtistCollection artistCollection, IPlayableBase context, CancellationToken cancellationToken = default);

        /// <summary>
        /// Plays a specific media from <see cref="NextItems"/>.
        /// </summary>
        /// <param name="queueIndex">The index of an item in <see cref="NextItems"/>.</param>
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PlayFromNext(int queueIndex, CancellationToken cancellationToken = default);

        /// <summary>
        /// Plays a specific media from <see cref="PreviousItems"/>.
        /// </summary>
        /// <param name="queueIndex">The index of an item in <see cref="PreviousItems"/>.</param>
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PlayFromPrevious(int queueIndex, CancellationToken cancellationToken = default);

        /// <summary>
        /// Advances to the next track. If there is no next track, playback is paused.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task NextAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Goes to the previous track.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PreviousAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Inserts an item into the <see cref="NextItems"/>.
        /// </summary>
        /// <param name="index">The index to insert the item at.</param>
        /// <param name="sourceConfig">The item to insert.</param>
        void InsertNext(int index, PlaybackItem sourceConfig);

        /// <summary>
        /// Removes an item from the <see cref="NextItems"/>.
        /// </summary>
        /// <param name="index">The index to insert the item at.</param>
        void RemoveNext(int index);

        /// <summary>
        /// Clears all items from <see cref="NextItems"/>.
        /// </summary>
        void ClearNext();

        /// <summary>
        /// Adds an item to the top of <see cref="PreviousItems"/>.
        /// </summary>
        /// <param name="sourceConfig">The item to insert.</param>
        void PushPrevious(PlaybackItem sourceConfig);

        /// <summary>
        /// Removes and returns item from top of the <see cref="PreviousItems"/> stack.
        /// </summary>
        /// <param name="index">The index to insert the item at.</param>
        /// <returns>The <see cref="IMediaSourceConfig"/> that was in the requested index.</returns>
        PlaybackItem PopPrevious(int index);

        /// <summary>
        /// Clears all items from <see cref="PreviousItems"/>.
        /// </summary>
        void ClearPrevious();

        /// <summary>
        /// Toggles shuffle on or off.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ToggleShuffleAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Asks the device to toggle to the next repeat state.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ToggleRepeatAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets the repeat state to a specific state.
        /// </summary>
        /// <param name="state">The new repeat state.</param>
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SetRepeatStateAsync(RepeatState state, CancellationToken cancellationToken = default);

        /// <summary>
        /// Fires when <see cref="ShuffleState"/> changes.
        /// </summary>
        event EventHandler<bool> ShuffleStateChanged;

        /// <summary>
        /// Fires when <see cref="RepeatState"/> changes.
        /// </summary>
        event EventHandler<RepeatState> RepeatStateChanged;

        /// <summary>
        /// Fires when the <see cref="NextItems"/> are updated.
        /// </summary>
        event CollectionChangedEventHandler<PlaybackItem>? NextItemsChanged;

        /// <summary>
        /// Fires when the <see cref="PreviousItems"/> are updated.
        /// </summary>
        event CollectionChangedEventHandler<PlaybackItem>? PreviousItemsChanged;

        /// <summary>
        /// Fires when the <see cref="CurrentItem"/> is changed.
        /// </summary>
        event EventHandler<PlaybackItem?>? CurrentItemChanged;

        /// <summary>
        /// Raised when a quantum of data is processed. 
        /// </summary>
         event EventHandler<float[]>? QuantumProcessed;
    }
}
