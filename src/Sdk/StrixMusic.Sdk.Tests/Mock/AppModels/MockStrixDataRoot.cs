using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OwlCore.ComponentModel;
using StrixMusic.Sdk.AdapterModels;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;

namespace StrixMusic.Sdk.Tests.Mock.AppModels
{
    public class MockStrixDataRoot : IStrixDataRoot
    {
        private readonly List<IDevice> _devices;

        public MockStrixDataRoot()
        {
            _devices = new List<IDevice>();
        }

        public bool Equals(ICore? other) => false;

        public IReadOnlyList<ICore> Sources { get; } = new List<ICore>();

        public Task InitAsync(CancellationToken cancellationToken = default)
        {
            IsInitialized = true;
            return Task.CompletedTask;
        }

        public bool IsInitialized { get; private set; }

        public ValueTask DisposeAsync()
        {
            IsInitialized = false;
            return default;
        }

        public MergedCollectionConfig MergeConfig { get; } = new();

        public IReadOnlyList<IDevice> Devices => _devices;

        public ILibrary Library { get; } = new MockLibrary();

        public IPlayableCollectionGroup? Pins { get; } = new MockPlayableCollectionGroup();

        public ISearch? Search { get; } = new MockSearch();

        public IRecentlyPlayed? RecentlyPlayed { get; } = new MockRecentlyPlayed();

        public IDiscoverables? Discoverables { get; } = new MockDiscoverables();
        public event EventHandler<IPlayableCollectionGroup>? PinsChanged;
        public event EventHandler<ISearch>? SearchChanged;
        public event EventHandler<IRecentlyPlayed>? RecentlyPlayedChanged;
        public event EventHandler<IDiscoverables>? DiscoverablesChanged;

        public event CollectionChangedEventHandler<IDevice>? DevicesChanged;
        public event EventHandler? SourcesChanged;
    }

}
