using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StrixMusic.Sdk.PluginModels;
using StrixMusic.Sdk.Plugins.Model;
using StrixMusic.Sdk.Tests.Mock.AppModels;

namespace StrixMusic.Sdk.Tests.PluginModels
{
    [TestClass]
    public class PluginImportTests
    {
        private static readonly ModelPluginMetadata _imagePluginMetadata = new("", "Image plugin import test", "", new Version());
        private static readonly ModelPluginMetadata _metadata = new("", "", "", new Version());

        [TestMethod]
        public async Task LibraryPluginWrapperShouldNotApplyPluginsMultipleTimes()
        {
            var library = new MockLibrary();
            var image = new MockImage();

            var libraryPluginWrapper = new LibraryPluginWrapper(library, new MockStrixDataRoot(), new PlayablePluginContainer(), new ImagePluginContainer());
            await libraryPluginWrapper.AddImageAsync(image, 0);

            var images = await libraryPluginWrapper.GetImagesAsync(1, 0).ToListAsync();

            Assert.AreEqual(1, ((IPluginWrapper)images[0]).ActivePlugins.Image.Count, "Too many plugins were applied.");
        }

        [TestMethod]
        public async Task DiscoverablesPluginWrapperShouldNotApplyPluginsMultipleTimes()
        {
            var discoverables = new MockDiscoverables();
            var image = new MockImage();

            var discoverablesPluginWrapper = new DiscoverablesPluginWrapper(discoverables, new MockStrixDataRoot(), new PlayablePluginContainer(), new ImagePluginContainer());
            await discoverablesPluginWrapper.AddImageAsync(image, 0);

            var images = await discoverablesPluginWrapper.GetImagesAsync(1, 0).ToListAsync();

            Assert.AreEqual(1, ((IPluginWrapper)images[0]).ActivePlugins.Image.Count, "Too many plugins were applied.");
        }

        [TestMethod]
        public async Task RecentlyPlayedPluginShouldNotApplyPluginsMultipleTimes()
        {
            var recentlyPlayed = new MockRecentlyPlayed();
            var image = new MockImage();

            var recentlyPlayedPluginWrapper = new RecentlyPlayedPluginWrapper(recentlyPlayed, new MockStrixDataRoot(), new PlayablePluginContainer(), new ImagePluginContainer());
            await recentlyPlayedPluginWrapper.AddImageAsync(image, 0);

            var images = await recentlyPlayedPluginWrapper.GetImagesAsync(1, 0).ToListAsync();

            Assert.AreEqual(1, ((IPluginWrapper)images[0]).ActivePlugins.Image.Count, "Too many plugins were applied.");
        }

        [TestMethod]
        public async Task SearchHistoryPluginWrapperShouldNotApplyPluginsMultipleTimes()
        {
            var searchHistory = new MockSearchHistory();
            var image = new MockImage();

            var searchHistoryPluginWrapper = new SearchHistoryPluginWrapper(searchHistory, new MockStrixDataRoot(), new PlayablePluginContainer(), new ImagePluginContainer());
            await searchHistoryPluginWrapper.AddImageAsync(image, 0);

            var images = await searchHistoryPluginWrapper.GetImagesAsync(1, 0).ToListAsync();

            Assert.AreEqual(1, ((IPluginWrapper)images[0]).ActivePlugins.Image.Count, "Too many plugins were applied.");
        }

        public class ImagePluginContainer : SdkModelPlugin
        {
            public ImagePluginContainer() : base(_imagePluginMetadata)
            {
                Image.Add(x => new ImagePlugin());
            }
        }

        public class ImagePlugin : ImagePluginBase
        {
            public ImagePlugin() 
                : base(_imagePluginMetadata, new MockImage(), new MockStrixDataRoot())
            {
            }
        }

        public class PlayablePluginContainer : SdkModelPlugin
        {
            public PlayablePluginContainer() : base(_metadata)
            {
                Playable.Add(x => new PlayablePlugin());
            }
        }

        public class PlayablePlugin : PlayablePluginBase
        {
            public PlayablePlugin() 
                : base(_metadata, new MockTrack(), new MockStrixDataRoot())
            {
            }
        }
    }
}
