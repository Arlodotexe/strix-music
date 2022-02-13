using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace StrixMusic.Sdk.Tests.Plugins.Models.GlobalPluginConnector
{
    [TestClass]
    public class DownloadableTests
    {
        [TestMethod]
        public void AccessedThroughPlayable()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugins();
            Sdk.Plugins.Model.GlobalModelPluginConnector.Enable(plugins);

            plugins.Downloadable.Insert(0, x => new DownloadablePluginBaseTests.FullyCustom(x));

            var plugin = plugins.Playable.Execute(new PlayablePluginBaseTests.Unimplemented());

            Helpers.AssertAllMembersThrowOnAccess<AccessedException<DownloadablePluginBaseTests.FullyCustom>, DownloadablePluginBaseTests.FullyCustom>(plugin, typesToExclude: typeof(IAsyncDisposable));
        }

        [TestMethod]
        public void NotBlockingPlayable()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugins();
            Sdk.Plugins.Model.GlobalModelPluginConnector.Enable(plugins);

            plugins.Downloadable.Insert(0, x => new DownloadablePluginBaseTests.FullyCustom(x));
            plugins.Playable.Insert(0, x => new PlayablePluginBaseTests.FullyCustom(x));

            var plugin = plugins.Playable.Execute(new PlayablePluginBaseTests.Unimplemented());

            // Ensure a Playable plugin can still override Downloadable members.
            Helpers.AssertAllMembersThrowOnAccess<AccessedException<PlayablePluginBaseTests.FullyCustom>, DownloadablePluginBaseTests.FullyCustom>(plugin, typesToExclude: typeof(IAsyncDisposable));
        }

        [TestMethod]
        public void DisposingPlayable()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugins();
            Sdk.Plugins.Model.GlobalModelPluginConnector.Enable(plugins);

            plugins.Downloadable.Insert(0, x => new DownloadablePluginBaseTests.FullyCustom(x));

            var plugin = plugins.Playable.Execute(new PlayablePluginBaseTests.Unimplemented());

            Helpers.AssertAllThrowsOnMemberAccess<IAsyncDisposable>(plugin, expectedExceptions: new[] {
                typeof(AccessedException<DownloadablePluginBaseTests.FullyCustom>),
                typeof(AccessedException<PlayablePluginBaseTests.Unimplemented>),
            });
        }

        [TestMethod]
        public void AccessedThroughTrackCollection()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugins();
            Sdk.Plugins.Model.GlobalModelPluginConnector.Enable(plugins);

            plugins.Downloadable.Insert(0, x => new DownloadablePluginBaseTests.FullyCustom(x));

            var plugin = plugins.TrackCollection.Execute(new TrackCollectionPluginBaseTests.Unimplemented());

            Helpers.AssertAllMembersThrowOnAccess<AccessedException<DownloadablePluginBaseTests.FullyCustom>, DownloadablePluginBaseTests.FullyCustom>(plugin, typesToExclude: typeof(IAsyncDisposable));
        }

        [TestMethod]
        public void NotBlockingTrackCollection()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugins();
            Sdk.Plugins.Model.GlobalModelPluginConnector.Enable(plugins);

            plugins.Downloadable.Insert(0, x => new DownloadablePluginBaseTests.FullyCustom(x));
            plugins.TrackCollection.Insert(0, x => new TrackCollectionPluginBaseTests.FullyCustom(x));

            var plugin = plugins.TrackCollection.Execute(new TrackCollectionPluginBaseTests.Unimplemented());

            // Ensure a TrackCollection plugin can still override Downloadable members.
            Helpers.AssertAllMembersThrowOnAccess<AccessedException<TrackCollectionPluginBaseTests.FullyCustom>, DownloadablePluginBaseTests.FullyCustom>(plugin, typesToExclude: typeof(IAsyncDisposable));
        }

        [TestMethod]
        public void DisposingTrackCollection()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugins();
            Sdk.Plugins.Model.GlobalModelPluginConnector.Enable(plugins);

            plugins.Downloadable.Insert(0, x => new DownloadablePluginBaseTests.FullyCustom(x));

            var plugin = plugins.TrackCollection.Execute(new TrackCollectionPluginBaseTests.Unimplemented());

            Helpers.AssertAllThrowsOnMemberAccess<IAsyncDisposable>(plugin, expectedExceptions: new[] {
                typeof(AccessedException<DownloadablePluginBaseTests.FullyCustom>),
                typeof(AccessedException<TrackCollectionPluginBaseTests.Unimplemented>),
            });
        }

        [TestMethod]
        public void AccessedThroughArtistCollection()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugins();
            Sdk.Plugins.Model.GlobalModelPluginConnector.Enable(plugins);

            plugins.Downloadable.Insert(0, x => new DownloadablePluginBaseTests.FullyCustom(x));

            var plugin = plugins.ArtistCollection.Execute(new ArtistCollectionPluginBaseTests.Unimplemented());

            Helpers.AssertAllMembersThrowOnAccess<AccessedException<DownloadablePluginBaseTests.FullyCustom>, DownloadablePluginBaseTests.FullyCustom>(plugin, typesToExclude: typeof(IAsyncDisposable));
        }

        [TestMethod]
        public void NotBlockingArtistCollection()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugins();
            Sdk.Plugins.Model.GlobalModelPluginConnector.Enable(plugins);

            plugins.Downloadable.Insert(0, x => new DownloadablePluginBaseTests.FullyCustom(x));
            plugins.ArtistCollection.Insert(0, x => new ArtistCollectionPluginBaseTests.FullyCustom(x));

            var plugin = plugins.ArtistCollection.Execute(new ArtistCollectionPluginBaseTests.Unimplemented());

            // Ensure an ArtistCollection plugin can still override Downloadable members.
            Helpers.AssertAllMembersThrowOnAccess<AccessedException<ArtistCollectionPluginBaseTests.FullyCustom>, DownloadablePluginBaseTests.FullyCustom>(plugin, typesToExclude: typeof(IAsyncDisposable));
        }

        [TestMethod]
        public void DisposingArtistCollection()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugins();
            Sdk.Plugins.Model.GlobalModelPluginConnector.Enable(plugins);

            plugins.Downloadable.Insert(0, x => new DownloadablePluginBaseTests.FullyCustom(x));

            var plugin = plugins.ArtistCollection.Execute(new ArtistCollectionPluginBaseTests.Unimplemented());

            Helpers.AssertAllThrowsOnMemberAccess<IAsyncDisposable>(plugin, expectedExceptions: new[] {
                typeof(AccessedException<DownloadablePluginBaseTests.FullyCustom>),
                typeof(AccessedException<ArtistCollectionPluginBaseTests.Unimplemented>),
            });
        }

        [TestMethod]
        public void AccessedThroughAlbumCollection()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugins();
            Sdk.Plugins.Model.GlobalModelPluginConnector.Enable(plugins);

            plugins.Downloadable.Insert(0, x => new DownloadablePluginBaseTests.FullyCustom(x));

            var plugin = plugins.AlbumCollection.Execute(new AlbumCollectionPluginBaseTests.Unimplemented());

            Helpers.AssertAllMembersThrowOnAccess<AccessedException<DownloadablePluginBaseTests.FullyCustom>, DownloadablePluginBaseTests.FullyCustom>(plugin, typesToExclude: typeof(IAsyncDisposable));
        }

        [TestMethod]
        public void NotBlockingAlbumCollection()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugins();
            Sdk.Plugins.Model.GlobalModelPluginConnector.Enable(plugins);

            plugins.Downloadable.Insert(0, x => new DownloadablePluginBaseTests.FullyCustom(x));
            plugins.AlbumCollection.Insert(0, x => new AlbumCollectionPluginBaseTests.FullyCustom(x));

            var plugin = plugins.AlbumCollection.Execute(new AlbumCollectionPluginBaseTests.Unimplemented());

            // Ensure an AlbumCollection plugin can still override Downloadable members.
            Helpers.AssertAllMembersThrowOnAccess<AccessedException<AlbumCollectionPluginBaseTests.FullyCustom>, DownloadablePluginBaseTests.FullyCustom>(plugin, typesToExclude: typeof(IAsyncDisposable));
        }

        [TestMethod]
        public void DisposingAlbumCollection()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugins();
            Sdk.Plugins.Model.GlobalModelPluginConnector.Enable(plugins);

            plugins.Downloadable.Insert(0, x => new DownloadablePluginBaseTests.FullyCustom(x));

            var plugin = plugins.AlbumCollection.Execute(new AlbumCollectionPluginBaseTests.Unimplemented());

            Helpers.AssertAllThrowsOnMemberAccess<IAsyncDisposable>(plugin, expectedExceptions: new[] {
                typeof(AccessedException<DownloadablePluginBaseTests.FullyCustom>),
                typeof(AccessedException<AlbumCollectionPluginBaseTests.Unimplemented>),
            });
        }
    }
}
