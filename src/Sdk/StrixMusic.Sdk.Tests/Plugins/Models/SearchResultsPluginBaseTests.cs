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
    public class SearchResultsPluginBaseTests
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
            AlbumCollection = 8,
            TrackCollection = 16,
            PlaylistCollection = 32,
            ImageCollection = 64,
            UrlCollection = 128,
        }

        [TestMethod, Timeout(5000)]
        public void NoPlugins()
        {
            var builder = new SdkModelPlugin(SdkTestPluginMetadata.Metadata).SearchResults;
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
            var builder = new SdkModelPlugin(SdkTestPluginMetadata.Metadata).SearchResults;
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
            Helpers.AssertAllMembersThrowOnAccess<AccessedException<Unimplemented>, NoOverride>(noOverride, customFilter: NoInner);
        }

        [TestMethod, Timeout(5000)]
        public void PluginFullyCustom()
        {
            // No plugins.
            var builder = new SdkModelPlugin(SdkTestPluginMetadata.Metadata).SearchResults;
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
            Helpers.AssertAllThrowsOnMemberAccess<AccessedException<FullyCustom>>(allCustom, customFilter: NoInnerOrSources);
        }

        [TestMethod, Timeout(5000)]
        [AllEnumFlagCombinations(typeof(PossiblePlugins))]
        public void PluginFullyCustomWith_AllCombinations(PossiblePlugins data)
        {
            var builder = new SdkModelPlugin(SdkTestPluginMetadata.Metadata).SearchResults;
            var defaultImplementation = new Unimplemented();
            builder.Add(x => new NoOverride(x)
                {
                    InnerDownloadable = data.HasFlag(PossiblePlugins.Downloadable) ? new DownloadablePluginBaseTests.Unimplemented() : x,
                    InnerPlayable = data.HasFlag(PossiblePlugins.Playable) ? new PlayablePluginBaseTests.Unimplemented() : x,
                    InnerArtistCollection = data.HasFlag(PossiblePlugins.ArtistCollection) ? new ArtistCollectionPluginBaseTests.Unimplemented() : x,
                    InnerAlbumCollection = data.HasFlag(PossiblePlugins.AlbumCollection) ? new AlbumCollectionPluginBaseTests.Unimplemented() : x,
                    InnerTrackCollection = data.HasFlag(PossiblePlugins.TrackCollection) ? new TrackCollectionPluginBaseTests.Unimplemented() : x,
                    InnerPlaylistCollection = data.HasFlag(PossiblePlugins.PlaylistCollection) ? new PlaylistCollectionPluginBaseTests.Unimplemented() : x,
                    InnerImageCollection = data.HasFlag(PossiblePlugins.ImageCollection) ? new ImageCollectionPluginBaseTests.Unimplemented() : x,
                    InnerUrlCollection = data.HasFlag(PossiblePlugins.UrlCollection) ? new UrlCollectionPluginBaseTests.Unimplemented() : x,
                }
            );

            var finalImpl = builder.Execute(defaultImplementation);

            Assert.AreNotSame(finalImpl, defaultImplementation);
            Assert.IsInstanceOfType(finalImpl, typeof(NoOverride));

            var expectedExceptionsWhenDisposing = new List<Type>
            {
                typeof(AccessedException<Unimplemented>),
            };

            if (data.HasFlag(PossiblePlugins.Downloadable))
            {
                expectedExceptionsWhenDisposing.Add(typeof(AccessedException<DownloadablePluginBaseTests.Unimplemented>));

                Helpers.AssertAllMembersThrowOnAccess<AccessedException<DownloadablePluginBaseTests.Unimplemented>,
                    DownloadablePluginBaseTests.Unimplemented>(
                    finalImpl,
                    customFilter: NoInnerOrSources,
                    typesToExclude: typeof(IAsyncDisposable)
                );
            }

            if (data.HasFlag(PossiblePlugins.Playable))
            {
                expectedExceptionsWhenDisposing.Add(typeof(AccessedException<PlayablePluginBaseTests.Unimplemented>));

                Helpers.AssertAllMembersThrowOnAccess<AccessedException<PlayablePluginBaseTests.Unimplemented>,
                    PlayablePluginBaseTests.Unimplemented>(
                    finalImpl,
                    customFilter: NoInnerOrSources,
                    typesToExclude: new[]
                    {
                        typeof(IAsyncDisposable),
                        typeof(DownloadablePluginBaseTests.Unimplemented),
                        typeof(ImageCollectionPluginBaseTests.Unimplemented),
                        typeof(UrlCollectionPluginBaseTests.Unimplemented)
                    }
                );
            }

            if (data.HasFlag(PossiblePlugins.ArtistCollection))
            {
                expectedExceptionsWhenDisposing.Add(typeof(AccessedException<ArtistCollectionPluginBaseTests.Unimplemented>));

                Helpers.AssertAllMembersThrowOnAccess<AccessedException<ArtistCollectionPluginBaseTests.Unimplemented>,
                    ArtistCollectionPluginBaseTests.Unimplemented>(
                    finalImpl,
                    customFilter: NoInnerOrSources,
                    typesToExclude: new[]
                    {
                        typeof(IAsyncDisposable),
                        typeof(DownloadablePluginBaseTests.Unimplemented),
                        typeof(ImageCollectionPluginBaseTests.Unimplemented),
                        typeof(UrlCollectionPluginBaseTests.Unimplemented),
                        typeof(PlayablePluginBaseTests.Unimplemented),
                        typeof(IPlayableCollectionItem),
                    }
                );
            }

            if (data.HasFlag(PossiblePlugins.AlbumCollection))
            {
                expectedExceptionsWhenDisposing.Add(typeof(AccessedException<AlbumCollectionPluginBaseTests.Unimplemented>));

                Helpers.AssertAllMembersThrowOnAccess<AccessedException<AlbumCollectionPluginBaseTests.Unimplemented>,
                    AlbumCollectionPluginBaseTests.Unimplemented>(
                    finalImpl,
                    customFilter: NoInnerOrSources,
                    typesToExclude: new[]
                    {
                        typeof(IAsyncDisposable),
                        typeof(DownloadablePluginBaseTests.Unimplemented),
                        typeof(ImageCollectionPluginBaseTests.Unimplemented),
                        typeof(UrlCollectionPluginBaseTests.Unimplemented),
                        typeof(PlayablePluginBaseTests.Unimplemented),
                        typeof(IPlayableCollectionItem),
                    }
                );
            }

            if (data.HasFlag(PossiblePlugins.PlaylistCollection))
            {
                expectedExceptionsWhenDisposing.Add(typeof(AccessedException<PlaylistCollectionPluginBaseTests.Unimplemented>));

                Helpers.AssertAllMembersThrowOnAccess<AccessedException<PlaylistCollectionPluginBaseTests.Unimplemented>,
                    PlaylistCollectionPluginBaseTests.Unimplemented>(
                    finalImpl,
                    customFilter: NoInnerOrSources,
                    typesToExclude: new[]
                    {
                        typeof(IAsyncDisposable),
                        typeof(DownloadablePluginBaseTests.Unimplemented),
                        typeof(ImageCollectionPluginBaseTests.Unimplemented),
                        typeof(UrlCollectionPluginBaseTests.Unimplemented),
                        typeof(PlayablePluginBaseTests.Unimplemented),
                        typeof(IPlayableCollectionItem),
                    }
                );
            }

            if (data.HasFlag(PossiblePlugins.TrackCollection))
            {
                expectedExceptionsWhenDisposing.Add(typeof(AccessedException<TrackCollectionPluginBaseTests.Unimplemented>));

                Helpers.AssertAllMembersThrowOnAccess<AccessedException<TrackCollectionPluginBaseTests.Unimplemented>, TrackCollectionPluginBaseTests.Unimplemented>(
                    finalImpl,
                    customFilter: NoInnerOrSources,
                    typesToExclude: new[]
                    {
                        typeof(IAsyncDisposable),
                        typeof(DownloadablePluginBaseTests.Unimplemented),
                        typeof(ImageCollectionPluginBaseTests.Unimplemented),
                        typeof(UrlCollectionPluginBaseTests.Unimplemented),
                        typeof(PlayablePluginBaseTests.Unimplemented),
                        typeof(IPlayableCollectionItem),
                    }
                );
            }

            if (data.HasFlag(PossiblePlugins.ImageCollection))
            {
                expectedExceptionsWhenDisposing.Add(typeof(AccessedException<ImageCollectionPluginBaseTests.Unimplemented>));

                Helpers.AssertAllMembersThrowOnAccess<AccessedException<ImageCollectionPluginBaseTests.Unimplemented>,
                    ImageCollectionPluginBaseTests.Unimplemented>(
                    finalImpl,
                    customFilter: NoInnerOrSources,
                    typesToExclude: typeof(IAsyncDisposable)
                );
            }

            if (data.HasFlag(PossiblePlugins.UrlCollection))
            {
                expectedExceptionsWhenDisposing.Add(typeof(AccessedException<UrlCollectionPluginBaseTests.Unimplemented>));

                Helpers.AssertAllMembersThrowOnAccess<AccessedException<UrlCollectionPluginBaseTests.Unimplemented>, UrlCollectionPluginBaseTests.Unimplemented>(
                    finalImpl,
                    customFilter: NoInnerOrSources,
                    typesToExclude: typeof(IAsyncDisposable)
                );
            }

            Helpers.AssertAllThrowsOnMemberAccess<IAsyncDisposable>(
                finalImpl,
                customFilter: NoInnerOrSources,
                expectedExceptions: expectedExceptionsWhenDisposing.ToArray()
            );
        }

        [TestMethod, Timeout(5000)]
        [AllEnumFlagCombinations(typeof(PossiblePlugins))]
        public async Task DisposeAsync_AllCombinations(PossiblePlugins data)
        {
            var builder = new SdkModelPlugin(SdkTestPluginMetadata.Metadata).SearchResults;
            var defaultImplementation = new NotBlockingDisposeAsync();
            builder.Add(x => new NoOverride(x)
            {
                InnerDownloadable = data.HasFlag(PossiblePlugins.Downloadable) ? new DownloadablePluginBaseTests.NotBlockingDisposeAsync() : x,
                InnerPlayable = data.HasFlag(PossiblePlugins.Playable) ? new PlayablePluginBaseTests.NotBlockingDisposeAsync() : x,
                InnerArtistCollection = data.HasFlag(PossiblePlugins.ArtistCollection) ? new ArtistCollectionPluginBaseTests.NotBlockingDisposeAsync() : x,
                InnerAlbumCollection = data.HasFlag(PossiblePlugins.AlbumCollection) ? new AlbumCollectionPluginBaseTests.NotBlockingDisposeAsync() : x,
                InnerTrackCollection = data.HasFlag(PossiblePlugins.TrackCollection) ? new TrackCollectionPluginBaseTests.NotBlockingDisposeAsync() : x,
                InnerPlaylistCollection = data.HasFlag(PossiblePlugins.PlaylistCollection) ? new PlaylistCollectionPluginBaseTests.NotBlockingDisposeAsync() : x,
                InnerImageCollection = data.HasFlag(PossiblePlugins.ImageCollection) ? new ImageCollectionPluginBaseTests.NotBlockingDisposeAsync() : x,
                InnerUrlCollection = data.HasFlag(PossiblePlugins.UrlCollection) ? new UrlCollectionPluginBaseTests.NotBlockingDisposeAsync() : x,
            });

            var finalImpl = builder.Execute(defaultImplementation);

            Assert.AreNotSame(finalImpl, defaultImplementation);
            Assert.IsInstanceOfType(finalImpl, typeof(NoOverride));

            await finalImpl.DisposeAsync();
        }

        internal class FullyCustom : SearchResultsPluginBase
        {
            public FullyCustom(ISearchResults inner)
                : base(new ModelPluginMetadata("", nameof(FullyCustom), "", new Version()), inner)
            {
            }

            internal static AccessedException<FullyCustom> AccessedException { get; } = new();

            public override ValueTask DisposeAsync() => throw AccessedException;
            public override Task<bool> IsAddImageAvailableAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public override Task<bool> IsRemoveImageAvailableAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public override Task RemoveImageAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public override int TotalImageCount => throw AccessedException;
            public override event EventHandler<int>? ImagesCountChanged { add => throw AccessedException; remove => throw AccessedException; }
            public override int TotalUrlCount => throw AccessedException;
            public override Task RemoveUrlAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public override Task<bool> IsAddUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public override Task<bool> IsRemoveUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public override event EventHandler<int>? UrlsCountChanged { add => throw AccessedException; remove => throw AccessedException; }
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
            public override event EventHandler<PlaybackState>? PlaybackStateChanged { add => throw AccessedException; remove => throw AccessedException; }
            public override event EventHandler<string>? NameChanged { add => throw AccessedException; remove => throw AccessedException; }
            public override event EventHandler<string?>? DescriptionChanged { add => throw AccessedException; remove => throw AccessedException; }
            public override event EventHandler<TimeSpan>? DurationChanged { add => throw AccessedException; remove => throw AccessedException; }
            public override event EventHandler<DateTime?>? LastPlayedChanged { add => throw AccessedException; remove => throw AccessedException; }
            public override event EventHandler<bool>? IsChangeNameAsyncAvailableChanged { add => throw AccessedException; remove => throw AccessedException; }
            public override event EventHandler<bool>? IsChangeDescriptionAsyncAvailableChanged { add => throw AccessedException; remove => throw AccessedException; }
            public override event EventHandler<bool>? IsChangeDurationAsyncAvailableChanged { add => throw AccessedException; remove => throw AccessedException; }
            public override DateTime? AddedAt => throw AccessedException;
            public override int TotalPlaylistItemsCount => throw AccessedException;
            public override bool IsPlayPlaylistCollectionAsyncAvailable => throw AccessedException;
            public override bool IsPausePlaylistCollectionAsyncAvailable => throw AccessedException;
            public override Task PlayPlaylistCollectionAsync(CancellationToken cancellationToken = default) => throw AccessedException;
            public override Task PausePlaylistCollectionAsync(CancellationToken cancellationToken = default) => throw AccessedException;
            public override Task RemovePlaylistItemAsync(int index,  CancellationToken cancellationToken = default) => throw AccessedException;
            public override Task<bool> IsAddPlaylistItemAvailableAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public override Task<bool> IsRemovePlaylistItemAvailableAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public override event EventHandler<bool>? IsPlayPlaylistCollectionAsyncAvailableChanged { add => throw AccessedException; remove => throw AccessedException; }
            public override event EventHandler<bool>? IsPausePlaylistCollectionAsyncAvailableChanged { add => throw AccessedException; remove => throw AccessedException; }
            public override event EventHandler<int>? PlaylistItemsCountChanged { add => throw AccessedException; remove => throw AccessedException; }
            public override int TotalTrackCount => throw AccessedException;
            public override bool IsPlayTrackCollectionAsyncAvailable => throw AccessedException;
            public override bool IsPauseTrackCollectionAsyncAvailable => throw AccessedException;
            public override Task PlayTrackCollectionAsync(CancellationToken cancellationToken = default) => throw AccessedException;
            public override Task PauseTrackCollectionAsync(CancellationToken cancellationToken = default) => throw AccessedException;
            public override Task RemoveTrackAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public override Task<bool> IsAddTrackAvailableAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public override Task<bool> IsRemoveTrackAvailableAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public override event EventHandler<bool>? IsPlayTrackCollectionAsyncAvailableChanged { add => throw AccessedException; remove => throw AccessedException; }
            public override event EventHandler<bool>? IsPauseTrackCollectionAsyncAvailableChanged { add => throw AccessedException; remove => throw AccessedException; }
            public override event EventHandler<int>? TracksCountChanged { add => throw AccessedException; remove => throw AccessedException; }
            public override int TotalAlbumItemsCount => throw AccessedException;
            public override bool IsPlayAlbumCollectionAsyncAvailable => throw AccessedException;
            public override bool IsPauseAlbumCollectionAsyncAvailable => throw AccessedException;
            public override Task PlayAlbumCollectionAsync(CancellationToken cancellationToken = default) => throw AccessedException;
            public override Task PauseAlbumCollectionAsync(CancellationToken cancellationToken = default) => throw AccessedException;
            public override Task RemoveAlbumItemAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public override Task<bool> IsAddAlbumItemAvailableAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public override Task<bool> IsRemoveAlbumItemAvailableAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public override event EventHandler<bool>? IsPlayAlbumCollectionAsyncAvailableChanged { add => throw AccessedException; remove => throw AccessedException; }
            public override event EventHandler<bool>? IsPauseAlbumCollectionAsyncAvailableChanged { add => throw AccessedException; remove => throw AccessedException; }
            public override event EventHandler<int>? AlbumItemsCountChanged { add => throw AccessedException; remove => throw AccessedException; }
            public override int TotalArtistItemsCount => throw AccessedException;
            public override bool IsPlayArtistCollectionAsyncAvailable => throw AccessedException;
            public override bool IsPauseArtistCollectionAsyncAvailable => throw AccessedException;
            public override Task PlayArtistCollectionAsync(CancellationToken cancellationToken = default) => throw AccessedException;
            public override Task PauseArtistCollectionAsync(CancellationToken cancellationToken = default) => throw AccessedException;
            public override Task RemoveArtistItemAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public override Task<bool> IsAddArtistItemAvailableAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public override Task<bool> IsRemoveArtistItemAvailableAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public override event EventHandler<bool>? IsPlayArtistCollectionAsyncAvailableChanged { add => throw AccessedException; remove => throw AccessedException; }
            public override event EventHandler<bool>? IsPauseArtistCollectionAsyncAvailableChanged { add => throw AccessedException; remove => throw AccessedException; }
            public override event EventHandler<int>? ArtistItemsCountChanged { add => throw AccessedException; remove => throw AccessedException; }
            public override Task PlayPlayableCollectionGroupAsync(CancellationToken cancellationToken = default) => throw AccessedException;
            public override Task PausePlayableCollectionGroupAsync(CancellationToken cancellationToken = default) => throw AccessedException;
            public override int TotalChildrenCount => throw AccessedException;
            public override Task RemoveChildAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public override Task<bool> IsAddChildAvailableAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public override Task<bool> IsRemoveChildAvailableAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public override event EventHandler<int>? ChildrenCountChanged { add => throw AccessedException; remove => throw AccessedException; }
            public override bool Equals(ICoreImageCollection? other) => throw AccessedException;
            public override IAsyncEnumerable<IImage> GetImagesAsync(int limit, int offset, CancellationToken cancellationToken = default) => throw AccessedException;
            public override Task AddImageAsync(IImage image, int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public override event CollectionChangedEventHandler<IImage>? ImagesChanged { add => throw AccessedException; remove => throw AccessedException; }
            public override bool Equals(ICoreUrlCollection? other) => throw AccessedException;
            public override IAsyncEnumerable<IUrl> GetUrlsAsync(int limit, int offset, CancellationToken cancellationToken = default) => throw AccessedException;
            public override Task AddUrlAsync(IUrl url, int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public override event CollectionChangedEventHandler<IUrl>? UrlsChanged { add => throw AccessedException; remove => throw AccessedException; }
            public override DownloadInfo DownloadInfo => throw AccessedException;
            public override Task StartDownloadOperationAsync(DownloadOperation operation, CancellationToken cancellationToken = default) => throw AccessedException;
            public override event EventHandler<DownloadInfo>? DownloadInfoChanged { add => throw AccessedException; remove => throw AccessedException; }
            public override bool Equals(ICorePlaylistCollectionItem? other) => throw AccessedException;
            public override bool Equals(ICorePlaylistCollection? other) => throw AccessedException;
            public override Task PlayPlaylistCollectionAsync(IPlaylistCollectionItem playlistItem, CancellationToken cancellationToken = default) => throw AccessedException;
            public override IAsyncEnumerable<IPlaylistCollectionItem> GetPlaylistItemsAsync(int limit, int offset, CancellationToken cancellationToken = default) => throw AccessedException;
            public override Task AddPlaylistItemAsync(IPlaylistCollectionItem playlistItem, int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public override event CollectionChangedEventHandler<IPlaylistCollectionItem>? PlaylistItemsChanged { add => throw AccessedException; remove => throw AccessedException; }
            public override bool Equals(ICoreTrackCollection? other) => throw AccessedException;
            public override Task PlayTrackCollectionAsync(ITrack track, CancellationToken cancellationToken = default) => throw AccessedException;
            public override IAsyncEnumerable<ITrack> GetTracksAsync(int limit, int offset, CancellationToken cancellationToken = default) => throw AccessedException;
            public override Task AddTrackAsync(ITrack track, int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public override event CollectionChangedEventHandler<ITrack>? TracksChanged { add => throw AccessedException; remove => throw AccessedException; }
            public override bool Equals(ICoreAlbumCollectionItem? other) => throw AccessedException;
            public override bool Equals(ICoreAlbumCollection? other) => throw AccessedException;
            public override Task PlayAlbumCollectionAsync(IAlbumCollectionItem albumItem, CancellationToken cancellationToken = default) => throw AccessedException;
            public override IAsyncEnumerable<IAlbumCollectionItem> GetAlbumItemsAsync(int limit, int offset, CancellationToken cancellationToken = default) => throw AccessedException;
            public override Task AddAlbumItemAsync(IAlbumCollectionItem album, int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public override event CollectionChangedEventHandler<IAlbumCollectionItem>? AlbumItemsChanged { add => throw AccessedException; remove => throw AccessedException; }
            public override bool Equals(ICoreArtistCollectionItem? other) => throw AccessedException;
            public override bool Equals(ICoreArtistCollection? other) => throw AccessedException;
            public override Task PlayArtistCollectionAsync(IArtistCollectionItem artistItem, CancellationToken cancellationToken = default) => throw AccessedException;
            public override IAsyncEnumerable<IArtistCollectionItem> GetArtistItemsAsync(int limit, int offset, CancellationToken cancellationToken = default) => throw AccessedException;
            public override Task AddArtistItemAsync(IArtistCollectionItem artistItem, int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public override event CollectionChangedEventHandler<IArtistCollectionItem>? ArtistItemsChanged { add => throw AccessedException; remove => throw AccessedException; }
            public override bool Equals(ICorePlayableCollectionGroupChildren? other) => throw AccessedException;
            public override Task PlayPlayableCollectionGroupAsync(IPlayableCollectionGroup collectionGroup, CancellationToken cancellationToken = default) => throw AccessedException;
            public override IAsyncEnumerable<IPlayableCollectionGroup> GetChildrenAsync(int limit, int offset, CancellationToken cancellationToken = default) => throw AccessedException;
            public override Task AddChildAsync(IPlayableCollectionGroup child, int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public override event CollectionChangedEventHandler<IPlayableCollectionGroup>? ChildItemsChanged { add => throw AccessedException; remove => throw AccessedException; }
            public override bool Equals(ICorePlayableCollectionGroup? other) => throw AccessedException;
            public override bool Equals(ICoreSearchResults? other) => throw AccessedException;
        }

        internal class NoOverride : SearchResultsPluginBase
        {
            public NoOverride(ISearchResults inner)
                : base(new ModelPluginMetadata("", nameof(NoOverride), "", new Version()), inner)
            {
            }
        }

        internal class NotBlockingDisposeAsync : SearchResultsPluginBase
        {
            public NotBlockingDisposeAsync()
                : base(new ModelPluginMetadata("", nameof(NotBlockingDisposeAsync), "", new Version()), new Unimplemented())
            {
            }

            /// <inheritdoc />
            public override ValueTask DisposeAsync() => default;
        }

        internal class Unimplemented : ISearchResults
        {
            internal static AccessedException<Unimplemented> AccessedException { get; } = new();

            public event EventHandler? SourcesChanged { add => throw AccessedException; remove => throw AccessedException; }
            public ValueTask DisposeAsync() => throw AccessedException;
            public Task<bool> IsAddImageAvailableAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public Task<bool> IsRemoveImageAvailableAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public Task RemoveImageAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public int TotalImageCount => throw AccessedException;
            public event EventHandler<int>? ImagesCountChanged { add => throw AccessedException; remove => throw AccessedException; }
            public int TotalUrlCount => throw AccessedException;
            public Task RemoveUrlAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public Task<bool> IsAddUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public Task<bool> IsRemoveUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public event EventHandler<int>? UrlsCountChanged { add => throw AccessedException; remove => throw AccessedException; }
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
            public event EventHandler<PlaybackState>? PlaybackStateChanged { add => throw AccessedException; remove => throw AccessedException; }
            public event EventHandler<string>? NameChanged { add => throw AccessedException; remove => throw AccessedException; }
            public event EventHandler<string?>? DescriptionChanged { add => throw AccessedException; remove => throw AccessedException; }
            public event EventHandler<TimeSpan>? DurationChanged { add => throw AccessedException; remove => throw AccessedException; }
            public event EventHandler<DateTime?>? LastPlayedChanged { add => throw AccessedException; remove => throw AccessedException; }
            public event EventHandler<bool>? IsChangeNameAsyncAvailableChanged { add => throw AccessedException; remove => throw AccessedException; }
            public event EventHandler<bool>? IsChangeDescriptionAsyncAvailableChanged { add => throw AccessedException; remove => throw AccessedException; }
            public event EventHandler<bool>? IsChangeDurationAsyncAvailableChanged { add => throw AccessedException; remove => throw AccessedException; }
            public DateTime? AddedAt => throw AccessedException;
            public int TotalPlaylistItemsCount => throw AccessedException;
            public bool IsPlayPlaylistCollectionAsyncAvailable => throw AccessedException;
            public bool IsPausePlaylistCollectionAsyncAvailable => throw AccessedException;
            public Task PlayPlaylistCollectionAsync(CancellationToken cancellationToken = default) => throw AccessedException;
            public Task PausePlaylistCollectionAsync(CancellationToken cancellationToken = default) => throw AccessedException;
            public Task RemovePlaylistItemAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public Task<bool> IsAddPlaylistItemAvailableAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public Task<bool> IsRemovePlaylistItemAvailableAsync(int index, CancellationToken cancellationToken) => throw AccessedException;
            public event EventHandler<bool>? IsPlayPlaylistCollectionAsyncAvailableChanged { add => throw AccessedException; remove => throw AccessedException; }
            public event EventHandler<bool>? IsPausePlaylistCollectionAsyncAvailableChanged { add => throw AccessedException; remove => throw AccessedException; }
            public event EventHandler<int>? PlaylistItemsCountChanged { add => throw AccessedException; remove => throw AccessedException; }
            public int TotalTrackCount => throw AccessedException;
            public bool IsPlayTrackCollectionAsyncAvailable => throw AccessedException;
            public bool IsPauseTrackCollectionAsyncAvailable => throw AccessedException;
            public Task PlayTrackCollectionAsync(CancellationToken cancellationToken = default) => throw AccessedException;
            public Task PauseTrackCollectionAsync(CancellationToken cancellationToken = default) => throw AccessedException;
            public Task RemoveTrackAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public Task<bool> IsAddTrackAvailableAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public Task<bool> IsRemoveTrackAvailableAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public event EventHandler<bool>? IsPlayTrackCollectionAsyncAvailableChanged { add => throw AccessedException; remove => throw AccessedException; }
            public event EventHandler<bool>? IsPauseTrackCollectionAsyncAvailableChanged { add => throw AccessedException; remove => throw AccessedException; }
            public event EventHandler<int>? TracksCountChanged { add => throw AccessedException; remove => throw AccessedException; }
            public int TotalAlbumItemsCount => throw AccessedException;
            public bool IsPlayAlbumCollectionAsyncAvailable => throw AccessedException;
            public bool IsPauseAlbumCollectionAsyncAvailable => throw AccessedException;
            public Task PlayAlbumCollectionAsync(CancellationToken cancellationToken = default) => throw AccessedException;
            public Task PauseAlbumCollectionAsync(CancellationToken cancellationToken = default) => throw AccessedException;
            public Task RemoveAlbumItemAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public Task<bool> IsAddAlbumItemAvailableAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public Task<bool> IsRemoveAlbumItemAvailableAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public event EventHandler<bool>? IsPlayAlbumCollectionAsyncAvailableChanged { add => throw AccessedException; remove => throw AccessedException; }
            public event EventHandler<bool>? IsPauseAlbumCollectionAsyncAvailableChanged { add => throw AccessedException; remove => throw AccessedException; }
            public event EventHandler<int>? AlbumItemsCountChanged { add => throw AccessedException; remove => throw AccessedException; }
            public int TotalArtistItemsCount => throw AccessedException;
            public bool IsPlayArtistCollectionAsyncAvailable => throw AccessedException;
            public bool IsPauseArtistCollectionAsyncAvailable => throw AccessedException;
            public Task PlayArtistCollectionAsync(CancellationToken cancellationToken = default) => throw AccessedException;
            public Task PauseArtistCollectionAsync(CancellationToken cancellationToken = default) => throw AccessedException;
            public Task RemoveArtistItemAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public Task<bool> IsAddArtistItemAvailableAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public Task<bool> IsRemoveArtistItemAvailableAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public event EventHandler<bool>? IsPlayArtistCollectionAsyncAvailableChanged { add => throw AccessedException; remove => throw AccessedException; }
            public event EventHandler<bool>? IsPauseArtistCollectionAsyncAvailableChanged { add => throw AccessedException; remove => throw AccessedException; }
            public event EventHandler<int>? ArtistItemsCountChanged { add => throw AccessedException; remove => throw AccessedException; }
            public Task PlayPlayableCollectionGroupAsync(CancellationToken cancellationToken = default) => throw AccessedException;
            public Task PausePlayableCollectionGroupAsync(CancellationToken cancellationToken = default) => throw AccessedException;
            public int TotalChildrenCount => throw AccessedException;
            public Task RemoveChildAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public Task<bool> IsAddChildAvailableAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public Task<bool> IsRemoveChildAvailableAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public event EventHandler<int>? ChildrenCountChanged { add => throw AccessedException; remove => throw AccessedException; }
            public bool Equals(ICoreImageCollection? other) => throw AccessedException;
            IReadOnlyList<ICoreImageCollection> IMerged<ICoreImageCollection>.Sources => throw AccessedException;
            IReadOnlyList<ICoreUrlCollection> IMerged<ICoreUrlCollection>.Sources => throw AccessedException;
            IReadOnlyList<ICorePlaylistCollectionItem> IMerged<ICorePlaylistCollectionItem>.Sources => throw AccessedException;
            IReadOnlyList<ICorePlaylistCollection> IMerged<ICorePlaylistCollection>.Sources => throw AccessedException;
            IReadOnlyList<ICoreTrackCollection> IMerged<ICoreTrackCollection>.Sources => throw AccessedException;
            IReadOnlyList<ICoreAlbumCollectionItem> IMerged<ICoreAlbumCollectionItem>.Sources => throw AccessedException;
            IReadOnlyList<ICoreAlbumCollection> IMerged<ICoreAlbumCollection>.Sources => throw AccessedException;
            IReadOnlyList<ICoreArtistCollectionItem> IMerged<ICoreArtistCollectionItem>.Sources => throw AccessedException;
            IReadOnlyList<ICoreArtistCollection> IMerged<ICoreArtistCollection>.Sources => throw AccessedException;
            IReadOnlyList<ICorePlayableCollectionGroupChildren> IMerged<ICorePlayableCollectionGroupChildren>.Sources => throw AccessedException;
            IReadOnlyList<ICorePlayableCollectionGroup> IMerged<ICorePlayableCollectionGroup>.Sources => throw AccessedException;
            public IAsyncEnumerable<IImage> GetImagesAsync(int limit, int offset, CancellationToken cancellationToken = default) => throw AccessedException;
            public Task AddImageAsync(IImage image, int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public event CollectionChangedEventHandler<IImage>? ImagesChanged { add => throw AccessedException; remove => throw AccessedException; }
            public bool Equals(ICoreUrlCollection? other) => throw AccessedException;
            public IAsyncEnumerable<IUrl> GetUrlsAsync(int limit, int offset, CancellationToken cancellationToken = default) => throw AccessedException;
            public Task AddUrlAsync(IUrl url, int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public event CollectionChangedEventHandler<IUrl>? UrlsChanged { add => throw AccessedException; remove => throw AccessedException; }
            public DownloadInfo DownloadInfo => throw AccessedException;
            public Task StartDownloadOperationAsync(DownloadOperation operation, CancellationToken cancellationToken = default) => throw AccessedException;
            public event EventHandler<DownloadInfo>? DownloadInfoChanged { add => throw AccessedException; remove => throw AccessedException; }
            public bool Equals(ICorePlaylistCollectionItem? other) => throw AccessedException;
            public bool Equals(ICorePlaylistCollection? other) => throw AccessedException;
            public Task PlayPlaylistCollectionAsync(IPlaylistCollectionItem playlistItem, CancellationToken cancellationToken = default) => throw AccessedException;
            public IAsyncEnumerable<IPlaylistCollectionItem> GetPlaylistItemsAsync(int limit, int offset, CancellationToken cancellationToken = default) => throw AccessedException;
            public Task AddPlaylistItemAsync(IPlaylistCollectionItem playlistItem, int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public event CollectionChangedEventHandler<IPlaylistCollectionItem>? PlaylistItemsChanged { add => throw AccessedException; remove => throw AccessedException; }
            public bool Equals(ICoreTrackCollection? other) => throw AccessedException;
            public Task PlayTrackCollectionAsync(ITrack track, CancellationToken cancellationToken = default) => throw AccessedException;
            public IAsyncEnumerable<ITrack> GetTracksAsync(int limit, int offset, CancellationToken cancellationToken = default) => throw AccessedException;
            public Task AddTrackAsync(ITrack track, int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public event CollectionChangedEventHandler<ITrack>? TracksChanged { add => throw AccessedException; remove => throw AccessedException; }
            public bool Equals(ICoreAlbumCollectionItem? other) => throw AccessedException;
            public bool Equals(ICoreAlbumCollection? other) => throw AccessedException;
            public Task PlayAlbumCollectionAsync(IAlbumCollectionItem albumItem, CancellationToken cancellationToken = default) => throw AccessedException;
            public IAsyncEnumerable<IAlbumCollectionItem> GetAlbumItemsAsync(int limit, int offset, CancellationToken cancellationToken = default) => throw AccessedException;
            public Task AddAlbumItemAsync(IAlbumCollectionItem album, int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public event CollectionChangedEventHandler<IAlbumCollectionItem>? AlbumItemsChanged { add => throw AccessedException; remove => throw AccessedException; }
            public bool Equals(ICoreArtistCollectionItem? other) => throw AccessedException;
            public bool Equals(ICoreArtistCollection? other) => throw AccessedException;
            public Task PlayArtistCollectionAsync(IArtistCollectionItem artistItem, CancellationToken cancellationToken = default) => throw AccessedException;
            public IAsyncEnumerable<IArtistCollectionItem> GetArtistItemsAsync(int limit, int offset, CancellationToken cancellationToken = default) => throw AccessedException;
            public Task AddArtistItemAsync(IArtistCollectionItem artistItem, int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public event CollectionChangedEventHandler<IArtistCollectionItem>? ArtistItemsChanged { add => throw AccessedException; remove => throw AccessedException; }
            public bool Equals(ICorePlayableCollectionGroupChildren? other) => throw AccessedException;
            public Task PlayPlayableCollectionGroupAsync(IPlayableCollectionGroup collectionGroup, CancellationToken cancellationToken = default) => throw AccessedException;
            public IAsyncEnumerable<IPlayableCollectionGroup> GetChildrenAsync(int limit, int offset, CancellationToken cancellationToken = default) => throw AccessedException;
            public Task AddChildAsync(IPlayableCollectionGroup child, int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public event CollectionChangedEventHandler<IPlayableCollectionGroup>? ChildItemsChanged { add => throw AccessedException; remove => throw AccessedException; }
            public bool Equals(ICorePlayableCollectionGroup? other) => throw AccessedException;
            public IReadOnlyList<ICoreSearchResults> Sources => throw AccessedException;
            public bool Equals(ICoreSearchResults? other) => throw AccessedException;
        }
    }
}
