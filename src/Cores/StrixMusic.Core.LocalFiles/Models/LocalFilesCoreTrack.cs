using OwlCore.Collections;
using OwlCore.Events;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.MediaPlayback;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using StrixMusic.Sdk.Services.FileMetadataManager;
using StrixMusic.Sdk.Services.FileMetadataManager.Models;

namespace StrixMusic.Core.LocalFiles.Models
{
    ///NOTE: There are some methods set to NotAvailabletemporarily although they are supported, so the playback can be implemented.
    /// <inheritdoc />
    public class LocalFilesCoreTrack : ICoreTrack
    {
        private TrackMetadata _trackMetadata;
        private IFileMetadataManager _fileMetadataManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalFilesCoreTrack"/> class.
        /// </summary>
        /// <param name="sourceCore">The source core.</param>
        public LocalFilesCoreTrack(ICore sourceCore, TrackMetadata trackMetadata)
        {
            SourceCore = sourceCore;
            _trackMetadata = trackMetadata;
        }

        /// <inheritdoc/>
        public event EventHandler<ICoreAlbum?>? AlbumChanged;

        /// <inheritdoc/>
        public event EventHandler<int?>? TrackNumberChanged;

        /// <inheritdoc/>
        public event EventHandler<CultureInfo?>? LanguageChanged;

        /// <inheritdoc/>
        public event EventHandler<ICoreLyrics?>? LyricsChanged;

        /// <inheritdoc/>
        public event EventHandler<bool>? IsExplicitChanged;

        /// <inheritdoc/>
        public event EventHandler<PlaybackState>? PlaybackStateChanged;

        /// <inheritdoc/>
        public event EventHandler<string>? NameChanged;

        /// <inheritdoc/>
        public event EventHandler<string?>? DescriptionChanged;

        /// <inheritdoc/>
        public event EventHandler<Uri?>? UrlChanged;

        /// <inheritdoc/>
        public event EventHandler<TimeSpan>? DurationChanged;

        /// <inheritdoc />
        public event EventHandler<DateTime?>? LastPlayedChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsPlayAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsPauseAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeNameAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeDescriptionAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeDurationAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<int>? ImagesCountChanged;

        /// <inheritdoc />
        public event EventHandler<int>? ArtistItemsCountChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ICoreImage>? ImagesChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ICoreArtistCollectionItem>? ArtistItemsChanged;

        /// <inheritdoc/>
        public string Id => _trackMetadata.Id;

        /// <inheritdoc/>
        public TrackType Type => TrackType.Song;

        /// <inheritdoc />
        public int TotalArtistItemsCount => _trackMetadata.ArtistIds.Count;

        /// <inheritdoc />
        public int TotalImageCount { get; } = 0;

        /// <inheritdoc/>
        public ICoreAlbum? Album { get; }

        /// <inheritdoc/>
        public SynchronizedObservableCollection<string>? Genres => new SynchronizedObservableCollection<string>();

        /// <inheritdoc/>
        /// <remarks>Is not passed into the constructor. Should be set on object creation.</remarks>
        public int? TrackNumber => Convert.ToInt32(_trackMetadata.TrackNumber);

        /// <inheritdoc />
        public int? DiscNumber { get; }

        /// <inheritdoc/>
        public CultureInfo? Language { get; }

        /// <inheritdoc/>
        public ICoreLyrics? Lyrics => null;

        /// <inheritdoc/>
        public bool IsExplicit => false;

        /// <inheritdoc/>
        public ICore SourceCore { get; }

        /// <inheritdoc/>
        public Uri? LocalTrackPath => _trackMetadata.Source;

        /// <inheritdoc/>
        public Uri? Url => null;

        /// <inheritdoc/>
        public string Name => _trackMetadata.Title ?? "No Title";

        /// <inheritdoc/>
        public string? Description => _trackMetadata.Description;

        /// <inheritdoc/>
        public PlaybackState PlaybackState { get; private set; }

        /// <inheritdoc/>
        public TimeSpan Duration => _trackMetadata.Duration ?? new TimeSpan(0, 0, 0);

