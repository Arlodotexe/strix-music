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
    public class ImageCollectionPluginBaseTests
    {
        [TestMethod, Timeout(1000)]
        public void NoPlugins()
        {
            var builder = new Sdk.Plugins.PluginManager().ModelPlugins.ImageCollection;
            var finalTestClass = new Unimplemented();

            var emptyChain = builder.Execute(finalTestClass);

            Assert.AreSame(emptyChain, finalTestClass);
            Helpers.AssertAllThrowsOnMemberAccess<AccessedException<Unimplemented>>(finalTestClass);
            Helpers.AssertAllThrowsOnMemberAccess<AccessedException<Unimplemented>>(emptyChain);
        }

        [TestMethod, Timeout(1000)]
        public void PluginNoOverride()
        {
            var builder = new Sdk.Plugins.PluginManager().ModelPlugins.ImageCollection;
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
            var builder = new Sdk.Plugins.PluginManager().ModelPlugins.ImageCollection;
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

        public class FullyCustom : ImageCollectionPluginBase
        {
            public FullyCustom(IImageCollection inner)
                : base(new ModelPluginMetadata("", nameof(FullyCustom), new Version()), inner)
            {
            }

            internal static AccessedException<FullyCustom> AccessedException { get; } = new();

            public override int TotalImageCount => throw AccessedException;

            public override IReadOnlyList<ICoreImageCollection> Sources => throw AccessedException;

            public override IReadOnlyList<ICore> SourceCores => throw AccessedException;

            public override event CollectionChangedEventHandler<IImage>? ImagesChanged { add => throw AccessedException; remove => throw AccessedException; }
            public override event EventHandler<int>? ImagesCountChanged { add => throw AccessedException; remove => throw AccessedException; }

            public override Task AddImageAsync(IImage image, int index) => throw AccessedException;

            public override ValueTask DisposeAsync() => throw AccessedException;

            public override bool Equals(ICoreImageCollection? other) => throw AccessedException;

            public override Task<IReadOnlyList<IImage>> GetImagesAsync(int limit, int offset) => throw AccessedException;

            public override Task<bool> IsAddImageAvailableAsync(int index) => throw AccessedException;

            public override Task<bool> IsRemoveImageAvailableAsync(int index) => throw AccessedException;

            public override Task RemoveImageAsync(int index) => throw AccessedException;
        }

        public class NoOverride : ImageCollectionPluginBase
        {
            public NoOverride(IImageCollection inner)
                : base(new ModelPluginMetadata("", nameof(NoOverride), new Version()), inner)
            {
            }
        }

        public class Unimplemented : IImageCollection
        {
            internal static AccessedException<Unimplemented> AccessedException { get; } = new();

            public int TotalImageCount => throw AccessedException;

            public IReadOnlyList<ICoreImageCollection> Sources => throw AccessedException;

            public IReadOnlyList<ICore> SourceCores => throw AccessedException;

            public event CollectionChangedEventHandler<IImage>? ImagesChanged { add => throw AccessedException; remove => throw AccessedException; }
            public event EventHandler<int>? ImagesCountChanged { add => throw AccessedException; remove => throw AccessedException; }

            public Task AddImageAsync(IImage image, int index) => throw AccessedException;

            public ValueTask DisposeAsync() => throw AccessedException;

            public bool Equals(ICoreImageCollection? other) => throw AccessedException;

            public Task<IReadOnlyList<IImage>> GetImagesAsync(int limit, int offset) => throw AccessedException;

            public Task<bool> IsAddImageAvailableAsync(int index) => throw AccessedException;

            public Task<bool> IsRemoveImageAvailableAsync(int index) => throw AccessedException;

            public Task RemoveImageAsync(int index) => throw AccessedException;
        }
    }
}
