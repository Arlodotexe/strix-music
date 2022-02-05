using Microsoft.VisualStudio.TestTools.UnitTesting;
using OwlCore.Events;
using StrixMusic.Sdk.Models;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Plugins.Model;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Tests.Plugins.Models
{
    [TestClass]
    public class GenreCollectionPluginBaseTests
    {
        [TestMethod, Timeout(1000)]
        public void NoPlugins()
        {
            var builder = new Sdk.Plugins.PluginManager().ModelPlugins.GenreCollection;
            var finalTestClass = new Unimplemented();

            var emptyChain = builder.Execute(finalTestClass);

            Assert.AreSame(emptyChain, finalTestClass);
            Helpers.AssertAllThrowsOnMemberAccess<AccessedException<Unimplemented>>(finalTestClass);
            Helpers.AssertAllThrowsOnMemberAccess<AccessedException<Unimplemented>>(emptyChain);
        }

        [TestMethod, Timeout(1000)]
        public void PluginNoOverride()
        {
            var builder = new Sdk.Plugins.PluginManager().ModelPlugins.GenreCollection;
            var finalTestClass = new Unimplemented();

            var emptyChain = builder.Execute(finalTestClass);

            Assert.AreSame(emptyChain, finalTestClass);
            Helpers.AssertAllThrowsOnMemberAccess<AccessedException<Unimplemented>>(finalTestClass);
            Helpers.AssertAllThrowsOnMemberAccess<AccessedException<Unimplemented>>(emptyChain);

            builder.Add(x => new NoOverride(x));
            var noOverride = builder.Execute(finalTestClass);

            Assert.AreNotSame(noOverride, emptyChain);
            Assert.AreNotSame(noOverride, finalTestClass);
            Helpers.AssertAllThrowsOnMemberAccess<AccessedException<Unimplemented>>(noOverride, customFilter: x => !x.Name.Contains("Inner"));
        }

        [TestMethod, Timeout(1000)]
        public void PluginFullyCustom()
        {
            var builder = new Sdk.Plugins.PluginManager().ModelPlugins.GenreCollection;
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
            Helpers.AssertAllThrowsOnMemberAccess<AccessedException<Unimplemented>>(noOverride, customFilter: x => !x.Name.Contains("Inner"));

            // Fully custom
            builder.Add(x => new FullyCustom(x));
            var allCustom = builder.Execute(finalTestClass);

            Assert.AreNotSame(noOverride, emptyChain);
            Assert.AreNotSame(noOverride, finalTestClass);
            Helpers.AssertAllThrowsOnMemberAccess<AccessedException<FullyCustom>>(allCustom, customFilter: x => !x.Name.Contains("Inner"));
        }

        public class FullyCustom : GenreCollectionPluginBase
        {
            public FullyCustom(IGenreCollection inner)
                : base(new ModelPluginMetadata("", nameof(FullyCustom), new Version()), inner)
            {
            }

            internal static AccessedException<FullyCustom> AccessedException { get; } = new();

            public override int TotalGenreCount => throw AccessedException;

            public override IReadOnlyList<ICoreGenreCollection> Sources => throw AccessedException;

            public override IReadOnlyList<ICore> SourceCores => throw AccessedException;

            public override event CollectionChangedEventHandler<IGenre>? GenresChanged { add => throw AccessedException; remove => throw AccessedException; }
            public override event EventHandler<int>? GenresCountChanged { add => throw AccessedException; remove => throw AccessedException; }

            public override Task AddGenreAsync(IGenre Genre, int index) => throw AccessedException;

            public override ValueTask DisposeAsync() => throw AccessedException;

            public override bool Equals(ICoreGenreCollection? other) => throw AccessedException;

            public override Task<IReadOnlyList<IGenre>> GetGenresAsync(int limit, int offset) => throw AccessedException;

            public override Task<bool> IsAddGenreAvailableAsync(int index) => throw AccessedException;

            public override Task<bool> IsRemoveGenreAvailableAsync(int index) => throw AccessedException;

            public override Task RemoveGenreAsync(int index) => throw AccessedException;
        }

        public class NoOverride : GenreCollectionPluginBase
        {
            public NoOverride(IGenreCollection inner)
                : base(new ModelPluginMetadata("", nameof(NoOverride), new Version()), inner)
            {
            }
        }

        public class Unimplemented : IGenreCollection
        {
            internal static AccessedException<Unimplemented> AccessedException { get; } = new();

            public int TotalGenreCount => throw AccessedException;

            public IReadOnlyList<ICoreGenreCollection> Sources => throw AccessedException;

            public IReadOnlyList<ICore> SourceCores => throw AccessedException;

            public event CollectionChangedEventHandler<IGenre>? GenresChanged { add => throw AccessedException; remove => throw AccessedException; }
            public event EventHandler<int>? GenresCountChanged { add => throw AccessedException; remove => throw AccessedException; }

            public Task AddGenreAsync(IGenre Genre, int index) => throw AccessedException;

            public ValueTask DisposeAsync() => throw AccessedException;

            public bool Equals(ICoreGenreCollection? other) => throw AccessedException;

            public Task<IReadOnlyList<IGenre>> GetGenresAsync(int limit, int offset) => throw AccessedException;

            public Task<bool> IsAddGenreAvailableAsync(int index) => throw AccessedException;

            public Task<bool> IsRemoveGenreAvailableAsync(int index) => throw AccessedException;

            public Task RemoveGenreAsync(int index) => throw AccessedException;
        }
    }
}
