using StrixMusic.Sdk.Data.Core;
using System;

namespace StrixMusic.Sdk.Tests.Mock.Core.Search
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
