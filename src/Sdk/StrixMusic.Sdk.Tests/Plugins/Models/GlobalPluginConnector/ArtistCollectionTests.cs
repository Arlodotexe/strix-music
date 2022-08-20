using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;
using StrixMusic.Sdk.BaseModels;

namespace StrixMusic.Sdk.Tests.Plugins.Models.GlobalModelPluginConnector
{
    [TestClass]
    public class ArtistCollectionTests
    {
        private static bool NoInner(MemberInfo x) => !x.Name.Contains("Inner");
        private static bool NoInnerOrSources(MemberInfo x) => NoInner(x) && !x.Name.ToLower().Contains("sources");

        [TestMethod]
        public void AccessedThroughAlbum()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugin(SdkTestPluginMetadata.Metadata);
            plugins.ArtistCollection.Add(x => new ArtistCollectionPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).Album.Execute(new AlbumPluginBaseTests.Unimplemented());

            Helpers.AssertAllMembersThrowOnAccess<AccessedException<ArtistCollectionPluginBaseTests.FullyCustom>, ArtistCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources,
                typesToExclude: new[]
                {
                    typeof(IPlayableCollectionItem)
                });
        }

        [TestMethod]
        public void NotBlockingAlbum()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugin(SdkTestPluginMetadata.Metadata);
            plugins.ArtistCollection.Add(x => new ArtistCollectionPluginBaseTests.FullyCustom(x));
            plugins.Album.Add(x => new AlbumPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).Album.Execute(new AlbumPluginBaseTests.Unimplemented());

            // Ensure an Album plugin can still be accessed through ArtistCollection members.
            Helpers.AssertAllMembersThrowOnAccess<AccessedException<AlbumPluginBaseTests.FullyCustom>, ArtistCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources);
        }

        [TestMethod]
        public void AccessedThroughTrack()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugin(SdkTestPluginMetadata.Metadata);
            plugins.ArtistCollection.Add(x => new ArtistCollectionPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).Track.Execute(new TrackPluginBaseTests.Unimplemented());

            Helpers.AssertAllMembersThrowOnAccess<AccessedException<ArtistCollectionPluginBaseTests.FullyCustom>, ArtistCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources,
                typesToExclude: new[]
                {
                    typeof(IPlayableCollectionItem)
                });
        }

        [TestMethod]
        public void NotBlockingTrack()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugin(SdkTestPluginMetadata.Metadata);
            plugins.ArtistCollection.Add(x => new ArtistCollectionPluginBaseTests.FullyCustom(x));
            plugins.Track.Add(x => new TrackPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).Track.Execute(new TrackPluginBaseTests.Unimplemented());

            // Ensure an Track plugin can still be accessed through ArtistCollection members.
            Helpers.AssertAllMembersThrowOnAccess<AccessedException<TrackPluginBaseTests.FullyCustom>, ArtistCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources);
        }

        [TestMethod]
        public void AccessedThroughPlayableCollectionGroup()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugin(SdkTestPluginMetadata.Metadata);
            plugins.ArtistCollection.Add(x => new ArtistCollectionPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).PlayableCollectionGroup.Execute(new PlayableCollectionGroupPluginBaseTests.Unimplemented());

            Helpers.AssertAllMembersThrowOnAccess<AccessedException<ArtistCollectionPluginBaseTests.FullyCustom>, ArtistCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources,
                typesToExclude: new[]
                {
                    typeof(IPlayableCollectionItem)
                });
        }

        [TestMethod]
        public void NotBlockingPlayableCollectionGroup()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugin(SdkTestPluginMetadata.Metadata);
            plugins.ArtistCollection.Add(x => new ArtistCollectionPluginBaseTests.FullyCustom(x));
            plugins.PlayableCollectionGroup.Add(x => new PlayableCollectionGroupPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).PlayableCollectionGroup.Execute(new PlayableCollectionGroupPluginBaseTests.Unimplemented());

            // Ensure an PlayableCollectionGroup plugin can still be accessed through ArtistCollection members.
            Helpers.AssertAllMembersThrowOnAccess<AccessedException<PlayableCollectionGroupPluginBaseTests.FullyCustom>, ArtistCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources);
        }

        [TestMethod]
        public void AccessedThroughLibrary()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugin(SdkTestPluginMetadata.Metadata);
            plugins.ArtistCollection.Add(x => new ArtistCollectionPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).Library.Execute(new LibraryPluginBaseTests.Unimplemented());

            Helpers.AssertAllMembersThrowOnAccess<AccessedException<ArtistCollectionPluginBaseTests.FullyCustom>, ArtistCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources,
                typesToExclude: new[]
                {
                    typeof(IPlayableCollectionItem)
                });
        }

        [TestMethod]
        public void NotBlockingLibrary()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugin(SdkTestPluginMetadata.Metadata);
            plugins.ArtistCollection.Add(x => new ArtistCollectionPluginBaseTests.FullyCustom(x));
            plugins.Library.Add(x => new LibraryPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).Library.Execute(new LibraryPluginBaseTests.Unimplemented());

            // Ensure an Library plugin can still be accessed through ArtistCollection members.
            Helpers.AssertAllMembersThrowOnAccess<AccessedException<LibraryPluginBaseTests.FullyCustom>, ArtistCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources);
        }

        [TestMethod]
        public void AccessedThroughDiscoverables()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugin(SdkTestPluginMetadata.Metadata);
            plugins.ArtistCollection.Add(x => new ArtistCollectionPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).Discoverables.Execute(new DiscoverablesPluginBaseTests.Unimplemented());

            Helpers.AssertAllMembersThrowOnAccess<AccessedException<ArtistCollectionPluginBaseTests.FullyCustom>, ArtistCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources,
                typesToExclude: new[]
                {
                    typeof(IPlayableCollectionItem)
                });
        }

        [TestMethod]
        public void NotBlockingDiscoverables()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugin(SdkTestPluginMetadata.Metadata);
            plugins.ArtistCollection.Add(x => new ArtistCollectionPluginBaseTests.FullyCustom(x));
            plugins.Discoverables.Add(x => new DiscoverablesPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).Discoverables.Execute(new DiscoverablesPluginBaseTests.Unimplemented());

            // Ensure an Discoverables plugin can still be accessed through ArtistCollection members.
            Helpers.AssertAllMembersThrowOnAccess<AccessedException<DiscoverablesPluginBaseTests.FullyCustom>, ArtistCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources);
        }

        [TestMethod]
        public void AccessedThroughRecentlyPlayed()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugin(SdkTestPluginMetadata.Metadata);
            plugins.ArtistCollection.Add(x => new ArtistCollectionPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).RecentlyPlayed.Execute(new RecentlyPlayedPluginBaseTests.Unimplemented());

            Helpers.AssertAllMembersThrowOnAccess<AccessedException<ArtistCollectionPluginBaseTests.FullyCustom>, ArtistCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources,
                typesToExclude: new[]
                {
                    typeof(IPlayableCollectionItem)
                });
        }

        [TestMethod]
        public void NotBlockingRecentlyPlayed()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugin(SdkTestPluginMetadata.Metadata);
            plugins.ArtistCollection.Add(x => new ArtistCollectionPluginBaseTests.FullyCustom(x));
            plugins.RecentlyPlayed.Add(x => new RecentlyPlayedPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).RecentlyPlayed.Execute(new RecentlyPlayedPluginBaseTests.Unimplemented());

            // Ensure an RecentlyPlayed plugin can still be accessed through ArtistCollection members.
            Helpers.AssertAllMembersThrowOnAccess<AccessedException<RecentlyPlayedPluginBaseTests.FullyCustom>, ArtistCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources);
        }

        [TestMethod]
        public void AccessedThroughSearchHistory()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugin(SdkTestPluginMetadata.Metadata);
            plugins.ArtistCollection.Add(x => new ArtistCollectionPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).SearchHistory.Execute(new SearchHistoryPluginBaseTests.Unimplemented());

            Helpers.AssertAllMembersThrowOnAccess<AccessedException<ArtistCollectionPluginBaseTests.FullyCustom>, ArtistCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources,
                typesToExclude: new[]
                {
                    typeof(IPlayableCollectionItem)
                });
        }

        [TestMethod]
        public void NotBlockingSearchHistory()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugin(SdkTestPluginMetadata.Metadata);
            plugins.ArtistCollection.Add(x => new ArtistCollectionPluginBaseTests.FullyCustom(x));
            plugins.SearchHistory.Add(x => new SearchHistoryPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).SearchHistory.Execute(new SearchHistoryPluginBaseTests.Unimplemented());

            // Ensure an SearchHistory plugin can still be accessed through ArtistCollection members.
            Helpers.AssertAllMembersThrowOnAccess<AccessedException<SearchHistoryPluginBaseTests.FullyCustom>, ArtistCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources);
        }

        [TestMethod]
        public void AccessedThroughSearchResults()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugin(SdkTestPluginMetadata.Metadata);
            plugins.ArtistCollection.Add(x => new ArtistCollectionPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).SearchResults.Execute(new SearchResultsPluginBaseTests.Unimplemented());

            Helpers.AssertAllMembersThrowOnAccess<AccessedException<ArtistCollectionPluginBaseTests.FullyCustom>, ArtistCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources,
                typesToExclude: new[]
                {
                    typeof(IPlayableCollectionItem)
                });
        }

        [TestMethod]
        public void NotBlockingSearchResults()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugin(SdkTestPluginMetadata.Metadata);
            plugins.ArtistCollection.Add(x => new ArtistCollectionPluginBaseTests.FullyCustom(x));
            plugins.SearchResults.Add(x => new SearchResultsPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).SearchResults.Execute(new SearchResultsPluginBaseTests.Unimplemented());

            // Ensure an SearchResults plugin can still be accessed through ArtistCollection members.
            Helpers.AssertAllMembersThrowOnAccess<AccessedException<SearchResultsPluginBaseTests.FullyCustom>, ArtistCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources);
        }
    }
}
