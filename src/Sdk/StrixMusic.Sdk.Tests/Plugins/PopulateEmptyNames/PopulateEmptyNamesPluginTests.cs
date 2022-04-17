using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StrixMusic.Sdk.AdapterModels;
using StrixMusic.Sdk.Plugins.Model;
using StrixMusic.Sdk.Plugins.PopulateEmptyNames;
using StrixMusic.Sdk.Tests.Mock;
using StrixMusic.Sdk.Tests.Mock.Core;

namespace StrixMusic.Sdk.Tests.Plugins.PopulateEmptyNames
{
    [TestClass]
    public class PopulateEmptyNamesPluginTests
    {
        private const string IncorrectPluginCountMsg = "Exactly one empty name plugin should exist per item type.";

        [TestMethod]
        public void PopulatesEmptyNames()
        {
            var plugin = CreatePlugin();

            Assert.AreNotEqual(string.Empty, plugin.EmptyAlbumName);
            Assert.AreNotEqual(string.Empty, plugin.EmptyArtistName);
            Assert.AreNotEqual(string.Empty, plugin.EmptyTrackName);
            Assert.AreNotEqual(string.Empty, plugin.EmptyPlaylistName);
            Assert.AreNotEqual(string.Empty, plugin.EmptyDefaultName);

            Assert.AreEqual(1, plugin.Album.Count, IncorrectPluginCountMsg);
            Assert.AreEqual(1, plugin.Artist.Count, IncorrectPluginCountMsg);
            Assert.AreEqual(1, plugin.Playlist.Count, IncorrectPluginCountMsg);
            Assert.AreEqual(1, plugin.Track.Count, IncorrectPluginCountMsg);
            Assert.AreEqual(1, plugin.Playable.Count, IncorrectPluginCountMsg);

            var core = new MockCore();
            var album = (MergedAlbum)MockItemFactory.CreateAlbum(core);
            var artist = (MergedArtist)MockItemFactory.CreateArtist(core);
            var playlist = (MergedPlaylist)MockItemFactory.CreatePlaylist(core);
            var track = (MergedTrack)MockItemFactory.CreateTrack(core);
            var playableCollectionGroup = (MergedPlayableCollectionGroup)MockItemFactory.CreatePlayableCollectionGroup(core);

            album.Name = string.Empty;
            artist.Name = string.Empty;
            playlist.Name = string.Empty;
            track.Name = string.Empty;
            playableCollectionGroup.Name = string.Empty;

            var albumWithPlugin = plugin.Album.Execute(album);
            var artistWithPlugin = plugin.Artist.Execute(artist);
            var playlistWithPlugin = plugin.Playlist.Execute(playlist);
            var trackWithPlugin = plugin.Track.Execute(track);
            var playableWithPlugin = plugin.Playable.Execute(playableCollectionGroup);

            Assert.AreEqual(plugin.EmptyAlbumName, albumWithPlugin.Name);
            Assert.AreEqual(plugin.EmptyArtistName, artistWithPlugin.Name);
            Assert.AreEqual(plugin.EmptyPlaylistName, playlistWithPlugin.Name);
            Assert.AreEqual(plugin.EmptyTrackName, trackWithPlugin.Name);
            Assert.AreEqual(plugin.EmptyDefaultName, playableWithPlugin.Name);
        }

        [TestMethod]
        public void DoesNotOverrideNonEmptyString()
        {
            var plugin = CreatePlugin();

            Assert.AreNotEqual(string.Empty, plugin.EmptyAlbumName);
            Assert.AreNotEqual(string.Empty, plugin.EmptyArtistName);
            Assert.AreNotEqual(string.Empty, plugin.EmptyTrackName);
            Assert.AreNotEqual(string.Empty, plugin.EmptyPlaylistName);
            Assert.AreNotEqual(string.Empty, plugin.EmptyDefaultName);

            Assert.AreEqual(1, plugin.Album.Count, IncorrectPluginCountMsg);
            Assert.AreEqual(1, plugin.Artist.Count, IncorrectPluginCountMsg);
            Assert.AreEqual(1, plugin.Playlist.Count, IncorrectPluginCountMsg);
            Assert.AreEqual(1, plugin.Track.Count, IncorrectPluginCountMsg);
            Assert.AreEqual(1, plugin.Playable.Count, IncorrectPluginCountMsg);

            var core = new MockCore();
            var album = (MergedAlbum)MockItemFactory.CreateAlbum(core);
            var artist = (MergedArtist)MockItemFactory.CreateArtist(core);
            var playlist = (MergedPlaylist)MockItemFactory.CreatePlaylist(core);
            var track = (MergedTrack)MockItemFactory.CreateTrack(core);
            var playableCollectionGroup = (MergedPlayableCollectionGroup)MockItemFactory.CreatePlayableCollectionGroup(core);

            var albumWithPlugin = plugin.Album.Execute(album);
            var artistWithPlugin = plugin.Artist.Execute(artist);
            var playlistWithPlugin = plugin.Playlist.Execute(playlist);
            var trackWithPlugin = plugin.Track.Execute(track);
            var playableWithPlugin = plugin.Playable.Execute(playableCollectionGroup);

            // Ensure original values are not empty
            Assert.AreNotEqual(string.Empty, album.Name);
            Assert.AreNotEqual(string.Empty, artist.Name);
            Assert.AreNotEqual(string.Empty, playlist.Name);
            Assert.AreNotEqual(string.Empty, track.Name);
            Assert.AreNotEqual(string.Empty, playableCollectionGroup.Name);

            // Ensure original values were not overriden
            Assert.AreEqual(album.Name, albumWithPlugin.Name);
            Assert.AreEqual(artist.Name, artistWithPlugin.Name);
            Assert.AreEqual(playlist.Name, playlistWithPlugin.Name);
            Assert.AreEqual(track.Name, trackWithPlugin.Name);
            Assert.AreEqual(playableCollectionGroup.Name, playableWithPlugin.Name);
        }

