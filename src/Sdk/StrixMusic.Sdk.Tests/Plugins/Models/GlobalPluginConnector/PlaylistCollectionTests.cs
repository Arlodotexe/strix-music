using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;
using StrixMusic.Sdk.Models.Base;

namespace StrixMusic.Sdk.Tests.Plugins.Models.GlobalModelPluginConnector
{
    [TestClass]
    public class PlaylistCollectionTests
    {
        private static bool NoInner(MemberInfo x) => !x.Name.Contains("Inner");
        private static bool NoInnerOrSources(MemberInfo x) => NoInner(x) && x.Name != "get_Sources" && x.Name != "get_SourceCores";

        [TestMethod]
        public void AccessedThroughPlayableCollectionGroup()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugins();
            plugins.PlaylistCollection.Add(x => new PlaylistCollectionPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).PlayableCollectionGroup.Execute(new PlayableCollectionGroupPluginBaseTests.Unimplemented());

            Helpers.AssertAllMembersThrowOnAccess<AccessedException<PlaylistCollectionPluginBaseTests.FullyCustom>, PlaylistCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources,
                typesToExclude: new[]
                {
                    typeof(IAsyncDisposable),
                    typeof(IPlayableCollectionItem)
                });
        }

        [TestMethod]
        public void NotBlockingPlayableCollectionGroup()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugins();
            plugins.PlaylistCollection.Add(x => new PlaylistCollectionPluginBaseTests.FullyCustom(x));
            plugins.PlayableCollectionGroup.Add(x => new PlayableCollectionGroupPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).PlayableCollectionGroup.Execute(new PlayableCollectionGroupPluginBaseTests.Unimplemented());

            // Ensure an PlayableCollectionGroup plugin can still be accessed through PlaylistCollection members.
            Helpers.AssertAllMembersThrowOnAccess<AccessedException<PlayableCollectionGroupPluginBaseTests.FullyCustom>, PlaylistCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources,
                typesToExclude: typeof(IAsyncDisposable));
        }

        [TestMethod]
        public void DisposingPlayableCollectionGroup()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugins();
            plugins.PlaylistCollection.Add(x => new PlaylistCollectionPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).PlayableCollectionGroup.Execute(new PlayableCollectionGroupPluginBaseTests.Unimplemented());

            Helpers.AssertAllThrowsOnMemberAccess<IAsyncDisposable>(value: plugin, expectedExceptions: new[]
            {
                typeof(AccessedException<PlaylistCollectionPluginBaseTests.FullyCustom>),
                typeof(AccessedException<PlayableCollectionGroupPluginBaseTests.Unimplemented>),
            });
        }

        [TestMethod]
        public void AccessedThroughLibrary()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugins();
            plugins.PlaylistCollection.Add(x => new PlaylistCollectionPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).Library.Execute(new LibraryPluginBaseTests.Unimplemented());

            Helpers.AssertAllMembersThrowOnAccess<AccessedException<PlaylistCollectionPluginBaseTests.FullyCustom>, PlaylistCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources,
                typesToExclude: new[]
                {
                    typeof(IAsyncDisposable),
                    typeof(IPlayableCollectionItem)
                });
        }

        [TestMethod]
        public void NotBlockingLibrary()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugins();
            plugins.PlaylistCollection.Add(x => new PlaylistCollectionPluginBaseTests.FullyCustom(x));
            plugins.Library.Add(x => new LibraryPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).Library.Execute(new LibraryPluginBaseTests.Unimplemented());

            // Ensure an Library plugin can still be accessed through PlaylistCollection members.
            Helpers.AssertAllMembersThrowOnAccess<AccessedException<LibraryPluginBaseTests.FullyCustom>, PlaylistCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources,
                typesToExclude: typeof(IAsyncDisposable));
        }

        [TestMethod]
        public void DisposingLibrary()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugins();
            plugins.PlaylistCollection.Add(x => new PlaylistCollectionPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).Library.Execute(new LibraryPluginBaseTests.Unimplemented());

            Helpers.AssertAllThrowsOnMemberAccess<IAsyncDisposable>(value: plugin, expectedExceptions: new[]
            {
                typeof(AccessedException<PlaylistCollectionPluginBaseTests.FullyCustom>),
                typeof(AccessedException<LibraryPluginBaseTests.Unimplemented>),
            });
        }

        [TestMethod]
        public void AccessedThroughDiscoverables()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugins();
            plugins.PlaylistCollection.Add(x => new PlaylistCollectionPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).Discoverables.Execute(new DiscoverablesPluginBaseTests.Unimplemented());

            Helpers.AssertAllMembersThrowOnAccess<AccessedException<PlaylistCollectionPluginBaseTests.FullyCustom>, PlaylistCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources,
                typesToExclude: new[]
                {
                    typeof(IAsyncDisposable),
                    typeof(IPlayableCollectionItem)
                });
        }

        [TestMethod]
        public void NotBlockingDiscoverables()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugins();
            plugins.PlaylistCollection.Add(x => new PlaylistCollectionPluginBaseTests.FullyCustom(x));
            plugins.Discoverables.Add(x => new DiscoverablesPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).Discoverables.Execute(new DiscoverablesPluginBaseTests.Unimplemented());

            // Ensure an Discoverables plugin can still be accessed through PlaylistCollection members.
            Helpers.AssertAllMembersThrowOnAccess<AccessedException<DiscoverablesPluginBaseTests.FullyCustom>, PlaylistCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources,
                typesToExclude: typeof(IAsyncDisposable));
        }

        [TestMethod]
        public void DisposingDiscoverables()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugins();
            plugins.PlaylistCollection.Add(x => new PlaylistCollectionPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).Discoverables.Execute(new DiscoverablesPluginBaseTests.Unimplemented());

            Helpers.AssertAllThrowsOnMemberAccess<IAsyncDisposable>(value: plugin, expectedExceptions: new[]
            {
                typeof(AccessedException<PlaylistCollectionPluginBaseTests.FullyCustom>),
                typeof(AccessedException<DiscoverablesPluginBaseTests.Unimplemented>),
            });
        }

        [TestMethod]
        public void AccessedThroughSearchHistory()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugins();
            plugins.PlaylistCollection.Add(x => new PlaylistCollectionPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).SearchHistory.Execute(new SearchHistoryPluginBaseTests.Unimplemented());

            Helpers.AssertAllMembersThrowOnAccess<AccessedException<PlaylistCollectionPluginBaseTests.FullyCustom>, PlaylistCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources,
                typesToExclude: new[]
                {
                    typeof(IAsyncDisposable),
                    typeof(IPlayableCollectionItem)
                });
        }

        [TestMethod]
        public void NotBlockingSearchHistory()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugins();
            plugins.PlaylistCollection.Add(x => new PlaylistCollectionPluginBaseTests.FullyCustom(x));
            plugins.SearchHistory.Add(x => new SearchHistoryPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).SearchHistory.Execute(new SearchHistoryPluginBaseTests.Unimplemented());

            // Ensure an SearchHistory plugin can still be accessed through PlaylistCollection members.
            Helpers.AssertAllMembersThrowOnAccess<AccessedException<SearchHistoryPluginBaseTests.FullyCustom>, PlaylistCollectionPluginBaseTests.FullyCustom>(
                value: plugin,
                customFilter: NoInnerOrSources,
                typesToExclude: typeof(IAsyncDisposable));
        }

        [TestMethod]
        public void DisposingSearchHistory()
        {
            var plugins = new Sdk.Plugins.Model.SdkModelPlugins();
            plugins.PlaylistCollection.Add(x => new PlaylistCollectionPluginBaseTests.FullyCustom(x));

            var plugin = StrixMusic.Sdk.Plugins.Model.GlobalModelPluginConnector.Create(plugins).SearchHistory.Execute(new SearchHistoryPluginBaseTests.Unimplemented());

            Helpers.AssertAllThrowsOnMemberAccess<IAsyncDisposable>(value: plugin, expectedExceptions: new[]
            {
                typeof(AccessedException<PlaylistCollectionPluginBaseTests.FullyCustom>),
                typeof(AccessedException<SearchHistoryPluginBaseTests.Unimplemented>),
            });
        }
    }
}
