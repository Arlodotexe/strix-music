using StrixMusic.Sdk.Models.Core;
using System;

namespace StrixMusic.Cores.Remote.OwlCore.Tests.Mock.Search
{
    public class MockCoreSearchQuery : ICoreSearchQuery
    {
        public MockCoreSearchQuery(ICore sourceCore, string query)
        {
            SourceCore = sourceCore;
            Query = query;
        }

        public string Query { get; set; }

        public DateTime CreatedAt { get; set; }

        public ICore SourceCore { get; set; }
    }
}
