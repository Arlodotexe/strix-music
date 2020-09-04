using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using StrixMusic.Core.Mock.Deserialization;
using StrixMusic.Core.Mock.Models;
using StrixMusic.Core.Mock.Services;
using StrixMusic.CoreInterfaces;
using StrixMusic.CoreInterfaces.Interfaces;
using StrixMusic.CoreInterfaces.Interfaces.CoreConfig;

namespace StrixMusic.Core.Mock
{
    /// <summary>
    /// Mock Core
    /// </summary>
    public class MockCore : ICore
    {
        private ServiceProvider _serviceProvider;
        /// <summary>
        /// Initializes a new instance of the <see cref="MockCore"/> class.
        /// </summary>
        /// <param name="instanceId"></param>
        public MockCore(string instanceId)
        {
            InstanceId = instanceId;
            CoreConfig = new MockCoreConfig(this);
        }

        /// <inheritdoc/>
        public MockLibrary _serializedLibaray { get; set; }

        /// <inheritdoc/>
        public ICoreConfig CoreConfig { get; }

        /// <inheritdoc/>
        public CoreState CoreState => throw new NotImplementedException();

        /// <inheritdoc/>
        public string Name => throw new NotImplementedException();

        /// <inheritdoc/>
        public IUser User => throw new NotImplementedException();

        /// <inheritdoc/>
        public IReadOnlyList<IDevice> Devices => throw new NotImplementedException();

        /// <inheritdoc/>
        public ILibrary Library => throw new NotImplementedException();

        /// <inheritdoc/>
        public IRecentlyPlayed RecentlyPlayed => throw new NotImplementedException();

        /// <inheritdoc/>
        public IPlayableCollectionGroup Discoverables => throw new NotImplementedException();

        /// <inheritdoc/>
        public string InstanceId { get; }

        /// <inheritdoc/>
        public event EventHandler<CoreState> CoreStateChanged;

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<IDevice>> DevicesChanged;

        /// <inheritdoc/>
        public ValueTask DisposeAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<IAsyncEnumerable<string>> GetSearchAutoCompleteAsync(string query)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<ISearchResults> GetSearchResultsAsync(string query)
        {
            return Task.FromResult(new MockSearchResults(_serializedLibaray) as ISearchResults);
        }

        /// <inheritdoc/>
        public async Task InitAsync()
        {
            var service = CoreConfig.Services.BuildServiceProvider().GetService(typeof(JsonMockCoreDataService)) as JsonMockCoreDataService;
            _serializedLibaray = await service.GetLibraryAsync();
        }
    }
}
