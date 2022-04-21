using Microsoft.VisualStudio.TestTools.UnitTesting;
using StrixMusic.Sdk.Plugins.Model;
using System;
using System.Collections.Generic;
using System.Reflection;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;

namespace StrixMusic.Sdk.Tests.Plugins.Models
{
    [TestClass]
    public class LyricsPluginBaseTests
    {
        private static bool NoInner(MemberInfo x) => !x.Name.Contains("Inner");
        private static bool NoInnerOrSources(MemberInfo x) => NoInner(x) && x.Name != "get_Sources" && x.Name != "get_SourceCores";

        [TestMethod, Timeout(1000)]
        public void NoPlugins()
        {
            var builder = new SdkModelPlugin(SdkTestPluginMetadata.Metadata).Lyrics;
            var finalTestClass = new Unimplemented();

            var emptyChain = builder.Execute(finalTestClass);

            Assert.AreSame(emptyChain, finalTestClass);
            Helpers.AssertAllThrowsOnMemberAccess<AccessedException<Unimplemented>>(finalTestClass);
            Helpers.AssertAllThrowsOnMemberAccess<AccessedException<Unimplemented>>(emptyChain);
        }

        [TestMethod, Timeout(1000)]
        public void PluginNoOverride()
        {
            var builder = new SdkModelPlugin(SdkTestPluginMetadata.Metadata).Lyrics;
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
            var builder = new SdkModelPlugin(SdkTestPluginMetadata.Metadata).Lyrics;
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

        public class FullyCustom : LyricsPluginBase
        {
            public FullyCustom(ILyrics inner)
                : base(new ModelPluginMetadata("", nameof(FullyCustom), "", new Version()), inner)
            {
            }

            internal static AccessedException<FullyCustom> AccessedException { get; } = new();

            public override ITrack Track => throw AccessedException;

            public override Dictionary<TimeSpan, string>? TimedLyrics => throw AccessedException;

            public override string? TextLyrics => throw AccessedException;

            public override bool Equals(ICoreLyrics? other)
            {
                throw AccessedException;
            }
        }

        public class NoOverride : LyricsPluginBase
        {
            public NoOverride(ILyrics inner)
                : base(new ModelPluginMetadata("", nameof(NoOverride), "", new Version()), inner)
            {
            }
        }

        public class Unimplemented : ILyrics
        {
            internal static AccessedException<Unimplemented> AccessedException { get; } = new();

            public ITrack Track => throw AccessedException;

            public Dictionary<TimeSpan, string>? TimedLyrics => throw AccessedException;

            public string? TextLyrics => throw AccessedException;

            public IReadOnlyList<ICoreLyrics> Sources => throw AccessedException;

            public IReadOnlyList<ICore> SourceCores => throw AccessedException;

            public bool Equals(ICoreLyrics? other)
            {
                throw AccessedException;
            }
        }
    }
}
