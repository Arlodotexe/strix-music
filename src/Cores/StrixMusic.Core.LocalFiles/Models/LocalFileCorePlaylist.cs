using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.Collections;
using OwlCore.Events;
using OwlCore.Extensions;
using StrixMusic.Core.LocalFiles.Services;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Services.FileMetadataManager;
using StrixMusic.Sdk.Services.FileMetadataManager.Models;

namespace StrixMusic.Core.LocalFiles.Models
{
    /// <summary>
    /// Wraps around <see cref="PlaylistMetadata"/> to provide playlist information extracted from a file to the Strix SDK.
    /// </summary>
    public class LocalFileCorePlaylist : ICorePlaylist, IDisposable
    {
        private readonly IFileMetadataManager _fileMetadataManager;
        private PlaylistMetadata _playlistMetadata;

        /// <summary>
        /// Creates a new instance of <see cref="LocalFileCorePlaylist"/>
        /// </summary>
        public LocalFileCorePlaylist(ICore sourceCore, PlaylistMetadata playlistMetadata)
        {
            SourceCore = sourceCore;
            _playlistMetadata = playlistMetadata;

            Guard.IsNotNull(playlistMetadata, nameof(playlistMetadata));
            Guard.IsNotNullOrWhiteSpace(playlistMetadata.Id, nameof(playlistMetadata.Id));

            Id = playlistMetadata.Id;
            _fileMetadataManager = sourceCore.GetService<IFileMetadataManager>();

            Duration = playlistMetadata.Duration ?? default;

            AttachEvents();
        }

        private void AttachEvents()
        {
            _fileMetadataManager.Playlists.MetadataUpdated += Albums_MetadataUpdated;
        }

        private void DetachEvents()
        {
            _fileMetadataManager.Playlists.MetadataUpdated -= Albums_MetadataUpdated;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsPlayTrackCollectionAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsPauseTrackCollectionAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<int>? TrackItemsCountChanged;

        /// <inheritdoc />
        public event EventHandler<int>? ImagesCountChanged;

        /// <inheritdoc />
        public event EventHandler<PlaybackState>? PlaybackStateChanged;

        /// <inheritdoc />
        public event EventHandler<string>? NameChanged;

        /// <inheritdoc />
        public event EventHandler<string?>? DescriptionChanged;

        /// <inheritdoc />
        public event EventHandler<Uri?>? UrlChanged;

        /// <inheritdoc />
        public event EventHandler<TimeSpan>? DurationChanged;

        /// <inheritdoc />
        public event EventHandler<DateTime?>? LastPlayedChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeNameAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeDescriptionAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeDurationAsyncAvailableChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ICoreTrack>? TrackItemsChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ICoreImage>? ImagesChanged;

        /// <inheritdoc />
        public int TotalImageCount { get; }

        /// <inheritdoc />
        public string Id { get; set; }

        /// <inheritdoc />
        public Uri? Url => _playlistMetadata?.Url ?? null;

        /// <inheritdoc />
        public string Name => _playlistMetadata?.Title ?? String.Empty;

        /// <inheritdoc />
        public string? Description => _playlistMetadata.Description;

        /// <inheritdoc />
        public DateTime? LastPlayed { get; }

        /// <inheritdoc />
        public PlaybackState PlaybackState { get; }

        /// <inheritdoc />
        public TimeSpan Duration { get; private set; }

        /// <inheritdoc />
        public bool IsChangeNameAsyncAvailable => false;

        /// <inheritdoc />
        public bool IsChangeDescriptionAsyncAvailable => false;

        /// <inheritdoc />
        public bool IsChangeDurationAsyncAvailable => false;

        /// <inheritdoc />
        public SynchronizedObservableCollection<string>? Genres { get; }

        /// <inheritdoc />
        public ICoreUserProfile? Owner { get; }

        /// <inheritdoc />
        public ICorePlayableCollectionGroup? RelatedItems { get; }

        /// <inheritdoc />
        public DateTime? AddedAt { get; }

        /// <inheritdoc />
        public int TotalTracksCount => _playlistMetadata.TrackIds?.Count ?? 0;

        /// <inheritdoc />
        public bool IsPlayTrackCollectionAsyncAvailable => false;

        /// <inheritdoc />
        public bool IsPauseTrackCollectionAsyncAvailable => false;

        /// <inheritdoc />
        public Task<bool> IsAddImageAvailable(int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveImageAvailable(int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task RemoveImageAsync(int index)
        {
            throw new NotSupportedException();
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
        public Task PlayTrackCollectionAsync()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task PauseTrackCollectionAsync()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task RemoveTrackAsync(int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task<bool> IsAddTrackAvailable(int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveTrackAvailable(int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task<bool> IsAddGenreAvailable(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveGenreAvailable(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public ICore SourceCore { get; }

        /// <inheritdoc />
        public IAsyncEnumerable<ICoreImage> GetImagesAsync(int limit, int offset)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task AddImageAsync(ICoreImage image, int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task AddTrackAsync(ICoreTrack track, int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task PlayTrackCollectionAsync(ICoreTrack track)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<ICoreTrack> GetTracksAsync(int limit, int offset)
        {
            var trackIds = _playlistMetadata.TrackIds;

            Guard.IsNotNull(trackIds, nameof(trackIds));

            var tracks = new List<TrackMetadata>();
            foreach (var id in trackIds)
            {
                var track = await _fileMetadataManager.Tracks.GetTrackById(id);

                Guard.IsNotNull(track, nameof(track));
                tracks.Add(track);
            }

            foreach (var item in tracks)
            {
                yield return new LocalFilesCoreTrack(SourceCore, item);
            }
        }

        private void Dispose(bool disposing)
        {
            ReleaseUnmanagedResources();
            if (disposing)
            {
                Genres?.Dispose();
            }
        }

        private void Albums_MetadataUpdated(object sender, IEnumerable<PlaylistMetadata> e)
        {
            foreach (var metadata in e)
            {
                if (metadata.Id != Id)
                    return;

                Guard.IsNotNull(metadata.TrackIds, nameof(metadata.TrackIds));

                var previousData = _playlistMetadata;
                _playlistMetadata = metadata;

                if (metadata.Title != previousData.Title)
                    NameChanged?.Invoke(this, Name);

                if (metadata.Description != previousData.Description)
                    DescriptionChanged?.Invoke(this, Description);

                if (metadata.Duration != previousData.Duration)
                    DurationChanged?.Invoke(this, Duration);

                // TODO genres, post genres do-over

                if (metadata.TrackIds.Count != (previousData.TrackIds?.Count ?? 0))
                    TrackItemsCountChanged?.Invoke(this, metadata.TrackIds.Count);
            }
        }

        private void ReleaseUnmanagedResources()
        {
            DetachEvents();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc />
        ~LocalFileCorePlaylist()
        {
            Dispose(false);
        }
    }
}