        /// <inheritdoc />
        public DateTime? LastPlayed { get; }

        /// <inheritdoc />
        public DateTime? AddedAt { get; }

        /// <inheritdoc/>
        public ICorePlayableCollectionGroup? RelatedItems => null;

        /// <inheritdoc/>
        public bool IsChangeAlbumAsyncAvailable => false;

        /// <inheritdoc/>
        public bool IsChangeTrackNumberAsyncAvailable => false;

        /// <inheritdoc/>
        public bool IsChangeLanguageAsyncAvailable => false;

        /// <inheritdoc/>
        public bool IsChangeLyricsAsyncAvailable => false;

        /// <inheritdoc/>
        public bool IsChangeIsExplicitAsyncAvailable => false;

        /// <inheritdoc/>
        public bool IsPlayAsyncAvailable => false;

        /// <inheritdoc/>
        public bool IsPauseAsyncAvailable => false;

        /// <inheritdoc/>
        public bool IsChangeNameAsyncAvailable => false;

        /// <inheritdoc/>
        public bool IsChangeDescriptionAsyncAvailable => false;

        /// <inheritdoc/>
        public bool IsChangeDurationAsyncAvailable => false;

        /// <inheritdoc/>
        public Task<bool> IsAddImageAvailable(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc/>
        public Task<bool> IsAddGenreAvailable(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc/>
        public Task<bool> IsAddArtistItemAvailable(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveImageAvailable(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveArtistItemAvailable(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveGenreAvailable(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc/>
        public Task ChangeAlbumAsync(ICoreAlbum? albums)
        {
            throw new NotSupportedException();//temporary for playback
        }

        /// <inheritdoc/>
        public Task ChangeDescriptionAsync(string? description)
        {
            throw new NotSupportedException();//temporary for playback
        }

        /// <inheritdoc/>
        public Task ChangeDurationAsync(TimeSpan duration)
        {
            throw new NotSupportedException();//temporary for playback
        }

        /// <inheritdoc/>
        public Task ChangeIsExplicitAsync(bool isExplicit)
        {
            throw new NotSupportedException();//temporary for playback
        }

        /// <inheritdoc/>
        public Task ChangeLanguageAsync(CultureInfo language)
        {
            throw new NotSupportedException();//temporary for playback
        }

        /// <inheritdoc/>
        public Task ChangeLyricsAsync(ICoreLyrics? lyrics)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public Task ChangeNameAsync(string name)
        {
            throw new NotSupportedException();//temporary for playback
        }

        /// <inheritdoc/>
        public Task ChangeTrackNumberAsync(int? trackNumber)
        {
            throw new NotSupportedException();//temporary for playback
        }

        /// <inheritdoc/>
        public Task PauseAsync()
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task PlayAsync()
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task AddArtistItemAsync(ICoreArtistCollectionItem artist, int index)
        {
            throw new NotSupportedException();//temporary for playback
        }

        /// <inheritdoc />
        public Task RemoveArtistItemAsync(int index)
        {
            throw new NotSupportedException();//temporary for playback
        }

        /// <inheritdoc />
        public Task AddImageAsync(ICoreImage image, int index)
        {
            throw new NotSupportedException();//temporary for playback
        }

        /// <inheritdoc />
        public Task RemoveImageAsync(int index)
        {
            throw new NotSupportedException();//temporary for playback
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<ICoreArtistCollectionItem> GetArtistItemsAsync(int limit, int offset)
        {
            var artistRepo = _fileMetadataManager.Artists;

            var artists = await artistRepo.GetArtistsByTrackId(Id, offset, limit);

            foreach (var artist in artists)
            {
                // just to test
                var tracks = await SourceCore.GetService<TrackRepository>().GetTracksByAlbumId(artist.Id, 0, 1000);
                yield return new LocalFilesCoreArtist(SourceCore, artist, tracks.Count);
            }
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<ICoreImage> GetImagesAsync(int limit, int offset)
        {
            //temporary just to show tracks
            yield return null;//temporary for playback
        }
    }
}
