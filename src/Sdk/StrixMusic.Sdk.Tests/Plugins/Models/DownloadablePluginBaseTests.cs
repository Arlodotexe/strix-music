using Microsoft.VisualStudio.TestTools.UnitTesting;
using StrixMusic.Sdk.Models;
using StrixMusic.Sdk.Plugins.Model;
using System;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Tests.Plugins.Models
{
    [TestClass]
    public class DownloadablePluginBaseTests
    {
        [TestMethod, Timeout(1000)]
        public void NoPlugins()
        {
            var builder = new SdkModelPlugins().Downloadable;
            var finalTestClass = new Unimplemented();

            var emptyChain = builder.Execute(finalTestClass);

            Assert.AreSame(emptyChain, finalTestClass);
            Helpers.AssertAllThrowsOnMemberAccess<AccessedException<Unimplemented>>(finalTestClass);
            Helpers.AssertAllThrowsOnMemberAccess<AccessedException<Unimplemented>>(emptyChain);
        }

        [TestMethod, Timeout(1000)]
        public void PluginNoOverride()
        {
            var builder = new SdkModelPlugins().Downloadable;
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
            var builder = new SdkModelPlugins().Downloadable;
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

        public class FullyCustom : DownloadablePluginBase
        {
            public FullyCustom(IDownloadable inner)
                : base(new ModelPluginMetadata("", nameof(FullyCustom), "", new Version()), inner)
            {
            }

            internal static AccessedException<FullyCustom> AccessedException { get; } = new();

            public override DownloadInfo DownloadInfo => throw AccessedException;
            public override event EventHandler<DownloadInfo>? DownloadInfoChanged { add => throw AccessedException; remove => throw AccessedException; }
            public override Task StartDownloadOperationAsync(DownloadOperation operation) => throw AccessedException;
            public override ValueTask DisposeAsync() => throw AccessedException;
        }

        public class NoOverride : DownloadablePluginBase
        {
            public NoOverride(IDownloadable inner)
                : base(new ModelPluginMetadata("", nameof(NoOverride), "", new Version()), inner)
            {
            }
        }

        internal class NotBlockingDisposeAsync : DownloadablePluginBase
        {
            public NotBlockingDisposeAsync()
                : base(new ModelPluginMetadata("", nameof(NotBlockingDisposeAsync), "", new Version()), new Unimplemented())
            {
            }

            /// <inheritdoc />
            public override ValueTask DisposeAsync() => default;
        }

        public class Unimplemented : IDownloadable
        {
            internal static AccessedException<Unimplemented> AccessedException { get; } = new();

            public DownloadInfo DownloadInfo => throw AccessedException;
            public event EventHandler<DownloadInfo>? DownloadInfoChanged { add => throw AccessedException; remove => throw AccessedException; }

            public ValueTask DisposeAsync() => throw AccessedException;

            public Task StartDownloadOperationAsync(DownloadOperation operation) => throw AccessedException;
        }
    }
}
