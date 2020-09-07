using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StrixMusic.CoreInterfaces;
using StrixMusic.CoreInterfaces.Enums;
using StrixMusic.CoreInterfaces.Interfaces;

namespace StrixMusic.Core.MusicBrainz.Models
{
    /// <inheritdoc />
    public abstract class MusicBrainzCollectionGroupBase : IPlayableCollectionGroup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MusicBrainzCollectionGroupBase"/> class.
        /// </summary>
        /// <param name="sourceCore">The instance of the core this object was created in.</param>
        public MusicBrainzCollectionGroupBase(ICore sourceCore)
        {
            SourceCore = sourceCore;
        }

        /// <inheritdoc />
        public IReadOnlyList<IPlayableCollectionGroup> Children => throw new NotImplementedException();

        /// <inheritdoc />
        public int TotalChildrenCount => throw new NotImplementedException();

        /// <inheritdoc />
        public IReadOnlyList<IPlaylist> Playlists => throw new NotImplementedException();

        /// <inheritdoc />
        public int TotalPlaylistCount => throw new NotImplementedException();

        /// <inheritdoc />
        public IReadOnlyList<ITrack> Tracks => throw new NotImplementedException();

        /// <inheritdoc />
        public int TotalTracksCount => throw new NotImplementedException();

        /// <inheritdoc />
        public IReadOnlyList<IAlbum> Albums => throw new NotImplementedException();

        /// <inheritdoc />
        public int TotalAlbumsCount => throw new NotImplementedException();

        /// <inheritdoc />
        public IReadOnlyList<IArtist> Artists => throw new NotImplementedException();

        /// <inheritdoc />
        public int TotalArtistsCount => throw new NotImplementedException();

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

        /// <inheritdoc/>
        public virtual bool IsPlayAsyncSupported => false;

        /// <inheritdoc/>
        public virtual bool IsPauseAsyncSupported => false;

        /// <inheritdoc/>
        public virtual bool IsChangeNameAsyncSupported => throw new NotImplementedException();

        /// <inheritdoc/>
        public virtual bool IsChangeImagesAsyncSupported => throw new NotImplementedException();

        /// <inheritdoc/>
        public virtual bool IsChangeDescriptionAsyncSupported => throw new NotImplementedException();

        /// <inheritdoc/>
        public virtual bool IsChangeDurationAsyncSupported => throw new NotImplementedException();

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

        /// <inheritdoc/>
        public event EventHandler<TimeSpan>? DurationChanged;

        /// <inheritdoc/>
        public abstract Task ChangeDescriptionAsync(string? description);

        /// <inheritdoc/>
        public abstract Task ChangeDurationAsync(TimeSpan duration);

        /// <inheritdoc/>
        public abstract Task ChangeImagesAsync(IReadOnlyList<IImage> images);

        /// <inheritdoc/>
        public abstract Task ChangeNameAsync(string name);

        /// <inheritdoc />
        public abstract Task PauseAsync();

        /// <inheritdoc />
        public abstract Task PlayAsync();

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
