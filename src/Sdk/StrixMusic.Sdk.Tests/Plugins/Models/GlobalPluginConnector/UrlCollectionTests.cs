using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;

namespace StrixMusic.Sdk.Tests.Plugins.Models.GlobalModelPluginConnector
{
    [TestClass]
    public class UrlCollectionTests
    {
        private static bool NoInner(MemberInfo x) => !x.Name.Contains("Inner");
        private static bool NoInnerOrSources(MemberInfo x) => NoInner(x) && !x.Name.ToLower().Contains("sources");

        [TestMethod]
        public void AccessedThroughPlayable()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugin(SdkTestPluginMetadata.Metadata);
            plugins.UrlCollection.Add(x => new UrlCollectionPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).Playable.Execute(new PlayablePluginBaseTests.Unimplemented());

            Helpers.AssertAllMembersThrowOnAccess<AccessedException<UrlCollectionPluginBaseTests.FullyCustom>, UrlCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources);
        }

        [TestMethod]
        public void NotBlockingPlayable()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugin(SdkTestPluginMetadata.Metadata);
            plugins.UrlCollection.Add(x => new UrlCollectionPluginBaseTests.FullyCustom(x));
            plugins.Playable.Add(x => new PlayablePluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).Playable.Execute(new PlayablePluginBaseTests.Unimplemented());

            // Ensure a Playable plugin can still be accessed through UrlCollection members.
            Helpers.AssertAllMembersThrowOnAccess<AccessedException<PlayablePluginBaseTests.FullyCustom>, UrlCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources);
        }

        [TestMethod]
        public void AccessedThroughTrackCollection()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugin(SdkTestPluginMetadata.Metadata);
            plugins.UrlCollection.Add(x => new UrlCollectionPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).TrackCollection.Execute(new TrackCollectionPluginBaseTests.Unimplemented());

            Helpers.AssertAllMembersThrowOnAccess<AccessedException<UrlCollectionPluginBaseTests.FullyCustom>, UrlCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources);
        }

        [TestMethod]
        public void NotBlockingTrackCollection()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugin(SdkTestPluginMetadata.Metadata);
            plugins.UrlCollection.Add(x => new UrlCollectionPluginBaseTests.FullyCustom(x));
            plugins.TrackCollection.Add(x => new TrackCollectionPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).TrackCollection.Execute(new TrackCollectionPluginBaseTests.Unimplemented());

            // Ensure a TrackCollection plugin can still be accessed through UrlCollection members.
            Helpers.AssertAllMembersThrowOnAccess<AccessedException<TrackCollectionPluginBaseTests.FullyCustom>, UrlCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources);
        }

        [TestMethod]
        public void AccessedThroughArtistCollection()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugin(SdkTestPluginMetadata.Metadata);
            plugins.UrlCollection.Add(x => new UrlCollectionPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).ArtistCollection.Execute(new ArtistCollectionPluginBaseTests.Unimplemented());

            Helpers.AssertAllMembersThrowOnAccess<AccessedException<UrlCollectionPluginBaseTests.FullyCustom>, UrlCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources);
        }

        [TestMethod]
        public void NotBlockingArtistCollection()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugin(SdkTestPluginMetadata.Metadata);
            plugins.UrlCollection.Add(x => new UrlCollectionPluginBaseTests.FullyCustom(x));
            plugins.ArtistCollection.Add(x => new ArtistCollectionPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).ArtistCollection.Execute(new ArtistCollectionPluginBaseTests.Unimplemented());

            // Ensure an ArtistCollection plugin can still be accessed through UrlCollection members.
            Helpers.AssertAllMembersThrowOnAccess<AccessedException<ArtistCollectionPluginBaseTests.FullyCustom>, UrlCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources);
        }

        [TestMethod]
        public void AccessedThroughAlbumCollection()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugin(SdkTestPluginMetadata.Metadata);
            plugins.UrlCollection.Add(x => new UrlCollectionPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).AlbumCollection.Execute(new AlbumCollectionPluginBaseTests.Unimplemented());

            Helpers.AssertAllMembersThrowOnAccess<AccessedException<UrlCollectionPluginBaseTests.FullyCustom>, UrlCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources);
        }

        [TestMethod]
        public void NotBlockingAlbumCollection()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugin(SdkTestPluginMetadata.Metadata);
            plugins.UrlCollection.Add(x => new UrlCollectionPluginBaseTests.FullyCustom(x));
            plugins.AlbumCollection.Add(x => new AlbumCollectionPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).AlbumCollection.Execute(new AlbumCollectionPluginBaseTests.Unimplemented());

            // Ensure an AlbumCollection plugin can still be accessed through UrlCollection members.
            Helpers.AssertAllMembersThrowOnAccess<AccessedException<AlbumCollectionPluginBaseTests.FullyCustom>, UrlCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources);
        }

        [TestMethod]
        public void AccessedThroughAlbum()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugin(SdkTestPluginMetadata.Metadata);
            plugins.UrlCollection.Add(x => new UrlCollectionPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).Album.Execute(new AlbumPluginBaseTests.Unimplemented());

            Helpers.AssertAllMembersThrowOnAccess<AccessedException<UrlCollectionPluginBaseTests.FullyCustom>, UrlCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources);
        }

        [TestMethod]
        public void NotBlockingAlbum()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugin(SdkTestPluginMetadata.Metadata);
            plugins.UrlCollection.Add(x => new UrlCollectionPluginBaseTests.FullyCustom(x));
            plugins.Album.Add(x => new AlbumPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).Album.Execute(new AlbumPluginBaseTests.Unimplemented());

            // Ensure an Album plugin can still be accessed through UrlCollection members.
            Helpers.AssertAllMembersThrowOnAccess<AccessedException<AlbumPluginBaseTests.FullyCustom>, UrlCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources);
        }

        [TestMethod]
        public void AccessedThroughArtist()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugin(SdkTestPluginMetadata.Metadata);
            plugins.UrlCollection.Add(x => new UrlCollectionPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).Artist.Execute(new ArtistPluginBaseTests.Unimplemented());

            Helpers.AssertAllMembersThrowOnAccess<AccessedException<UrlCollectionPluginBaseTests.FullyCustom>, UrlCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources);
        }

        [TestMethod]
        public void NotBlockingArtist()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugin(SdkTestPluginMetadata.Metadata);
            plugins.UrlCollection.Add(x => new UrlCollectionPluginBaseTests.FullyCustom(x));
            plugins.Artist.Add(x => new ArtistPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).Artist.Execute(new ArtistPluginBaseTests.Unimplemented());

            // Ensure an Artist plugin can still be accessed through UrlCollection members.
            Helpers.AssertAllMembersThrowOnAccess<AccessedException<ArtistPluginBaseTests.FullyCustom>, UrlCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources);
        }

        [TestMethod]
        public void AccessedThroughPlaylist()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugin(SdkTestPluginMetadata.Metadata);
            plugins.UrlCollection.Add(x => new UrlCollectionPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).Playlist.Execute(new PlaylistPluginBaseTests.Unimplemented());

            Helpers.AssertAllMembersThrowOnAccess<AccessedException<UrlCollectionPluginBaseTests.FullyCustom>, UrlCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources);
        }

        [TestMethod]
        public void NotBlockingPlaylist()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugin(SdkTestPluginMetadata.Metadata);
            plugins.UrlCollection.Add(x => new UrlCollectionPluginBaseTests.FullyCustom(x));
            plugins.Playlist.Add(x => new PlaylistPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).Playlist.Execute(new PlaylistPluginBaseTests.Unimplemented());

            // Ensure an Playlist plugin can still be accessed through UrlCollection members.
            Helpers.AssertAllMembersThrowOnAccess<AccessedException<PlaylistPluginBaseTests.FullyCustom>, UrlCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources);
        }

        [TestMethod]
        public void AccessedThroughTrack()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugin(SdkTestPluginMetadata.Metadata);
            plugins.UrlCollection.Add(x => new UrlCollectionPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).Track.Execute(new TrackPluginBaseTests.Unimplemented());

            Helpers.AssertAllMembersThrowOnAccess<AccessedException<UrlCollectionPluginBaseTests.FullyCustom>, UrlCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources);
        }

        [TestMethod]
        public void NotBlockingTrack()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugin(SdkTestPluginMetadata.Metadata);
            plugins.UrlCollection.Add(x => new UrlCollectionPluginBaseTests.FullyCustom(x));
            plugins.Track.Add(x => new TrackPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).Track.Execute(new TrackPluginBaseTests.Unimplemented());

            // Ensure an Track plugin can still be accessed through UrlCollection members.
            Helpers.AssertAllMembersThrowOnAccess<AccessedException<TrackPluginBaseTests.FullyCustom>, UrlCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources);
        }

        [TestMethod]
        public void AccessedThroughPlayableCollectionGroup()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugin(SdkTestPluginMetadata.Metadata);
            plugins.UrlCollection.Add(x => new UrlCollectionPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).PlayableCollectionGroup.Execute(new PlayableCollectionGroupPluginBaseTests.Unimplemented());

            Helpers.AssertAllMembersThrowOnAccess<AccessedException<UrlCollectionPluginBaseTests.FullyCustom>, UrlCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources);
        }

        [TestMethod]
        public void NotBlockingPlayableCollectionGroup()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugin(SdkTestPluginMetadata.Metadata);
            plugins.UrlCollection.Add(x => new UrlCollectionPluginBaseTests.FullyCustom(x));
            plugins.PlayableCollectionGroup.Add(x => new PlayableCollectionGroupPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).PlayableCollectionGroup.Execute(new PlayableCollectionGroupPluginBaseTests.Unimplemented());

            // Ensure an PlayableCollectionGroup plugin can still be accessed through UrlCollection members.
            Helpers.AssertAllMembersThrowOnAccess<AccessedException<PlayableCollectionGroupPluginBaseTests.FullyCustom>, UrlCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources);
        }

        [TestMethod]
        public void AccessedThroughLibrary()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugin(SdkTestPluginMetadata.Metadata);
            plugins.UrlCollection.Add(x => new UrlCollectionPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).Library.Execute(new LibraryPluginBaseTests.Unimplemented());

            Helpers.AssertAllMembersThrowOnAccess<AccessedException<UrlCollectionPluginBaseTests.FullyCustom>, UrlCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources);
        }

        [TestMethod]
        public void NotBlockingLibrary()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugin(SdkTestPluginMetadata.Metadata);
            plugins.UrlCollection.Add(x => new UrlCollectionPluginBaseTests.FullyCustom(x));
            plugins.Library.Add(x => new LibraryPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).Library.Execute(new LibraryPluginBaseTests.Unimplemented());

            // Ensure an Library plugin can still be accessed through UrlCollection members.
            Helpers.AssertAllMembersThrowOnAccess<AccessedException<LibraryPluginBaseTests.FullyCustom>, UrlCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources);
        }

        [TestMethod]
        public void AccessedThroughDiscoverables()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugin(SdkTestPluginMetadata.Metadata);
            plugins.UrlCollection.Add(x => new UrlCollectionPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).Discoverables.Execute(new DiscoverablesPluginBaseTests.Unimplemented());

            Helpers.AssertAllMembersThrowOnAccess<AccessedException<UrlCollectionPluginBaseTests.FullyCustom>, UrlCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources);
        }

        [TestMethod]
        public void NotBlockingDiscoverables()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugin(SdkTestPluginMetadata.Metadata);
            plugins.UrlCollection.Add(x => new UrlCollectionPluginBaseTests.FullyCustom(x));
            plugins.Discoverables.Add(x => new DiscoverablesPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).Discoverables.Execute(new DiscoverablesPluginBaseTests.Unimplemented());

            // Ensure an Discoverables plugin can still be accessed through UrlCollection members.
            Helpers.AssertAllMembersThrowOnAccess<AccessedException<DiscoverablesPluginBaseTests.FullyCustom>, UrlCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources);
        }

        [TestMethod]
        public void AccessedThroughRecentlyPlayed()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugin(SdkTestPluginMetadata.Metadata);
            plugins.UrlCollection.Add(x => new UrlCollectionPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).RecentlyPlayed.Execute(new RecentlyPlayedPluginBaseTests.Unimplemented());

            Helpers.AssertAllMembersThrowOnAccess<AccessedException<UrlCollectionPluginBaseTests.FullyCustom>, UrlCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources);
        }

        [TestMethod]
        public void NotBlockingRecentlyPlayed()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugin(SdkTestPluginMetadata.Metadata);
            plugins.UrlCollection.Add(x => new UrlCollectionPluginBaseTests.FullyCustom(x));
            plugins.RecentlyPlayed.Add(x => new RecentlyPlayedPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).RecentlyPlayed.Execute(new RecentlyPlayedPluginBaseTests.Unimplemented());

            // Ensure an RecentlyPlayed plugin can still be accessed through UrlCollection members.
            Helpers.AssertAllMembersThrowOnAccess<AccessedException<RecentlyPlayedPluginBaseTests.FullyCustom>, UrlCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources);
        }

        [TestMethod]
        public void AccessedThroughSearchHistory()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugin(SdkTestPluginMetadata.Metadata);
            plugins.UrlCollection.Add(x => new UrlCollectionPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).SearchHistory.Execute(new SearchHistoryPluginBaseTests.Unimplemented());

            Helpers.AssertAllMembersThrowOnAccess<AccessedException<UrlCollectionPluginBaseTests.FullyCustom>, UrlCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources);
        }

        [TestMethod]
        public void NotBlockingSearchHistory()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugin(SdkTestPluginMetadata.Metadata);
            plugins.UrlCollection.Add(x => new UrlCollectionPluginBaseTests.FullyCustom(x));
            plugins.SearchHistory.Add(x => new SearchHistoryPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).SearchHistory.Execute(new SearchHistoryPluginBaseTests.Unimplemented());

            // Ensure an SearchHistory plugin can still be accessed through UrlCollection members.
            Helpers.AssertAllMembersThrowOnAccess<AccessedException<SearchHistoryPluginBaseTests.FullyCustom>, UrlCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources);
        }

        [TestMethod]
        public void AccessedThroughSearchResults()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugin(SdkTestPluginMetadata.Metadata);
            plugins.UrlCollection.Add(x => new UrlCollectionPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).SearchResults.Execute(new SearchResultsPluginBaseTests.Unimplemented());

            Helpers.AssertAllMembersThrowOnAccess<AccessedException<UrlCollectionPluginBaseTests.FullyCustom>, UrlCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources);
        }

        [TestMethod]
        public void NotBlockingSearchResults()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugin(SdkTestPluginMetadata.Metadata);
            plugins.UrlCollection.Add(x => new UrlCollectionPluginBaseTests.FullyCustom(x));
            plugins.SearchResults.Add(x => new SearchResultsPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).SearchResults.Execute(new SearchResultsPluginBaseTests.Unimplemented());

            // Ensure an SearchResults plugin can still be accessed through UrlCollection members.
            Helpers.AssertAllMembersThrowOnAccess<AccessedException<SearchResultsPluginBaseTests.FullyCustom>, UrlCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources);
        }
    }
}
