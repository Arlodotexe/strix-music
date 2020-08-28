using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StrixMusic.CoreInterfaces;
using StrixMusic.CoreInterfaces.Enums;
using StrixMusic.CoreInterfaces.Interfaces;

namespace StrixMusic.Core.Dummy.Implementations
{
    /// <inheritdoc/>
    public class DummyArtist : IArtist
    {
        /// <inheritdoc/>
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        /// <inheritdoc/>
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// List of full <see cref="DummyAlbum"/>s to be used within the DummyCore.
        /// </summary>
        public List<DummyAlbum> DummyAlbums { get; set; } = new List<DummyAlbum>();

        /// <summary>
        /// List of the Ids of <see cref="DummyAlbum"/>s to the <see cref="DummyArtist"/>
        /// </summary>
        [JsonProperty("album_ids")]
        public List<string>? AlbumIds { get; set; }

        /// <inheritdoc/>
        public IReadOnlyList<IImage> Images => throw new NotImplementedException();

        /// <inheritdoc/>
        public Uri Url => throw new NotImplementedException();

        /// <inheritdoc/>
        public string Description => throw new NotImplementedException();

        /// <inheritdoc/>
        public IUserProfile Owner => throw new NotImplementedException();

        /// <inheritdoc/>
        public PlaybackState PlaybackState => throw new NotImplementedException();

        /// <inheritdoc/>
        public ITrack PlayingTrack => throw new NotImplementedException();

        /// <inheritdoc/>
        public ICore SourceCore => DummyCore!;

        /// <summary>
        /// The <see cref="DummyCore"/> where the <see cref="DummyArtist"/> is from.
        /// </summary>
        public DummyCore? DummyCore { get; set; }

        /// <inheritdoc/>
        public IReadOnlyList<ITrack> TopTracks => throw new NotImplementedException();

        /// <inheritdoc/>
        public IReadOnlyList<IArtist> RelatedArtists => throw new NotImplementedException();

        /// <inheritdoc/>
        public IReadOnlyList<IAlbum> Albums => DummyAlbums!;

        /// <inheritdoc/>
        public int TotalAlbumsCount => Albums.Count;

        /// <inheritdoc/>
        public int TotalTopTracksCount => TopTracks.Count;

        /// <inheritdoc/>
        public int TotalRelatedArtistsCount => RelatedArtists.Count;



        /// <inheritdoc/>
        public IReadOnlyList<ITrack> Tracks => throw new NotImplementedException();

        /// <inheritdoc/>
        public int TotalTracksCount => throw new NotImplementedException();

        /// <inheritdoc/>
        public TimeSpan Duration => throw new NotImplementedException();

        /// <inheritdoc/>
        public IReadOnlyList<IPlayableCollectionGroup> RelatedItems => throw new NotImplementedException();

        /// <inheritdoc/>
        public int TotalRelatedItemsCount => throw new NotImplementedException();

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<IAlbum>>? AlbumsChanged;

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<IArtist>>? RelatedArtistsChanged;

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<ITrack>>? TracksChanged;

        /// <inheritdoc/>
        public event EventHandler<PlaybackState>? PlaybackStateChanged;

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<IImage>>? ImagesChanged;

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<IPlayableCollectionGroup>> RelatedItemsChanged;

        /// <inheritdoc/>
        public event EventHandler<string>? NameChanged
        {
            add
            {
                NameChanged += value;
            }

            remove
            {
                NameChanged -= value;
            }
        }

        /// <inheritdoc/>
        public event EventHandler<string?> DescriptionChanged
        {
            add
            {
                NameChanged += value;
            }

            remove
            {
                NameChanged -= value;
            }
        }

        /// <inheritdoc/>
        public event EventHandler<Uri?> UrlChanged
        {
            add
            {
                UrlChanged += value;
            }

            remove
            {
                UrlChanged -= value;
            }
        }

        /// <inheritdoc/>
        public void Play()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        /// <inheritdoc/>
        public void Pause()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task PopulateAlbumsAsync(int limit, int offset = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task PopulateRelatedArtistsAsync(int limit, int offset = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task PopulateTracksAsync(int limit, int offset = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task PlayAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task PauseAsync()
        {
            throw new NotImplementedException();
        }

        public Task PopulateRelatedItemsAsync(int limit, int offset = 0)
        {
            throw new NotImplementedException();
        }
    }
}
