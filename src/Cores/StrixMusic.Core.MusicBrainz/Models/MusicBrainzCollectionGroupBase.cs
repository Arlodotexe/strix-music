using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StrixMusic.Sdk.Enums;
using StrixMusic.Sdk.Events;
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

            SourceTracks = new List<ITrack>();
            SourceArtists = new List<IArtist>();
            SourceAlbums = new List<IAlbum>();
            SourcePlaylists = new List<IPlaylist>();
            SourceChildren = new List<IPlayableCollectionGroup>();
        }

        /// <inheritdoc cref="Tracks"/>
        protected List<ITrack> SourceTracks { get; }

        /// <inheritdoc cref="Artists"/>
        protected List<IArtist> SourceArtists { get; }

        /// <inheritdoc cref="Albums"/>
        protected List<IAlbum> SourceAlbums { get; }

        /// <inheritdoc cref="Playlists"/>
        protected List<IPlaylist> SourcePlaylists { get; }

        /// <inheritdoc cref="Children"/>
        protected List<IPlayableCollectionGroup> SourceChildren { get; }

        /// <inheritdoc />
        public IReadOnlyList<IPlayableCollectionGroup> Children => SourceChildren;

        /// <inheritdoc />
        public IReadOnlyList<IPlaylist> Playlists => SourcePlaylists;

        /// <inheritdoc />
        public IReadOnlyList<ITrack> Tracks => SourceTracks;

        /// <inheritdoc />
        public IReadOnlyList<IAlbum> Albums => SourceAlbums;

        /// <inheritdoc />
        public IReadOnlyList<IArtist> Artists => SourceArtists;

        /// <inheritdoc />
        public ICore SourceCore { get; }

        /// <inheritdoc />
        public abstract string Id { get; protected set; }

        /// <inheritdoc />
        public abstract Uri? Url { get; protected set; }

        /// <inheritdoc />
        public abstract string Name { get; protected set; }

        /// <inheritdoc />
        public abstract IReadOnlyList<IImage> Images { get; protected set; }

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
        public bool IsChangeImagesAsyncSupported => false;

        /// <inheritdoc />
        public bool IsChangeDescriptionAsyncSupported => false;

        /// <inheritdoc/>
        public bool IsChangeDurationAsyncSupported => false;

        /// <inheritdoc />
        public abstract event EventHandler<CollectionChangedEventArgs<IPlayableCollectionGroup>>? ChildrenChanged;

        /// <inheritdoc />
        public abstract event EventHandler<CollectionChangedEventArgs<IPlaylist>>? PlaylistsChanged;

        /// <inheritdoc />
        public abstract event EventHandler<CollectionChangedEventArgs<ITrack>>? TracksChanged;

        /// <inheritdoc />
        public abstract event EventHandler<CollectionChangedEventArgs<IAlbum>>? AlbumsChanged;

        /// <inheritdoc />
        public abstract event EventHandler<CollectionChangedEventArgs<IArtist>>? ArtistsChanged;

        /// <inheritdoc />
        public event EventHandler<PlaybackState>? PlaybackStateChanged;

        /// <inheritdoc />
        public event EventHandler<string>? NameChanged;

        /// <inheritdoc />
        public event EventHandler<string?>? DescriptionChanged;

        /// <inheritdoc />
        public event EventHandler<Uri?>? UrlChanged;

        /// <inheritdoc />
        public event EventHandler<CollectionChangedEventArgs<IImage>>? ImagesChanged;

        /// <inheritdoc />
        public event EventHandler<TimeSpan>? DurationChanged;

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
        public Task ChangeImagesAsync(IReadOnlyList<IImage> images)
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
        public abstract Task<IReadOnlyList<IAlbum>> PopulateAlbumsAsync(int limit, int offset = 0);

        /// <inheritdoc />
        public abstract Task<IReadOnlyList<IArtist>> PopulateArtistsAsync(int limit, int offset = 0);

        /// <inheritdoc />
        public abstract Task<IReadOnlyList<IPlayableCollectionGroup>> PopulateChildrenAsync(int limit, int offset = 0);

        /// <inheritdoc />
        public abstract Task<IReadOnlyList<IPlaylist>> PopulatePlaylistsAsync(int limit, int offset = 0);

        /// <inheritdoc />
        public abstract Task<IReadOnlyList<ITrack>> PopulateTracksAsync(int limit, int offset = 0);
    }
}
