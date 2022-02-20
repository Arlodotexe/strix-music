﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using OwlCore.Events;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Models;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Models.Merged;
using StrixMusic.Sdk.Plugins.Model;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Tests.Plugins.Models
{
    [TestClass]
    public class PlayablePluginBaseTests
    {
        private static bool NoInner(MemberInfo x) => !x.Name.Contains("Inner");
        private static bool NoInnerOrSources(MemberInfo x) => NoInner(x) && x.Name != "get_Sources" && x.Name != "get_SourceCores";

        [TestMethod, Timeout(1000)]
        public void NoPlugins()
        {
            var builder = new Sdk.Plugins.PluginManager().ModelPlugins.Playable;
            var finalTestClass = new Unimplemented();

            var emptyChain = builder.Execute(finalTestClass);
            Assert.AreSame(emptyChain, finalTestClass);

            Helpers.AssertAllThrowsOnMemberAccess<AccessedException<Unimplemented>>(finalTestClass);
            Helpers.AssertAllThrowsOnMemberAccess<AccessedException<Unimplemented>>(emptyChain);
        }

        [TestMethod, Timeout(1000)]
        public void PluginNoOverride()
        {
            // No plugins.
            var builder = new Sdk.Plugins.PluginManager().ModelPlugins.Playable;
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

        [TestMethod, Timeout(1000)]
        public void PluginFullyCustom()
        {
            // No plugins.
            var builder = new Sdk.Plugins.PluginManager().ModelPlugins.Playable;
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
        public void PluginFullyCustomWith_Downloadable()
        {
            var builder = new Sdk.Plugins.PluginManager().ModelPlugins.Playable;
            var finalTestClass = new Unimplemented();
            builder.Add(x => new NoOverride(x) {InnerDownloadable = new DownloadablePluginBaseTests.Unimplemented()});

            var finalImpl = builder.Execute(finalTestClass);

            Assert.AreNotSame(finalImpl, finalTestClass);
            Assert.IsInstanceOfType(finalImpl, typeof(NoOverride));
            Helpers.AssertAllMembersThrowOnAccess<AccessedException<DownloadablePluginBaseTests.Unimplemented>, DownloadablePluginBaseTests.Unimplemented>(finalImpl, customFilter: NoInner, typesToExclude: typeof(IAsyncDisposable));
            Helpers.AssertAllMembersThrowOnAccess<AccessedException<Unimplemented>, NoOverride>(finalImpl, customFilter: NoInner, typesToExclude: new[] {typeof(IAsyncDisposable), typeof(DownloadablePluginBaseTests.Unimplemented)});

            Helpers.AssertAllThrowsOnMemberAccess<IAsyncDisposable>(finalImpl, customFilter: NoInner, expectedExceptions: new[]
            {
                typeof(AccessedException<PlayablePluginBaseTests.Unimplemented>),
                typeof(AccessedException<DownloadablePluginBaseTests.Unimplemented>),
            });
        }

        [TestMethod, Timeout(5000)]
        public void PluginFullyCustomWith_ImageCollection()
        {
            var builder = new Sdk.Plugins.PluginManager().ModelPlugins.Playable;
            var finalTestClass = new Unimplemented();
            builder.Add(x => new NoOverride(x) {InnerImageCollection = new ImageCollectionPluginBaseTests.Unimplemented()});

            var finalImpl = builder.Execute(finalTestClass);

            Assert.AreNotSame(finalImpl, finalTestClass);
            Assert.IsInstanceOfType(finalImpl, typeof(NoOverride));
            Helpers.AssertAllMembersThrowOnAccess<AccessedException<ImageCollectionPluginBaseTests.Unimplemented>, ImageCollectionPluginBaseTests.Unimplemented>(finalImpl, customFilter: NoInner, typesToExclude: typeof(IAsyncDisposable));
            Helpers.AssertAllMembersThrowOnAccess<AccessedException<Unimplemented>, NoOverride>(finalImpl, customFilter: NoInner, typesToExclude: new[] {typeof(IAsyncDisposable), typeof(ImageCollectionPluginBaseTests.Unimplemented)});

            Helpers.AssertAllThrowsOnMemberAccess<IAsyncDisposable>(finalImpl, customFilter: NoInner, expectedExceptions: new[]
            {
                typeof(AccessedException<PlayablePluginBaseTests.Unimplemented>),
                typeof(AccessedException<ImageCollectionPluginBaseTests.Unimplemented>),
            });
        }

        [TestMethod, Timeout(5000)]
        public void PluginFullyCustomWith_UrlCollection()
        {
            var builder = new Sdk.Plugins.PluginManager().ModelPlugins.Playable;
            var finalTestClass = new Unimplemented();
            builder.Add(x => new NoOverride(x) {InnerUrlCollection = new UrlCollectionPluginBaseTests.Unimplemented()});

            var finalImpl = builder.Execute(finalTestClass);

            Assert.AreNotSame(finalImpl, finalTestClass);
            Assert.IsInstanceOfType(finalImpl, typeof(NoOverride));
            Helpers.AssertAllMembersThrowOnAccess<AccessedException<UrlCollectionPluginBaseTests.Unimplemented>, UrlCollectionPluginBaseTests.Unimplemented>(finalImpl, customFilter: NoInner, typesToExclude: typeof(IAsyncDisposable));
            Helpers.AssertAllMembersThrowOnAccess<AccessedException<Unimplemented>, NoOverride>(finalImpl, customFilter: NoInner, typesToExclude: new[] {typeof(IAsyncDisposable), typeof(UrlCollectionPluginBaseTests.Unimplemented)});

            Helpers.AssertAllThrowsOnMemberAccess<IAsyncDisposable>(finalImpl, customFilter: NoInner, expectedExceptions: new[]
            {
                typeof(AccessedException<PlayablePluginBaseTests.Unimplemented>),
                typeof(AccessedException<UrlCollectionPluginBaseTests.Unimplemented>),
            });
        }

        [TestMethod, Timeout(5000)]
        public void PluginFullyCustomWith_Downloadable_ImageCollection()
        {
            var builder = new Sdk.Plugins.PluginManager().ModelPlugins.Playable;
            var finalTestClass = new Unimplemented();
            builder.Add(x => new NoOverride(x)
            {
                InnerDownloadable = new DownloadablePluginBaseTests.Unimplemented(),
                InnerImageCollection = new ImageCollectionPluginBaseTests.Unimplemented()
            });

            var finalImpl = builder.Execute(finalTestClass);

            Assert.AreNotSame(finalImpl, finalTestClass);
            Assert.IsInstanceOfType(finalImpl, typeof(NoOverride));
            Helpers.AssertAllMembersThrowOnAccess<AccessedException<DownloadablePluginBaseTests.Unimplemented>, DownloadablePluginBaseTests.Unimplemented>(finalImpl, customFilter: NoInner, typesToExclude: typeof(IAsyncDisposable));
            Helpers.AssertAllMembersThrowOnAccess<AccessedException<ImageCollectionPluginBaseTests.Unimplemented>, ImageCollectionPluginBaseTests.Unimplemented>(finalImpl, customFilter: NoInner, typesToExclude: typeof(IAsyncDisposable));
            Helpers.AssertAllMembersThrowOnAccess<AccessedException<Unimplemented>, NoOverride>(finalImpl, customFilter: NoInner, typesToExclude: new[] {typeof(IAsyncDisposable), typeof(DownloadablePluginBaseTests.Unimplemented), typeof(ImageCollectionPluginBaseTests.Unimplemented)});

            Helpers.AssertAllThrowsOnMemberAccess<IAsyncDisposable>(finalImpl, customFilter: NoInner, expectedExceptions: new[]
            {
                typeof(AccessedException<PlayablePluginBaseTests.Unimplemented>),
                typeof(AccessedException<DownloadablePluginBaseTests.Unimplemented>),
                typeof(AccessedException<ImageCollectionPluginBaseTests.Unimplemented>),
            });
        }

        [TestMethod, Timeout(5000)]
        public void PluginFullyCustomWith_Downloadable_UrlCollection()
        {
            var builder = new Sdk.Plugins.PluginManager().ModelPlugins.Playable;
            var finalTestClass = new Unimplemented();
            builder.Add(x => new NoOverride(x)
            {
                InnerDownloadable = new DownloadablePluginBaseTests.Unimplemented(),
                InnerUrlCollection = new UrlCollectionPluginBaseTests.Unimplemented()
            });

            var finalImpl = builder.Execute(finalTestClass);

            Assert.AreNotSame(finalImpl, finalTestClass);
            Assert.IsInstanceOfType(finalImpl, typeof(NoOverride));
            Helpers.AssertAllMembersThrowOnAccess<AccessedException<DownloadablePluginBaseTests.Unimplemented>, DownloadablePluginBaseTests.Unimplemented>(finalImpl, customFilter: NoInner, typesToExclude: typeof(IAsyncDisposable));
            Helpers.AssertAllMembersThrowOnAccess<AccessedException<UrlCollectionPluginBaseTests.Unimplemented>, UrlCollectionPluginBaseTests.Unimplemented>(finalImpl, customFilter: NoInner, typesToExclude: typeof(IAsyncDisposable));
            Helpers.AssertAllMembersThrowOnAccess<AccessedException<Unimplemented>, NoOverride>(finalImpl, customFilter: NoInner, typesToExclude: new[] {typeof(IAsyncDisposable), typeof(DownloadablePluginBaseTests.Unimplemented), typeof(UrlCollectionPluginBaseTests.Unimplemented)});

            Helpers.AssertAllThrowsOnMemberAccess<IAsyncDisposable>(finalImpl, customFilter: NoInner, expectedExceptions: new[]
            {
                typeof(AccessedException<PlayablePluginBaseTests.Unimplemented>),
                typeof(AccessedException<DownloadablePluginBaseTests.Unimplemented>),
                typeof(AccessedException<UrlCollectionPluginBaseTests.Unimplemented>),
            });
        }

        [TestMethod, Timeout(5000)]
        public void PluginFullyCustomWith_Downloadable_ImageCollection_UrlCollection()
        {
            var builder = new Sdk.Plugins.PluginManager().ModelPlugins.Playable;

            var defaultImplementation = new Unimplemented();
            builder.Add(x => new NoOverride(x)
            {
                InnerDownloadable = new DownloadablePluginBaseTests.Unimplemented(),
                InnerImageCollection = new ImageCollectionPluginBaseTests.Unimplemented(),
                InnerUrlCollection = new UrlCollectionPluginBaseTests.Unimplemented(),
            });

            var finalImpl = builder.Execute(defaultImplementation);

            Assert.AreNotSame(finalImpl, defaultImplementation);
            Assert.IsInstanceOfType(finalImpl, typeof(NoOverride));
            Helpers.AssertAllMembersThrowOnAccess<AccessedException<DownloadablePluginBaseTests.Unimplemented>, DownloadablePluginBaseTests.Unimplemented>(finalImpl, customFilter: NoInner, typesToExclude: typeof(IAsyncDisposable));
            Helpers.AssertAllMembersThrowOnAccess<AccessedException<ImageCollectionPluginBaseTests.Unimplemented>, ImageCollectionPluginBaseTests.Unimplemented>(finalImpl, customFilter: NoInner, typesToExclude: typeof(IAsyncDisposable));
            Helpers.AssertAllMembersThrowOnAccess<AccessedException<UrlCollectionPluginBaseTests.Unimplemented>, UrlCollectionPluginBaseTests.Unimplemented>(finalImpl, customFilter: NoInner, typesToExclude: typeof(IAsyncDisposable));
            Helpers.AssertAllMembersThrowOnAccess<AccessedException<Unimplemented>, NoOverride>(finalImpl, customFilter: NoInner, typesToExclude: new[] {typeof(IAsyncDisposable), typeof(DownloadablePluginBaseTests.Unimplemented), typeof(ImageCollectionPluginBaseTests.Unimplemented), typeof(UrlCollectionPluginBaseTests.Unimplemented)});

            Helpers.AssertAllThrowsOnMemberAccess<IAsyncDisposable>(finalImpl, customFilter: NoInner, expectedExceptions: new[]
            {
                typeof(AccessedException<PlayablePluginBaseTests.Unimplemented>),
                typeof(AccessedException<DownloadablePluginBaseTests.Unimplemented>),
                typeof(AccessedException<ImageCollectionPluginBaseTests.Unimplemented>),
                typeof(AccessedException<UrlCollectionPluginBaseTests.Unimplemented>),
            });
        }

        [TestMethod, Timeout(5000)]
        public void PluginFullyCustomWith_ImageCollection_UrlCollection()
        {
            var builder = new Sdk.Plugins.PluginManager().ModelPlugins.Playable;
            var finalTestClass = new Unimplemented();
            builder.Add(x => new NoOverride(x)
            {
                InnerUrlCollection = new UrlCollectionPluginBaseTests.Unimplemented(),
                InnerImageCollection = new ImageCollectionPluginBaseTests.Unimplemented()
            });

            var finalImpl = builder.Execute(finalTestClass);

            Assert.AreNotSame(finalImpl, finalTestClass);
            Assert.IsInstanceOfType(finalImpl, typeof(NoOverride));
            Helpers.AssertAllMembersThrowOnAccess<AccessedException<UrlCollectionPluginBaseTests.Unimplemented>, UrlCollectionPluginBaseTests.Unimplemented>(finalImpl, customFilter: NoInner, typesToExclude: typeof(IAsyncDisposable));
            Helpers.AssertAllMembersThrowOnAccess<AccessedException<ImageCollectionPluginBaseTests.Unimplemented>, ImageCollectionPluginBaseTests.Unimplemented>(finalImpl, customFilter: NoInner, typesToExclude: typeof(IAsyncDisposable));
            Helpers.AssertAllMembersThrowOnAccess<AccessedException<Unimplemented>, NoOverride>(finalImpl, customFilter: NoInner, typesToExclude: new[] {typeof(IAsyncDisposable), typeof(UrlCollectionPluginBaseTests.Unimplemented), typeof(ImageCollectionPluginBaseTests.Unimplemented)});

            Helpers.AssertAllThrowsOnMemberAccess<IAsyncDisposable>(finalImpl, customFilter: NoInner, expectedExceptions: new[]
            {
                typeof(AccessedException<PlayablePluginBaseTests.Unimplemented>),
                typeof(AccessedException<UrlCollectionPluginBaseTests.Unimplemented>),
                typeof(AccessedException<ImageCollectionPluginBaseTests.Unimplemented>),
            });
        }

        internal class FullyCustom : PlayablePluginBase
        {
            public FullyCustom(IPlayable inner)
                : base(new ModelPluginMetadata("", nameof(FullyCustom), "", new Version()), inner)
            {
            }

            internal static AccessedException<FullyCustom> AccessedException { get; } = new AccessedException<FullyCustom>();

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

        internal class NoOverride : PlayablePluginBase
        {
            public NoOverride(IPlayable inner)
                : base(new ModelPluginMetadata("", nameof(NoOverride), "", new Version()), inner)
            {
            }
        }

        internal class Unimplemented : IPlayable
        {
            internal static AccessedException<Unimplemented> AccessedException { get; } = new AccessedException<Unimplemented>();

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