using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using OwlCore.Events;
using OwlCore.Extensions;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.MediaPlayback.LocalDevice;
using StrixMusic.Sdk.Services.Settings;

namespace StrixMusic.Sdk.Services.MediaPlayback
{
    /// <inheritdoc />
    public class PlaybackHandlerService : IPlaybackHandlerService
    {
        private readonly ISettingsService _settingsService;
        private readonly Dictionary<ICore, IAudioPlayerService> _audioPlayerRegistry;
        private readonly List<IMediaSourceConfig> _nextItems;
        private readonly Stack<IMediaSourceConfig> _prevItems;

        private IAudioPlayerService? _currentPlayerService;
        private RepeatState _repeatState;
        private bool _shuffleState;

        private int[]? _shuffledNextItemsIndices;

        /// <summary>
        /// Creates a new instance of <see cref="PlaybackHandlerService"/>.
        /// </summary>
        public PlaybackHandlerService()
        {
            _audioPlayerRegistry = new Dictionary<ICore, IAudioPlayerService>();
            _settingsService = Ioc.Default.GetRequiredService<ISettingsService>();


            _prevItems = new Stack<IMediaSourceConfig>();
            _nextItems = new List<IMediaSourceConfig>();
        }

        private void AttachEvents()
        {
            if (_currentPlayerService is null)
                throw new InvalidOperationException();

            _currentPlayerService.PositionChanged += PlayerService_PositionChanged;

            _currentPlayerService.PositionChanged += PositionChanged;
            _currentPlayerService.PlaybackSpeedChanged += PlaybackSpeedChanged;
            _currentPlayerService.PlaybackStateChanged += PlaybackStateChanged;
            _currentPlayerService.VolumeChanged += VolumeChanged;
            _currentPlayerService.QuantumProcessed += QuantumProcessed;
        }

        private void DetachEvents()
        {
            if (_currentPlayerService is null)
                throw new InvalidOperationException();

            _currentPlayerService.PositionChanged -= PlayerService_PositionChanged;

            _currentPlayerService.PositionChanged -= PositionChanged;
            _currentPlayerService.PlaybackSpeedChanged -= PlaybackSpeedChanged;
            _currentPlayerService.PlaybackStateChanged -= PlaybackStateChanged;
            _currentPlayerService.VolumeChanged -= VolumeChanged;
            _currentPlayerService.QuantumProcessed -= QuantumProcessed;
        }

