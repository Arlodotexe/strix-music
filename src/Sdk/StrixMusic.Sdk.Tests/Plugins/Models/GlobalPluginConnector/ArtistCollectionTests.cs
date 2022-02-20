using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;
using StrixMusic.Sdk.Models.Base;

namespace StrixMusic.Sdk.Tests.Plugins.Models.GlobalModelPluginConnector
{
    [TestClass]
    public class ArtistCollectionTests
    {
        private static bool NoInner(MemberInfo x) => !x.Name.Contains("Inner");
        private static bool NoInnerOrSources(MemberInfo x) => NoInner(x) && x.Name != "get_Sources" && x.Name != "get_SourceCores";

        [TestMethod]
        public void AccessedThroughAlbum()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugins();
            plugins.ArtistCollection.Add(x => new ArtistCollectionPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).Album.Execute(new AlbumPluginBaseTests.Unimplemented());

            Helpers.AssertAllMembersThrowOnAccess<AccessedException<ArtistCollectionPluginBaseTests.FullyCustom>, ArtistCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources,
                typesToExclude: new[]
                {
                    typeof(IAsyncDisposable),
                    typeof(IPlayableCollectionItem)
                });
        }

        [TestMethod]
        public void NotBlockingAlbum()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugins();
            plugins.ArtistCollection.Add(x => new ArtistCollectionPluginBaseTests.FullyCustom(x));
            plugins.Album.Add(x => new AlbumPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).Album.Execute(new AlbumPluginBaseTests.Unimplemented());

            // Ensure an Album plugin can still override ArtistCollection members.
            Helpers.AssertAllMembersThrowOnAccess<AccessedException<AlbumPluginBaseTests.FullyCustom>, ArtistCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources,
                typesToExclude: typeof(IAsyncDisposable));
        }

        [TestMethod]
        public void DisposingAlbum()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugins();
            plugins.ArtistCollection.Add(x => new ArtistCollectionPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).Album.Execute(new AlbumPluginBaseTests.Unimplemented());

            Helpers.AssertAllThrowsOnMemberAccess<IAsyncDisposable>(value: plugin, expectedExceptions: new[]
            {
                typeof(AccessedException<ArtistCollectionPluginBaseTests.FullyCustom>),
                typeof(AccessedException<AlbumPluginBaseTests.Unimplemented>),
            });
        }

        [TestMethod]
        public void AccessedThroughTrack()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugins();
            plugins.ArtistCollection.Add(x => new ArtistCollectionPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).Track.Execute(new TrackPluginBaseTests.Unimplemented());

            Helpers.AssertAllMembersThrowOnAccess<AccessedException<ArtistCollectionPluginBaseTests.FullyCustom>, ArtistCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources,
                typesToExclude: new[]
                {
                    typeof(IAsyncDisposable),
                    typeof(IPlayableCollectionItem)
                });
        }

        [TestMethod]
        public void NotBlockingTrack()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugins();
            plugins.ArtistCollection.Add(x => new ArtistCollectionPluginBaseTests.FullyCustom(x));
            plugins.Track.Add(x => new TrackPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).Track.Execute(new TrackPluginBaseTests.Unimplemented());

            // Ensure an Track plugin can still override ArtistCollection members.
            Helpers.AssertAllMembersThrowOnAccess<AccessedException<TrackPluginBaseTests.FullyCustom>, ArtistCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources,
                typesToExclude: typeof(IAsyncDisposable));
        }

        [TestMethod]
        public void DisposingTrack()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugins();
            plugins.ArtistCollection.Add(x => new ArtistCollectionPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).Track.Execute(new TrackPluginBaseTests.Unimplemented());

            Helpers.AssertAllThrowsOnMemberAccess<IAsyncDisposable>(value: plugin, expectedExceptions: new[]
            {
                typeof(AccessedException<ArtistCollectionPluginBaseTests.FullyCustom>),
                typeof(AccessedException<TrackPluginBaseTests.Unimplemented>),
            });
        }
    }
}
