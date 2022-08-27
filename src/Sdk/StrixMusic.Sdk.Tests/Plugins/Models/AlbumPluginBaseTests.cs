using Microsoft.VisualStudio.TestTools.UnitTesting;
using OwlCore.Events;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Plugins.Model;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using StrixMusic.Sdk.AdapterModels;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.BaseModels;
using StrixMusic.Sdk.CoreModels;

namespace StrixMusic.Sdk.Tests.Plugins.Models
{
    [TestClass]
    public class AlbumPluginBaseTests
    {
        private static bool NoInner(MemberInfo x) => !x.Name.Contains("Inner");
        private static bool NoInnerOrSources(MemberInfo x) => NoInner(x) && !x.Name.ToLower().Contains("sources");

        [Flags]
        public enum PossiblePlugins
        {
            None = 0,
            Playable = 1,
            Downloadable = 2,
            ArtistCollection = 4,
            TrackCollection = 8,
            ImageCollection = 16,
            GenreCollection = 32,
            UrlCollection = 64,
        }

        [TestMethod, Timeout(5000)]
        public void NoPlugins()
        {
            var builder = new SdkModelPlugin(SdkTestPluginMetadata.Metadata).Album;
            var finalTestClass = new Unimplemented();

            var emptyChain = builder.Execute(finalTestClass);
            Assert.AreSame(emptyChain, finalTestClass);

            Helpers.AssertAllThrowsOnMemberAccess<AccessedException<Unimplemented>>(finalTestClass);
            Helpers.AssertAllThrowsOnMemberAccess<AccessedException<Unimplemented>>(emptyChain);
        }

        [TestMethod, Timeout(5000)]
        public void PluginNoOverride()
        {
            // No plugins.
            var builder = new SdkModelPlugin(SdkTestPluginMetadata.Metadata).Album;
            var finalTestClass = new Unimplemented();

            var emptyChain = builder.Execute(finalTestClass);

            Assert.AreSame(emptyChain, finalTestClass);

            Helpers.AssertAllThrowsOnMemberAccess<AccessedException<Unimplemented>>(finalTestClass);
            Helpers.AssertAllThrowsOnMemberAccess<AccessedException<Unimplemented>>(emptyChain);

            // No overrides.
            builder.Add(x => new NoOverride(x));
            var noOverride = builder.Execute(finalTestClass);

            Assert.AreNotSame(noOverride, emptyChain);
            Assert.AreNotSame(noOverride, finalTestClass);
            Helpers.AssertAllMembersThrowOnAccess<AccessedException<Unimplemented>, NoOverride>(
                noOverride,
                customFilter: NoInner
            );
        }

        [TestMethod, Timeout(5000)]
        public void PluginFullyCustom()
        {
            // No plugins.
            var builder = new SdkModelPlugin(SdkTestPluginMetadata.Metadata).Album;
            var finalTestClass = new Unimplemented();

            var emptyChain = builder.Execute(finalTestClass);

            Assert.AreSame(emptyChain, finalTestClass);

            Helpers.AssertAllThrowsOnMemberAccess<AccessedException<Unimplemented>>(finalTestClass);
            Helpers.AssertAllThrowsOnMemberAccess<AccessedException<Unimplemented>>(emptyChain);

            // No overrides.
            builder.Add(x => new NoOverride(x));
            var noOverride = builder.Execute(finalTestClass);

            Assert.AreNotSame(noOverride, emptyChain);
            Assert.AreNotSame(noOverride, finalTestClass);
            Helpers.AssertAllThrowsOnMemberAccess<AccessedException<Unimplemented>>(noOverride, customFilter: NoInner);

            // Fully custom
            builder.Add(x => new FullyCustom(x));
            var allCustom = builder.Execute(finalTestClass);

            Assert.AreNotSame(noOverride, emptyChain);
            Assert.AreNotSame(noOverride, finalTestClass);
            Helpers.AssertAllThrowsOnMemberAccess<AccessedException<FullyCustom>>(
                allCustom,
                customFilter: NoInnerOrSources
            );
        }