        private async void PlayerService_PositionChanged(object sender, TimeSpan e)
        {
            Guard.IsNotNull(_currentPlayerService?.CurrentSource, nameof(_currentPlayerService.CurrentSource));

            // If the song is not over
            if (_currentPlayerService.CurrentSource.Track.Duration > e)
                return;

            switch (_repeatState)
            {
                case RepeatState.All when NextItems.Count == 0:
                    // Move all items from previous back into Next
                    _nextItems.AddRange(PreviousItems);
                    _prevItems.Clear();

                    await PlayFromNext(0);
                    return;
                case RepeatState.One:
                    await NextAsync(false);
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
        public event EventHandler<IMediaSourceConfig>? CurrentItemChanged;

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
        public bool ShuffleState { get; }

        /// <inheritdoc />
        public RepeatState RepeatState { get; }

        /// <inheritdoc />
        public TimeSpan Position => _currentPlayerService?.Position ?? TimeSpan.Zero;

        /// <inheritdoc />
        public PlaybackState PlaybackState => _currentPlayerService?.PlaybackState ?? PlaybackState.None;

        /// <inheritdoc />
        public double Volume => _currentPlayerService?.Volume ?? 1;

        /// <inheritdoc />
        public double PlaybackSpeed => _currentPlayerService?.PlaybackSpeed ?? 1;

        /// <inheritdoc />
        public void RegisterAudioPlayer(IAudioPlayerService audioPlayer, ICore core) => _audioPlayerRegistry.Add(core, audioPlayer);

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
        public async Task Play(ITrack track, IPlayableBase context, IReadOnlyList<ITrack> completeTrackQueue)
        {
            var mainViewModel = MainViewModel.Singleton;

            Guard.IsTrue(mainViewModel?.Library?.IsInitialized ?? false, nameof(mainViewModel.Library.IsInitialized));

            var core = await GetPlaybackCore(track);
            var activeDevice = mainViewModel?.ActiveDevice;
            var localDevice = mainViewModel?.LocalDevice?.Model as StrixDevice;

            Guard.IsNotNull(localDevice, nameof(localDevice));

            // Pause the active player first.
            if (!(activeDevice is null))
            {
                _ = activeDevice.PauseAsync();
            }

            // If there is no active device, activate the associated local device.
            if (activeDevice is null)
            {
                await localDevice.SwitchToAsync();
            }

            Guard.IsNotNull(activeDevice, nameof(activeDevice));

            // Don't continue if playing this track isn't supported by the core.
            if (activeDevice.SourceCore?.CoreConfig.PlaybackType == MediaPlayerType.None)
                return;

            // If the active device is controlled remotely, the rest is handled there.
            if (activeDevice.Type == DeviceType.Remote)
            {
                // TODO 
                //await context.PlayAsync();
                return;
            }

            // Setup for local playback
            for (var i = 0; i < completeTrackQueue.Count; i++)
            {
                var item = completeTrackQueue[i];
                var coreTrack = item.GetSources<ICoreTrack>().First(x => x.Id == item.Id);

                var mediaSource = await core.GetMediaSource(coreTrack);
                if (mediaSource is null)
                    continue;

                InsertNext(i, mediaSource);
            }

            localDevice.SetPlaybackData(context, track);
            await PlayFromNext(0);
        }

        private async Task<ICore> GetPlaybackCore(ITrack track)
        {
            var coreRanking = await _settingsService.GetValue<IReadOnlyList<string>>(nameof(SettingsKeys.CoreRanking));

            // Find highest ranking core from the items merged into the track being played.
            foreach (var instanceId in coreRanking)
            {
                var core = track.GetSourceCores<ICoreTrack>().FirstOrDefault(x => x.InstanceId == instanceId);
                if (core != default)
                    return core;
            }

            return ThrowHelper.ThrowInvalidOperationException<ICore>($"None of the source cores from the given {nameof(track)} are registered in {nameof(coreRanking)}");
        }

        /// <inheritdoc />
        public async Task PlayFromNext(int queueIndex)
        {
            if (_shuffleState && _shuffledNextItemsIndices != null)
                queueIndex = _shuffledNextItemsIndices[queueIndex];

            var mediaSource = NextItems.ElementAtOrDefault(queueIndex);

            if (mediaSource is null)
                ThrowHelper.ThrowArgumentOutOfRangeException(nameof(queueIndex));

            if (_currentPlayerService != null)
            {
                await _currentPlayerService.PauseAsync();
                DetachEvents();
            }

            _currentPlayerService = _audioPlayerRegistry[mediaSource.Track.SourceCore];
            AttachEvents();

            // TODO shift queue, move tracks before the played item into previous
            await _currentPlayerService.Play(mediaSource);
            CurrentItem = mediaSource;
            CurrentItemChanged?.Invoke(this, mediaSource);
        }

        /// <inheritdoc />
        public async Task PlayFromPrevious(int queueIndex)
        {
            Guard.IsNotNull(_currentPlayerService, nameof(_currentPlayerService));

            var mediaSource = PreviousItems.ElementAtOrDefault(queueIndex);

            Guard.IsNotNull(mediaSource, nameof(mediaSource));

            await _currentPlayerService.PauseAsync();
            DetachEvents();

            _currentPlayerService = _audioPlayerRegistry[mediaSource.Track.SourceCore];
            AttachEvents();

            // TODO shift queue, move tracks after the played item into next
            await _currentPlayerService.Play(mediaSource);
            CurrentItem = mediaSource;
            CurrentItemChanged?.Invoke(this, mediaSource);
        }

        /// <inheritdoc />
        public Task NextAsync() => NextAsync(true);

        private async Task NextAsync(bool shouldRemoveFromQueue)
        {
            Guard.IsNotNull(_currentPlayerService?.CurrentSource, nameof(_currentPlayerService.CurrentSource));

            var nextIndex = 0;

            if (_shuffleState && _shuffledNextItemsIndices != null)
                nextIndex = _shuffledNextItemsIndices[nextIndex];

            await _currentPlayerService.PauseAsync();
            DetachEvents();

            var nextItem = NextItems[nextIndex];

            if (shouldRemoveFromQueue)
            {
                _prevItems.Push(_currentPlayerService.CurrentSource);
                _nextItems.Remove(nextItem);

                var removedItems = new List<CollectionChangedItem<IMediaSourceConfig>>()
                {
                    new CollectionChangedItem<IMediaSourceConfig>(nextItem, nextIndex),
                };

                var addedItems = Array.Empty<CollectionChangedItem<IMediaSourceConfig>>();

                NextItemsChanged?.Invoke(this, addedItems, removedItems);
            }

            _currentPlayerService = _audioPlayerRegistry[nextItem.Track.SourceCore];
            AttachEvents();

            await _currentPlayerService.Play(nextItem);
            CurrentItem = nextItem;
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

            _nextItems.Insert(index, sourceConfig);
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

            _prevItems.Push(sourceConfig);

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

            var newItem = shouldRemoveFromQueue ? _prevItems.Pop() : _prevItems.Peek();

            _currentPlayerService = _audioPlayerRegistry[newItem.Track.SourceCore];
            AttachEvents();

            await _currentPlayerService.Play(newItem);
        }

        /// <inheritdoc />
        public Task ToggleShuffleAsync()
        {
            _shuffleState = !_shuffleState;

            if (_shuffleState)
            {
                var totalItems = NextItems.Count;

                _shuffledNextItemsIndices = new int[totalItems];

                for (var i = 0; i < totalItems; i++)
                    _shuffledNextItemsIndices[i] = i;

                _shuffledNextItemsIndices.Shuffle();
            }
            else
            {
                _shuffledNextItemsIndices = null;
            }

            ShuffleStateChanged?.Invoke(this, _shuffleState);

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
    }
}