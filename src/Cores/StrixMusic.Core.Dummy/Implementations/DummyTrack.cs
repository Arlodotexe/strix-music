using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StrixMusic.CoreInterfaces;
using StrixMusic.CoreInterfaces.Enums;
using StrixMusic.CoreInterfaces.Interfaces;

namespace StrixMusic.Core.Dummy.Implementations
{
    /// <inheritdoc/>
    public class DummyTrack : ITrack
    {
        /// <inheritdoc/>
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        /// <inheritdoc/>
        public IReadOnlyList<IImage> Images => throw new NotImplementedException();

        /// <inheritdoc/>
        public Uri Url => throw new NotImplementedException();

        /// <inheritdoc/>
        public TrackType Type => TrackType.Song;

        /// <inheritdoc/>
        [JsonProperty("title")]
        public string Name { get; set; } = string.Empty;

        /// <inheritdoc/>
        public IArtist Artist => DummyArtist!;

        /// <summary>
        /// Full <see cref="DummyArtist"/> to be used within the DummyCore.
        /// </summary>
        public DummyArtist? DummyArtist { get; set; }

        /// <summary>
        /// The Id of the <see cref="DummyArtist"/>.
        /// </summary>
        [JsonProperty("artist_id")]
        public string ArtistId { get; set; } = string.Empty;

        /// <inheritdoc/>
        public IAlbum Album => DummyAlbum!;

        /// <summary>
        /// Full <see cref="DummyAlbum"/> to be used within the DummyCore.
        /// </summary>
        public DummyAlbum? DummyAlbum { get; set; }

        /// <summary>
        /// The Id of the <see cref="DummyAlbum"/>.
        /// </summary>
        [JsonProperty("album_id")]
        public string AlbumId { get; set; } = string.Empty;

        /// <inheritdoc/>
        public DateTime? DatePublished => throw new NotImplementedException();

        /// <inheritdoc/>
        public IReadOnlyList<string> Genres => throw new NotImplementedException();

        /// <inheritdoc/>
        public int? TrackNumber => throw new NotImplementedException();

        /// <inheritdoc/>
        public int PlayCount => throw new NotImplementedException();

        /// <inheritdoc/>
        public CultureInfo Language => throw new NotImplementedException();

        /// <inheritdoc/>
        public ILyrics Lyrics => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool IsExplicit { get; }

        /// <inheritdoc/>
        public TimeSpan Duration { get; }

        /// <inheritdoc/>
        public string Description => throw new NotImplementedException();

        /// <inheritdoc/>
        public ICore SourceCore => DummyCore!;

        /// <summary>
        /// The <see cref="DummyCore"/> where the <see cref="DummyTrack"/> is from.
        /// </summary>
        public DummyCore? DummyCore { get; set; }

        /// <inheritdoc/>
        public PlaybackState PlaybackState => throw new NotImplementedException();

        /// <inheritdoc/>
        public IReadOnlyList<IPlayableCollectionGroup> RelatedItems => throw new NotImplementedException();

        /// <inheritdoc/>
        public int TotalRelatedItemsCount => throw new NotImplementedException();

        /// <inheritdoc/>
        IReadOnlyList<IArtist> ITrack.Artists => throw new NotImplementedException();

        /// <inheritdoc/>
        int? ITrack.PlayCount => throw new NotImplementedException();

        /// <inheritdoc/>
        public event EventHandler<PlaybackState>? PlaybackStateChanged;

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<IImage>>? ImagesChanged;

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<IPlayableCollectionGroup>>? RelatedItemsChanged;

        /// <inheritdoc/>
        public event EventHandler<string>? NameChanged
        {
            add
            {
                NameChanged += value;
            }

            remove
            {
                NameChanged += value;
            }
        }

        /// <inheritdoc/>
        public event EventHandler<string?> DescriptionChanged
        {
            add
            {
                DescriptionChanged += value;
            }

            remove
            {
                DescriptionChanged += value;
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
                UrlChanged += value;
            }
        }

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<IArtist>> ArtistsChanged
        {
            add
            {
                ArtistsChanged += value;
            }

            remove
            {
                ArtistsChanged -= value;
            }
        }

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<string>> GenresChanged
        {
            add
            {
                GenresChanged += value;
            }

            remove
            {
                GenresChanged -= value;
            }
        }

        /// <inheritdoc/>
        public event EventHandler<IAlbum?> AlbumChanged
        {
            add
            {
                AlbumChanged += value;
            }

            remove
            {
                AlbumChanged -= value;
            }
        }

        /// <inheritdoc/>
        public event EventHandler<DateTime?> DatePublishedChanged
        {
            add
            {
                DatePublishedChanged += value;
            }

            remove
            {
                DatePublishedChanged -= value;
            }
        }

        /// <inheritdoc/>
        public event EventHandler<int?> TrackNumberChanged
        {
            add
            {
                TrackNumberChanged += value;
            }

            remove
            {
                TrackNumberChanged -= value;
            }
        }

        /// <inheritdoc/>
        public event EventHandler<int?> PlayCountChanged
        {
            add
            {
                PlayCountChanged += value;
            }

            remove
            {
                PlayCountChanged -= value;
            }
        }

        /// <inheritdoc/>
        public event EventHandler<CultureInfo?> LanguageChanged
        {
            add
            {
                LanguageChanged += value;
            }

            remove
            {
                LanguageChanged -= value;
            }
        }

        /// <inheritdoc/>
        public event EventHandler<ILyrics?> LyricsChanged
        {
            add
            {
                LyricsChanged += value;
            }

            remove
            {
                LyricsChanged -= value;
            }
        }

        /// <inheritdoc/>
        public event EventHandler<bool> IsExplicitChanged
        {
            add
            {
                IsExplicitChanged += value;
            }

            remove
            {
                IsExplicitChanged -= value;
            }
        }

        /// <inheritdoc/>
        public void Pause()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task PauseAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void Play()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task PlayAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task PopulateRelatedItemsAsync(int limit, int offset = 0)
        {
            throw new NotImplementedException();
        }
    }
}
