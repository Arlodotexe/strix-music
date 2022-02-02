using Microsoft.VisualStudio.TestTools.UnitTesting;
using OwlCore.ComponentModel;
using OwlCore.Events;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Models;
using StrixMusic.Sdk.Models.Base;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Models.Merged;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Tests.Plugins.Models
{
    [TestClass]
    public class PlayablePluginBaseTests
    {
        static bool NoInner(MemberInfo x) => !x.Name.Contains("Inner");

        [TestMethod]
        public void NoPlugins()
        {
            var builder = new Sdk.Plugins.PluginManager().ModelPlugins.Playable;
            var finalTestClass = new UnimplementedPlayable();

            var emptyChain = builder.Execute(finalTestClass);
            Assert.AreSame(emptyChain, finalTestClass);

            Helpers.AssertAllThrowsOnMemberAccess<AccessedException<UnimplementedPlayable>>(finalTestClass);
            Helpers.AssertAllThrowsOnMemberAccess<AccessedException<UnimplementedPlayable>>(emptyChain);
        }

        [TestMethod]
        public void PluginNoOverride()
        {
            // No plugins.
            var builder = new Sdk.Plugins.PluginManager().ModelPlugins.Playable;
            var finalTestClass = new UnimplementedPlayable();

            var emptyChain = builder.Execute(finalTestClass);

            Assert.AreSame(emptyChain, finalTestClass);

            Helpers.AssertAllThrowsOnMemberAccess<AccessedException<UnimplementedPlayable>>(finalTestClass);
            Helpers.AssertAllThrowsOnMemberAccess<AccessedException<UnimplementedPlayable>>(emptyChain);

            // No overrides.
            builder.Add(x => new NoOverrideCustomPlayable(x));
            var noOverride = builder.Execute(finalTestClass);

            Assert.AreNotSame(noOverride, emptyChain);
            Assert.AreNotSame(noOverride, finalTestClass);
            Helpers.AssertAllThrowsOnMemberAccess<AccessedException<UnimplementedPlayable>, NoOverrideCustomPlayable>(noOverride, customFilter: NoInner);
        }

        [TestMethod, Timeout(1000)]
        public void PluginFullyCustom()
        {
            // No plugins.
            var builder = new Sdk.Plugins.PluginManager().ModelPlugins.Playable;
            var finalTestClass = new UnimplementedPlayable();

            var emptyChain = builder.Execute(finalTestClass);

            Assert.AreSame(emptyChain, finalTestClass);

            Helpers.AssertAllThrowsOnMemberAccess<AccessedException<UnimplementedPlayable>>(finalTestClass);
            Helpers.AssertAllThrowsOnMemberAccess<AccessedException<UnimplementedPlayable>>(emptyChain);

            // No overrides.
            builder.Add(x => new NoOverrideCustomPlayable(x));
            var noOverride = builder.Execute(finalTestClass);

            Assert.AreNotSame(noOverride, emptyChain);
            Assert.AreNotSame(noOverride, finalTestClass);
            Helpers.AssertAllThrowsOnMemberAccess<AccessedException<UnimplementedPlayable>>(noOverride, customFilter: NoInner);

            // Fully custom
            builder.Add(x => new FullyCustomPlayable(x));
            var allCustom = builder.Execute(finalTestClass);

            Assert.AreNotSame(noOverride, emptyChain);
            Assert.AreNotSame(noOverride, finalTestClass);
            Helpers.AssertAllThrowsOnMemberAccess<AccessedException<FullyCustomPlayable>>(allCustom, customFilter: NoInner);
        }

        [TestMethod, Timeout(5000)]
        public void PluginFullyCustom_WithDownloadable()
        {
            // No plugins.
            var builder = new Sdk.Plugins.PluginManager().ModelPlugins.Playable;
            var finalTestClass = new UnimplementedPlayable();
            builder.Add(x => new NoOverrideCustomPlayable(x) { InnerDownloadable = new DownloadablePluginBaseTests.UnimplementedDownloadable() });

            var finalImpl = builder.Execute(finalTestClass);

            Assert.AreNotSame(finalImpl, finalTestClass);
            Assert.IsInstanceOfType(finalImpl, typeof(NoOverrideCustomPlayable));
            Helpers.AssertAllThrowsOnMemberAccess<AccessedException<DownloadablePluginBaseTests.UnimplementedDownloadable>, DownloadablePluginBaseTests.UnimplementedDownloadable>(finalImpl, customFilter: NoInner);
            Helpers.AssertAllThrowsOnMemberAccess<AccessedException<UnimplementedPlayable>, NoOverrideCustomPlayable>(finalImpl, customFilter: NoInner, typesToExclude: typeof(DownloadablePluginBaseTests.UnimplementedDownloadable));
        }

        internal class FullyCustomPlayable : Sdk.Plugins.Model.PlayablePluginBase
        {
            public FullyCustomPlayable(IPlayable inner)
                : base(inner)
            {
            }

            internal static AccessedException<FullyCustomPlayable> AccessedException { get; } = new AccessedException<FullyCustomPlayable>();

            public override DownloadInfo DownloadInfo => throw AccessedException;
            public override string Id => throw AccessedException;
            public override string Name => throw AccessedException;
            public override string? Description => throw AccessedException;
            public override DateTime? LastPlayed => throw AccessedException;
            public override PlaybackState PlaybackState => throw AccessedException;
            public override TimeSpan Duration => throw AccessedException;
            public override bool IsChangeNameAsyncAvailable => throw AccessedException;
            public override bool IsChangeDescriptionAsyncAvailable => throw AccessedException;
            public override bool IsChangeDurationAsyncAvailable => throw AccessedException;
            public override int TotalImageCount => throw AccessedException;
            public override int TotalUrlCount => throw AccessedException;
            public override IReadOnlyList<ICoreImageCollection> Sources => throw AccessedException;
            public override IReadOnlyList<ICore> SourceCores => throw AccessedException;

            public override event EventHandler<DownloadInfo>? DownloadInfoChanged { add => throw AccessedException; remove => throw AccessedException; }
            public override event EventHandler<PlaybackState>? PlaybackStateChanged { add => throw AccessedException; remove => throw AccessedException; }
            public override event EventHandler<string>? NameChanged { add => throw AccessedException; remove => throw AccessedException; }
            public override event EventHandler<string?>? DescriptionChanged { add => throw AccessedException; remove => throw AccessedException; }
            public override event EventHandler<TimeSpan>? DurationChanged { add => throw AccessedException; remove => throw AccessedException; }
            public override event EventHandler<DateTime?>? LastPlayedChanged { add => throw AccessedException; remove => throw AccessedException; }
            public override event EventHandler<bool>? IsChangeNameAsyncAvailableChanged { add => throw AccessedException; remove => throw AccessedException; }
            public override event EventHandler<bool>? IsChangeDescriptionAsyncAvailableChanged { add => throw AccessedException; remove => throw AccessedException; }
            public override event EventHandler<bool>? IsChangeDurationAsyncAvailableChanged { add => throw AccessedException; remove => throw AccessedException; }
            public override event EventHandler<int>? ImagesCountChanged { add => throw AccessedException; remove => throw AccessedException; }
            public override event EventHandler<int>? UrlsCountChanged { add => throw AccessedException; remove => throw AccessedException; }
            public override event CollectionChangedEventHandler<IImage>? ImagesChanged { add => throw AccessedException; remove => throw AccessedException; }
            public override event CollectionChangedEventHandler<IUrl>? UrlsChanged { add => throw AccessedException; remove => throw AccessedException; }

            public override Task AddImageAsync(IImage image, int index) => throw AccessedException;
            public override Task AddUrlAsync(IUrl url, int index) => throw AccessedException;
            public override Task ChangeDescriptionAsync(string? description) => throw AccessedException;
            public override Task ChangeDurationAsync(TimeSpan duration) => throw AccessedException;
            public override Task ChangeNameAsync(string name) => throw AccessedException;
            public override ValueTask DisposeAsync() => throw AccessedException;
            public override bool Equals(ICoreImageCollection? other) => throw AccessedException;
            public override bool Equals(ICoreUrlCollection? other) => throw AccessedException;
            public override Task<IReadOnlyList<IImage>> GetImagesAsync(int limit, int offset) => throw AccessedException;
            public override Task<IReadOnlyList<IUrl>> GetUrlsAsync(int limit, int offset) => throw AccessedException;
            public override Task<bool> IsAddImageAvailableAsync(int index) => throw AccessedException;
            public override Task<bool> IsAddUrlAvailableAsync(int index) => throw AccessedException;
            public override Task<bool> IsRemoveImageAvailableAsync(int index) => throw AccessedException;
            public override Task<bool> IsRemoveUrlAvailableAsync(int index) => throw AccessedException;
            public override Task RemoveImageAsync(int index) => throw AccessedException;
            public override Task RemoveUrlAsync(int index) => throw AccessedException;
            public override Task StartDownloadOperationAsync(DownloadOperation operation) => throw AccessedException;
        }

        internal class NoOverrideCustomPlayable : Sdk.Plugins.Model.PlayablePluginBase
        {
            public NoOverrideCustomPlayable(IPlayable inner)
                : base(inner)
            {
            }
        }

        internal class UnimplementedPlayable : IPlayable
        {
            internal static AccessedException<UnimplementedPlayable> AccessedException { get; } = new AccessedException<UnimplementedPlayable>();

            public DownloadInfo DownloadInfo => throw AccessedException;
            public string Id => throw AccessedException;
            public string Name => throw AccessedException;
            public string? Description => throw AccessedException;
            public DateTime? LastPlayed => throw AccessedException;
            public PlaybackState PlaybackState => throw AccessedException;
            public TimeSpan Duration => throw AccessedException;
            public bool IsChangeNameAsyncAvailable => throw AccessedException;
            public bool IsChangeDescriptionAsyncAvailable => throw AccessedException;
            public bool IsChangeDurationAsyncAvailable => throw AccessedException;
            public int TotalImageCount => throw AccessedException;
            public int TotalUrlCount => throw AccessedException;
            public IReadOnlyList<ICoreImageCollection> Sources => throw AccessedException;
            public IReadOnlyList<ICore> SourceCores => throw AccessedException;
            IReadOnlyList<ICoreUrlCollection> IMerged<ICoreUrlCollection>.Sources => throw AccessedException;

            public event EventHandler<DownloadInfo>? DownloadInfoChanged { add => throw AccessedException; remove => throw AccessedException; }
            public event EventHandler<PlaybackState>? PlaybackStateChanged { add => throw AccessedException; remove => throw AccessedException; }
            public event EventHandler<string>? NameChanged { add => throw AccessedException; remove => throw AccessedException; }
            public event EventHandler<string?>? DescriptionChanged { add => throw AccessedException; remove => throw AccessedException; }
            public event EventHandler<TimeSpan>? DurationChanged { add => throw AccessedException; remove => throw AccessedException; }
            public event EventHandler<DateTime?>? LastPlayedChanged { add => throw AccessedException; remove => throw AccessedException; }
            public event EventHandler<bool>? IsChangeNameAsyncAvailableChanged { add => throw AccessedException; remove => throw AccessedException; }
            public event EventHandler<bool>? IsChangeDescriptionAsyncAvailableChanged { add => throw AccessedException; remove => throw AccessedException; }
            public event EventHandler<bool>? IsChangeDurationAsyncAvailableChanged { add => throw AccessedException; remove => throw AccessedException; }
            public event EventHandler<int>? ImagesCountChanged { add => throw AccessedException; remove => throw AccessedException; }
            public event EventHandler<int>? UrlsCountChanged { add => throw AccessedException; remove => throw AccessedException; }
            public event CollectionChangedEventHandler<IImage>? ImagesChanged { add => throw AccessedException; remove => throw AccessedException; }
            public event CollectionChangedEventHandler<IUrl>? UrlsChanged { add => throw AccessedException; remove => throw AccessedException; }

            public Task AddImageAsync(IImage image, int index) => throw AccessedException;
            public Task AddUrlAsync(IUrl url, int index) => throw AccessedException;
            public Task ChangeDescriptionAsync(string? description) => throw AccessedException;
            public Task ChangeDurationAsync(TimeSpan duration) => throw AccessedException;
            public Task ChangeNameAsync(string name) => throw AccessedException;
            public ValueTask DisposeAsync() => throw AccessedException;
            public bool Equals(ICoreImageCollection? other) => throw AccessedException;
            public bool Equals(ICoreUrlCollection? other) => throw AccessedException;
            public Task<IReadOnlyList<IImage>> GetImagesAsync(int limit, int offset) => throw AccessedException;
            public Task<IReadOnlyList<IUrl>> GetUrlsAsync(int limit, int offset) => throw AccessedException;
            public Task<bool> IsAddImageAvailableAsync(int index) => throw AccessedException;
            public Task<bool> IsAddUrlAvailableAsync(int index) => throw AccessedException;
            public Task<bool> IsRemoveImageAvailableAsync(int index) => throw AccessedException;
            public Task<bool> IsRemoveUrlAvailableAsync(int index) => throw AccessedException;
            public Task RemoveImageAsync(int index) => throw AccessedException;
            public Task RemoveUrlAsync(int index) => throw AccessedException;
            public Task StartDownloadOperationAsync(DownloadOperation operation) => throw AccessedException;
        }
    }
}
