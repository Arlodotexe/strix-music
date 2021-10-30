using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.Events;
using OwlCore.Extensions;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Data.Merged;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.MediaPlayback.LocalDevice;
using StrixMusic.Sdk.ViewModels;

namespace StrixMusic.Sdk.Services.MediaPlayback
{
    /// <inheritdoc />
    public partial class PlaybackHandlerService : IPlaybackHandlerService
    {
        private readonly Dictionary<string, IAudioPlayerService> _audioPlayerRegistry;
        private readonly List<IMediaSourceConfig> _prevItems;
        private List<IMediaSourceConfig> _nextItems;

        private int[] _shuffleMap;

        private StrixDevice? _strixDevice;
        private IAudioPlayerService? _currentPlayerService;
        private RepeatState _repeatState;
        private bool _shuffleState;

        /// <summary>
        /// Creates a new instance of <see cref="PlaybackHandlerService"/>.
        /// </summary>
        public PlaybackHandlerService(List<IMediaSourceConfig> prevItems, List<IMediaSourceConfig> nextItems, IAudioPlayerService currentPlayerService)
        {
            _currentPlayerService = currentPlayerService;

            _audioPlayerRegistry = new Dictionary<string, IAudioPlayerService>();
            _prevItems = prevItems;
            _nextItems = nextItems;
            _shuffleMap = Array.Empty<int>();
        }

        /// <summary>
        /// Sets the local playback device for this playback handler.
        /// </summary>
        /// <param name="strixDevice"></param>
        public void SetStrixDevice(StrixDevice strixDevice)
        {
            _strixDevice = strixDevice;
        }

        private void AttachEvents()
        {
            if (_currentPlayerService is null)
                throw new InvalidOperationException();

            _currentPlayerService.PositionChanged += PositionChanged;
            _currentPlayerService.PlaybackSpeedChanged += PlaybackSpeedChanged;
            _currentPlayerService.PlaybackStateChanged += CurrentPlayerService_PlaybackStateChanged;
            _currentPlayerService.CurrentSourceChanged += CurrentPlayerService_CurrentSourceChanged;
            _currentPlayerService.VolumeChanged += VolumeChanged;
            _currentPlayerService.QuantumProcessed += QuantumProcessed;
        }

        private void DetachEvents()
        {
            if (_currentPlayerService is null)
                throw new InvalidOperationException();

            _currentPlayerService.PositionChanged -= PositionChanged;
            _currentPlayerService.PlaybackSpeedChanged -= PlaybackSpeedChanged;
            _currentPlayerService.PlaybackStateChanged -= CurrentPlayerService_PlaybackStateChanged;
            _currentPlayerService.CurrentSourceChanged -= CurrentPlayerService_CurrentSourceChanged;
            _currentPlayerService.VolumeChanged -= VolumeChanged;
            _currentPlayerService.QuantumProcessed -= QuantumProcessed;
        }

        private void CurrentPlayerService_CurrentSourceChanged(object sender, IMediaSourceConfig? e)
        {
            CurrentItem = e;
            CurrentItemChanged?.Invoke(this, e);
        }

