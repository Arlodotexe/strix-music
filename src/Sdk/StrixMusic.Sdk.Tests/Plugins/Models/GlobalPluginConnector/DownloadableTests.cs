using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;

namespace StrixMusic.Sdk.Tests.Plugins.Models.GlobalModelPluginConnector
{
    [TestClass]
    public class DownloadableTests
    {
        private static bool NoInner(MemberInfo x) => !x.Name.Contains("Inner");
        private static bool NoInnerOrSources(MemberInfo x) => NoInner(x) && x.Name != "get_Sources" && x.Name != "get_SourceCores";

        [TestMethod]
        public void AccessedThroughPlayable()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugins();
            plugins.Downloadable.Add(x => new DownloadablePluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).Playable.Execute(new PlayablePluginBaseTests.Unimplemented());

            Helpers.AssertAllMembersThrowOnAccess<AccessedException<DownloadablePluginBaseTests.FullyCustom>, DownloadablePluginBaseTests.FullyCustom>(plugin, typesToExclude: typeof(IAsyncDisposable));
        }

        [TestMethod]
        public void NotBlockingPlayable()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugins();
            plugins.Downloadable.Add(x => new DownloadablePluginBaseTests.FullyCustom(x));
            plugins.Playable.Add(x => new PlayablePluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).Playable.Execute(new PlayablePluginBaseTests.Unimplemented());

