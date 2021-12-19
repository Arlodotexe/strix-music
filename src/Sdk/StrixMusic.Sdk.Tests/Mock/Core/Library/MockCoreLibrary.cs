﻿using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Tests.Mock.Core.Items;
using System.Collections.Generic;

namespace StrixMusic.Sdk.Tests.Mock.Core.Library
{
    public class MockCoreLibrary : MockCorePlayableCollectionGroupBase, ICoreLibrary
    {
        public MockCoreLibrary(ICore sourceCore)
            : base(sourceCore, nameof(MockCoreLibrary), "Library")
        {
        }

        public override async IAsyncEnumerable<ICoreTrack> GetTracksAsync(int limit, int offset)
        {
            await System.Threading.Tasks.Task.CompletedTask;
            yield return MockItemFactory.CreateTrack(SourceCore);
        }

        public override async IAsyncEnumerable<ICoreAlbum> GetAlbumItemsAsync(int limit, int offset)
        {
            await System.Threading.Tasks.Task.CompletedTask;
            yield return MockItemFactory.CreateAlbum(SourceCore);
        }

        public override async IAsyncEnumerable<ICoreArtist> GetArtistItemsAsync(int limit, int offset)
        {
            await System.Threading.Tasks.Task.CompletedTask;
            yield return MockItemFactory.CreateArtist(SourceCore);
        }
    }
}