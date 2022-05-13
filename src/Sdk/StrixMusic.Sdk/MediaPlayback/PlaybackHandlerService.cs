// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using OwlCore.Events;
using OwlCore.Extensions;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.BaseModels;
using StrixMusic.Sdk.MediaPlayback.LocalDevice;

namespace StrixMusic.Sdk.MediaPlayback
{
    /// <summary>
    /// Manages an internal queue, handles playback, and delegates playback commands to an <see cref="IAudioPlayerService"/>.
    /// </summary>
    public sealed partial class PlaybackHandlerService : IPlaybackHandlerService
    {
        private readonly Dictionary<string, IAudioPlayerService> _audioPlayerRegistry = new();
        private readonly List<PlaybackItem> _prevItems = new();
        private readonly List<PlaybackItem> _nextItems = new();

        private int[] _shuffleMap;
        private IAudioPlayerService? _currentPlayerService;
        private RepeatState _repeatState;
        private bool _shuffleState;
        private StrixDevice _localDevice;

        /// <summary>
        /// Creates a new instance of <see cref="PlaybackHandlerService"/>.
        /// </summary>
        public PlaybackHandlerService()
        {
            _shuffleMap = Array.Empty<int>();
            _localDevice = new StrixDevice(this);
        }

        private void AttachEvents(IAudioPlayerService audioPlayerService)
        {
            audioPlayerService.PositionChanged += PositionChanged;
            audioPlayerService.PlaybackSpeedChanged += PlaybackSpeedChanged;
            audioPlayerService.PlaybackStateChanged += CurrentPlayerService_PlaybackStateChanged;
            audioPlayerService.CurrentSourceChanged += CurrentPlayerService_CurrentSourceChanged;
            audioPlayerService.VolumeChanged += VolumeChanged;
            audioPlayerService.QuantumProcessed += QuantumProcessed;
        }

        private void DetachEvents(IAudioPlayerService audioPlayerService)
        {
            audioPlayerService.PositionChanged -= PositionChanged;
            audioPlayerService.PlaybackSpeedChanged -= PlaybackSpeedChanged;
            audioPlayerService.PlaybackStateChanged -= CurrentPlayerService_PlaybackStateChanged;
            audioPlayerService.CurrentSourceChanged -= CurrentPlayerService_CurrentSourceChanged;
            audioPlayerService.VolumeChanged -= VolumeChanged;
            audioPlayerService.QuantumProcessed -= QuantumProcessed;
        }

        private void CurrentPlayerService_CurrentSourceChanged(object sender, PlaybackItem? e)
        {
            CurrentItem = e;
            CurrentItemChanged?.Invoke(this, e);
        }

        private async void CurrentPlayerService_PlaybackStateChanged(object sender, PlaybackState e)
        {
            // Since the player itself can't be queued, we use this as a sentinel value for advancing the queue.
            if (e == PlaybackState.Loaded)
            {
                await AutoAdvanceQueue();
            }

            PlaybackStateChanged?.Invoke(this, e);
        }

        private async Task AutoAdvanceQueue()
        {
            switch (_repeatState)
            {
                case RepeatState.All when NextItems.Count == 0:
                    // Move all items from previous back into Next
                    _nextItems.AddRange(_prevItems);
                    _prevItems.Clear();

                    await PlayFromNext(0);
                    return;
                case RepeatState.One:
                    await NextAsync();
                    return;
                case RepeatState.None:
                    await NextAsync();
                    break;
                default:
                    await NextAsync();
                    break;
            }
        }

        /// <inheritdoc />
        public event EventHandler<RepeatState>? RepeatStateChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? ShuffleStateChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<PlaybackItem>? NextItemsChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<PlaybackItem>? PreviousItemsChanged;

        /// <inheritdoc />
        public event EventHandler<PlaybackItem?>? CurrentItemChanged;

        /// <inheritdoc />
        public event EventHandler<TimeSpan>? PositionChanged;

        /// <inheritdoc />
        public event EventHandler<PlaybackState>? PlaybackStateChanged;