            // Ensure a Playable plugin can still override Downloadable members.
            Helpers.AssertAllMembersThrowOnAccess<AccessedException<PlayablePluginBaseTests.FullyCustom>, DownloadablePluginBaseTests.FullyCustom>(plugin, typesToExclude: typeof(IAsyncDisposable));
        }

        [TestMethod]
        public void DisposingPlayable()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugins();
            plugins.Downloadable.Add(x => new DownloadablePluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).Playable.Execute(new PlayablePluginBaseTests.Unimplemented());

            Helpers.AssertAllThrowsOnMemberAccess<IAsyncDisposable>(plugin, expectedExceptions: new[]
            {
                typeof(AccessedException<DownloadablePluginBaseTests.FullyCustom>),
                typeof(AccessedException<PlayablePluginBaseTests.Unimplemented>),
            });
        }

        [TestMethod]
        public void AccessedThroughTrackCollection()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugins();
            plugins.Downloadable.Add(x => new DownloadablePluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).TrackCollection.Execute(new TrackCollectionPluginBaseTests.Unimplemented());

            Helpers.AssertAllMembersThrowOnAccess<AccessedException<DownloadablePluginBaseTests.FullyCustom>, DownloadablePluginBaseTests.FullyCustom>(plugin, typesToExclude: typeof(IAsyncDisposable));
        }

        [TestMethod]
        public void NotBlockingTrackCollection()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugins();
            plugins.Downloadable.Add(x => new DownloadablePluginBaseTests.FullyCustom(x));
            plugins.TrackCollection.Add(x => new TrackCollectionPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).TrackCollection.Execute(new TrackCollectionPluginBaseTests.Unimplemented());

            // Ensure a TrackCollection plugin can still override Downloadable members.
            Helpers.AssertAllMembersThrowOnAccess<AccessedException<TrackCollectionPluginBaseTests.FullyCustom>, DownloadablePluginBaseTests.FullyCustom>(plugin, typesToExclude: typeof(IAsyncDisposable));
        }

        [TestMethod]
        public void DisposingTrackCollection()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugins();
            plugins.Downloadable.Add(x => new DownloadablePluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).TrackCollection.Execute(new TrackCollectionPluginBaseTests.Unimplemented());

            Helpers.AssertAllThrowsOnMemberAccess<IAsyncDisposable>(plugin, expectedExceptions: new[]
            {
                typeof(AccessedException<DownloadablePluginBaseTests.FullyCustom>),
                typeof(AccessedException<TrackCollectionPluginBaseTests.Unimplemented>),
            });
        }

        [TestMethod]
        public void AccessedThroughArtistCollection()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugins();
            plugins.Downloadable.Add(x => new DownloadablePluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).ArtistCollection.Execute(new ArtistCollectionPluginBaseTests.Unimplemented());

            Helpers.AssertAllMembersThrowOnAccess<AccessedException<DownloadablePluginBaseTests.FullyCustom>, DownloadablePluginBaseTests.FullyCustom>(plugin, typesToExclude: typeof(IAsyncDisposable));
        }

        [TestMethod]
        public void NotBlockingArtistCollection()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugins();
            plugins.Downloadable.Add(x => new DownloadablePluginBaseTests.FullyCustom(x));
            plugins.ArtistCollection.Add(x => new ArtistCollectionPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).ArtistCollection.Execute(new ArtistCollectionPluginBaseTests.Unimplemented());

            // Ensure an ArtistCollection plugin can still override Downloadable members.
            Helpers.AssertAllMembersThrowOnAccess<AccessedException<ArtistCollectionPluginBaseTests.FullyCustom>, DownloadablePluginBaseTests.FullyCustom>(plugin, typesToExclude: typeof(IAsyncDisposable));
        }

        [TestMethod]
        public void DisposingArtistCollection()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugins();
            plugins.Downloadable.Add(x => new DownloadablePluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).ArtistCollection.Execute(new ArtistCollectionPluginBaseTests.Unimplemented());

            Helpers.AssertAllThrowsOnMemberAccess<IAsyncDisposable>(plugin, expectedExceptions: new[]
            {
                typeof(AccessedException<DownloadablePluginBaseTests.FullyCustom>),
                typeof(AccessedException<ArtistCollectionPluginBaseTests.Unimplemented>),
            });
        }

        [TestMethod]
        public void AccessedThroughAlbumCollection()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugins();
            plugins.Downloadable.Add(x => new DownloadablePluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).AlbumCollection.Execute(new AlbumCollectionPluginBaseTests.Unimplemented());

            Helpers.AssertAllMembersThrowOnAccess<AccessedException<DownloadablePluginBaseTests.FullyCustom>, DownloadablePluginBaseTests.FullyCustom>(plugin, typesToExclude: typeof(IAsyncDisposable));
        }

        [TestMethod]
        public void NotBlockingAlbumCollection()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugins();
            plugins.Downloadable.Add(x => new DownloadablePluginBaseTests.FullyCustom(x));
            plugins.AlbumCollection.Add(x => new AlbumCollectionPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).AlbumCollection.Execute(new AlbumCollectionPluginBaseTests.Unimplemented());

            // Ensure an AlbumCollection plugin can still override Downloadable members.
            Helpers.AssertAllMembersThrowOnAccess<AccessedException<AlbumCollectionPluginBaseTests.FullyCustom>, DownloadablePluginBaseTests.FullyCustom>(plugin, typesToExclude: typeof(IAsyncDisposable));
        }

        [TestMethod]
        public void DisposingAlbumCollection()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugins();
            plugins.Downloadable.Add(x => new DownloadablePluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).AlbumCollection.Execute(new AlbumCollectionPluginBaseTests.Unimplemented());

            Helpers.AssertAllThrowsOnMemberAccess<IAsyncDisposable>(plugin, expectedExceptions: new[]
            {
                typeof(AccessedException<DownloadablePluginBaseTests.FullyCustom>),
                typeof(AccessedException<AlbumCollectionPluginBaseTests.Unimplemented>),
            });
        }

        [TestMethod]
        public void AccessedThroughAlbum()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugins();
            plugins.Downloadable.Add(x => new DownloadablePluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).Album.Execute(new AlbumPluginBaseTests.Unimplemented());

            Helpers.AssertAllMembersThrowOnAccess<AccessedException<DownloadablePluginBaseTests.FullyCustom>, DownloadablePluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources,
                typesToExclude: typeof(IAsyncDisposable));
        }

        [TestMethod]
        public void NotBlockingAlbum()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugins();
            plugins.Downloadable.Add(x => new DownloadablePluginBaseTests.FullyCustom(x));
            plugins.Album.Add(x => new AlbumPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).Album.Execute(new AlbumPluginBaseTests.Unimplemented());

            // Ensure an Album plugin can still override Downloadable members.
            Helpers.AssertAllMembersThrowOnAccess<AccessedException<AlbumPluginBaseTests.FullyCustom>, DownloadablePluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources,
                typesToExclude: typeof(IAsyncDisposable));
        }

        [TestMethod]
        public void DisposingAlbum()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugins();
            plugins.Downloadable.Add(x => new DownloadablePluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).Album.Execute(new AlbumPluginBaseTests.Unimplemented());

            Helpers.AssertAllThrowsOnMemberAccess<IAsyncDisposable>(value: plugin, expectedExceptions: new[]
            {
                typeof(AccessedException<DownloadablePluginBaseTests.FullyCustom>),
                typeof(AccessedException<AlbumPluginBaseTests.Unimplemented>),
            });
        }
    }
}
