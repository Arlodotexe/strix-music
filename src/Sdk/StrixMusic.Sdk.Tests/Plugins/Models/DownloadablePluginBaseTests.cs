using Microsoft.VisualStudio.TestTools.UnitTesting;
using OwlCore.ComponentModel;
using StrixMusic.Sdk.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Tests.Plugins.Models
{
    [TestClass]
    public class DownloadablePluginBaseTests
    {

        [TestMethod, Timeout(1000)]
        public void NoPlugins()
        {
            var builder = new Sdk.Plugins.PluginManager().ModelPlugins.Downloadable;
            var finalTestClass = new UnimplementedDownloadable();

            var emptyChain = builder.Execute(finalTestClass);

            Assert.AreSame(emptyChain, finalTestClass);
            Helpers.AssertAllThrowsOnMemberAccess<AccessedException<UnimplementedDownloadable>>(finalTestClass);
            Helpers.AssertAllThrowsOnMemberAccess<AccessedException<UnimplementedDownloadable>>(emptyChain);
        }

        [TestMethod, Timeout(1000)]
        public void PluginNoOverride()
        {
            var builder = new Sdk.Plugins.PluginManager().ModelPlugins.Downloadable;
            var finalTestClass = new UnimplementedDownloadable();

            var emptyChain = builder.Execute(finalTestClass);

            Assert.AreSame(emptyChain, finalTestClass);
            Helpers.AssertAllThrowsOnMemberAccess<AccessedException<UnimplementedDownloadable>>(finalTestClass);
            Helpers.AssertAllThrowsOnMemberAccess<AccessedException<UnimplementedDownloadable>>(emptyChain);

            builder.Add(x => new NoOverrideCustomDownloader(x));
            var noOverride = builder.Execute(finalTestClass);

            Assert.AreNotSame(noOverride, emptyChain);
            Assert.AreNotSame(noOverride, finalTestClass);
            Helpers.AssertAllThrowsOnMemberAccess<AccessedException<UnimplementedDownloadable>>(noOverride, customFilter: x => !x.Name.Contains("Inner"));
        }

        [TestMethod, Timeout(1000)]
        public void PluginFullyCustom()
        {
            var builder = new Sdk.Plugins.PluginManager().ModelPlugins.Downloadable;
            var finalTestClass = new UnimplementedDownloadable();

            var emptyChain = builder.Execute(finalTestClass);

            Assert.AreSame(emptyChain, finalTestClass);
            Helpers.AssertAllThrowsOnMemberAccess<AccessedException<UnimplementedDownloadable>>(finalTestClass);
            Helpers.AssertAllThrowsOnMemberAccess<AccessedException<UnimplementedDownloadable>>(emptyChain);

            // No override
            builder.Add(x => new NoOverrideCustomDownloader(x));
            var noOverride = builder.Execute(finalTestClass);

            Assert.AreNotSame(noOverride, emptyChain);
            Assert.AreNotSame(noOverride, finalTestClass);
            Helpers.AssertAllThrowsOnMemberAccess<AccessedException<UnimplementedDownloadable>>(noOverride, customFilter: x => !x.Name.Contains("Inner"));

            // Fully custom
            builder.Add(x => new FullyCustomDownloader(x));
            var allCustom = builder.Execute(finalTestClass);

            Assert.AreNotSame(noOverride, emptyChain);
            Assert.AreNotSame(noOverride, finalTestClass);
            Helpers.AssertAllThrowsOnMemberAccess<AccessedException<FullyCustomDownloader>>(allCustom, customFilter: x => !x.Name.Contains("Inner"));
        }

        public class FullyCustomDownloader : Sdk.Plugins.Model.DownloadablePluginBase
        {
            public FullyCustomDownloader(IDownloadable inner)
                : base(inner)
            {
            }

            internal static AccessedException<FullyCustomDownloader> AccessedException { get; } = new();

            public override DownloadInfo DownloadInfo => throw AccessedException;
            public override event EventHandler<DownloadInfo>? DownloadInfoChanged { add => throw AccessedException; remove => throw AccessedException; }
            public override Task StartDownloadOperationAsync(DownloadOperation operation) => throw AccessedException;
        }

        public class NoOverrideCustomDownloader : Sdk.Plugins.Model.DownloadablePluginBase
        {
            public NoOverrideCustomDownloader(IDownloadable inner)
                : base(inner)
            {
            }
        }

        public class UnimplementedDownloadable : IDownloadable
        {
            internal static AccessedException<UnimplementedDownloadable> AccessedException { get; } = new();

            public DownloadInfo DownloadInfo => throw AccessedException;
            public event EventHandler<DownloadInfo>? DownloadInfoChanged { add => throw AccessedException; remove => throw AccessedException; }
            public Task StartDownloadOperationAsync(DownloadOperation operation) => throw AccessedException;
        }
    }
}
