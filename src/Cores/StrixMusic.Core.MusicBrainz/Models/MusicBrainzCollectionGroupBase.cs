using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OwlCore.Collections;
using StrixMusic.Sdk.Enums;
using StrixMusic.Sdk.Interfaces;

namespace StrixMusic.Core.MusicBrainz.Models
{
    /// <inheritdoc />
    public abstract class MusicBrainzCollectionGroupBase : IPlayableCollectionGroup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MusicBrainzCollectionGroupBase"/> class.
        /// </summary>
        /// <param name="sourceCore">The instance of the core this object was created in.</param>
        protected MusicBrainzCollectionGroupBase(ICore sourceCore)
        {
            SourceCore = sourceCore;
        }

        /// <inheritdoc />
        public SynchronizedObservableCollection<IPlayableCollectionGroup> Children { get; } = new SynchronizedObservableCollection<IPlayableCollectionGroup>();

        /// <inheritdoc />
        public SynchronizedObservableCollection<IPlaylist> Playlists { get; } = new SynchronizedObservableCollection<IPlaylist>();

        /// <inheritdoc />
        public SynchronizedObservableCollection<ITrack> Tracks { get; } = new SynchronizedObservableCollection<ITrack>();

        /// <inheritdoc />
        public SynchronizedObservableCollection<IAlbum> Albums { get; } = new SynchronizedObservableCollection<IAlbum>();

        /// <inheritdoc />
        public SynchronizedObservableCollection<IArtist> Artists { get; } = new SynchronizedObservableCollection<IArtist>();

        /// <inheritdoc />
        public ICore SourceCore { get; }

        /// <inheritdoc />
        public abstract string Id { get; protected set; }

        /// <inheritdoc />
        public abstract Uri? Url { get; protected set; }

        /// <inheritdoc />
        public abstract string Name { get; protected set; }

        /// <inheritdoc />
        public abstract SynchronizedObservableCollection<IImage> Images { get; protected set; }

        /// <inheritdoc />
        public abstract string? Description { get; protected set; }

        /// <inheritdoc />
        public PlaybackState PlaybackState => PlaybackState.None;

        /// <inheritdoc />
        public TimeSpan Duration => TimeSpan.Zero;

        /// <inheritdoc />
        public abstract int TotalAlbumsCount { get; internal set; }

        /// <inheritdoc />
        public abstract int TotalArtistsCount { get; internal set; }

        /// <inheritdoc />
        public abstract int TotalTracksCount { get; internal set; }

        /// <inheritdoc />
        public abstract int TotalPlaylistCount { get; internal set; }

        /// <inheritdoc />
        public abstract int TotalChildrenCount { get; internal set; }

        /// <inheritdoc />
        public bool IsPlayAsyncSupported => false;

        /// <inheritdoc />
        public bool IsPauseAsyncSupported => false;

        /// <inheritdoc />
        public bool IsChangeNameAsyncSupported => false;

        /// <inheritdoc />
        public bool IsChangeDescriptionAsyncSupported => false;

        /// <inheritdoc/>
        public bool IsChangeDurationAsyncSupported => false;

        /// <inheritdoc/>
        public SynchronizedObservableCollection<bool> IsRemoveImageSupportedMap { get; } = new SynchronizedObservableCollection<bool>();

        /// <inheritdoc/>
        public SynchronizedObservableCollection<bool> IsRemoveAlbumSupportedMap { get; } = new SynchronizedObservableCollection<bool>();

        /// <inheritdoc/>
        public SynchronizedObservableCollection<bool> IsRemoveArtistSupportedMap { get; } = new SynchronizedObservableCollection<bool>();

        /// <inheritdoc/>
        public SynchronizedObservableCollection<bool> IsRemoveTrackSupportedMap { get; } = new SynchronizedObservableCollection<bool>();

        /// <inheritdoc/>
        public SynchronizedObservableCollection<bool> IsRemovePlaylistSupportedMap { get; } = new SynchronizedObservableCollection<bool>();

        /// <inheritdoc/>
        public SynchronizedObservableCollection<bool> IsRemoveChildSupportedMap { get; } = new SynchronizedObservableCollection<bool>();

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

        /// <inheritdoc/>
        public Task<bool> IsAddChildSupported(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc/>
        public Task<bool> IsAddPlaylistSupported(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc/>
        public Task<bool> IsAddTrackSupported(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc/>
        public Task<bool> IsAddArtistSupported(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc/>
        public Task<bool> IsAddAlbumSupported(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc/>
        public Task<bool> IsAddImageSupported(int index)
        {
            return Task.FromResult(false);
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
        public Task ChangeNameAsync(string name)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task PauseAsync()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task PlayAsync()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public abstract IAsyncEnumerable<IAlbum> GetAlbumsAsync(int limit, int offset = 0);

        /// <inheritdoc />
        public abstract Task PopulateMoreAlbumsAsync(int limit);

        /// <inheritdoc />
        public abstract IAsyncEnumerable<IArtist> GetArtistsAsync(int limit, int offset = 0);

        /// <inheritdoc />
        public abstract Task PopulateMoreArtistsAsync(int limit);

        /// <inheritdoc />
        public abstract IAsyncEnumerable<IPlayableCollectionGroup> GetChildrenAsync(int limit, int offset = 0);

        /// <inheritdoc />
        public abstract Task PopulateMoreChildrenAsync(int limit);

        /// <inheritdoc />
        public abstract IAsyncEnumerable<IPlaylist> GetPlaylistsAsync(int limit, int offset = 0);

        /// <inheritdoc />
        public abstract Task PopulateMorePlaylistsAsync(int limit);

        /// <inheritdoc />
        public abstract IAsyncEnumerable<ITrack> GetTracksAsync(int limit, int offset = 0);

        /// <inheritdoc />
        public abstract Task PopulateMoreTracksAsync(int limit);
    }
}