        private async void CurrentPlayerService_PlaybackStateChanged(object sender, PlaybackState e)
        {
            // Since the player itself can't be queued, we use this as a sentinel value for advancing the queue.
            if (e == PlaybackState.Queued)
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
        public event CollectionChangedEventHandler<IMediaSourceConfig>? NextItemsChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IMediaSourceConfig>? PreviousItemsChanged;

        /// <inheritdoc />
        public event EventHandler<IMediaSourceConfig?>? CurrentItemChanged;

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

        /// <inheritdoc />
        public IReadOnlyList<IMediaSourceConfig> NextItems => _nextItems;

        /// <inheritdoc />
        public IReadOnlyCollection<IMediaSourceConfig> PreviousItems => _prevItems;

        /// <inheritdoc />
        public IMediaSourceConfig? CurrentItem { get; private set; }

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
        public Task SeekAsync(TimeSpan position) => _currentPlayerService?.SeekAsync(position) ?? Task.CompletedTask;

        /// <inheritdoc />
        public Task ChangePlaybackSpeedAsync(double speed) => _currentPlayerService?.ChangePlaybackSpeedAsync(speed) ?? Task.CompletedTask;

        /// <inheritdoc />
        public Task ResumeAsync() => _currentPlayerService?.ResumeAsync() ?? Task.CompletedTask;

        /// <inheritdoc />
        public Task PauseAsync() => _currentPlayerService?.PauseAsync() ?? Task.CompletedTask;

        /// <inheritdoc />
        public Task ChangeVolumeAsync(double volume) => _currentPlayerService?.ResumeAsync() ?? Task.CompletedTask;

        /// <inheritdoc />
        public async Task PlayFromNext(int queueIndex)
        {
            if (_currentPlayerService != null)
            {
                await _currentPlayerService.PauseAsync();
                DetachEvents();
            }

            var mediaSource = NextItems.ElementAtOrDefault(queueIndex);

            if (mediaSource is null)
                return;

            _currentPlayerService = _audioPlayerRegistry[mediaSource.Track.SourceCore.InstanceId];
            AttachEvents();

            // TODO shift queue, move tracks before the played index into previous
            // also account for shuffle
            _nextItems.Remove(mediaSource);
            await _currentPlayerService.Play(mediaSource);
        }

        /// <inheritdoc />
        public async Task PlayFromPrevious(int queueIndex)
        {
            Guard.IsNotNull(_currentPlayerService, nameof(_currentPlayerService));

            var mediaSource = PreviousItems.ElementAtOrDefault(queueIndex);

            Guard.IsNotNull(mediaSource, nameof(mediaSource));

            await _currentPlayerService.PauseAsync();
            DetachEvents();

            _currentPlayerService = _audioPlayerRegistry[mediaSource.Track.SourceCore.InstanceId];
            AttachEvents();

            // TODO shift queue, move tracks after the played item into next
            await _currentPlayerService.Play(mediaSource);
        }

        /// <inheritdoc />
        public async Task NextAsync()
        {
            Guard.IsNotNull(_currentPlayerService?.CurrentSource, nameof(_currentPlayerService.CurrentSource));

            var nextIndex = 0;

            await _currentPlayerService.PauseAsync();
            DetachEvents();

            if (RepeatState == RepeatState.All && NextItems.Count == 0)
            {
                // Move all items from previous back into Next
                _nextItems.AddRange(_prevItems);
                _prevItems.Clear();
            }

            if (NextItems.Count <= nextIndex)
                return;

            IMediaSourceConfig? nextItem = null;

            if (RepeatState == RepeatState.One && !(CurrentItem is null))
            {
                nextItem = CurrentItem;
            }
            else
            {
                nextItem = NextItems[nextIndex];

                // Move NowPlaying into previous
                _prevItems.Add(_currentPlayerService.CurrentSource);

                // Take the next item out of the queue (becomes NowPlaying)
                _nextItems.Remove(nextItem);
            }

            var removedItems = new List<CollectionChangedItem<IMediaSourceConfig>>()
                {
                    new CollectionChangedItem<IMediaSourceConfig>(nextItem, nextIndex),
                };

            var addedItems = Array.Empty<CollectionChangedItem<IMediaSourceConfig>>();

            NextItemsChanged?.Invoke(this, addedItems, removedItems);

            _currentPlayerService = _audioPlayerRegistry[nextItem.Track.SourceCore.InstanceId];
            AttachEvents();

            // TODO See DeviceViewModel.NowPlaying.
            var track = new TrackViewModel(new MergedTrack(nextItem.Track.IntoList()));

            Guard.IsNotNull(_strixDevice?.PlaybackContext, nameof(_strixDevice.PlaybackContext));

            _strixDevice.SetPlaybackData(_strixDevice.PlaybackContext, track);

            await _currentPlayerService.Play(nextItem);

            CurrentItemChanged?.Invoke(this, nextItem);
        }

        /// <inheritdoc />
        public Task PreviousAsync() => PreviousAsync(true);

        /// <inheritdoc />
        public void InsertNext(int index, IMediaSourceConfig sourceConfig)
        {
            var addedItems = new List<CollectionChangedItem<IMediaSourceConfig>>()
            {
                new CollectionChangedItem<IMediaSourceConfig>(sourceConfig, index),
            };

            var removedItems = Array.Empty<CollectionChangedItem<IMediaSourceConfig>>();

            _nextItems.InsertOrAdd(index, sourceConfig);
            NextItemsChanged?.Invoke(this, addedItems, removedItems);
        }

        /// <inheritdoc />
        public void RemoveNext(int index)
        {
            var removedItems = new List<CollectionChangedItem<IMediaSourceConfig>>()
            {
                new CollectionChangedItem<IMediaSourceConfig>(NextItems[index], index),
            };

            var addedItems = Array.Empty<CollectionChangedItem<IMediaSourceConfig>>();

            _nextItems.RemoveAt(index);

            NextItemsChanged?.Invoke(this, addedItems, removedItems);
        }

        /// <inheritdoc />
        public void ClearNext() => _nextItems.Clear();

        /// <inheritdoc />
        public void PushPrevious(IMediaSourceConfig sourceConfig)
        {
            var addedItems = new List<CollectionChangedItem<IMediaSourceConfig>>()
            {
                new CollectionChangedItem<IMediaSourceConfig>(sourceConfig, PreviousItems.Count),
            };

            var removedItems = Array.Empty<CollectionChangedItem<IMediaSourceConfig>>();

            _prevItems.Add(sourceConfig);

            PreviousItemsChanged?.Invoke(this, addedItems, removedItems);
        }

        /// <inheritdoc />
        public IMediaSourceConfig PopPrevious(int index)
        {
            var returnItem = _prevItems.Pop();

            var addedItems = new List<CollectionChangedItem<IMediaSourceConfig>>()
            {
                new CollectionChangedItem<IMediaSourceConfig>(returnItem, PreviousItems.Count),
            };

            var removedItems = Array.Empty<CollectionChangedItem<IMediaSourceConfig>>();

            PreviousItemsChanged?.Invoke(this, addedItems, removedItems);

            return returnItem;
        }

        /// <inheritdoc />
        public void ClearPrevious() => _prevItems.Clear();

        private async Task PreviousAsync(bool shouldRemoveFromQueue)
        {
            Guard.IsNotNull(_currentPlayerService?.CurrentSource, nameof(_currentPlayerService.CurrentSource));

            await _currentPlayerService.PauseAsync();
            DetachEvents();

            var currentItem = _currentPlayerService.CurrentSource;

            _nextItems.Insert(0, currentItem);

            var newItem = shouldRemoveFromQueue ? _prevItems.Pop() : _prevItems.Last();

            _currentPlayerService = _audioPlayerRegistry[newItem.Track.SourceCore.InstanceId];
            AttachEvents();

            // TODO See DeviceViewModel.NowPlaying.
            var track = new TrackViewModel(new MergedTrack(newItem.Track.IntoList()));

            Guard.IsNotNull(_strixDevice?.PlaybackContext, nameof(_strixDevice.PlaybackContext));

            _strixDevice.SetPlaybackData(_strixDevice.PlaybackContext, track);
            await _currentPlayerService.Play(newItem);
            CurrentItem = newItem;
            CurrentItemChanged?.Invoke(this, newItem);
        }

        /// <inheritdoc />
        public Task ToggleShuffleAsync()
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

            // Does unshuffle in O(n)
            UnshuffleInternal();

            void UnshuffleInternal(int i = 0)
            {
                var originalIndex = _shuffleMap[i];

                if (originalIndex == -1)
                    return;

                // Sentinal value, lets us know we've handled that index.
                _shuffleMap[i] = -1;

                if (originalIndex < originalCurrentItemIndex)
                {
                    // Previous items
                    // Hold onto current item so we don't overwrite a reference.
                    var itemToSave = _prevItems[i];

                    // Look ahead to the index we're overwriting 
                    // Unshuffle the existing item at that index
                    UnshuffleInternal(originalIndex);

                    // Restore the item to it's original index.
                    _prevItems[originalIndex] = itemToSave;
                }
                else if (originalIndex == originalCurrentItemIndex)
                {
                    // Current item is unaffected when unshuffling.
                    return;
                }
                else if (originalIndex > originalCurrentItemIndex)
                {
                    // Next items
                    // Hold onto current item so we don't overwrite a reference.
                    var itemToSave = _nextItems[i];

                    // Look ahead to the index we're overwriting 
                    // Unshuffle the existing item at that index
                    UnshuffleInternal(originalIndex);

                    // Restore the item to it's original index.
                    _nextItems[originalIndex] = itemToSave;
                }
            }
        }

