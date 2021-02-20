using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OwlCore.Events;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.ViewModels;

namespace StrixMusic.Sdk.Services.MediaPlayback
{
    /// <summary>
    /// Manages an internal queue, handles playback, and delegates playback commands to an <see cref="IAudioPlayerService"/>.
    /// </summary>
    public interface IPlaybackHandlerService : IAudioPlayerBase
    {
        /// <summary>
        /// Associating an <see cref="IAudioPlayerService"/> with a specific <see cref="ICore"/>.
        /// </summary>
        /// <param name="audioPlayer">The player that will be used exclusively by the given core.</param>
        /// <param name="core">The core to associate the audio player with.</param>
        void RegisterAudioPlayer(IAudioPlayerService audioPlayer, ICore core);

        /// <summary>
        /// The items that should be played next.
        /// </summary>
        IReadOnlyList<IMediaSourceConfig> NextItems { get; }

        /// <summary>
        /// Items that precede the currently playing item. Used to go to the previous track in the playback context.
        /// </summary>
        IReadOnlyCollection<IMediaSourceConfig> PreviousItems { get; }

        /// <summary>
        /// The currently playing item.
        /// </summary>
        IMediaSourceConfig? CurrentItem { get; }

        /// <summary>
        /// True if the player is using a shuffled track list.
        /// </summary>
        bool ShuffleState { get; }

        /// <inheritdoc cref="Sdk.MediaPlayback.RepeatState"/>
        RepeatState RepeatState { get; }

        /// <summary>
        /// Loads the given track collection into the play queue and plays the given track.
        /// </summary>
        /// <param name="track">The track to play.</param>
        /// <param name="context">The playback context.</param>
        /// <param name="trackCollection">The tracks to use in the queue.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PlayAsync(ITrack track, IPlayableBase context, ITrackCollectionViewModel trackCollection);

        /// <summary>
        /// Plays a specific media from <see cref="NextItems"/>.
        /// </summary>
        /// <param name="queueIndex">The index of an item in <see cref="NextItems"/>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PlayFromNext(int queueIndex);

        /// <summary>
        /// Plays a specific media from <see cref="PreviousItems"/>.
        /// </summary>
        /// <param name="queueIndex">The index of an item in <see cref="PreviousItems"/>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PlayFromPrevious(int queueIndex);

        /// <summary>
        /// Advances to the next track. If there is no next track, playback is paused.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task NextAsync();

        /// <summary>
        /// Goes to the previous track.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PreviousAsync();

        /// <summary>
        /// Inserts an item into the <see cref="NextItems"/>.
        /// </summary>
        /// <param name="index">The index to insert the item at.</param>
        /// <param name="sourceConfig">The item to insert.</param>
        void InsertNext(int index, IMediaSourceConfig sourceConfig);

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
        void PushPrevious(IMediaSourceConfig sourceConfig);

        /// <summary>
        /// Removes and returns item from top of the <see cref="PreviousItems"/> stack.
        /// </summary>
        /// <param name="index">The index to insert the item at.</param>
        /// <returns>The <see cref="IMediaSourceConfig"/> that was in the requested index.</returns>
        IMediaSourceConfig PopPrevious(int index);

        /// <summary>
        /// Clears all items from <see cref="PreviousItems"/>.
        /// </summary>
        void ClearPrevious();

        /// <summary>
        /// Toggles shuffle on or off.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ToggleShuffleAsync();

        /// <summary>
        /// Asks the device to toggle to the next repeat state.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ToggleRepeatAsync();

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
        event CollectionChangedEventHandler<IMediaSourceConfig>? NextItemsChanged;

        /// <summary>
        /// Fires when the <see cref="PreviousItems"/> are updated.
        /// </summary>
        event CollectionChangedEventHandler<IMediaSourceConfig>? PreviousItemsChanged;

        /// <summary>
        /// Fires when the <see cref="CurrentItem"/> is changed.
        /// </summary>
        event EventHandler<IMediaSourceConfig>? CurrentItemChanged;

        /// <summary>
        /// Raised when a quantum of data is processed. 
        /// </summary>
        public event EventHandler<float[]>? QuantumProcessed;
    }
}