        [TestMethod]
        public void AppliesReassignedEmptyValues()
        {
            var plugin = CreatePlugin();

            Assert.AreNotEqual(string.Empty, plugin.EmptyAlbumName);
            Assert.AreNotEqual(string.Empty, plugin.EmptyArtistName);
            Assert.AreNotEqual(string.Empty, plugin.EmptyTrackName);
            Assert.AreNotEqual(string.Empty, plugin.EmptyPlaylistName);
            Assert.AreNotEqual(string.Empty, plugin.EmptyDefaultName);

            var core = new MockCore();
            var album = (MergedAlbum)MockItemFactory.CreateAlbum(core);
            var artist = (MergedArtist)MockItemFactory.CreateArtist(core);
            var playlist = (MergedPlaylist)MockItemFactory.CreatePlaylist(core);
            var track = (MergedTrack)MockItemFactory.CreateTrack(core);
            var playableCollectionGroup = (MergedPlayableCollectionGroup)MockItemFactory.CreatePlayableCollectionGroup(core);

            album.Name = string.Empty;
            artist.Name = string.Empty;
            playlist.Name = string.Empty;
            track.Name = string.Empty;
            playableCollectionGroup.Name = string.Empty;

            AssertModelWithPluginEqualProvidedValues();

            plugin.EmptyAlbumName = Guid.NewGuid().ToString();
            plugin.EmptyArtistName = Guid.NewGuid().ToString();
            plugin.EmptyTrackName = Guid.NewGuid().ToString();
            plugin.EmptyPlaylistName = Guid.NewGuid().ToString();
            plugin.EmptyDefaultName = Guid.NewGuid().ToString();

            AssertModelWithPluginEqualProvidedValues();

            void AssertModelWithPluginEqualProvidedValues()
            {
                Assert.AreEqual(1, plugin.Album.Count, IncorrectPluginCountMsg);
                Assert.AreEqual(1, plugin.Artist.Count, IncorrectPluginCountMsg);
                Assert.AreEqual(1, plugin.Playlist.Count, IncorrectPluginCountMsg);
                Assert.AreEqual(1, plugin.Track.Count, IncorrectPluginCountMsg);
                Assert.AreEqual(1, plugin.Playable.Count, IncorrectPluginCountMsg);

                var albumWithPlugin = plugin.Album.Execute(album);
                var artistWithPlugin = plugin.Artist.Execute(artist);
                var playlistWithPlugin = plugin.Playlist.Execute(playlist);
                var trackWithPlugin = plugin.Track.Execute(track);
                var playableWithPlugin = plugin.Playable.Execute(playableCollectionGroup);

                Assert.AreEqual(plugin.EmptyAlbumName, albumWithPlugin.Name);
                Assert.AreEqual(plugin.EmptyArtistName, artistWithPlugin.Name);
                Assert.AreEqual(plugin.EmptyPlaylistName, playlistWithPlugin.Name);
                Assert.AreEqual(plugin.EmptyTrackName, trackWithPlugin.Name);
                Assert.AreEqual(plugin.EmptyDefaultName, playableWithPlugin.Name);
            }
        }

