using StrixMusic.CoreInterfaces;
using StrixMusic.CoreInterfaces.Interfaces;
using StrixMusic.CoreInterfaces.Interfaces.CoreConfig;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StrixMusic.Core.Mock
{
    public class MockCore : ICore
    {
        public ICoreConfig CoreConfig { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public CoreState CoreState => throw new NotImplementedException();

        public string Name => throw new NotImplementedException();

        public IUser User => throw new NotImplementedException();

        public IReadOnlyList<IDevice> Devices => throw new NotImplementedException();

        public ILibrary Library => throw new NotImplementedException();

        public IRecentlyPlayed RecentlyPlayed => throw new NotImplementedException();

        public IPlayableCollectionGroup Discoverables => throw new NotImplementedException();

        public event EventHandler<CoreState> CoreStateChanged;
        public event EventHandler<CollectionChangedEventArgs<IDevice>> DevicesChanged;

        public ValueTask DisposeAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IAsyncEnumerable<string>> GetSearchAutoCompleteAsync(string query)
        {
            throw new NotImplementedException();
        }

        public Task<ISearchResults> GetSearchResultsAsync(string query)
        {
            throw new NotImplementedException();
        }

        public Task InitAsync()
        {
            return Task.CompletedTask;
        }
    }
}
