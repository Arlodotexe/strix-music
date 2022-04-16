using System;
using OwlCore.Extensions;
using StrixMusic.Sdk.Models;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Cores.OwlCoreRpc.Tests.Mock;
using StrixMusic.Cores.OwlCoreRpc.Tests.Mock.Items;
using StrixMusic.Sdk.AdapterModels;

namespace StrixMusic.Sdk.Tests.Mock
{
    internal static class MockItemFactory
    {
        #warning TODO: Clean up when removing merged item dependency on settings service.
        public static ITrack CreateTrack(ICore sourceCore) => new MergedTrack(MockCoreItemFactory.CreateTrack(sourceCore).IntoList(), null!);

        public static IArtist CreateArtist(ICore sourceCore) => new MergedArtist(MockCoreItemFactory.CreateArtist(sourceCore).IntoList(), null!);

        public static IAlbum CreateAlbum(ICore sourceCore) => new MergedAlbum(MockCoreItemFactory.CreateAlbum(sourceCore).IntoList(), null!);

        public static IPlaylist CreatePlaylist(ICore sourceCore) => new MergedPlaylist(MockCoreItemFactory.CreatePlaylist(sourceCore).IntoList(), null!);

        public static IPlayableCollectionGroup CreatePlayableCollectionGroup(ICore sourceCore) => new MergedPlayableCollectionGroup(MockCoreItemFactory.CreatePlayableCollectionGroup(sourceCore).IntoList(), null!);

        public static IImage CreateImage(ICore sourceCore) => new MergedImage(MockCoreItemFactory.CreateImage(sourceCore).IntoList());

        public static IUrl CreateUrl(ICore sourceCore) => new MergedUrl(MockCoreItemFactory.CreateUrl(sourceCore).IntoList());
    }
}