        [TestMethod, Timeout(5000)]
        [AllEnumFlagCombinations(typeof(PossiblePlugins))]
        public void PluginFullyCustomWith_AllCombinations(PossiblePlugins data)
        {
            var builder = new SdkModelPlugin(SdkTestPluginMetadata.Metadata).Album;
            var defaultImplementation = new Unimplemented();
            builder.Add(x => new NoOverride(x)
                {
                    InnerDownloadable = data.HasFlag(PossiblePlugins.Downloadable) ? new DownloadablePluginBaseTests.Unimplemented() : x,
                    InnerPlayable = data.HasFlag(PossiblePlugins.Playable) ? new PlayablePluginBaseTests.Unimplemented() : x,
                    InnerArtistCollection = data.HasFlag(PossiblePlugins.ArtistCollection) ? new ArtistCollectionPluginBaseTests.Unimplemented() : x,
                    InnerGenreCollection = data.HasFlag(PossiblePlugins.GenreCollection) ? new GenreCollectionPluginBaseTests.Unimplemented() : x,
                    InnerTrackCollection = data.HasFlag(PossiblePlugins.TrackCollection) ? new TrackCollectionPluginBaseTests.Unimplemented() : x,
                    InnerImageCollection = data.HasFlag(PossiblePlugins.ImageCollection) ? new ImageCollectionPluginBaseTests.Unimplemented() : x,
                    InnerUrlCollection = data.HasFlag(PossiblePlugins.UrlCollection) ? new UrlCollectionPluginBaseTests.Unimplemented() : x,
                }
            );

            var finalImpl = builder.Execute(defaultImplementation);

            Assert.AreNotSame(finalImpl, defaultImplementation);
            Assert.IsInstanceOfType(finalImpl, typeof(NoOverride));

            if (data.HasFlag(PossiblePlugins.Downloadable))
            {
                Helpers.AssertAllMembersThrowOnAccess<AccessedException<DownloadablePluginBaseTests.Unimplemented>,
                    DownloadablePluginBaseTests.Unimplemented>(
                    finalImpl,
                    customFilter: NoInnerOrSources
                );
            }

            if (data.HasFlag(PossiblePlugins.Playable))
            {
                Helpers.AssertAllMembersThrowOnAccess<AccessedException<PlayablePluginBaseTests.Unimplemented>,
                    PlayablePluginBaseTests.Unimplemented>(
                    finalImpl,
                    customFilter: NoInnerOrSources,
                    typesToExclude: new[]
                    {
                        typeof(DownloadablePluginBaseTests.Unimplemented),
                        typeof(ImageCollectionPluginBaseTests.Unimplemented),
                        typeof(UrlCollectionPluginBaseTests.Unimplemented)
                    }
                );
            }

            if (data.HasFlag(PossiblePlugins.ArtistCollection))
            {
                Helpers.AssertAllMembersThrowOnAccess<AccessedException<ArtistCollectionPluginBaseTests.Unimplemented>,
                    ArtistCollectionPluginBaseTests.Unimplemented>(
                    finalImpl,
                    customFilter: NoInnerOrSources,
                    typesToExclude: new[]
                    {
                        typeof(DownloadablePluginBaseTests.Unimplemented),
                        typeof(ImageCollectionPluginBaseTests.Unimplemented),
                        typeof(UrlCollectionPluginBaseTests.Unimplemented),
                        typeof(PlayablePluginBaseTests.Unimplemented),
                        typeof(IPlayableCollectionItem),
                    }
                );
            }

            if (data.HasFlag(PossiblePlugins.GenreCollection))
            {
                Helpers.AssertAllMembersThrowOnAccess<AccessedException<GenreCollectionPluginBaseTests.Unimplemented>,
                    GenreCollectionPluginBaseTests.Unimplemented>(
                    finalImpl,
                    customFilter: NoInnerOrSources
                );
            }

            if (data.HasFlag(PossiblePlugins.ImageCollection))
            {
                Helpers.AssertAllMembersThrowOnAccess<AccessedException<ImageCollectionPluginBaseTests.Unimplemented>,
                    ImageCollectionPluginBaseTests.Unimplemented>(
                    finalImpl,
                    customFilter: NoInnerOrSources
                );
            }

            if (data.HasFlag(PossiblePlugins.TrackCollection))
            {
                Helpers.AssertAllMembersThrowOnAccess<AccessedException<TrackCollectionPluginBaseTests.Unimplemented>, TrackCollectionPluginBaseTests.Unimplemented>(
                    finalImpl,
                    customFilter: NoInnerOrSources,
                    typesToExclude: new[]
                    {
                        typeof(DownloadablePluginBaseTests.Unimplemented),
                        typeof(ImageCollectionPluginBaseTests.Unimplemented),
                        typeof(UrlCollectionPluginBaseTests.Unimplemented),
                        typeof(PlayablePluginBaseTests.Unimplemented),
                        typeof(IPlayableCollectionItem),
                    }
                );
            }

            if (data.HasFlag(PossiblePlugins.UrlCollection))
            {
                Helpers.AssertAllMembersThrowOnAccess<AccessedException<UrlCollectionPluginBaseTests.Unimplemented>, UrlCollectionPluginBaseTests.Unimplemented>(
                    finalImpl,
                    customFilter: NoInnerOrSources
                );
            }
        }

