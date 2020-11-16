using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.Collections;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Sdk.Data.Merged
{
    /// <summary>
    /// A concrete class that merged multiple <see cref="ICoreTrack"/>s.
    /// </summary>
    public class MergedTrack : ITrack, IMerged<ICoreTrack>
    {
        private readonly ICoreTrack _preferredSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="MergedTrack"/> class.
        /// </summary>
        /// <param name="tracks">The <see cref="ICoreTrack"/>s to merge together.</param>
        public MergedTrack(IEnumerable<ICoreTrack> tracks)
        {
            if (tracks == null)
                throw new ArgumentNullException(nameof(tracks));

            var coreTracks = tracks as ICoreTrack[] ?? tracks.ToArray();

            Sources = coreTracks.ToList();

            // TODO: Use top Preferred core.
            _preferredSource = coreTracks.First();

            foreach (var item in coreTracks)
            {
                TotalArtistItemsCount += item.TotalArtistItemsCount;
            }

            Images = new SynchronizedObservableCollection<IImage>();
        }

        /// <inheritdoc/>
        public event EventHandler<IAlbum?>? AlbumChanged;

        /// <inheritdoc/>
        public event EventHandler<int?>? TrackNumberChanged;

        /// <inheritdoc/>
        public event EventHandler<CultureInfo?>? LanguageChanged;

        /// <inheritdoc/>
        public event EventHandler<ILyrics?>? LyricsChanged;

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

        /// <inheritdoc/>
        public string Id => _preferredSource.Id;

        /// <inheritdoc/>
        public Uri? Url => _preferredSource.Url;

        /// <inheritdoc/>
        public string Name => _preferredSource.Name;

        /// <inheritdoc/>
        public TrackType Type => _preferredSource.Type;

        /// <inheritdoc/>
        public int TotalArtistItemsCount { get; }

        /// <inheritdoc/>
        public SynchronizedObservableCollection<string>? Genres { get; }

        /// <inheritdoc/>
        public SynchronizedObservableCollection<IImage> Images { get; }

        /// <inheritdoc/>
        public IPlayableCollectionGroup? RelatedItems { get; }

        /// <inheritdoc/>
        public IAlbum? Album { get; }

        /// <inheritdoc/>
        public int? TrackNumber => _preferredSource.TrackNumber;

        /// <inheritdoc/>
        public int? DiscNumber => _preferredSource.DiscNumber;

        /// <inheritdoc/>
        public CultureInfo? Language => _preferredSource.Language;

        /// <inheritdoc/>
        public ILyrics? Lyrics { get; }

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
        public Task<bool> IsRemoveImageSupported(int index)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<bool> IsRemoveGenreSupported(int index)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<bool> IsRemoveArtistSupported(int index)
        {
            throw new NotImplementedException();
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
        public Task ChangeAlbumAsync(IAlbum? album)
        {
            var sourceToChange = album?.GetSources<ICoreAlbum>().First(x => x.SourceCore == _preferredSource.SourceCore);

            return _preferredSource.ChangeAlbumAsync(sourceToChange);
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
            var sourceToChange = lyrics?.GetSources().First(x => x.SourceCore == _preferredSource.SourceCore);

            return _preferredSource.ChangeLyricsAsync(sourceToChange);
        }

        /// <inheritdoc/>
        public Task ChangeIsExplicitAsync(bool isExplicit) => _preferredSource.ChangeIsExplicitAsync(isExplicit);

        /// <inheritdoc/>
        public Task ChangeNameAsync(string name) => _preferredSource.ChangeNameAsync(name);

        /// <inheritdoc/>
        public Task ChangeDescriptionAsync(string? description) => _preferredSource.ChangeDescriptionAsync(description);

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
                   && other?.TrackNumber?.Equals(TrackNumber) == true;
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj) => Equals(obj as ITrack);

        /// <inheritdoc/>
        public override int GetHashCode() => _preferredSource.Id.GetHashCode();

        /// <inheritdoc/>
        public Task<bool> IsAddArtistSupported(int index)
        {
            return _preferredSource.IsAddArtistSupported(index);
        }

        /// <inheritdoc/>
        public Task<bool> IsAddImageSupported(int index) => _preferredSource.IsAddImageSupported(index);

        /// <inheritdoc/>
        public Task<bool> IsAddGenreSupported(int index) => _preferredSource.IsAddGenreSupported(index);

        /// <inheritdoc/>
        public async Task<IReadOnlyList<IArtistCollectionItem>> GetArtistItemsAsync(int limit, int offset)
        {
            foreach (var coreTrack in Sources)
            {
                await foreach (var item in coreTrack.GetArtistItemsAsync(limit, offset))
                {

                }
            }
        }

        private List<IArtist> Artists { get; } = new List<IArtist>();

        private List<IArtistCollection> ArtistCollections { get; } = new List<IArtistCollection>();

        /// <inheritdoc/>
        public Task AddArtistItemAsync(IArtistCollectionItem artist, int index)
        {
            var sourceToChange = artist?.GetSources().First(x => x.SourceCore == _preferredSource.SourceCore);

            Guard.IsNotNull(sourceToChange, nameof(sourceToChange));

            return _preferredSource.AddArtistItemAsync(sourceToChange, index);
        }

        /// <inheritdoc/>
        public Task RemoveArtistAsync(int index) => _preferredSource.RemoveArtistAsync(index);

        /// <inheritdoc cref="ISdkMember{T}.SourceCores"/>
        public IReadOnlyList<ICore> SourceCores => Sources.Select(x => x.SourceCore).ToList();

        /// <inheritdoc />
        IReadOnlyList<ICoreTrack> ISdkMember<ICoreTrack>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreGenreCollection> ISdkMember<ICoreGenreCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreArtistCollection> ISdkMember<ICoreArtistCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreImageCollection> ISdkMember<ICoreImageCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreArtistCollectionItem> ISdkMember<ICoreArtistCollectionItem>.Sources => Sources;

        /// <summary>
        /// The original sources for this merged item.
        /// </summary>
        public IReadOnlyList<ICoreTrack> Sources { get; }
    }
}
