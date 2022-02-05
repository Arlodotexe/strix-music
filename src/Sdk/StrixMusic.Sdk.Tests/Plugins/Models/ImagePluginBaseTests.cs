using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    public class ImagePluginBaseTests
    {
        [TestMethod, Timeout(1000)]
        public void NoPlugins()
        {
            var builder = new Sdk.Plugins.PluginManager().ModelPlugins.Image;
            var finalTestClass = new Unimplemented();

            var emptyChain = builder.Execute(finalTestClass);

            Assert.AreSame(emptyChain, finalTestClass);
            Helpers.AssertAllThrowsOnMemberAccess<AccessedException<Unimplemented>>(finalTestClass);
            Helpers.AssertAllThrowsOnMemberAccess<AccessedException<Unimplemented>>(emptyChain);
        }

        [TestMethod, Timeout(1000)]
        public void PluginNoOverride()
        {
            var builder = new Sdk.Plugins.PluginManager().ModelPlugins.Image;
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
            var builder = new Sdk.Plugins.PluginManager().ModelPlugins.Image;
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

        public class FullyCustom : ImagePluginBase
        {
            public FullyCustom(IImage inner)
                : base(new ModelPluginMetadata("", nameof(FullyCustom), new Version()), inner)
            {
            }

            internal static AccessedException<FullyCustom> AccessedException { get; } = new();

            public override Uri Uri => throw AccessedException;

            public override double Height => throw AccessedException;

            public override double Width => throw AccessedException;

            public override IReadOnlyList<ICoreImage> Sources => throw AccessedException;

            public override IReadOnlyList<ICore> SourceCores => throw AccessedException;

            public override ValueTask DisposeAsync() => throw AccessedException;

            public override bool Equals(ICoreImage? other) => throw AccessedException;
        }

        public class NoOverride : ImagePluginBase
        {
            public NoOverride(IImage inner)
                : base(new ModelPluginMetadata("", nameof(NoOverride), new Version()), inner)
            {
            }
        }

        public class Unimplemented : IImage
        {
            internal static AccessedException<Unimplemented> AccessedException { get; } = new();

            public Uri Uri => throw AccessedException;

            public double Height => throw AccessedException;

            public double Width => throw AccessedException;

            public IReadOnlyList<ICoreImage> Sources => throw AccessedException;

            public IReadOnlyList<ICore> SourceCores => throw AccessedException;

            public ValueTask DisposeAsync() => throw AccessedException;

            public bool Equals(ICoreImage? other) => throw AccessedException;
        }
    }
}