        internal class FullyCustom : AlbumPluginBase
        {
            public FullyCustom(IAlbum inner)
                : base(new ModelPluginMetadata("", nameof(FullyCustom), "", new Version()), inner)
            {
            }

            internal static AccessedException<FullyCustom> AccessedException { get; } =
                new AccessedException<FullyCustom>();

            public override Task<bool> IsAddImageAvailableAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public override Task<bool> IsRemoveImageAvailableAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public override Task RemoveImageAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public override int TotalImageCount => throw AccessedException;

            public override event EventHandler<int>? ImagesCountChanged
            {
                add => throw AccessedException;
                remove => throw AccessedException;
            }

            public override int TotalUrlCount => throw AccessedException;
            public override Task RemoveUrlAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public override Task<bool> IsAddUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public override Task<bool> IsRemoveUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;

            public override event EventHandler<int>? UrlsCountChanged
            {
                add => throw AccessedException;
                remove => throw AccessedException;
            }

            public override string Id => throw AccessedException;
            public override string Name => throw AccessedException;
            public override string? Description => throw AccessedException;
            public override DateTime? LastPlayed => throw AccessedException;
            public override PlaybackState PlaybackState => throw AccessedException;
            public override TimeSpan Duration => throw AccessedException;
            public override bool IsChangeNameAsyncAvailable => throw AccessedException;
            public override bool IsChangeDescriptionAsyncAvailable => throw AccessedException;
            public override bool IsChangeDurationAsyncAvailable => throw AccessedException;
            public override Task ChangeNameAsync(string name, CancellationToken cancellationToken = default) => throw AccessedException;
            public override Task ChangeDescriptionAsync(string? description, CancellationToken cancellationToken = default) => throw AccessedException;
            public override Task ChangeDurationAsync(TimeSpan duration, CancellationToken cancellationToken = default) => throw AccessedException;

            public override event EventHandler<PlaybackState>? PlaybackStateChanged
            {
                add => throw AccessedException;
                remove => throw AccessedException;
            }

            public override event EventHandler<string>? NameChanged
            {
                add => throw AccessedException;
                remove => throw AccessedException;
            }

            public override event EventHandler<string?>? DescriptionChanged
            {
                add => throw AccessedException;
                remove => throw AccessedException;
            }

            public override event EventHandler<TimeSpan>? DurationChanged
            {
                add => throw AccessedException;
                remove => throw AccessedException;
            }

            public override event EventHandler<DateTime?>? LastPlayedChanged
            {
                add => throw AccessedException;
                remove => throw AccessedException;
            }

            public override event EventHandler<bool>? IsChangeNameAsyncAvailableChanged
            {
                add => throw AccessedException;
                remove => throw AccessedException;
            }

            public override event EventHandler<bool>? IsChangeDescriptionAsyncAvailableChanged
            {
                add => throw AccessedException;
                remove => throw AccessedException;
            }

            public override event EventHandler<bool>? IsChangeDurationAsyncAvailableChanged
            {
                add => throw AccessedException;
                remove => throw AccessedException;
            }