        [DataRow("album")]
        [DataRow("artist")]
        [DataRow("playlist")]
        [DataRow("track")]
        [DataRow("default")]
        [TestMethod]
        public void AssigningEmptyValueRemovesPlugin(string target)
        {
            var plugin = CreatePlugin();

            Assert.AreEqual(1, plugin.Album.Count, IncorrectPluginCountMsg);
            Assert.AreEqual(1, plugin.Artist.Count, IncorrectPluginCountMsg);
            Assert.AreEqual(1, plugin.Playlist.Count, IncorrectPluginCountMsg);
            Assert.AreEqual(1, plugin.Track.Count, IncorrectPluginCountMsg);
            Assert.AreEqual(1, plugin.Playable.Count, IncorrectPluginCountMsg);

            Assert.AreNotEqual(string.Empty, plugin.EmptyAlbumName);
            Assert.AreNotEqual(string.Empty, plugin.EmptyArtistName);
            Assert.AreNotEqual(string.Empty, plugin.EmptyTrackName);
            Assert.AreNotEqual(string.Empty, plugin.EmptyPlaylistName);
            Assert.AreNotEqual(string.Empty, plugin.EmptyDefaultName);

            switch (target)
            {
                case "album":
                    plugin.EmptyAlbumName = string.Empty;
                    Assert.AreEqual(0, plugin.Album.Count);
                    break;
                case "artist":
                    plugin.EmptyArtistName = string.Empty;
                    Assert.AreEqual(0, plugin.Artist.Count);
                    break;
                case "playlist":
                    plugin.EmptyPlaylistName = string.Empty;
                    Assert.AreEqual(0, plugin.Playlist.Count);
                    break;
                case "track":
                    plugin.EmptyTrackName = string.Empty;
                    Assert.AreEqual(0, plugin.Track.Count);
                    break;
                case "default":
                    plugin.EmptyDefaultName = string.Empty;
                    Assert.AreEqual(0, plugin.Playable.Count);
                    break;
                default:
                    Assert.Fail();
                    break;
            }
        }

        [DataRow("album")]
        [DataRow("artist")]
        [DataRow("playlist")]
        [DataRow("track")]
        [TestMethod]
        public void GlobalPluginConnectorAppliesDefaultTo(string target)
        {
            var plugin = CreatePlugin();

            Assert.AreNotEqual(string.Empty, plugin.EmptyAlbumName);
            Assert.AreNotEqual(string.Empty, plugin.EmptyArtistName);
            Assert.AreNotEqual(string.Empty, plugin.EmptyTrackName);
            Assert.AreNotEqual(string.Empty, plugin.EmptyPlaylistName);
            Assert.AreNotEqual(string.Empty, plugin.EmptyDefaultName);

            var core = new MockCore();
            var album = (MergedAlbum)MockItemFactory.CreateAlbum(core);
            var artist = (MergedArtist)MockItemFactory.CreateArtist(core);
            var playlist = (MergedPlaylist)MockItemFactory.CreatePlaylist(core);
            var track = (MergedTrack)MockItemFactory.CreateTrack(core);

            album.Name = string.Empty;
            artist.Name = string.Empty;
            playlist.Name = string.Empty;
            track.Name = string.Empty;
            
            // Clear all plugins that should be covered by default
            plugin.EmptyAlbumName = string.Empty;
            plugin.EmptyArtistName = string.Empty;
            plugin.EmptyPlaylistName = string.Empty;
            plugin.EmptyTrackName = string.Empty;
            plugin.Album.Clear();
            plugin.Artist.Clear();
            plugin.Playlist.Clear();
            plugin.Track.Clear();

            Assert.AreEqual(0, plugin.Album.Count);
            Assert.AreEqual(0, plugin.Artist.Count);
            Assert.AreEqual(0, plugin.Playlist.Count);
            Assert.AreEqual(0, plugin.Track.Count);
            Assert.AreEqual(1, plugin.Playable.Count, IncorrectPluginCountMsg);

            var connectedPlugin = GlobalModelPluginConnector.Create(plugin);
            
            switch (target)
            {
                case "album":
                    Assert.AreNotEqual(album.Name, connectedPlugin.Album.Execute(album).Name);
                    break;
                case "artist":
                    Assert.AreNotEqual(artist.Name, connectedPlugin.Artist.Execute(artist).Name);
                    break;
                case "playlist":
                    Assert.AreNotEqual(playlist.Name, connectedPlugin.Playlist.Execute(playlist).Name);
                    break;
                case "track":
                    Assert.AreNotEqual(track.Name, connectedPlugin.Track.Execute(track).Name);
                    break;
                default:
                    Assert.Fail();
                    break;
            }
        }

        private static PopulateEmptyNamesPlugin CreatePlugin() => new()
        {
            EmptyAlbumName = "EmptyAlbumName",
            EmptyArtistName = "EmptyArtistName",
            EmptyPlaylistName = "EmptyPlaylistName",
            EmptyTrackName = "EmptyTrackName",
            EmptyDefaultName = "EmptyName",
        };
    }
}
