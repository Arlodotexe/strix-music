using System;
using StrixMusic.Sdk.CoreModels;

namespace StrixMusic.Cores.OwlCoreRpc.Tests.Mock.Search
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
