// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OwlCore.ComponentModel;
using OwlCore.Events;
using StrixMusic.Sdk.AdapterModels;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Sdk.Plugins.Model
{
    /// <summary>
    /// An implementation of <see cref="ITrack"/> which delegates all member access to the <see cref="Inner"/> implementation,
    /// unless the member is overridden in a derived class which changes the behavior.
    /// </summary>
    public class TrackPluginBase : IModelPlugin, ITrack, IDelegatable<ITrack>
    {
        /// <summary>
        /// Creates a new instance of <see cref="TrackPluginBase"/>.
        /// </summary>
        /// <param name="registration">Metadata about the plugin which was provided during registration.</param>
        /// <param name="inner">The implementation which all member access is delegated to, unless the member is overridden in a derived class which changes the behavior.</param>
        protected internal TrackPluginBase(ModelPluginMetadata registration, ITrack inner)
        {
            Metadata = registration;
            Inner = inner;
            InnerArtistCollection = inner;
            InnerImageCollection = inner;
            InnerUrlCollection = inner;
            InnerGenreCollection = inner;
            InnerPlayable = inner;
            InnerDownloadable = inner;
        }

        /// <summary>
        /// Metadata about the plugin which was provided during registration.
        /// </summary>
        public ModelPluginMetadata Metadata { get; }

        /// <inheritdoc/>
        public ITrack Inner { get; set; }

        /// <summary>
        /// A wrapped implementation which member access can be delegated to. Defaults to <see cref="Inner"/>.
        /// </summary>
        public IPlayable InnerPlayable { get; set; }

        /// <summary>
        /// A wrapped implementation which member access can be delegated to. Defaults to <see cref="Inner"/>.
        /// </summary>
        public IDownloadable InnerDownloadable { get; set; }

        /// <summary>
        /// A wrapped implementation which member access can be delegated to. Defaults to <see cref="Inner"/>.
        /// </summary>
        public IArtistCollection InnerArtistCollection { get; set; }

        /// <summary>
        /// A wrapped implementation which member access can be delegated to. Defaults to <see cref="Inner"/>.
        /// </summary>
        public IImageCollection InnerImageCollection { get; set; }

        /// <summary>
        /// A wrapped implementation which member access can be delegated to. Defaults to <see cref="Inner"/>.
        /// </summary>
        public IGenreCollection InnerGenreCollection { get; set; }

        /// <summary>
        /// A wrapped implementation which member access can be delegated to. Defaults to <see cref="Inner"/>.
        /// </summary>
        public IUrlCollection InnerUrlCollection { get; set; }

        /// <inheritdoc/>
        public event EventHandler? SourcesChanged
        {
            add => Inner.SourcesChanged += value;
            remove => Inner.SourcesChanged -= value;
        }

        /// <inheritdoc />
        public virtual DownloadInfo DownloadInfo => InnerDownloadable.DownloadInfo;

        /// <inheritdoc />
        public virtual Task StartDownloadOperationAsync(DownloadOperation operation, CancellationToken cancellationToken = default) => InnerDownloadable.StartDownloadOperationAsync(operation, cancellationToken);

        /// <inheritdoc />
        public virtual event EventHandler<DownloadInfo>? DownloadInfoChanged
        {
            add => InnerDownloadable.DownloadInfoChanged += value;
            remove => InnerDownloadable.DownloadInfoChanged -= value;
        }

        /// <inheritdoc />
        public virtual Task<bool> IsAddImageAvailableAsync(int index, CancellationToken cancellationToken = default) => InnerImageCollection.IsAddImageAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public virtual Task<bool> IsRemoveImageAvailableAsync(int index, CancellationToken cancellationToken = default) => InnerImageCollection.IsRemoveImageAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public virtual Task RemoveImageAsync(int index, CancellationToken cancellationToken = default) => InnerImageCollection.RemoveImageAsync(index, cancellationToken);

        /// <inheritdoc />
        public virtual int TotalImageCount => InnerImageCollection.TotalImageCount;

        /// <inheritdoc />
        public virtual event EventHandler<int>? ImagesCountChanged
        {
            add => InnerImageCollection.ImagesCountChanged += value;
            remove => InnerImageCollection.ImagesCountChanged -= value;
        }

        /// <inheritdoc/>
        public IReadOnlyList<ICoreGenreCollection> Sources => InnerGenreCollection.Sources;

        /// <inheritdoc/>
        IReadOnlyList<ICoreTrack> IMerged<ICoreTrack>.Sources => ((IMerged<ICoreTrack>)Inner).Sources;

        /// <inheritdoc/>
        IReadOnlyList<ICoreImageCollection> IMerged<ICoreImageCollection>.Sources => InnerImageCollection.Sources;

        /// <inheritdoc/>
        IReadOnlyList<ICoreUrlCollection> IMerged<ICoreUrlCollection>.Sources => InnerUrlCollection.Sources;

        /// <inheritdoc/>
        IReadOnlyList<ICoreArtistCollectionItem> IMerged<ICoreArtistCollectionItem>.Sources =>
            ((IMerged<ICoreArtistCollectionItem>)InnerArtistCollection).Sources;

        /// <inheritdoc/>
        IReadOnlyList<ICoreArtistCollection> IMerged<ICoreArtistCollection>.Sources =>
            ((IMerged<ICoreArtistCollection>)InnerArtistCollection).Sources;

        /// <inheritdoc />
        public virtual IAsyncEnumerable<IImage> GetImagesAsync(int limit, int offset, CancellationToken cancellationToken = default) => InnerImageCollection.GetImagesAsync(limit, offset, cancellationToken);

        /// <inheritdoc />
        public virtual Task AddImageAsync(IImage image, int index, CancellationToken cancellationToken = default) => InnerImageCollection.AddImageAsync(image, index, cancellationToken);

        /// <inheritdoc />
        public virtual event CollectionChangedEventHandler<IImage>? ImagesChanged
        {
            add => InnerImageCollection.ImagesChanged += value;
            remove => InnerImageCollection.ImagesChanged -= value;
        }

        /// <inheritdoc />
        public virtual int TotalUrlCount => InnerUrlCollection.TotalUrlCount;

        /// <inheritdoc />
        public virtual Task RemoveUrlAsync(int index, CancellationToken cancellationToken = default) => InnerUrlCollection.RemoveUrlAsync(index, cancellationToken);

        /// <inheritdoc />
        public virtual Task<bool> IsAddUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => InnerUrlCollection.IsAddUrlAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public virtual Task<bool> IsRemoveUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => InnerUrlCollection.IsRemoveUrlAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public virtual event EventHandler<int>? UrlsCountChanged
        {
            add => InnerUrlCollection.UrlsCountChanged += value;
            remove => InnerUrlCollection.UrlsCountChanged -= value;
        }

        /// <inheritdoc />
        public virtual bool Equals(ICoreUrlCollection other) => InnerUrlCollection.Equals(other);

        /// <inheritdoc />
        public virtual IAsyncEnumerable<IUrl> GetUrlsAsync(int limit, int offset, CancellationToken cancellationToken = default) => InnerUrlCollection.GetUrlsAsync(limit, offset, cancellationToken);

        /// <inheritdoc />
        public virtual Task AddUrlAsync(IUrl url, int index, CancellationToken cancellationToken = default) => InnerUrlCollection.AddUrlAsync(url, index, cancellationToken);

        /// <inheritdoc />
        public virtual event CollectionChangedEventHandler<IUrl>? UrlsChanged
        {
            add => InnerUrlCollection.UrlsChanged += value;
            remove => InnerUrlCollection.UrlsChanged -= value;
        }

        /// <inheritdoc />
        public virtual string Id => InnerPlayable.Id;

        /// <inheritdoc />
        public virtual string Name => InnerPlayable.Name;

        /// <inheritdoc />
        public virtual string? Description => InnerPlayable.Description;

        /// <inheritdoc />
        public virtual DateTime? LastPlayed => InnerPlayable.LastPlayed;

        /// <inheritdoc />
        public virtual PlaybackState PlaybackState => InnerPlayable.PlaybackState;

        /// <inheritdoc />
        public virtual TimeSpan Duration => InnerPlayable.Duration;

        /// <inheritdoc />
        public virtual bool IsChangeNameAsyncAvailable => InnerPlayable.IsChangeNameAsyncAvailable;

        /// <inheritdoc />
        public virtual bool IsChangeDescriptionAsyncAvailable => InnerPlayable.IsChangeDescriptionAsyncAvailable;

        /// <inheritdoc />
        public virtual bool IsChangeDurationAsyncAvailable => InnerPlayable.IsChangeDurationAsyncAvailable;

        /// <inheritdoc />
        public virtual Task ChangeNameAsync(string name, CancellationToken cancellationToken = default) => InnerPlayable.ChangeNameAsync(name, cancellationToken);

        /// <inheritdoc />
        public virtual Task ChangeDescriptionAsync(string? description, CancellationToken cancellationToken = default) => InnerPlayable.ChangeDescriptionAsync(description, cancellationToken);

        /// <inheritdoc />
        public virtual Task ChangeDurationAsync(TimeSpan duration, CancellationToken cancellationToken = default) => InnerPlayable.ChangeDurationAsync(duration, cancellationToken);

        /// <inheritdoc />
        public virtual event EventHandler<PlaybackState>? PlaybackStateChanged
        {
            add => InnerPlayable.PlaybackStateChanged += value;
            remove => InnerPlayable.PlaybackStateChanged -= value;
        }

        /// <inheritdoc />
        public virtual event EventHandler<string>? NameChanged
        {
            add => InnerPlayable.NameChanged += value;
            remove => InnerPlayable.NameChanged -= value;
        }

        /// <inheritdoc />
        public virtual event EventHandler<string?>? DescriptionChanged
        {
            add => InnerPlayable.DescriptionChanged += value;
            remove => InnerPlayable.DescriptionChanged -= value;
        }

        /// <inheritdoc />
        public virtual event EventHandler<TimeSpan>? DurationChanged
        {
            add => InnerPlayable.DurationChanged += value;
            remove => InnerPlayable.DurationChanged -= value;
        }

        /// <inheritdoc />
        public virtual event EventHandler<DateTime?>? LastPlayedChanged
        {
            add => InnerPlayable.LastPlayedChanged += value;
            remove => InnerPlayable.LastPlayedChanged -= value;
        }

        /// <inheritdoc />
        public virtual event EventHandler<bool>? IsChangeNameAsyncAvailableChanged
        {
            add => InnerPlayable.IsChangeNameAsyncAvailableChanged += value;
            remove => InnerPlayable.IsChangeNameAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public virtual event EventHandler<bool>? IsChangeDescriptionAsyncAvailableChanged
        {
            add => InnerPlayable.IsChangeDescriptionAsyncAvailableChanged += value;
            remove => InnerPlayable.IsChangeDescriptionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public virtual event EventHandler<bool>? IsChangeDurationAsyncAvailableChanged
        {
            add => InnerPlayable.IsChangeDurationAsyncAvailableChanged += value;
            remove => InnerPlayable.IsChangeDurationAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public virtual int TotalGenreCount => InnerGenreCollection.TotalGenreCount;

        /// <inheritdoc />
        public virtual Task RemoveGenreAsync(int index, CancellationToken cancellationToken = default) => InnerGenreCollection.RemoveGenreAsync(index, cancellationToken);

        /// <inheritdoc />
        public virtual Task<bool> IsAddGenreAvailableAsync(int index, CancellationToken cancellationToken = default) => InnerGenreCollection.IsAddGenreAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public virtual Task<bool> IsRemoveGenreAvailableAsync(int index, CancellationToken cancellationToken = default) => InnerGenreCollection.IsRemoveGenreAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public virtual event EventHandler<int>? GenresCountChanged
        {
            add => InnerGenreCollection.GenresCountChanged += value;
            remove => InnerGenreCollection.GenresCountChanged -= value;
        }

        /// <inheritdoc />
        public virtual bool Equals(ICoreGenreCollection other) => InnerGenreCollection.Equals(other);

        /// <inheritdoc />
        public virtual IAsyncEnumerable<IGenre> GetGenresAsync(int limit, int offset, CancellationToken cancellationToken = default) => InnerGenreCollection.GetGenresAsync(limit, offset, cancellationToken);

        /// <inheritdoc />
        public virtual Task AddGenreAsync(IGenre genre, int index, CancellationToken cancellationToken = default) => InnerGenreCollection.AddGenreAsync(genre, index, cancellationToken);

        /// <inheritdoc />
        public virtual event CollectionChangedEventHandler<IGenre>? GenresChanged
        {
            add => InnerGenreCollection.GenresChanged += value;
            remove => InnerGenreCollection.GenresChanged -= value;
        }

        /// <inheritdoc />
        public virtual DateTime? AddedAt => Inner.AddedAt;

        /// <inheritdoc />
        public virtual int TotalArtistItemsCount => InnerArtistCollection.TotalArtistItemsCount;

        /// <inheritdoc />
        public virtual bool IsPlayArtistCollectionAsyncAvailable => InnerArtistCollection.IsPlayArtistCollectionAsyncAvailable;

        /// <inheritdoc />
        public virtual bool IsPauseArtistCollectionAsyncAvailable => InnerArtistCollection.IsPauseArtistCollectionAsyncAvailable;

        /// <inheritdoc />
        public virtual Task PlayArtistCollectionAsync(CancellationToken cancellationToken = default) => InnerArtistCollection.PlayArtistCollectionAsync(cancellationToken);

        /// <inheritdoc />
        public virtual Task PauseArtistCollectionAsync(CancellationToken cancellationToken = default) => InnerArtistCollection.PauseArtistCollectionAsync(cancellationToken);

        /// <inheritdoc />
        public virtual Task RemoveArtistItemAsync(int index, CancellationToken cancellationToken = default) => InnerArtistCollection.RemoveArtistItemAsync(index, cancellationToken);

        /// <inheritdoc />
        public virtual Task<bool> IsAddArtistItemAvailableAsync(int index, CancellationToken cancellationToken = default) => InnerArtistCollection.IsAddArtistItemAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public virtual Task<bool> IsRemoveArtistItemAvailableAsync(int index, CancellationToken cancellationToken = default) => InnerArtistCollection.IsRemoveArtistItemAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public virtual event EventHandler<bool>? IsPlayArtistCollectionAsyncAvailableChanged
        {
            add => InnerArtistCollection.IsPlayArtistCollectionAsyncAvailableChanged += value;
            remove => InnerArtistCollection.IsPlayArtistCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public virtual event EventHandler<bool>? IsPauseArtistCollectionAsyncAvailableChanged
        {
            add => InnerArtistCollection.IsPauseArtistCollectionAsyncAvailableChanged += value;
            remove => InnerArtistCollection.IsPauseArtistCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public virtual event EventHandler<int>? ArtistItemsCountChanged
        {
            add => InnerArtistCollection.ArtistItemsCountChanged += value;
            remove => InnerArtistCollection.ArtistItemsCountChanged -= value;
        }

        /// <inheritdoc />
        public virtual bool Equals(ICoreArtistCollectionItem other) => InnerArtistCollection.Equals(other);

        /// <inheritdoc />
        public virtual bool Equals(ICoreArtistCollection other) => InnerArtistCollection.Equals(other);

        /// <inheritdoc />
        public virtual Task PlayArtistCollectionAsync(IArtistCollectionItem artistItem, CancellationToken cancellationToken = default) => InnerArtistCollection.PlayArtistCollectionAsync(artistItem, cancellationToken);

        /// <inheritdoc />
        public virtual IAsyncEnumerable<IArtistCollectionItem> GetArtistItemsAsync(int limit, int offset, CancellationToken cancellationToken = default) => InnerArtistCollection.GetArtistItemsAsync(limit, offset, cancellationToken);

        /// <inheritdoc />
        public virtual Task AddArtistItemAsync(IArtistCollectionItem artistItem, int index, CancellationToken cancellationToken = default) => InnerArtistCollection.AddArtistItemAsync(artistItem, index, cancellationToken);

        /// <inheritdoc />
        public virtual event CollectionChangedEventHandler<IArtistCollectionItem>? ArtistItemsChanged
        {
            add => InnerArtistCollection.ArtistItemsChanged += value;
            remove => InnerArtistCollection.ArtistItemsChanged -= value;
        }

        /// <inheritdoc />
        public virtual TrackType Type => Inner.Type;

        /// <inheritdoc />
        public virtual int? TrackNumber => Inner.TrackNumber;

        /// <inheritdoc />
        public virtual int? DiscNumber => Inner.DiscNumber;

        /// <inheritdoc />
        public virtual CultureInfo? Language => Inner.Language;

        /// <inheritdoc />
        public virtual bool IsExplicit => Inner.IsExplicit;

        /// <inheritdoc />
        public virtual bool IsChangeAlbumAsyncAvailable => Inner.IsChangeAlbumAsyncAvailable;

        /// <inheritdoc />
        public virtual bool IsChangeTrackNumberAsyncAvailable => Inner.IsChangeTrackNumberAsyncAvailable;

        /// <inheritdoc />
        public virtual bool IsChangeLanguageAsyncAvailable => Inner.IsChangeLanguageAsyncAvailable;

        /// <inheritdoc />
        public virtual bool IsChangeLyricsAsyncAvailable => Inner.IsChangeLyricsAsyncAvailable;

        /// <inheritdoc />
        public virtual bool IsChangeIsExplicitAsyncAvailable => Inner.IsChangeIsExplicitAsyncAvailable;

        /// <inheritdoc />
        public virtual Task ChangeTrackNumberAsync(int? trackNumber, CancellationToken cancellationToken = default) => Inner.ChangeTrackNumberAsync(trackNumber, cancellationToken);

        /// <inheritdoc />
        public virtual Task ChangeLanguageAsync(CultureInfo language, CancellationToken cancellationToken = default) => Inner.ChangeLanguageAsync(language, cancellationToken);

        /// <inheritdoc />
        public virtual Task ChangeIsExplicitAsync(bool isExplicit, CancellationToken cancellationToken = default) => Inner.ChangeIsExplicitAsync(isExplicit, cancellationToken);

        /// <inheritdoc />
        public virtual event EventHandler<int?>? TrackNumberChanged
        {
            add => Inner.TrackNumberChanged += value;
            remove => Inner.TrackNumberChanged -= value;
        }

        /// <inheritdoc />
        public virtual event EventHandler<CultureInfo?>? LanguageChanged
        {
            add => Inner.LanguageChanged += value;
            remove => Inner.LanguageChanged -= value;
        }

        /// <inheritdoc />
        public virtual event EventHandler<bool>? IsExplicitChanged
        {
            add => Inner.IsExplicitChanged += value;
            remove => Inner.IsExplicitChanged -= value;
        }

        /// <inheritdoc />
        public virtual bool Equals(ICoreImageCollection other) => InnerImageCollection.Equals(other);

        /// <inheritdoc />
        public virtual bool Equals(ICoreTrack other) => Inner.Equals(other);

        /// <inheritdoc />
        public virtual IAlbum? Album => Inner.Album;

        /// <inheritdoc />
        public virtual ILyrics? Lyrics => Inner.Lyrics;

        /// <inheritdoc />
        public virtual IPlayableCollectionGroup? RelatedItems => Inner.RelatedItems;

        /// <inheritdoc />
        public virtual Task ChangeLyricsAsync(ILyrics? lyrics, CancellationToken cancellationToken = default) => Inner.ChangeLyricsAsync(lyrics, cancellationToken);

        /// <inheritdoc />
        public virtual event EventHandler<IAlbum?>? AlbumChanged
        {
            add => Inner.AlbumChanged += value;
            remove => Inner.AlbumChanged -= value;
        }

        /// <inheritdoc />
        public virtual event EventHandler<ILyrics?>? LyricsChanged
        {
            add => Inner.LyricsChanged += value;
            remove => Inner.LyricsChanged -= value;
        }

        /// <inheritdoc />
        public virtual Task ChangeAlbumAsync(IAlbum? album, CancellationToken cancellationToken = default) => Inner.ChangeAlbumAsync(album, cancellationToken);

        /// <inheritdoc/>
        public virtual ValueTask DisposeAsync()
        {
            var uniqueInstances = new HashSet<IAsyncDisposable>()
            {
                Inner,
                InnerArtistCollection,
                InnerDownloadable,
                InnerPlayable,
                InnerImageCollection,
                InnerUrlCollection,
                InnerGenreCollection,
            };

            return new ValueTask(uniqueInstances.AsParallel()
                .Select(x => x.DisposeAsync().AsTask())
                .Aggregate((x, y) => Task.WhenAll(x, y)));
        }
    }
}