            public override DateTime? AddedAt => throw AccessedException;
            public override int TotalArtistItemsCount => throw AccessedException;
            public override bool IsPlayArtistCollectionAsyncAvailable => throw AccessedException;
            public override bool IsPauseArtistCollectionAsyncAvailable => throw AccessedException;
            public override Task PlayArtistCollectionAsync(CancellationToken cancellationToken = default) => throw AccessedException;
            public override Task PauseArtistCollectionAsync(CancellationToken cancellationToken = default) => throw AccessedException;
            public override Task RemoveArtistItemAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public override Task<bool> IsAddArtistItemAvailableAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public override Task<bool> IsRemoveArtistItemAvailableAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public override event EventHandler<bool>? IsPlayArtistCollectionAsyncAvailableChanged;
            public override event EventHandler<bool>? IsPauseArtistCollectionAsyncAvailableChanged;
            public override event EventHandler<int>? ArtistItemsCountChanged;
            public override int TotalTrackCount => throw AccessedException;
            public override bool IsPlayTrackCollectionAsyncAvailable => throw AccessedException;
            public override bool IsPauseTrackCollectionAsyncAvailable => throw AccessedException;
            public override Task PlayTrackCollectionAsync(CancellationToken cancellationToken = default) => throw AccessedException;
            public override Task PauseTrackCollectionAsync(CancellationToken cancellationToken = default) => throw AccessedException;
            public override Task RemoveTrackAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public override Task<bool> IsAddTrackAvailableAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public override Task<bool> IsRemoveTrackAvailableAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;

            public override event EventHandler<bool>? IsPlayTrackCollectionAsyncAvailableChanged
            {
                add => throw AccessedException;
                remove => throw AccessedException;
            }

            public override event EventHandler<bool>? IsPauseTrackCollectionAsyncAvailableChanged
            {
                add => throw AccessedException;
                remove => throw AccessedException;
            }

            public override event EventHandler<int>? TracksCountChanged
            {
                add => throw AccessedException;
                remove => throw AccessedException;
            }

            public override int TotalGenreCount => throw AccessedException;
            public override Task RemoveGenreAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public override Task<bool> IsAddGenreAvailableAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public override Task<bool> IsRemoveGenreAvailableAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;

            public override event EventHandler<int>? GenresCountChanged
            {
                add => throw AccessedException;
                remove => throw AccessedException;
            }

            public override DateTime? DatePublished => throw AccessedException;
            public override bool IsChangeDatePublishedAsyncAvailable => throw AccessedException;
            public override Task ChangeDatePublishedAsync(DateTime datePublished, CancellationToken cancellationToken = default) => throw AccessedException;

            public override event EventHandler<DateTime?>? DatePublishedChanged
            {
                add => throw AccessedException;
                remove => throw AccessedException;
            }

            public override event EventHandler<bool>? IsChangeDatePublishedAsyncAvailableChanged
            {
                add => throw AccessedException;
                remove => throw AccessedException;
            }

            public override DownloadInfo DownloadInfo => throw AccessedException;
            public override Task StartDownloadOperationAsync(DownloadOperation operation, CancellationToken cancellationToken = default) => throw AccessedException;

            public override event EventHandler<DownloadInfo>? DownloadInfoChanged
            {
                add => throw AccessedException;
                remove => throw AccessedException;
            }

            public override bool Equals(ICoreImageCollection? other) => throw AccessedException;

            public override IAsyncEnumerable<IImage> GetImagesAsync(int limit, int offset, CancellationToken cancellationToken = default) =>
                throw AccessedException;

            public override Task AddImageAsync(IImage image, int index, CancellationToken cancellationToken = default) => throw AccessedException;

            public override event CollectionChangedEventHandler<IImage>? ImagesChanged
            {
                add => throw AccessedException;
                remove => throw AccessedException;
            }

            public override bool Equals(ICoreUrlCollection? other) => throw AccessedException;
            public override IAsyncEnumerable<IUrl> GetUrlsAsync(int limit, int offset, CancellationToken cancellationToken = default) => throw AccessedException;
            public override Task AddUrlAsync(IUrl url, int index, CancellationToken cancellationToken = default) => throw AccessedException;

            public override event CollectionChangedEventHandler<IUrl>? UrlsChanged
            {
                add => throw AccessedException;
                remove => throw AccessedException;
            }

