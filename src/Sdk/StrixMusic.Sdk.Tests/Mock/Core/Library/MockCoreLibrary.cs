using StrixMusic.Sdk.Models.Core;
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
    }
}
