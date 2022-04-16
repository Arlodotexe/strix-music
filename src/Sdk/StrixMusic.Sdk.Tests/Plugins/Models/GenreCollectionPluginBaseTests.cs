using Microsoft.VisualStudio.TestTools.UnitTesting;
using OwlCore.Events;
using StrixMusic.Sdk.Models;
using StrixMusic.Sdk.Plugins.Model;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using StrixMusic.Sdk.CoreModels;

namespace StrixMusic.Sdk.Tests.Plugins.Models
{
    [TestClass]
    public class GenreCollectionPluginBaseTests
    {
        private static bool NoInner(MemberInfo x) => !x.Name.Contains("Inner");
        private static bool NoInnerOrSources(MemberInfo x) => NoInner(x) && x.Name != "get_Sources" && x.Name != "get_SourceCores";

        [TestMethod, Timeout(1000)]
        public void NoPlugins()
        {
            var builder = new SdkModelPlugins().GenreCollection;
            var finalTestClass = new Unimplemented();

            var emptyChain = builder.Execute(finalTestClass);

            Assert.AreSame(emptyChain, finalTestClass);
            Helpers.AssertAllThrowsOnMemberAccess<AccessedException<Unimplemented>>(finalTestClass);
            Helpers.AssertAllThrowsOnMemberAccess<AccessedException<Unimplemented>>(emptyChain);
        }

        [TestMethod, Timeout(1000)]
        public void PluginNoOverride()
        {
            var builder = new SdkModelPlugins().GenreCollection;
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
            var builder = new SdkModelPlugins().GenreCollection;
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

        public class FullyCustom : GenreCollectionPluginBase
        {
            public FullyCustom(IGenreCollection inner)
                : base(new ModelPluginMetadata("", nameof(FullyCustom), "", new Version()), inner)
            {
            }

            internal static AccessedException<FullyCustom> AccessedException { get; } = new();

            public override int TotalGenreCount => throw AccessedException;

            public override event CollectionChangedEventHandler<IGenre>? GenresChanged { add => throw AccessedException; remove => throw AccessedException; }
            public override event EventHandler<int>? GenresCountChanged { add => throw AccessedException; remove => throw AccessedException; }

            public override Task AddGenreAsync(IGenre genre, int index, CancellationToken cancellationToken = default) => throw AccessedException;

            public override ValueTask DisposeAsync() => throw AccessedException;

            public override bool Equals(ICoreGenreCollection? other) => throw AccessedException;

            public override Task<IReadOnlyList<IGenre>> GetGenresAsync(int limit, int offset, CancellationToken cancellationToken = default) => throw AccessedException;

            public override Task<bool> IsAddGenreAvailableAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;

            public override Task<bool> IsRemoveGenreAvailableAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;

            public override Task RemoveGenreAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
        }

        public class NoOverride : GenreCollectionPluginBase
        {
            public NoOverride(IGenreCollection inner)
                : base(new ModelPluginMetadata("", nameof(NoOverride), "", new Version()), inner)
            {
            }
        }

        internal class NotBlockingDisposeAsync : GenreCollectionPluginBase
        {
            public NotBlockingDisposeAsync()
                : base(new ModelPluginMetadata("", nameof(NotBlockingDisposeAsync), "", new Version()), new Unimplemented())
            {
            }

            /// <inheritdoc />
            public override ValueTask DisposeAsync() => default;
        }

        public class Unimplemented : IGenreCollection
        {
            internal static AccessedException<Unimplemented> AccessedException { get; } = new();

            public int TotalGenreCount => throw AccessedException;

            public IReadOnlyList<ICoreGenreCollection> Sources => throw AccessedException;

            public IReadOnlyList<ICore> SourceCores => throw AccessedException;

            public event CollectionChangedEventHandler<IGenre>? GenresChanged { add => throw AccessedException; remove => throw AccessedException; }
            public event EventHandler<int>? GenresCountChanged { add => throw AccessedException; remove => throw AccessedException; }

            public Task AddGenreAsync(IGenre genre, int index, CancellationToken cancellationToken = default) => throw AccessedException;

            public ValueTask DisposeAsync() => throw AccessedException;

            public bool Equals(ICoreGenreCollection? other) => throw AccessedException;

            public Task<IReadOnlyList<IGenre>> GetGenresAsync(int limit, int offset, CancellationToken cancellationToken = default) => throw AccessedException;

            public Task<bool> IsAddGenreAvailableAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;

            public Task<bool> IsRemoveGenreAvailableAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;

            public Task RemoveGenreAsync(int index, CancellationToken cancellationToken = default) => throw AccessedException;
        }
    }
}
