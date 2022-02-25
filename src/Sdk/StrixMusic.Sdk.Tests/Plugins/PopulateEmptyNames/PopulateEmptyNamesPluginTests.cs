using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StrixMusic.Sdk.Models.Merged;
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