            public override bool Equals(ICoreAlbumCollectionItem? other) => throw AccessedException;
            public override bool Equals(ICoreArtistCollectionItem? other) => throw AccessedException;
            public override bool Equals(ICoreArtistCollection? other) => throw AccessedException;
            public override Task PlayArtistCollectionAsync(IArtistCollectionItem artistItem, CancellationToken cancellationToken = default) => throw AccessedException;

            public override IAsyncEnumerable<IArtistCollectionItem> GetArtistItemsAsync(int limit, int offset, CancellationToken cancellationToken = default) =>
                throw AccessedException;

            public override Task AddArtistItemAsync(IArtistCollectionItem artistItem, int index, CancellationToken cancellationToken = default) => throw AccessedException;

            public override event CollectionChangedEventHandler<IArtistCollectionItem>? ArtistItemsChanged
            {
                add => throw AccessedException;
                remove => throw AccessedException;
            }

            public override bool Equals(ICoreTrackCollection? other) => throw AccessedException;
            public override Task PlayTrackCollectionAsync(ITrack track, CancellationToken cancellationToken = default) => throw AccessedException;

            public override IAsyncEnumerable<ITrack> GetTracksAsync(int limit, int offset, CancellationToken cancellationToken = default) =>
                throw AccessedException;

            public override Task AddTrackAsync(ITrack track, int index, CancellationToken cancellationToken = default) => throw AccessedException;

            public override event CollectionChangedEventHandler<ITrack>? TracksChanged
            {
                add => throw AccessedException;
                remove => throw AccessedException;
            }

            public override bool Equals(ICoreGenreCollection? other) => throw AccessedException;

            public override IAsyncEnumerable<IGenre> GetGenresAsync(int limit, int offset, CancellationToken cancellationToken = default) =>
                throw AccessedException;

            public override Task AddGenreAsync(IGenre genre, int index, CancellationToken cancellationToken = default) => throw AccessedException;

            public override event CollectionChangedEventHandler<IGenre>? GenresChanged
            {
                add => throw AccessedException;
                remove => throw AccessedException;
            }

            public override bool Equals(ICoreAlbum? other) => throw AccessedException;
            public override IPlayableCollectionGroup? RelatedItems => throw AccessedException;
        }

        internal class NoOverride : AlbumPluginBase
        {
            public NoOverride(IAlbum inner)
                : base(new ModelPluginMetadata("", nameof(NoOverride), "", new Version()), inner)
            {
            }
        }

        internal class Unimplemented : IAlbum
        {
            internal static AccessedException<Unimplemented> AccessedException { get; } =
                new AccessedException<Unimplemented>();
            

            public event EventHandler? SourcesChanged { add => throw AccessedException; remove => throw AccessedException; }
            public Task<bool> IsAddImageAvailableAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public Task<bool> IsRemoveImageAvailableAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public Task RemoveImageAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public int TotalImageCount => throw AccessedException;

            public event EventHandler<int>? ImagesCountChanged
            {
                add => throw AccessedException;
                remove => throw AccessedException;
            }

            public int TotalUrlCount => throw AccessedException;
            public Task RemoveUrlAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public Task<bool> IsAddUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public Task<bool> IsRemoveUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;

            public event EventHandler<int>? UrlsCountChanged
            {
                add => throw AccessedException;
                remove => throw AccessedException;
            }

            public string Id => throw AccessedException;
            public string Name => throw AccessedException;
            public string? Description => throw AccessedException;
            public DateTime? LastPlayed => throw AccessedException;
            public PlaybackState PlaybackState => throw AccessedException;
            public TimeSpan Duration => throw AccessedException;
            public bool IsChangeNameAsyncAvailable => throw AccessedException;
            public bool IsChangeDescriptionAsyncAvailable => throw AccessedException;
            public bool IsChangeDurationAsyncAvailable => throw AccessedException;
            public Task ChangeNameAsync(string name, CancellationToken cancellationToken = default) => throw AccessedException;
            public Task ChangeDescriptionAsync(string? description, CancellationToken cancellationToken = default) => throw AccessedException;
            public Task ChangeDurationAsync(TimeSpan duration, CancellationToken cancellationToken = default) => throw AccessedException;

            public event EventHandler<PlaybackState>? PlaybackStateChanged
            {
                add => throw AccessedException;
                remove => throw AccessedException;
            }

