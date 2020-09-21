using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using OwlCore.Collections;
using StrixMusic.Sdk.Enums;
using StrixMusic.Sdk.Interfaces;

namespace StrixMusic.Sdk.MergedWrappers
{
    /// <summary>
    /// A concrete class that merged multiple <see cref="ITrack"/>s.
    /// </summary>
    public class MergedTrack : ITrack, IEquatable<ITrack>
    {
        private readonly ITrack _preferredSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="MergedPlayableCollectionGroupBase"/> class.
        /// </summary>
        /// <param name="tracks">The <see cref="ITrack"/>s to merge together.</param>
        public MergedTrack(ITrack[] tracks)
        {
            if (tracks == null)
            {
                throw new ArgumentNullException(nameof(tracks));
            }

            // TODO: Use top Preferred core.
            _preferredSource = tracks.First();

            foreach (var item in tracks)
            {
                // TODO: Don't populate here
                // TODO: Deal with merged artists
                TotalArtistsCount += item.TotalArtistsCount;
            }
        }

        /// <inheritdoc/>
        public ICore SourceCore => _preferredSource.SourceCore;

        /// <inheritdoc/>
        public string Id => _preferredSource.Id;

        /// <inheritdoc/>
        public Uri? Url => _preferredSource.Url;

        /// <inheritdoc/>
        public string Name => _preferredSource.Name;

        /// <inheritdoc/>
        public TrackType Type => _preferredSource.Type;

        /// <inheritdoc/>
        public int TotalArtistsCount { get; }

        /// <inheritdoc/>
        public SynchronizedObservableCollection<IArtist> Artists { get; } = new SynchronizedObservableCollection<IArtist>();

        /// <inheritdoc/>
        public SynchronizedObservableCollection<string>? Genres => _preferredSource.Genres;

        /// <inheritdoc/>
        public SynchronizedObservableCollection<IImage> Images => _preferredSource.Images;

        /// <inheritdoc/>
        public IPlayableCollectionGroup? RelatedItems => _preferredSource.RelatedItems;

        /// <inheritdoc/>
        public IAlbum? Album => _preferredSource.Album;

        /// <inheritdoc/>
        public int? TrackNumber => _preferredSource.TrackNumber;

        /// <inheritdoc/>
        public CultureInfo? Language => _preferredSource.Language;

        /// <inheritdoc/>
        public ILyrics? Lyrics => _preferredSource.Lyrics;

        /// <inheritdoc/>
        public bool IsExplicit => _preferredSource.IsExplicit;

        /// <inheritdoc/>
        public string? Description => _preferredSource.Description;

        /// <inheritdoc/>
        public PlaybackState PlaybackState => _preferredSource.PlaybackState;

        /// <inheritdoc/>
        public TimeSpan Duration => _preferredSource.Duration;

        /// <inheritdoc/>
        public bool IsChangeAlbumAsyncSupported => _preferredSource.IsChangeAlbumAsyncSupported;

        /// <inheritdoc/>
        public bool IsChangeTrackNumberAsyncSupported => _preferredSource.IsChangeTrackNumberAsyncSupported;

        /// <inheritdoc/>
        public bool IsChangeLanguageAsyncSupported => _preferredSource.IsChangeLanguageAsyncSupported;

        /// <inheritdoc/>
        public bool IsChangeLyricsAsyncSupported => _preferredSource.IsChangeLyricsAsyncSupported;

        /// <inheritdoc/>
        public bool IsChangeIsExplicitAsyncSupported => _preferredSource.IsChangeIsExplicitAsyncSupported;

        /// <inheritdoc/>
        public bool IsPlayAsyncSupported => _preferredSource.IsPlayAsyncSupported;

        /// <inheritdoc/>
        public bool IsPauseAsyncSupported => _preferredSource.IsPauseAsyncSupported;

        /// <inheritdoc/>
        public bool IsChangeNameAsyncSupported => _preferredSource.IsChangeNameAsyncSupported;

        /// <inheritdoc/>
        public bool IsChangeDescriptionAsyncSupported => _preferredSource.IsChangeDescriptionAsyncSupported;

        /// <inheritdoc/>
        public bool IsChangeDurationAsyncSupported => _preferredSource.IsChangeDurationAsyncSupported;

        /// <inheritdoc/>
        public SynchronizedObservableCollection<bool> IsRemoveArtistSupportedMap { get; } = new SynchronizedObservableCollection<bool>();

        /// <inheritdoc/>
        public SynchronizedObservableCollection<bool> IsRemoveImageSupportedMap { get; } = new SynchronizedObservableCollection<bool>();

        /// <inheritdoc/>
        public SynchronizedObservableCollection<bool> IsRemoveGenreSupportedMap { get; } = new SynchronizedObservableCollection<bool>();

