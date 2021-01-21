using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OwlCore.Collections;
using OwlCore.Events;
using OwlCore.Extensions;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Data.Merged;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Sdk.Data
{
    /// <summary>
    /// The UI should use this to create a new playlist that can be added to the backend.
    /// </summary>
    public class InitialPlaylistData : IPlaylist, IInitialData
    {
        private readonly IReadOnlyList<ICorePlaylist> _sources = new List<ICorePlaylist>();

        /// <summary>
        /// Holds any images that the user wants to add to the playlist on creation. These should point to a file that the app has access to.
        /// </summary>
        public List<IImage> Images { get; set; } = new List<IImage>();

        /// <summary>
        /// Holds any tracks that the user wants to add to the playlist on creation.
        /// </summary>
        public List<ITrack> Tracks { get; set; } = new List<ITrack>();

        /// <inheritdoc />
        public List<ICore>? TargetSourceCores { get; set; } = new List<ICore>();

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IImage>? ImagesChanged;

        /// <inheritdoc />
        public event EventHandler<int>? ImagesCountChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ITrack>? TrackItemsChanged;

        /// <inheritdoc />
        public event EventHandler<int>? TrackItemsCountChanged;

        /// <inheritdoc />
        public event EventHandler<string>? NameChanged;

        /// <inheritdoc />
        public event EventHandler<string?>? DescriptionChanged;

        /// <inheritdoc />
        public event EventHandler<Uri?>? UrlChanged;

        /// <inheritdoc />
        public event EventHandler<PlaybackState>? PlaybackStateChanged;

        /// <inheritdoc />
        public event EventHandler<TimeSpan>? DurationChanged;

        /// <inheritdoc />
        public event EventHandler<DateTime?>? LastPlayedChanged;

        /// <inheritdoc />
        public int TotalImageCount => Images.Count;

        /// <inheritdoc />
        public int TotalTracksCount => Tracks.Count;

        /// <inheritdoc />
        public SynchronizedObservableCollection<string>? Genres { get; }

        /// <inheritdoc />
        public string Id { get; set; } = string.Empty;

        /// <inheritdoc />
        public Uri? Url { get; set; }

        /// <inheritdoc />
        public string Name { get; set; } = string.Empty;

        /// <inheritdoc />
        public string? Description { get; set; }

        /// <inheritdoc />
        public DateTime? LastPlayed { get; }

        /// <inheritdoc />
        public PlaybackState PlaybackState { get; }

        /// <inheritdoc />
        public TimeSpan Duration { get; }

        /// <inheritdoc />
        public DateTime? AddedAt { get; }

        /// <inheritdoc />
        public IUserProfile? Owner { get; set; }

        /// <inheritdoc />
        public IPlayableCollectionGroup? RelatedItems { get; }

        /// <inheritdoc />
        public bool IsPlayAsyncSupported { get; }

        /// <inheritdoc />
        public bool IsPauseAsyncSupported { get; }

        /// <inheritdoc />
        public bool IsChangeNameAsyncSupported { get; }

        /// <inheritdoc />
        public bool IsChangeDescriptionAsyncSupported { get; }

        /// <inheritdoc />
        public bool IsChangeDurationAsyncSupported { get; }

        /// <inheritdoc />
        public Task<bool> IsRemoveTrackSupported(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> IsAddTrackSupported(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> IsAddGenreSupported(int index)
        {
            return Task.FromResult(true);
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveGenreSupported(int index)
        {
            return Task.FromResult(true);
        }

        /// <inheritdoc />
        public Task<bool> IsAddImageSupported(int index)
        {
            return Task.FromResult(true);
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveImageSupported(int index)
        {
            return Task.FromResult(true);
        }

        /// <inheritdoc />
        public Task PlayAsync()
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task PauseAsync()
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task ChangeNameAsync(string name)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task ChangeDescriptionAsync(string? description)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task ChangeDurationAsync(TimeSpan duration)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task AddTrackAsync(ITrack track, int index)
        {
            Tracks.InsertOrAdd(index, track);

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task RemoveTrackAsync(int index)
        {
            Tracks.RemoveAt(index);

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task AddImageAsync(IImage image, int index)
        {
            Images.InsertOrAdd(index, image);

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task RemoveImageAsync(int index)
        {
            Images.RemoveAt(index);

            return Task.CompletedTask;
        }

        /// <inheritdoc cref="IMerged{T}.SourceCores" />
        public IReadOnlyList<ICore> SourceCores { get; } = new List<ICore>();

        /// <inheritdoc />
        IReadOnlyList<ICoreImageCollection> IMerged<ICoreImageCollection>.Sources => _sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreTrackCollection> IMerged<ICoreTrackCollection>.Sources => _sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreGenreCollection> IMerged<ICoreGenreCollection>.Sources => _sources;

        /// <inheritdoc />
        IReadOnlyList<ICorePlaylistCollectionItem> IMerged<ICorePlaylistCollectionItem>.Sources => _sources;

        /// <inheritdoc />
        IReadOnlyList<ICorePlaylist> IMerged<ICorePlaylist>.Sources => _sources;

        /// <inheritdoc />
        public Task<IReadOnlyList<IImage>> GetImagesAsync(int limit, int offset)
        {
            return Task.FromResult<IReadOnlyList<IImage>>(Images.Skip(offset).Take(limit).ToList());
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<ITrack>> GetTracksAsync(int limit, int offset)
        {
            return Task.FromResult<IReadOnlyList<ITrack>>(Tracks.Skip(offset).Take(limit).ToList());
        }

        /// <inheritdoc />
        public bool Equals(ICoreImageCollection other) => false;

        /// <inheritdoc />
        public bool Equals(ICoreTrackCollection other) => false;

        /// <inheritdoc />
        public bool Equals(ICoreGenreCollection other) => false;

        /// <inheritdoc />
        public bool Equals(ICorePlaylistCollectionItem other) => false;

        /// <inheritdoc />
        public bool Equals(ICorePlaylist other) => false;
    }
}