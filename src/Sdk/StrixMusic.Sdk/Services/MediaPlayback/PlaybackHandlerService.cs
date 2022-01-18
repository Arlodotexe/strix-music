using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.Events;
using OwlCore.Extensions;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.MediaPlayback.LocalDevice;

namespace StrixMusic.Sdk.Services.MediaPlayback
{
    /// <summary>
    /// Manages an internal queue, handles playback, and delegates playback commands to an <see cref="IAudioPlayerService"/>.
    /// </summary>
    public sealed partial class PlaybackHandlerService : IPlaybackHandlerService
    {
        private static readonly Random _rng = new Random();

        private readonly Dictionary<string, IAudioPlayerService> _audioPlayerRegistry = new Dictionary<string, IAudioPlayerService>();
        private readonly List<IMediaSourceConfig> _prevItems = new List<IMediaSourceConfig>();
        private List<IMediaSourceConfig> _nextItems = new List<IMediaSourceConfig>();

        private int[] _shuffleMap;

        private StrixDevice? _strixDevice;
        private IAudioPlayerService? _currentPlayerService;
        private RepeatState _repeatState;
        private bool _shuffleState;

        /// <summary>
        /// Creates a new instance of <see cref="PlaybackHandlerService"/>.
        /// </summary>
        public PlaybackHandlerService()
        {
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

        private void CurrentPlayerService_CurrentSourceChanged(object sender, IMediaSourceConfig? e)
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
        public IMediaSourceConfig? CurrentItem { get; internal set; }

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
                DetachEvents(_currentPlayerService);
            }

            var mediaSource = NextItems.ElementAtOrDefault(queueIndex);

            if (mediaSource is null)
                return;

            _currentPlayerService = _audioPlayerRegistry[mediaSource.Track.SourceCore.InstanceId];
            AttachEvents(_currentPlayerService);

            if (CurrentItem != null)
                _prevItems.Add(CurrentItem);

            CurrentItem = mediaSource;
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
            DetachEvents(_currentPlayerService);

            _currentPlayerService = _audioPlayerRegistry[mediaSource.Track.SourceCore.InstanceId];
            AttachEvents(_currentPlayerService);

            // TODO shift queue, move tracks after the played item into next
            await _currentPlayerService.Play(mediaSource);
        }

        /// <inheritdoc />
        public async Task NextAsync()
        {
            if (_currentPlayerService == null && CurrentItem != null)
            {
                _currentPlayerService = _audioPlayerRegistry[CurrentItem.Track.SourceCore.InstanceId];
            }

            Guard.IsNotNull(_currentPlayerService?.CurrentSource, nameof(_currentPlayerService.CurrentSource));

            var nextIndex = 0;

            await _currentPlayerService.PauseAsync();
            DetachEvents(_currentPlayerService);

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
                CurrentItem = nextItem;

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
            AttachEvents(_currentPlayerService);

            await _currentPlayerService.Play(nextItem);
            _currentPlayerService.CurrentSource = CurrentItem;

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
            if (_currentPlayerService == null && CurrentItem != null)
            {
                _currentPlayerService = _audioPlayerRegistry[CurrentItem.Track.SourceCore.InstanceId];
            }

            Guard.IsNotNull(_currentPlayerService?.CurrentSource, nameof(_currentPlayerService.CurrentSource));

            await _currentPlayerService.PauseAsync();
            DetachEvents(_currentPlayerService);

            var currentItem = _currentPlayerService.CurrentSource;

            _nextItems.Insert(0, currentItem);

            var newItem = shouldRemoveFromQueue ? _prevItems.Pop() : _prevItems.Last();

            _currentPlayerService = _audioPlayerRegistry[newItem.Track.SourceCore.InstanceId];
            AttachEvents(_currentPlayerService);

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

            // The space complexity will remain O(n), because we are not cloning any list we are simply references same items each time.
            var unshuffledItems = new List<IMediaSourceConfig>();

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
            var list = new List<IMediaSourceConfig>();

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
            for (int i = 1; i < list.Count; i++)
            {
                _nextItems.Add(list[i]);
            }

            _prevItems.Clear();
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
            if (_currentPlayerService is not null)
                DetachEvents(_currentPlayerService);
        }
    }
}