        /// <inheritdoc />
        public event EventHandler<double>? VolumeChanged;

        /// <inheritdoc />
        public event EventHandler<double>? PlaybackSpeedChanged;

        /// <inheritdoc />
        public event EventHandler<float[]>? QuantumProcessed;

        /// <summary>
        /// Gets or sets the device which is being currently being used for playback, if any.
        /// </summary>
        public IDevice? ActiveDevice { get; set; }

        /// <summary>
        /// Gets a device which represents all local playback done by this <see cref="IPlaybackHandlerService"/>.
        /// </summary>
        public IDevice LocalDevice => _localDevice;

        /// <inheritdoc />
        public IReadOnlyList<PlaybackItem> NextItems => _nextItems;

        /// <inheritdoc />
        public IReadOnlyCollection<PlaybackItem> PreviousItems => _prevItems;

        /// <inheritdoc />
        public PlaybackItem? CurrentItem { get; internal set; }

        /// <summary>
        /// The collection which the <see cref="CurrentItem"/> is playing from.
        /// </summary>
        public IPlayableBase? CurrentItemContext { get; private set; }

        /// <inheritdoc />
        public bool ShuffleState => _shuffleState;

        /// <inheritdoc />
        public RepeatState RepeatState => _repeatState;

        /// <inheritdoc />
        public TimeSpan Position => _currentPlayerService?.Position ?? TimeSpan.Zero;

        /// <inheritdoc />
        public PlaybackState PlaybackState => _currentPlayerService?.PlaybackState ?? PlaybackState.None;

        /// <inheritdoc />
        public double Volume => _currentPlayerService?.Volume ?? 1;

        /// <inheritdoc />
        public double PlaybackSpeed => _currentPlayerService?.PlaybackSpeed ?? 1;

        /// <inheritdoc />
        public void RegisterAudioPlayer(IAudioPlayerService audioPlayer, string instanceId) => _audioPlayerRegistry.Add(instanceId, audioPlayer);

        /// <inheritdoc />
        public Task SeekAsync(TimeSpan position, CancellationToken cancellationToken = default) => _currentPlayerService?.SeekAsync(position, cancellationToken) ?? Task.CompletedTask;

        /// <inheritdoc />
        public Task ChangePlaybackSpeedAsync(double speed, CancellationToken cancellationToken = default) => _currentPlayerService?.ChangePlaybackSpeedAsync(speed, cancellationToken) ?? Task.CompletedTask;

        /// <inheritdoc />
        public Task ResumeAsync(CancellationToken cancellationToken = default) => _currentPlayerService?.ResumeAsync(cancellationToken) ?? Task.CompletedTask;

        /// <inheritdoc />
        public Task PauseAsync(CancellationToken cancellationToken = default) => _currentPlayerService?.PauseAsync(cancellationToken) ?? Task.CompletedTask;

        /// <inheritdoc />
        public Task ChangeVolumeAsync(double volume, CancellationToken cancellationToken = default) => _currentPlayerService?.ResumeAsync(cancellationToken) ?? Task.CompletedTask;

        /// <inheritdoc />
        public async Task PlayFromNext(int queueIndex, CancellationToken cancellationToken = default)
        {
            if (_currentPlayerService != null)
            {
                await _currentPlayerService.PauseAsync(cancellationToken);
                DetachEvents(_currentPlayerService);
            }

            var playbackItem = NextItems.ElementAtOrDefault(queueIndex);
            if (playbackItem is null)
                return;

            Guard.IsNotNull(playbackItem.MediaConfig, nameof(playbackItem.MediaConfig));

            _currentPlayerService = _audioPlayerRegistry[playbackItem.MediaConfig.Track.SourceCore.InstanceId];
            AttachEvents(_currentPlayerService);

            if (CurrentItem != null)
                _prevItems.Add(CurrentItem);

            CurrentItem = playbackItem;
            _nextItems.Remove(playbackItem);

            if (ActiveDevice == LocalDevice)
            {
                Guard.IsNotNull(playbackItem.MediaConfig, nameof(playbackItem.MediaConfig));
                _localDevice.SetPlaybackData(CurrentItemContext, playbackItem);
            }

            await _currentPlayerService.Play(playbackItem, cancellationToken);
        }