            public event EventHandler<string>? NameChanged
            {
                add => throw AccessedException;
                remove => throw AccessedException;
            }

            public event EventHandler<string?>? DescriptionChanged
            {
                add => throw AccessedException;
                remove => throw AccessedException;
            }

            public event EventHandler<TimeSpan>? DurationChanged
            {
                add => throw AccessedException;
                remove => throw AccessedException;
            }

            public event EventHandler<DateTime?>? LastPlayedChanged
            {
                add => throw AccessedException;
                remove => throw AccessedException;
            }

            public event EventHandler<bool>? IsChangeNameAsyncAvailableChanged
            {
                add => throw AccessedException;
                remove => throw AccessedException;
            }

            public event EventHandler<bool>? IsChangeDescriptionAsyncAvailableChanged
            {
                add => throw AccessedException;
                remove => throw AccessedException;
            }

            public event EventHandler<bool>? IsChangeDurationAsyncAvailableChanged
            {
                add => throw AccessedException;
                remove => throw AccessedException;
            }

            public DateTime? AddedAt => throw AccessedException;
            public int TotalArtistItemsCount => throw AccessedException;
            public bool IsPlayArtistCollectionAsyncAvailable => throw AccessedException;
            public bool IsPauseArtistCollectionAsyncAvailable => throw AccessedException;
            public Task PlayArtistCollectionAsync(CancellationToken cancellationToken = default) => throw AccessedException;
            public Task PauseArtistCollectionAsync(CancellationToken cancellationToken = default) => throw AccessedException;
            public Task RemoveArtistItemAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public Task<bool> IsAddArtistItemAvailableAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public Task<bool> IsRemoveArtistItemAvailableAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public event EventHandler<bool>? IsPlayArtistCollectionAsyncAvailableChanged;
            public event EventHandler<bool>? IsPauseArtistCollectionAsyncAvailableChanged;
            public event EventHandler<int>? ArtistItemsCountChanged;
            public int TotalTrackCount => throw AccessedException;
            public bool IsPlayTrackCollectionAsyncAvailable => throw AccessedException;
            public bool IsPauseTrackCollectionAsyncAvailable => throw AccessedException;
            public Task PlayTrackCollectionAsync(CancellationToken cancellationToken = default) => throw AccessedException;
            public Task PauseTrackCollectionAsync(CancellationToken cancellationToken = default) => throw AccessedException;
            public Task RemoveTrackAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public Task<bool> IsAddTrackAvailableAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public Task<bool> IsRemoveTrackAvailableAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;

            public event EventHandler<bool>? IsPlayTrackCollectionAsyncAvailableChanged
            {
                add => throw AccessedException;
                remove => throw AccessedException;
            }

            public event EventHandler<bool>? IsPauseTrackCollectionAsyncAvailableChanged
            {
                add => throw AccessedException;
                remove => throw AccessedException;
            }

            public event EventHandler<int>? TracksCountChanged
            {
                add => throw AccessedException;
                remove => throw AccessedException;
            }

            public int TotalGenreCount => throw AccessedException;
            public Task RemoveGenreAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public Task<bool> IsAddGenreAvailableAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public Task<bool> IsRemoveGenreAvailableAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;

            public event EventHandler<int>? GenresCountChanged
            {
                add => throw AccessedException;
                remove => throw AccessedException;
            }

            public DateTime? DatePublished => throw AccessedException;
            public bool IsChangeDatePublishedAsyncAvailable => throw AccessedException;
            public Task ChangeDatePublishedAsync(DateTime datePublished, CancellationToken cancellationToken = default) => throw AccessedException;

            public event EventHandler<DateTime?>? DatePublishedChanged
            {
                add => throw AccessedException;
                remove => throw AccessedException;
            }

            public event EventHandler<bool>? IsChangeDatePublishedAsyncAvailableChanged
            {
                add => throw AccessedException;
                remove => throw AccessedException;
            }

            public DownloadInfo DownloadInfo => throw AccessedException;
            public Task StartDownloadOperationAsync(DownloadOperation operation, CancellationToken cancellationToken = default) => throw AccessedException;

            public event EventHandler<DownloadInfo>? DownloadInfoChanged
            {
                add => throw AccessedException;
                remove => throw AccessedException;
            }

