using Microsoft.VisualStudio.TestTools.UnitTesting;
using StrixMusic.Sdk.Plugins.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Sdk.Tests.Mock.AppModels;

namespace StrixMusic.Sdk.Tests.Plugins.Models
{
    [TestClass]
    public class ImagePluginBaseTests
    {
        private static bool NoInner(MemberInfo x) => !x.Name.Contains("Inner");
        private static bool NoInnerOrSources(MemberInfo x) => NoInner(x) && !x.Name.ToLower().Contains("sources");

        [TestMethod, Timeout(5000)]
        public void NoPlugins()
        {
            var builder = new SdkModelPlugin(SdkTestPluginMetadata.Metadata).Image;
            var finalTestClass = new Unimplemented();

            var emptyChain = builder.Execute(finalTestClass);

            Assert.AreSame(emptyChain, finalTestClass);
            Helpers.AssertAllThrowsOnMemberAccess<AccessedException<Unimplemented>>(finalTestClass);
            Helpers.AssertAllThrowsOnMemberAccess<AccessedException<Unimplemented>>(emptyChain);
        }

        [TestMethod, Timeout(5000)]
        public void PluginNoOverride()
        {
            var builder = new SdkModelPlugin(SdkTestPluginMetadata.Metadata).Image;
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

        [TestMethod, Timeout(5000)]
        public void PluginFullyCustom()
        {
            var builder = new SdkModelPlugin(SdkTestPluginMetadata.Metadata).Image;
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

        public class FullyCustom : ImagePluginBase
        {
            public FullyCustom(IImage inner)
                : base(new ModelPluginMetadata("", nameof(FullyCustom), "", new Version()), inner, new MockStrixDataRoot())
            {
            }

            internal static AccessedException<FullyCustom> AccessedException { get; } = new();

            public override Task<Stream> OpenStreamAsync() => throw AccessedException;

            public override string? MimeType => throw AccessedException;

            public override double? Height => throw AccessedException;

            public override double? Width => throw AccessedException;

            public override bool Equals(ICoreImage? other) => throw AccessedException;
        }

        public class NoOverride : ImagePluginBase
        {
            public NoOverride(IImage inner)
                : base(new ModelPluginMetadata("", nameof(NoOverride), "", new Version()), inner, new MockStrixDataRoot())
            {
            }
        }

        public class Unimplemented : IImage
        {
            internal static AccessedException<Unimplemented> AccessedException { get; } = new();

            public event EventHandler? SourcesChanged { add => throw AccessedException; remove => throw AccessedException; }

            public Task<Stream> OpenStreamAsync() => throw AccessedException;

            public string? MimeType => throw AccessedException;
            
            public double? Height => throw AccessedException;

            public double? Width => throw AccessedException;

            public IReadOnlyList<ICoreImage> Sources => throw AccessedException;

            public bool Equals(ICoreImage? other) => throw AccessedException;
            public IStrixDataRoot Root  => throw AccessedException;
        }
    }
}