        /// <inheritdoc/>
        public event EventHandler<IAlbum?> AlbumChanged
        {
            add => _preferredSource.AlbumChanged += value;

            remove => _preferredSource.AlbumChanged -= value;
        }

        /// <inheritdoc/>
        public event EventHandler<int?> TrackNumberChanged
        {
            add => _preferredSource.TrackNumberChanged += value;

            remove => _preferredSource.TrackNumberChanged -= value;
        }

        /// <inheritdoc/>
        public event EventHandler<CultureInfo?> LanguageChanged
        {
            add => _preferredSource.LanguageChanged += value;

            remove => _preferredSource.LanguageChanged -= value;
        }

        /// <inheritdoc/>
        public event EventHandler<ILyrics?> LyricsChanged
        {
            add => _preferredSource.LyricsChanged += value;

            remove => _preferredSource.LyricsChanged -= value;
        }

        /// <inheritdoc/>
        public event EventHandler<bool> IsExplicitChanged
        {
            add => _preferredSource.IsExplicitChanged += value;

            remove => _preferredSource.IsExplicitChanged -= value;
        }

        /// <inheritdoc/>
        public event EventHandler<PlaybackState> PlaybackStateChanged
        {
            add => _preferredSource.PlaybackStateChanged += value;

            remove => _preferredSource.PlaybackStateChanged -= value;
        }

        /// <inheritdoc/>
        public event EventHandler<string> NameChanged
        {
            add => _preferredSource.NameChanged += value;

            remove => _preferredSource.NameChanged -= value;
        }

        /// <inheritdoc/>
        public event EventHandler<string?> DescriptionChanged
        {
            add => _preferredSource.DescriptionChanged += value;

            remove => _preferredSource.DescriptionChanged -= value;
        }

        /// <inheritdoc/>
        public event EventHandler<Uri?> UrlChanged
        {
            add => _preferredSource.UrlChanged += value;

            remove => _preferredSource.UrlChanged -= value;
        }

        /// <inheritdoc/>
        public event EventHandler<TimeSpan>? DurationChanged
        {
            add => _preferredSource.DurationChanged += value;

            remove => _preferredSource.DurationChanged -= value;
        }

        /// <inheritdoc/>
        public Task PauseAsync()
        {
            return _preferredSource.PauseAsync();
        }

        /// <inheritdoc/>
        public Task PlayAsync()
        {
            return _preferredSource.PlayAsync();
        }

        /// <inheritdoc/>
        public Task ChangeAlbumAsync(IAlbum? albums)
        {
            return _preferredSource.ChangeAlbumAsync(albums);
        }

        /// <inheritdoc/>
        public Task ChangeTrackNumberAsync(int? trackNumber)
        {
            return _preferredSource.ChangeTrackNumberAsync(trackNumber);
        }

        /// <inheritdoc/>
        public Task ChangeLanguageAsync(CultureInfo language)
        {
            return _preferredSource.ChangeLanguageAsync(language);
        }

        /// <inheritdoc/>
        public Task ChangeLyricsAsync(ILyrics? lyrics)
        {
            return _preferredSource.ChangeLyricsAsync(lyrics);
        }

        /// <inheritdoc/>
        public Task ChangeIsExplicitAsync(bool isExplicit)
        {
            return _preferredSource.ChangeIsExplicitAsync(isExplicit);
        }

        /// <inheritdoc/>
        public Task ChangeNameAsync(string name)
        {
            return _preferredSource.ChangeNameAsync(name);
        }

        /// <inheritdoc/>
        public Task ChangeDescriptionAsync(string? description)
        {
            return _preferredSource.ChangeDescriptionAsync(description);
        }

        /// <inheritdoc/>
        public Task ChangeDurationAsync(TimeSpan duration)
        {
            return _preferredSource.ChangeDurationAsync(duration);
        }

        /// <summary>
        /// Indicates whether the current item can be merged with another item of the same type.
        /// </summary>
        /// <param name="other">The item to check against.</param>
        /// <returns><see langword="True"/> if the item can be merged, otherwise <see langword="false"/></returns>
        public bool Equals(ITrack? other)
        {
            return other?.Name == Name
                && other?.Type.Equals(Type) == true
                && other?.Album?.Equals(Album) == true
                && other?.Language?.Equals(Language) == true
                && other?.TrackNumber?.Equals(TrackNumber) == true
                && other?.Artists?.SequenceEqual(Artists) == true;
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj) => Equals(obj as ITrack);

        /// <inheritdoc/>
        public override int GetHashCode() => _preferredSource.Id.GetHashCode();

        /// <inheritdoc/>
        public Task<bool> IsAddArtistSupported(int index)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<bool> IsAddImageSupported(int index)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<bool> IsAddGenreSupported(int index)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<IArtist> GetArtistsAsync(int limit, int offset)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task PopulateMoreArtistsAsync(int limit)
        {
            // TODO
            return _preferredSource.PopulateMoreArtistsAsync(limit);
        }
    }
}
