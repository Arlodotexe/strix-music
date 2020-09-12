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

        /// <inheritdoc />
        public IReadOnlyList<IPlayableCollectionGroup> Children => SourceChildren;

        /// <inheritdoc />
        public int TotalChildrenCount { get; internal set; }

        /// <inheritdoc />
        public IReadOnlyList<IPlaylist> Playlists => SourcePlaylists;

        /// <inheritdoc />
        public int TotalPlaylistCount { get; internal set; }

        /// <inheritdoc />
        public IReadOnlyList<ITrack> Tracks => SourceTracks;

        /// <inheritdoc />
        public int TotalTracksCount { get; internal set; }

        /// <inheritdoc />
        public IReadOnlyList<IAlbum> Albums => throw new NotImplementedException();

        /// <inheritdoc />
        public int TotalAlbumsCount { get; internal set; }

        /// <inheritdoc />
        public IReadOnlyList<IArtist> Artists => throw new NotImplementedException();

        /// <inheritdoc />
        public int TotalArtistsCount { get; internal set; }

        /// <inheritdoc />
        public ICore SourceCore { get; internal set; }

        /// <inheritdoc />
        public string Id => throw new NotImplementedException();

        /// <inheritdoc />
        public Uri Url => throw new NotImplementedException();

        /// <inheritdoc />
        public string Name => throw new NotImplementedException();

        /// <inheritdoc />
        public IReadOnlyList<IImage> Images => throw new NotImplementedException();

        /// <inheritdoc />
        public string Description => throw new NotImplementedException();

        /// <inheritdoc />
        public PlaybackState PlaybackState => throw new NotImplementedException();

        /// <inheritdoc />
        public TimeSpan Duration => throw new NotImplementedException();

        /// <inheritdoc />
        public virtual bool IsPlayAsyncSupported => false;

        /// <inheritdoc />
        public virtual bool IsPauseAsyncSupported => false;

        /// <inheritdoc />
        public virtual bool IsChangeNameAsyncSupported { get; }

        /// <inheritdoc />
        public virtual bool IsChangeImagesAsyncSupported { get; }

        /// <inheritdoc />
        public virtual bool IsChangeDescriptionAsyncSupported { get; }

        /// <inheritdoc/>
        public virtual bool IsChangeDurationAsyncSupported { get; }

        /// <inheritdoc />
        public event EventHandler<CollectionChangedEventArgs<IPlayableCollectionGroup>>? ChildrenChanged;

        /// <inheritdoc />
        public event EventHandler<CollectionChangedEventArgs<IPlaylist>>? PlaylistsChanged;

        /// <inheritdoc />
        public event EventHandler<CollectionChangedEventArgs<ITrack>>? TracksChanged;

        /// <inheritdoc />
        public event EventHandler<CollectionChangedEventArgs<IAlbum>>? AlbumsChanged;

        /// <inheritdoc />
        public event EventHandler<CollectionChangedEventArgs<IArtist>>? ArtistsChanged;

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
        public virtual Task ChangeDescriptionAsync(string? description)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public virtual Task ChangeDurationAsync(TimeSpan duration)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public virtual Task ChangeImagesAsync(IReadOnlyList<IImage> images)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public virtual Task ChangeNameAsync(string name)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public virtual Task PauseAsync()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public virtual Task PlayAsync()
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
