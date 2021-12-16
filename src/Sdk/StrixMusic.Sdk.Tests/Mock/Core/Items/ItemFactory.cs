using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Tests.Mock.Core.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Tests.Mock.Core
{
    internal static class MockItemFactory
    {
        public static ICoreTrack CreateTrack(ICore sourceCore) => new MockCoreTrack(sourceCore, "factoryTrack", "Test track");
        
        public static ICoreArtist CreateArtist(ICore sourceCore) => new MockCoreArtist(sourceCore, "factoryArtist", "Test Artist");
        
        public static ICoreAlbum CreateAlbum(ICore sourceCore) => new MockCoreAlbum(sourceCore, "factoryAlbum", "Test Album");

        public static ICorePlaylist CreatePlaylist(ICore sourceCore) => new MockCorePlaylist(sourceCore, "factoryPlaylist", "Test Playlist");
    }
}