        /// <inheritdoc />
        public async Task PlayFromPrevious(int queueIndex, CancellationToken cancellationToken = default)
        {
            Guard.IsNotNull(_currentPlayerService, nameof(_currentPlayerService));

            var playbackItem = PreviousItems.ElementAtOrDefault(queueIndex);

            Guard.IsNotNull(playbackItem, nameof(playbackItem));
            Guard.IsNotNull(playbackItem.MediaConfig, nameof(playbackItem.MediaConfig));

            await _currentPlayerService.PauseAsync(cancellationToken);
            DetachEvents(_currentPlayerService);

            _currentPlayerService = _audioPlayerRegistry[playbackItem.MediaConfig.Track.SourceCore.InstanceId];
            AttachEvents(_currentPlayerService);

            // TODO shift queue, move tracks after the played item into next
            await _currentPlayerService.Play(playbackItem, cancellationToken);
        }

        /// <inheritdoc />
        public async Task NextAsync(CancellationToken cancellationToken = default)
        {
            if (_currentPlayerService == null && CurrentItem != null)
            {
                Guard.IsNotNull(CurrentItem.MediaConfig, nameof(CurrentItem.MediaConfig));
                _currentPlayerService = _audioPlayerRegistry[CurrentItem.MediaConfig.Track.SourceCore.InstanceId];
            }

            Guard.IsNotNull(_currentPlayerService?.CurrentSource, nameof(_currentPlayerService.CurrentSource));

            var nextIndex = 0;

            await _currentPlayerService.PauseAsync(cancellationToken);
            DetachEvents(_currentPlayerService);

            if (RepeatState == RepeatState.All && NextItems.Count == 0)
            {
                // Move all items from previous back into Next
                _nextItems.AddRange(_prevItems);
                _prevItems.Clear();
            }

            if (NextItems.Count <= nextIndex)
                return;

            PlaybackItem? nextItem;

            if (RepeatState == RepeatState.One && CurrentItem is not null)
            {
                nextItem = CurrentItem;
            }
            else
            {
                nextItem = NextItems[nextIndex];
                CurrentItem = nextItem;

                // Move NowPlaying into previous
                _prevItems.Add(_currentPlayerService.CurrentSource);

                // Take the next item out of the queue (becomes NowPlaying)
                _nextItems.Remove(nextItem);
            }

            var removedItems = new List<CollectionChangedItem<PlaybackItem>>()
                {
                    new(nextItem, nextIndex),
                };

            var addedItems = Array.Empty<CollectionChangedItem<PlaybackItem>>();

            NextItemsChanged?.Invoke(this, addedItems, removedItems);

            var instanceId = CurrentItem?.MediaConfig?.Track.SourceCore.InstanceId;
            Guard.IsNotNull(instanceId, nameof(instanceId));

            _currentPlayerService = _audioPlayerRegistry[instanceId];
            AttachEvents(_currentPlayerService);

            await _currentPlayerService.Play(nextItem, cancellationToken);
            _currentPlayerService.CurrentSource = CurrentItem;

            CurrentItemChanged?.Invoke(this, nextItem);
        }

        /// <inheritdoc />
        public Task PreviousAsync(CancellationToken cancellationToken = default) => PreviousAsync(true);

