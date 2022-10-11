using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StrixMusic.Sdk.PluginModels;
using StrixMusic.Sdk.Plugins.Model;
using StrixMusic.Sdk.Tests.Mock.AppModels;

namespace StrixMusic.Sdk.Tests.PluginModels;

[TestClass]
public class PluginModelObjectTree
{
    private static SdkModelPlugin _modelPlugin = new(new ModelPluginMetadata(string.Empty, string.Empty, string.Empty, new Version()));
    
    [TestMethod]
    public void VerifyAppCore()
    {
        var mock = new MockStrixDataRoot();
        var plugin = new StrixDataRootPluginWrapper(mock, _modelPlugin);

        VerifyModelObjectTree.VerifyReturns<IPluginWrapper>(plugin);
    }
    
    [TestMethod]
    public async Task VerifyPlayableCollectionGroup()
    {
        var mock = new MockPlayableCollectionGroup();
        var plugin = new PlayableCollectionGroupPluginWrapper(mock, new MockStrixDataRoot(), _modelPlugin);

        await VerifyModelObjectTree.VerifyReturnsAsync<IPluginWrapper>(plugin);
    }
    
    [TestMethod]
    public async Task VerifyLibrary()
    {
        var mock = new MockLibrary();
        var plugin = new LibraryPluginWrapper(mock, new MockStrixDataRoot(), _modelPlugin);

        await VerifyModelObjectTree.VerifyReturnsAsync<IPluginWrapper>(plugin);
    }
    
    [TestMethod]
    public async Task VerifyDiscoverables()
    {
        var mock = new MockDiscoverables();
        var plugin = new DiscoverablesPluginWrapper(mock, new MockStrixDataRoot(), _modelPlugin);

        await VerifyModelObjectTree.VerifyReturnsAsync<IPluginWrapper>(plugin);
    }
    
    [TestMethod]
    public async Task VerifyRecentlyPlayed()
    {
        var mock = new MockRecentlyPlayed();
        var plugin = new RecentlyPlayedPluginWrapper(mock, new MockStrixDataRoot(), _modelPlugin);

        await VerifyModelObjectTree.VerifyReturnsAsync<IPluginWrapper>(plugin);
    }
    
    [TestMethod]
    public async Task VerifySearchHistory()
    {
        var mock = new MockSearchHistory();
        var plugin = new SearchHistoryPluginWrapper(mock, new MockStrixDataRoot(), _modelPlugin);

        await VerifyModelObjectTree.VerifyReturnsAsync<IPluginWrapper>(plugin);
    }
    
    [TestMethod]
    public async Task VerifyTrack()
    {
        var mock = new MockTrack();
        var plugin = new TrackPluginWrapper(mock, new MockStrixDataRoot(), _modelPlugin);

        await VerifyModelObjectTree.VerifyReturnsAsync<IPluginWrapper>(plugin);
    }
    
    [TestMethod]
    public async Task VerifyTrackCollection()
    {
        var mock = new MockTrackCollection();
        var plugin = new TrackCollectionPluginWrapper(mock, new MockStrixDataRoot(), _modelPlugin);

        await VerifyModelObjectTree.VerifyReturnsAsync<IPluginWrapper>(plugin);
    }
    
    [TestMethod]
    public async Task VerifyArtist()
    {
        var mock = new MockArtist();
        var plugin = new ArtistPluginWrapper(mock, new MockStrixDataRoot(), _modelPlugin);

        await VerifyModelObjectTree.VerifyReturnsAsync<IPluginWrapper>(plugin);
    }
    
    [TestMethod]
    public async Task VerifyArtistCollection()
    {
        var mock = new MockArtistCollection();
        var plugin = new ArtistCollectionPluginWrapper(mock, new MockStrixDataRoot(), _modelPlugin);

        await VerifyModelObjectTree.VerifyReturnsAsync<IPluginWrapper>(plugin);
    }
    
    [TestMethod]
    public async Task VerifyAlbum()
    {
        var mock = new MockAlbum();
        var plugin = new AlbumPluginWrapper(mock, new MockStrixDataRoot(), _modelPlugin);

        await VerifyModelObjectTree.VerifyReturnsAsync<IPluginWrapper>(plugin);
    }
    
    [TestMethod]
    public async Task VerifyAlbumCollection()
    {
        var mock = new MockAlbumCollection();
        var plugin = new AlbumCollectionPluginWrapper(mock, new MockStrixDataRoot(), _modelPlugin);

        await VerifyModelObjectTree.VerifyReturnsAsync<IPluginWrapper>(plugin);
    }
    
    [TestMethod]
    public async Task VerifyPlaylist()
    {
        var mock = new MockPlaylist
        {
            RelatedItems = new MockPlayableCollectionGroup(),
        };
        
        var plugin = new PlaylistPluginWrapper(mock, new MockStrixDataRoot(), _modelPlugin);

        await VerifyModelObjectTree.VerifyReturnsAsync<IPluginWrapper>(plugin);
    }
    
    [TestMethod]
    public async Task VerifyPlaylistCollection()
    {
        var mock = new MockPlaylistCollection();
        var plugin = new PlaylistCollectionPluginWrapper(mock, new MockStrixDataRoot(), _modelPlugin);

        await VerifyModelObjectTree.VerifyReturnsAsync<IPluginWrapper>(plugin);
    }
}
