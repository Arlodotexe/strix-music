using Microsoft.VisualStudio.TestTools.UnitTesting;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Plugins.Model;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using OwlCore.ComponentModel;
using StrixMusic.Sdk.AdapterModels;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Sdk.Tests.Mock.AppModels;

namespace StrixMusic.Sdk.Tests.Plugins.Models
{
    [TestClass]
    public class PlaylistCollectionPluginBaseTests
    {
        private static bool NoInner(MemberInfo x) => !x.Name.Contains("Inner");
        private static bool NoInnerOrSources(MemberInfo x) => NoInner(x) && !x.Name.ToLower().Contains("sources");

        [Flags]
        public enum PossiblePlugins
        {
            None = 0,
            Playable = 1,
            Downloadable = 2,
            ImageCollection = 4,
            UrlCollection = 8,
        }

        [TestMethod, Timeout(5000)]
        public void NoPlugins()
        {
            var builder = new SdkModelPlugin(SdkTestPluginMetadata.Metadata).PlaylistCollection;
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
            var builder = new SdkModelPlugin(SdkTestPluginMetadata.Metadata).PlaylistCollection;
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
            var builder = new SdkModelPlugin(SdkTestPluginMetadata.Metadata).PlaylistCollection;
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
            var builder = new SdkModelPlugin(SdkTestPluginMetadata.Metadata).PlaylistCollection;
            var defaultImplementation = new Unimplemented();
            builder.Add(x => new NoOverride(x)
            {
                InnerDownloadable = data.HasFlag(PossiblePlugins.Downloadable) ? new DownloadablePluginBaseTests.Unimplemented() : x,
                InnerPlayable = data.HasFlag(PossiblePlugins.Playable) ? new PlayablePluginBaseTests.Unimplemented() : x,
                InnerImageCollection = data.HasFlag(PossiblePlugins.ImageCollection) ? new ImageCollectionPluginBaseTests.Unimplemented() : x,
                InnerUrlCollection = data.HasFlag(PossiblePlugins.UrlCollection) ? new UrlCollectionPluginBaseTests.Unimplemented() : x,
            });

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

            if (data.HasFlag(PossiblePlugins.ImageCollection))
            {
                Helpers.AssertAllMembersThrowOnAccess<AccessedException<ImageCollectionPluginBaseTests.Unimplemented>,
                    ImageCollectionPluginBaseTests.Unimplemented>(
                    finalImpl,
                    customFilter: NoInnerOrSources
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


        internal class FullyCustom : PlaylistCollectionPluginBase
        {
            public FullyCustom(IPlaylistCollection inner)
                : base(new ModelPluginMetadata("", nameof(FullyCustom), "", new Version()), inner, new MockStrixDataRoot())
            {
            }

            internal static AccessedException<FullyCustom> AccessedException { get; } = new AccessedException<FullyCustom>();

            public override int TotalPlaylistItemsCount => throw AccessedException;
            public override bool IsPlayPlaylistCollectionAsyncAvailable => throw AccessedException;
            public override bool IsPausePlaylistCollectionAsyncAvailable => throw AccessedException;
            public override DateTime? AddedAt => throw AccessedException;
            public override string Id => throw AccessedException;
            public override string Name => throw AccessedException;
            public override string? Description => throw AccessedException;
            public override DateTime? LastPlayed => throw AccessedException;
            public override PlaybackState PlaybackState => throw AccessedException;
            public override TimeSpan Duration => throw AccessedException;
            public override bool IsChangeNameAsyncAvailable => throw AccessedException;
            public override bool IsChangeDescriptionAsyncAvailable => throw AccessedException;
            public override bool IsChangeDurationAsyncAvailable => throw AccessedException;
            public override DownloadInfo DownloadInfo => throw AccessedException;
            public override int TotalImageCount => throw AccessedException;
            public override int TotalUrlCount => throw AccessedException;

            public override event CollectionChangedEventHandler<IPlaylistCollectionItem>? PlaylistItemsChanged { add => throw AccessedException; remove => throw AccessedException; }
            public override event EventHandler<bool>? IsPlayPlaylistCollectionAsyncAvailableChanged { add => throw AccessedException; remove => throw AccessedException; }
            public override event EventHandler<bool>? IsPausePlaylistCollectionAsyncAvailableChanged { add => throw AccessedException; remove => throw AccessedException; }
            public override event EventHandler<int>? PlaylistItemsCountChanged { add => throw AccessedException; remove => throw AccessedException; }
            public override event EventHandler<PlaybackState>? PlaybackStateChanged { add => throw AccessedException; remove => throw AccessedException; }
            public override event EventHandler<string>? NameChanged { add => throw AccessedException; remove => throw AccessedException; }
            public override event EventHandler<string?>? DescriptionChanged { add => throw AccessedException; remove => throw AccessedException; }
            public override event EventHandler<TimeSpan>? DurationChanged { add => throw AccessedException; remove => throw AccessedException; }
            public override event EventHandler<DateTime?>? LastPlayedChanged { add => throw AccessedException; remove => throw AccessedException; }
            public override event EventHandler<bool>? IsChangeNameAsyncAvailableChanged { add => throw AccessedException; remove => throw AccessedException; }
            public override event EventHandler<bool>? IsChangeDescriptionAsyncAvailableChanged { add => throw AccessedException; remove => throw AccessedException; }
            public override event EventHandler<bool>? IsChangeDurationAsyncAvailableChanged { add => throw AccessedException; remove => throw AccessedException; }
            public override event EventHandler<DownloadInfo>? DownloadInfoChanged { add => throw AccessedException; remove => throw AccessedException; }
            public override event CollectionChangedEventHandler<IImage>? ImagesChanged { add => throw AccessedException; remove => throw AccessedException; }
            public override event EventHandler<int>? ImagesCountChanged { add => throw AccessedException; remove => throw AccessedException; }
            public override event CollectionChangedEventHandler<IUrl>? UrlsChanged { add => throw AccessedException; remove => throw AccessedException; }
            public override event EventHandler<int>? UrlsCountChanged { add => throw AccessedException; remove => throw AccessedException; }

            public override Task AddPlaylistItemAsync(IPlaylistCollectionItem playlistItem, int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public override Task AddImageAsync(IImage image, int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public override Task AddUrlAsync(IUrl url, int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public override Task ChangeDescriptionAsync(string? description, CancellationToken cancellationToken = default) => throw AccessedException;
            public override Task ChangeDurationAsync(TimeSpan duration, CancellationToken cancellationToken = default) => throw AccessedException;
            public override Task ChangeNameAsync(string name, CancellationToken cancellationToken = default) => throw AccessedException;
            public override bool Equals(ICorePlaylistCollectionItem? other) => throw AccessedException;
            public override bool Equals(ICoreImageCollection? other) => throw AccessedException;
            public override bool Equals(ICoreUrlCollection? other) => throw AccessedException;
            public override bool Equals(ICorePlaylistCollection? other) => throw AccessedException;
            public override IAsyncEnumerable<IPlaylistCollectionItem> GetPlaylistItemsAsync(int limit, int offset, CancellationToken cancellationToken = default) => throw AccessedException;
            public override IAsyncEnumerable<IImage> GetImagesAsync(int limit, int offset, CancellationToken cancellationToken = default) => throw AccessedException;
            public override IAsyncEnumerable<IUrl> GetUrlsAsync(int limit, int offset, CancellationToken cancellationToken = default) => throw AccessedException;
            public override Task<bool> IsAddPlaylistItemAvailableAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public override Task<bool> IsAddImageAvailableAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public override Task<bool> IsAddUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public override Task<bool> IsRemovePlaylistItemAvailableAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public override Task<bool> IsRemoveImageAvailableAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public override Task<bool> IsRemoveUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public override Task PausePlaylistCollectionAsync(CancellationToken cancellationToken = default) => throw AccessedException;
            public override Task PlayPlaylistCollectionAsync(IPlaylistCollectionItem playlistItem, CancellationToken cancellationToken = default) => throw AccessedException;
            public override Task PlayPlaylistCollectionAsync(CancellationToken cancellationToken = default) => throw AccessedException;
            public override Task RemovePlaylistItemAsync(int index,  CancellationToken cancellationToken = default) => throw AccessedException;
            public override Task RemoveImageAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public override Task RemoveUrlAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public override Task StartDownloadOperationAsync(DownloadOperation operation, CancellationToken cancellationToken = default) => throw AccessedException;
        }

        internal class NoOverride : PlaylistCollectionPluginBase
        {
            public NoOverride(IPlaylistCollection inner)
                : base(new ModelPluginMetadata("", nameof(NoOverride), "", new Version()), inner, new MockStrixDataRoot())
            {
            }
        }

        internal class Unimplemented : IPlaylistCollection
        {
            internal static AccessedException<Unimplemented> AccessedException { get; } = new AccessedException<Unimplemented>();

            public event EventHandler? SourcesChanged { add => throw AccessedException; remove => throw AccessedException; }
            public int TotalPlaylistItemsCount => throw AccessedException;
            public bool IsPlayPlaylistCollectionAsyncAvailable => throw AccessedException;
            public bool IsPausePlaylistCollectionAsyncAvailable => throw AccessedException;
            public DateTime? AddedAt => throw AccessedException;
            public IReadOnlyList<ICorePlaylistCollectionItem> Sources => throw AccessedException;
            public string Id => throw AccessedException;
            public string Name => throw AccessedException;
            public string? Description => throw AccessedException;
            public DateTime? LastPlayed => throw AccessedException;
            public PlaybackState PlaybackState => throw AccessedException;
            public TimeSpan Duration => throw AccessedException;
            public bool IsChangeNameAsyncAvailable => throw AccessedException;
            public bool IsChangeDescriptionAsyncAvailable => throw AccessedException;
            public bool IsChangeDurationAsyncAvailable => throw AccessedException;
            public DownloadInfo DownloadInfo => throw AccessedException;
            public int TotalImageCount => throw AccessedException;
            public int TotalUrlCount => throw AccessedException;
            IReadOnlyList<ICoreImageCollection> IMerged<ICoreImageCollection>.Sources => throw AccessedException;
            IReadOnlyList<ICoreUrlCollection> IMerged<ICoreUrlCollection>.Sources => throw AccessedException;
            IReadOnlyList<ICorePlaylistCollection> IMerged<ICorePlaylistCollection>.Sources => throw AccessedException;

            public event CollectionChangedEventHandler<IPlaylistCollectionItem>? PlaylistItemsChanged { add => throw AccessedException; remove => throw AccessedException; }
            public event EventHandler<bool>? IsPlayPlaylistCollectionAsyncAvailableChanged { add => throw AccessedException; remove => throw AccessedException; }
            public event EventHandler<bool>? IsPausePlaylistCollectionAsyncAvailableChanged { add => throw AccessedException; remove => throw AccessedException; }
            public event EventHandler<int>? PlaylistItemsCountChanged { add => throw AccessedException; remove => throw AccessedException; }
            public event EventHandler<PlaybackState>? PlaybackStateChanged { add => throw AccessedException; remove => throw AccessedException; }
            public event EventHandler<string>? NameChanged { add => throw AccessedException; remove => throw AccessedException; }
            public event EventHandler<string?>? DescriptionChanged { add => throw AccessedException; remove => throw AccessedException; }
            public event EventHandler<TimeSpan>? DurationChanged { add => throw AccessedException; remove => throw AccessedException; }
            public event EventHandler<DateTime?>? LastPlayedChanged { add => throw AccessedException; remove => throw AccessedException; }
            public event EventHandler<bool>? IsChangeNameAsyncAvailableChanged { add => throw AccessedException; remove => throw AccessedException; }
            public event EventHandler<bool>? IsChangeDescriptionAsyncAvailableChanged { add => throw AccessedException; remove => throw AccessedException; }
            public event EventHandler<bool>? IsChangeDurationAsyncAvailableChanged { add => throw AccessedException; remove => throw AccessedException; }
            public event EventHandler<DownloadInfo>? DownloadInfoChanged { add => throw AccessedException; remove => throw AccessedException; }
            public event CollectionChangedEventHandler<IImage>? ImagesChanged { add => throw AccessedException; remove => throw AccessedException; }
            public event EventHandler<int>? ImagesCountChanged { add => throw AccessedException; remove => throw AccessedException; }
            public event CollectionChangedEventHandler<IUrl>? UrlsChanged { add => throw AccessedException; remove => throw AccessedException; }
            public event EventHandler<int>? UrlsCountChanged { add => throw AccessedException; remove => throw AccessedException; }

            public Task AddPlaylistItemAsync(IPlaylistCollectionItem playlistItem, int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public Task AddImageAsync(IImage image, int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public Task AddUrlAsync(IUrl url, int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public Task ChangeDescriptionAsync(string? description, CancellationToken cancellationToken = default) => throw AccessedException;
            public Task ChangeDurationAsync(TimeSpan duration, CancellationToken cancellationToken = default) => throw AccessedException;
            public Task ChangeNameAsync(string name, CancellationToken cancellationToken = default) => throw AccessedException;
            public bool Equals(ICorePlaylistCollectionItem? other) => throw AccessedException;
            public bool Equals(ICoreImageCollection? other) => throw AccessedException;
            public bool Equals(ICoreUrlCollection? other) => throw AccessedException;
            public bool Equals(ICorePlaylistCollection? other) => throw AccessedException;
            public IAsyncEnumerable<IPlaylistCollectionItem> GetPlaylistItemsAsync(int limit, int offset, CancellationToken cancellationToken = default) => throw AccessedException;
            public IAsyncEnumerable<IImage> GetImagesAsync(int limit, int offset, CancellationToken cancellationToken = default) => throw AccessedException;
            public IAsyncEnumerable<IUrl> GetUrlsAsync(int limit, int offset, CancellationToken cancellationToken = default) => throw AccessedException;
            public Task<bool> IsAddPlaylistItemAvailableAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public Task<bool> IsAddImageAvailableAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public Task<bool> IsAddUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public Task<bool> IsRemovePlaylistItemAvailableAsync(int index, CancellationToken cancellationToken) => throw AccessedException;
            public Task<bool> IsRemoveImageAvailableAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public Task<bool> IsRemoveUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public Task PausePlaylistCollectionAsync(CancellationToken cancellationToken = default) => throw AccessedException;
            public Task PlayPlaylistCollectionAsync(IPlaylistCollectionItem playlistItem, CancellationToken cancellationToken = default) => throw AccessedException;
            public Task PlayPlaylistCollectionAsync(CancellationToken cancellationToken = default) => throw AccessedException;
            public Task RemovePlaylistItemAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public Task RemoveImageAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public Task RemoveUrlAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
            public Task StartDownloadOperationAsync(DownloadOperation operation, CancellationToken cancellationToken = default) => throw AccessedException;
            public IStrixDataRoot Root  => throw AccessedException;
        }
    }
}
