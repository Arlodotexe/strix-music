using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;

namespace StrixMusic.Sdk.Tests.Plugins.Models.GlobalPluginConnector
{
    [TestClass]
    public class ImageCollectionTests
    {
        static bool NoInner(MemberInfo x) => !x.Name.Contains("Inner");
        static bool NoInnerOrSources(MemberInfo x) => NoInner(x) && x.Name != "get_Sources" && x.Name != "get_SourceCores";

        [TestMethod]
        public void AccessedThroughPlayable()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugins();
            plugins.ImageCollection.Insert(0, x => new ImageCollectionPluginBaseTests.FullyCustom(x));

            var plugin = plugins.Playable.Execute(new PlayablePluginBaseTests.Unimplemented());

            Helpers.AssertAllMembersThrowOnAccess<AccessedException<ImageCollectionPluginBaseTests.FullyCustom>, ImageCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                typesToExclude: typeof(IAsyncDisposable));
        }

        [TestMethod]
        public void NotBlockingPlayable()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugins();
            plugins.ImageCollection.Insert(0, x => new ImageCollectionPluginBaseTests.FullyCustom(x));
            plugins.Playable.Insert(0, x => new PlayablePluginBaseTests.FullyCustom(x));

            var plugin = plugins.Playable.Execute(new PlayablePluginBaseTests.Unimplemented());

            // Ensure a Playable plugin can still override ImageCollection members.
            Helpers.AssertAllMembersThrowOnAccess<AccessedException<PlayablePluginBaseTests.FullyCustom>, ImageCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                typesToExclude: typeof(IAsyncDisposable));
        }

        [TestMethod]
        public void DisposingPlayable()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugins();
            plugins.ImageCollection.Insert(0, x => new ImageCollectionPluginBaseTests.FullyCustom(x));

            var plugin = plugins.Playable.Execute(new PlayablePluginBaseTests.Unimplemented());

            Helpers.AssertAllThrowsOnMemberAccess<IAsyncDisposable>(plugin, expectedExceptions: new[] {
                typeof(AccessedException<ImageCollectionPluginBaseTests.FullyCustom>),
                typeof(AccessedException<PlayablePluginBaseTests.Unimplemented>),
            });
        }

        [TestMethod]
        public void AccessedThroughTrackCollection()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugins();
            plugins.ImageCollection.Insert(0, x => new ImageCollectionPluginBaseTests.FullyCustom(x));

            var plugin = plugins.TrackCollection.Execute(new TrackCollectionPluginBaseTests.Unimplemented());

            Helpers.AssertAllMembersThrowOnAccess<AccessedException<ImageCollectionPluginBaseTests.FullyCustom>, ImageCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources,
                typesToExclude: typeof(IAsyncDisposable));
        }

        [TestMethod]
        public void NotBlockingTrackCollection()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugins();
            plugins.ImageCollection.Insert(0, x => new ImageCollectionPluginBaseTests.FullyCustom(x));
            plugins.TrackCollection.Insert(0, x => new TrackCollectionPluginBaseTests.FullyCustom(x));

            var plugin = plugins.TrackCollection.Execute(new TrackCollectionPluginBaseTests.Unimplemented());

            // Ensure a TrackCollection plugin can still override ImageCollection members.
            Helpers.AssertAllMembersThrowOnAccess<AccessedException<TrackCollectionPluginBaseTests.FullyCustom>, ImageCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources,
                typesToExclude: typeof(IAsyncDisposable));
        }

        [TestMethod]
        public void DisposingTrackCollection()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugins();
            plugins.ImageCollection.Insert(0, x => new ImageCollectionPluginBaseTests.FullyCustom(x));

            var plugin = plugins.TrackCollection.Execute(new TrackCollectionPluginBaseTests.Unimplemented());

            Helpers.AssertAllThrowsOnMemberAccess<IAsyncDisposable>(plugin, customFilter: NoInnerOrSources, expectedExceptions: new[] {
                typeof(AccessedException<ImageCollectionPluginBaseTests.FullyCustom>),
                typeof(AccessedException<TrackCollectionPluginBaseTests.Unimplemented>),
            });
        }

        [TestMethod]
        public void AccessedThroughArtistCollection()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugins();
            plugins.ImageCollection.Insert(0, x => new ImageCollectionPluginBaseTests.FullyCustom(x));

            var plugin = plugins.ArtistCollection.Execute(new ArtistCollectionPluginBaseTests.Unimplemented());

            Helpers.AssertAllMembersThrowOnAccess<AccessedException<ImageCollectionPluginBaseTests.FullyCustom>, ImageCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources,
                typesToExclude: typeof(IAsyncDisposable));
        }

        [TestMethod]
        public void NotBlockingArtistCollection()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugins();
            plugins.ImageCollection.Insert(0, x => new ImageCollectionPluginBaseTests.FullyCustom(x));
            plugins.ArtistCollection.Insert(0, x => new ArtistCollectionPluginBaseTests.FullyCustom(x));

            var plugin = plugins.ArtistCollection.Execute(new ArtistCollectionPluginBaseTests.Unimplemented());

            // Ensure an ArtistCollection plugin can still override ImageCollection members.
            Helpers.AssertAllMembersThrowOnAccess<AccessedException<ArtistCollectionPluginBaseTests.FullyCustom>, ImageCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources,
                typesToExclude: typeof(IAsyncDisposable));
        }

        [TestMethod]
        public void DisposingArtistCollection()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugins();
            plugins.ImageCollection.Insert(0, x => new ImageCollectionPluginBaseTests.FullyCustom(x));

            var plugin = plugins.ArtistCollection.Execute(new ArtistCollectionPluginBaseTests.Unimplemented());

            Helpers.AssertAllThrowsOnMemberAccess<IAsyncDisposable>(plugin, customFilter: NoInnerOrSources, expectedExceptions: new[] {
                typeof(AccessedException<ImageCollectionPluginBaseTests.FullyCustom>),
                typeof(AccessedException<ArtistCollectionPluginBaseTests.Unimplemented>),
            });
        }

        [TestMethod]
        public void AccessedThroughAlbumCollection()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugins();
            plugins.ImageCollection.Insert(0, x => new ImageCollectionPluginBaseTests.FullyCustom(x));

            var plugin = plugins.AlbumCollection.Execute(new AlbumCollectionPluginBaseTests.Unimplemented());

            Helpers.AssertAllMembersThrowOnAccess<AccessedException<ImageCollectionPluginBaseTests.FullyCustom>, ImageCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources,
                typesToExclude: typeof(IAsyncDisposable));
        }

        [TestMethod]
        public void NotBlockingAlbumCollection()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugins();
            plugins.ImageCollection.Insert(0, x => new ImageCollectionPluginBaseTests.FullyCustom(x));
            plugins.AlbumCollection.Insert(0, x => new AlbumCollectionPluginBaseTests.FullyCustom(x));

            var plugin = plugins.AlbumCollection.Execute(new AlbumCollectionPluginBaseTests.Unimplemented());

            // Ensure an AlbumCollection plugin can still override ImageCollection members.
            Helpers.AssertAllMembersThrowOnAccess<AccessedException<AlbumCollectionPluginBaseTests.FullyCustom>, ImageCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources,
                typesToExclude: typeof(IAsyncDisposable));
        }

        [TestMethod]
        public void DisposingAlbumCollection()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugins();
            plugins.ImageCollection.Insert(0, x => new ImageCollectionPluginBaseTests.FullyCustom(x));

            var plugin = plugins.AlbumCollection.Execute(new AlbumCollectionPluginBaseTests.Unimplemented());

            Helpers.AssertAllThrowsOnMemberAccess<IAsyncDisposable>(value: plugin, expectedExceptions: new[] {
                typeof(AccessedException<ImageCollectionPluginBaseTests.FullyCustom>),
                typeof(AccessedException<AlbumCollectionPluginBaseTests.Unimplemented>),
            });
        }
    }
}