        /// <inheritdoc />
        public void InsertNext(int index, PlaybackItem sourceConfig)
        {
            var addedItems = new List<CollectionChangedItem<PlaybackItem>>()
            {
                new(sourceConfig, index),
            };

            var removedItems = Array.Empty<CollectionChangedItem<PlaybackItem>>();

            _nextItems.InsertOrAdd(index, sourceConfig);

            // Handle case when the list is shuffled.
            if (_shuffleState)
            {
                var originalIndex = _prevItems.Count + index + (CurrentItem == null ? 0 : 1);

                // Needs to be converted to list because InsertOrAdd isn't supported on the fixed number of collection such as arrays.
                var shuffleList = _shuffleMap.ToList();
                shuffleList.InsertOrAdd(originalIndex, originalIndex);
                _shuffleMap = shuffleList.ToArray();

                for (int i = 0; i < _shuffleMap.Length; i++)
                {
                    // Adjust the all indexes for all elements with the original index greater than or equal to the newly added index.
                    if (_shuffleMap[i] >= originalIndex && i != originalIndex)
                        _shuffleMap[i]++;
                }
            }

            NextItemsChanged?.Invoke(this, addedItems, removedItems);
        }

        /// <inheritdoc />
        public void RemoveNext(int index)
        {
            var removedItems = new List<CollectionChangedItem<PlaybackItem>>()
            {
                new(NextItems[index], index),
            };

            var addedItems = Array.Empty<CollectionChangedItem<PlaybackItem>>();

            _nextItems.RemoveAt(index);

            // Handle case when the list is shuffled.
            if (_shuffleState)
            {
                var indexInShuffledList = _prevItems.Count + index + (CurrentItem == null ? 0 : 1);
                var originalIndex = _shuffleMap[indexInShuffledList];

                // Needs to be converted to list so we can remove an element from the array using the index.
                // After removing the element, we're decrementing all original indexes in the shufflemap greater than the original index of the removed element, so the tracks can be unshuffled correctly.
                var shuffleList = _shuffleMap.ToList();
                shuffleList.RemoveAt(indexInShuffledList);
                _shuffleMap = shuffleList.ToArray();

                for (int i = 0; i < _shuffleMap.Length; i++)
                {
                    if (_shuffleMap[i] > originalIndex)
                        _shuffleMap[i]--;
                }
            }

            NextItemsChanged?.Invoke(this, addedItems, removedItems);
        }

        /// <inheritdoc />
        public void ClearNext() => _nextItems.Clear();

        /// <inheritdoc />
        public void PushPrevious(PlaybackItem sourceConfig)
        {
            var addedItems = new List<CollectionChangedItem<PlaybackItem>>()
            {
                new(sourceConfig, PreviousItems.Count),
            };

            var removedItems = Array.Empty<CollectionChangedItem<PlaybackItem>>();

            _prevItems.Add(sourceConfig);

            PreviousItemsChanged?.Invoke(this, addedItems, removedItems);
        }

        /// <inheritdoc />
        public PlaybackItem PopPrevious(int index)
        {
            var returnItem = _prevItems.Pop();

            var addedItems = new List<CollectionChangedItem<PlaybackItem>>()
            {
                new(returnItem, PreviousItems.Count),
            };

            var removedItems = Array.Empty<CollectionChangedItem<PlaybackItem>>();

            PreviousItemsChanged?.Invoke(this, addedItems, removedItems);

            return returnItem;
        }

        /// <inheritdoc />
        public void ClearPrevious() => _prevItems.Clear();

        private async Task PreviousAsync(bool shouldRemoveFromQueue)
        {
            if (_currentPlayerService == null && CurrentItem != null)
            {
                var instanceId = CurrentItem?.MediaConfig?.Track.SourceCore.InstanceId;
                Guard.IsNotNull(instanceId, nameof(instanceId));

                _currentPlayerService = _audioPlayerRegistry[instanceId];
            }

            Guard.IsNotNull(_currentPlayerService?.CurrentSource, nameof(_currentPlayerService.CurrentSource));

            await _currentPlayerService.PauseAsync();
            DetachEvents(_currentPlayerService);

            var currentItem = _currentPlayerService.CurrentSource;

            _nextItems.Insert(0, currentItem);

            var newItem = shouldRemoveFromQueue ? _prevItems.Pop() : _prevItems.Last();

            var instId = CurrentItem?.MediaConfig?.Track.SourceCore.InstanceId;
            Guard.IsNotNull(instId, nameof(instId));

            _currentPlayerService = _audioPlayerRegistry[instId];
            AttachEvents(_currentPlayerService);

            await _currentPlayerService.Play(newItem);
            CurrentItem = newItem;

            CurrentItemChanged?.Invoke(this, newItem);
        }