        private void ShuffleOnInternal()
        {
            _shuffleState = true;
            _shuffleMap = Array.Empty<int>();

            // Count all tracks that are queued.
            var totalTracks = _prevItems.Count + _nextItems.Count;
            if (!(CurrentItem is null))
                totalTracks++;

            // Create and shuffle a list of indexes (shuffle map).
            // Can be used to restore original indexes later.
            var shuffleMap = new int[totalTracks];

            // Could be improved, shuffle could happen inside loop below.
            for (int i = 0; i < totalTracks; i++)
                shuffleMap[i] = i;

            shuffleMap.Shuffle();

            _nextItems.AddRange(_prevItems);

            // Make room for the previous Items, so they can be inserted in the start without overwriting the next items.
            if (_prevItems.Count > 0)
            {
                for (int i = _nextItems.Count - 1; i >= 0; i--)
                {
                    if (i - 1 > 0)
                        _nextItems[i] = _nextItems[i - 1];
                }
            }

            // Populate and shuffle next items using shuffle map
            for (int i = 0; i < totalTracks; i++)
            {
                if (i < _prevItems.Count)
                {
                    _nextItems.ReplaceOrAdd(i, _prevItems[i]);
                }

                if (i == _prevItems.Count)
                {
                    // Don't insert CurrentItem into next.
                    continue;
                }

                // if greater than prevItems
                if (i > _prevItems.Count)
                {
                    // Since we don't insert the CurrentItem
                    // everything in _nextItems must be shifted to prevent skipping / going out of range. 
                    var itemOffset = i - 1;

                    _nextItems.ReplaceOrAdd(itemOffset, _nextItems[itemOffset]);
                }
            }

            // Previous items no longer needed past this point.
            _prevItems.Clear();

            // Temporary storage for the unshuffled list, This doesn't increase the Time/Space complexity of the method above O(n).
            var tempShuffledList = new IMediaSourceConfig[_nextItems.Count];

            for (int i = 0; i < shuffleMap.Length; i++)
            {
                tempShuffledList[i] = _nextItems[shuffleMap[i]];
            }

            _nextItems = tempShuffledList.ToList();
        }

        /// <inheritdoc />
        public Task SetRepeatStateAsync(RepeatState state)
        {
            _repeatState = state;

            RepeatStateChanged?.Invoke(this, _repeatState);

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task ToggleRepeatAsync()
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
            DetachEvents();
        }
    }
}