using Microsoft.VisualStudio.TestTools.UnitTesting;
using OwlCore.Events;
using StrixMusic.Sdk.Plugins.Model;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;

namespace StrixMusic.Sdk.Tests.Plugins.Models
{
    [TestClass]
    public class UrlCollectionPluginBaseTests
    {
        private static bool NoInner(MemberInfo x) => !x.Name.Contains("Inner");
        private static bool NoInnerOrSources(MemberInfo x) => NoInner(x) && !x.Name.ToLower().Contains("sources");

        [TestMethod, Timeout(1000)]
        public void NoPlugins()
        {
            var builder = new SdkModelPlugin(SdkTestPluginMetadata.Metadata).UrlCollection;
            var finalTestClass = new Unimplemented();

            var emptyChain = builder.Execute(finalTestClass);

            Assert.AreSame(emptyChain, finalTestClass);
            Helpers.AssertAllThrowsOnMemberAccess<AccessedException<Unimplemented>>(finalTestClass);
            Helpers.AssertAllThrowsOnMemberAccess<AccessedException<Unimplemented>>(emptyChain);
        }

        [TestMethod, Timeout(1000)]
        public void PluginNoOverride()
        {
            var builder = new SdkModelPlugin(SdkTestPluginMetadata.Metadata).UrlCollection;
            var finalTestClass = new Unimplemented();

            var emptyChain = builder.Execute(finalTestClass);

            Assert.AreSame(emptyChain, finalTestClass);
            Helpers.AssertAllThrowsOnMemberAccess<AccessedException<Unimplemented>>(finalTestClass);
            Helpers.AssertAllThrowsOnMemberAccess<AccessedException<Unimplemented>>(emptyChain);

            builder.Add(x => new NoOverride(x));
            var noOverride = builder.Execute(finalTestClass);

            Assert.AreNotSame(noOverride, emptyChain);
            Assert.AreNotSame(noOverride, finalTestClass);
            Helpers.AssertAllThrowsOnMemberAccess<AccessedException<Unimplemented>>(noOverride, customFilter: NoInner);
        }

        [TestMethod, Timeout(1000)]
        public void PluginFullyCustom()
        {
            var builder = new SdkModelPlugin(SdkTestPluginMetadata.Metadata).UrlCollection;
            var finalTestClass = new Unimplemented();

            var emptyChain = builder.Execute(finalTestClass);

            Assert.AreSame(emptyChain, finalTestClass);
            Helpers.AssertAllThrowsOnMemberAccess<AccessedException<Unimplemented>>(finalTestClass);
            Helpers.AssertAllThrowsOnMemberAccess<AccessedException<Unimplemented>>(emptyChain);

            // No override
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

        public class FullyCustom : UrlCollectionPluginBase
        {
            public FullyCustom(IUrlCollection inner)
                : base(new ModelPluginMetadata("", nameof(FullyCustom), "", new Version()), inner)
            {
            }

            internal static AccessedException<FullyCustom> AccessedException { get; } = new();

            public override int TotalUrlCount => throw AccessedException;

            public override event CollectionChangedEventHandler<IUrl>? UrlsChanged { add => throw AccessedException; remove => throw AccessedException; }
            public override event EventHandler<int>? UrlsCountChanged { add => throw AccessedException; remove => throw AccessedException; }

            public override Task AddUrlAsync(IUrl url, int index, CancellationToken cancellationToken = default) => throw AccessedException;

            public override ValueTask DisposeAsync() => throw AccessedException;

            public override bool Equals(ICoreUrlCollection? other) => throw AccessedException;

            public override IAsyncEnumerable<IUrl> GetUrlsAsync(int limit, int offset, CancellationToken cancellationToken = default) => throw AccessedException;

            public override Task<bool> IsAddUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;

            public override Task<bool> IsRemoveUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;

            public override Task RemoveUrlAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
        }

        public class NoOverride : UrlCollectionPluginBase
        {
            public NoOverride(IUrlCollection inner)
                : base(new ModelPluginMetadata("", nameof(NoOverride), "", new Version()), inner)
            {
            }
        }

        internal class NotBlockingDisposeAsync : UrlCollectionPluginBase
        {
            public NotBlockingDisposeAsync()
                : base(new ModelPluginMetadata("", nameof(NotBlockingDisposeAsync), "", new Version()), new Unimplemented())
            {
            }

            /// <inheritdoc />
            public override ValueTask DisposeAsync() => default;
        }

        public class Unimplemented : IUrlCollection
        {
            internal static AccessedException<Unimplemented> AccessedException { get; } = new();

            public event EventHandler? SourcesChanged { add => throw AccessedException; remove => throw AccessedException; }
            public int TotalUrlCount => throw AccessedException;

            public IReadOnlyList<ICoreUrlCollection> Sources => throw AccessedException;

            public event CollectionChangedEventHandler<IUrl>? UrlsChanged { add => throw AccessedException; remove => throw AccessedException; }
            public event EventHandler<int>? UrlsCountChanged { add => throw AccessedException; remove => throw AccessedException; }

            public Task AddUrlAsync(IUrl url, int index, CancellationToken cancellationToken = default) => throw AccessedException;

            public ValueTask DisposeAsync() => throw AccessedException;

            public bool Equals(ICoreUrlCollection? other) => throw AccessedException;

            public IAsyncEnumerable<IUrl> GetUrlsAsync(int limit, int offset, CancellationToken cancellationToken = default) => throw AccessedException;

            public Task<bool> IsAddUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;

            public Task<bool> IsRemoveUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;

            public Task RemoveUrlAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
        }
    }
}
