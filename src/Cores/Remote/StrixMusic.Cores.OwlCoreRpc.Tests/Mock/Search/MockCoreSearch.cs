using StrixMusic.Sdk.Models.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StrixMusic.Cores.OwlCoreRpc.Tests.Mock.Search
{
    public class MockCoreSearch : ICoreSearch
    {
        public MockCoreSearch(ICore sourceCore)
        {
            SourceCore = sourceCore;
            SearchHistory = new MockCoreSearchHistory(sourceCore);
        }

        public ICoreSearchHistory? SearchHistory { get; set; }

        public ICore SourceCore { get; set; }

        public ValueTask DisposeAsync()
        {
            throw new System.NotImplementedException();
        }

        public IAsyncEnumerable<ICoreSearchQuery> GetRecentSearchQueries()
        {
            throw new System.NotImplementedException();
        }

        public IAsyncEnumerable<string> GetSearchAutoCompleteAsync(string query)
        {
            return AsyncEnumerable.Empty<string>();
        }

        public Task<ICoreSearchResults> GetSearchResultsAsync(string query)
        {
            return Task.FromResult<ICoreSearchResults>(new MockCoreSearchResults(SourceCore, query));
        }
    }
}