        /// <inheritdoc />
        public Task ToggleShuffleAsync(CancellationToken cancellationToken = default)
        {
            _shuffleState = !_shuffleState;

            if (ShuffleState)
                ShuffleOnInternal();
            else
                ShuffleOffInternal();

            ShuffleStateChanged?.Invoke(this, _shuffleState);

            return Task.CompletedTask;
        }

        private void ShuffleOffInternal()
        {
            _shuffleState = false;

            if (_shuffleMap.Length == 0)
                return;

            var originalCurrentItemIndex = _shuffleMap[_prevItems.Count];

            // The space complexity will remain O(n), because we are not cloning any list we are simply references same items each time.
            var unshuffledItems = new List<PlaybackItem>();

            unshuffledItems.AddRange(_prevItems);

            if (CurrentItem != null)
                unshuffledItems.Add(CurrentItem);

            unshuffledItems.AddRange(_nextItems);
            unshuffledItems.Unshuffle(_shuffleMap);

            _nextItems.Clear();
            _prevItems.Clear();

            // The time complexity will also remain remain at O(n).
            for (int i = 0; i < unshuffledItems.Count; i++)
            {
                if (i < originalCurrentItemIndex)
                {
                    // Pushing everything before the originalCurrentItemIndex to previous items.
                    _prevItems.Add(unshuffledItems[i]);
                }
                else if (i == originalCurrentItemIndex && CurrentItem != null)
                {
                    // We will not add current item to the _nextItems if its already playing.
                    // We will not set currentItem so the current running track remains unaffected.
                }
                else
                {
                    // Pushing everything after the originalCurrentItemIndex to next Items.
                    _nextItems.Add(unshuffledItems[i]);
                }
            }
        }

        private void ShuffleOnInternal()
        {
            _shuffleState = true;
            _shuffleMap = Array.Empty<int>();

            // This list is only used for shuffle purpose, at the end we will extract nextitems out of it.
            var list = new List<PlaybackItem>();

            list.AddRange(_prevItems);

            if (CurrentItem != null)
                list.Add(CurrentItem);

            list.AddRange(_nextItems);

            _shuffleMap = list.Shuffle();

            // Swapping the items to make sure the CurrentItem and its original index in the map and the temp list is 0th index.
            var temp = _shuffleMap[0];
            var tempItem = list[0];
            var orignalCurrentIndex = _prevItems.Count;
            var newCurrentIndex = Array.IndexOf(_shuffleMap, orignalCurrentIndex);

            if (CurrentItem != null)
            {
                list[0] = CurrentItem;
                _shuffleMap[0] = orignalCurrentIndex;

                _shuffleMap[newCurrentIndex] = temp;
                list[newCurrentIndex] = tempItem;
            }

            // Populate everything in the list except the item at 0th index in nextItems because its the CurrentItem.
            _nextItems.Clear();
            for (var i = 1; i < list.Count; i++)
            {
                _nextItems.Add(list[i]);
            }

            _prevItems.Clear();
        }

        /// <inheritdoc />
        public Task SetRepeatStateAsync(RepeatState state, CancellationToken cancellationToken = default)
        {
            _repeatState = state;

            RepeatStateChanged?.Invoke(this, _repeatState);

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task ToggleRepeatAsync(CancellationToken cancellationToken = default)
        {
            _repeatState = _repeatState switch
            {
                RepeatState.None => RepeatState.One,
                RepeatState.One => RepeatState.All,
                RepeatState.All => RepeatState.None,
                _ => ThrowHelper.ThrowArgumentOutOfRangeException<RepeatState>(nameof(RepeatState)),
            };

            RepeatStateChanged?.Invoke(this, _repeatState);

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (_currentPlayerService is not null)
                DetachEvents(_currentPlayerService);
        }
    }
}