            public bool Equals(ICoreImageCollection? other) => throw AccessedException;
            IReadOnlyList<ICoreImageCollection> IMerged<ICoreImageCollection>.Sources => throw AccessedException;
            IReadOnlyList<ICoreUrlCollection> IMerged<ICoreUrlCollection>.Sources => throw AccessedException;

            IReadOnlyList<ICoreAlbumCollectionItem> IMerged<ICoreAlbumCollectionItem>.Sources =>
                throw AccessedException;

            IReadOnlyList<ICoreArtistCollectionItem> IMerged<ICoreArtistCollectionItem>.Sources =>
                throw AccessedException;

            IReadOnlyList<ICoreArtistCollection> IMerged<ICoreArtistCollection>.Sources => throw AccessedException;
            IReadOnlyList<ICoreTrackCollection> IMerged<ICoreTrackCollection>.Sources => throw AccessedException;
            IReadOnlyList<ICoreGenreCollection> IMerged<ICoreGenreCollection>.Sources => throw AccessedException;
            IReadOnlyList<ICoreAlbum> IMerged<ICoreAlbum>.Sources => throw AccessedException;
            public IAsyncEnumerable<IImage> GetImagesAsync(int limit, int offset, CancellationToken cancellationToken = default) => throw AccessedException;
            public Task AddImageAsync(IImage image, int index, CancellationToken cancellationToken = default) => throw AccessedException;

            public event CollectionChangedEventHandler<IImage>? ImagesChanged
            {
                add => throw AccessedException;
                remove => throw AccessedException;
            }

            public bool Equals(ICoreUrlCollection? other) => throw AccessedException;
            public IAsyncEnumerable<IUrl> GetUrlsAsync(int limit, int offset, CancellationToken cancellationToken = default) => throw AccessedException;
            public Task AddUrlAsync(IUrl url, int index, CancellationToken cancellationToken = default) => throw AccessedException;

            public event CollectionChangedEventHandler<IUrl>? UrlsChanged
            {
                add => throw AccessedException;
                remove => throw AccessedException;
            }

            public bool Equals(ICoreAlbumCollectionItem? other) => throw AccessedException;
            public bool Equals(ICoreArtistCollectionItem? other) => throw AccessedException;
            public bool Equals(ICoreArtistCollection? other) => throw AccessedException;
            public Task PlayArtistCollectionAsync(IArtistCollectionItem artistItem, CancellationToken cancellationToken = default) => throw AccessedException;

            public IAsyncEnumerable<IArtistCollectionItem> GetArtistItemsAsync(int limit, int offset, CancellationToken cancellationToken = default) =>
                throw AccessedException;

            public Task AddArtistItemAsync(IArtistCollectionItem artistItem, int index, CancellationToken cancellationToken = default) => throw AccessedException;

            public event CollectionChangedEventHandler<IArtistCollectionItem>? ArtistItemsChanged
            {
                add => throw AccessedException;
                remove => throw AccessedException;
            }

            public bool Equals(ICoreTrackCollection? other) => throw AccessedException;
            public Task PlayTrackCollectionAsync(ITrack track, CancellationToken cancellationToken = default) => throw AccessedException;
            public IAsyncEnumerable<ITrack> GetTracksAsync(int limit, int offset, CancellationToken cancellationToken = default) => throw AccessedException;
            public Task AddTrackAsync(ITrack track, int index, CancellationToken cancellationToken = default) => throw AccessedException;

            public event CollectionChangedEventHandler<ITrack>? TracksChanged
            {
                add => throw AccessedException;
                remove => throw AccessedException;
            }

            public bool Equals(ICoreGenreCollection? other) => throw AccessedException;
            public IAsyncEnumerable<IGenre> GetGenresAsync(int limit, int offset, CancellationToken cancellationToken = default) => throw AccessedException;
            public Task AddGenreAsync(IGenre genre, int index, CancellationToken cancellationToken = default) => throw AccessedException;

            public event CollectionChangedEventHandler<IGenre>? GenresChanged
            {
                add => throw AccessedException;
                remove => throw AccessedException;
            }

            public bool Equals(ICoreAlbum? other) => throw AccessedException;
            public IPlayableCollectionGroup? RelatedItems => throw AccessedException;
        }
    }
}
