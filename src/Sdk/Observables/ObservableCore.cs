using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using StrixMusic.Sdk.Enums;
using StrixMusic.Sdk.Interfaces;

namespace StrixMusic.Sdk.Observables
{
    /// <summary>
    /// Contains information about a <see cref="ICore"/>
    /// </summary>
    public class ObservableCore : ObservableObject, ICore
    {
        private readonly ICore _core;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableCore"/> class.
        /// </summary>
        /// <param name="core">The base <see cref="ICore"/></param>
        /// <remarks>
        /// Creating a new <see cref="ObservableCore"/> will register itself into <see cref="MainViewModel.LoadedCores"/>.
        /// </remarks>
        public ObservableCore(ICore core)
        {
            _core = core;

            MainViewModel.Singleton?.LoadedCores.Add(this);

            Devices = new ObservableCollection<IDevice>(_core.Devices.Select(x => new ObservableDevice(x)));
            Library = new ObservableLibrary(_core.Library);
            RecentlyPlayed = new ObservableRecentlyPlayed(_core.RecentlyPlayed);
            Pins = new ObservableCollection<IPlayable>(_core.Pins);
            Discoverables = new ObservableDiscoverables(_core.Discoverables);

            AttachEvents();
        }

        private void AttachEvents()
        {
            _core.CoreStateChanged += Core_CoreStateChanged;
        }

        private void DetachEvents()
        {
            _core.CoreStateChanged -= Core_CoreStateChanged;
        }

        /// <inheritdoc cref="ICore.CoreState" />
        private void Core_CoreStateChanged(object sender, CoreState e) => CoreState = e;

        /// <inheritdoc />
        public string InstanceId => _core.InstanceId;

        /// <inheritdoc cref="ICore.Name" />
        public string Name => _core.Name;

        /// <inheritdoc cref="ICore.User" />
        public IUser User => _core.User;

        /// <inheritdoc cref="ICore.CoreConfig" />
        public ICoreConfig CoreConfig => _core.CoreConfig;

        /// <inheritdoc cref="ICore.CoreState" />
        public CoreState CoreState
        {
            get => _core.CoreState;
            set => SetProperty(() => _core.CoreState, value);
        }

        /// <inheritdoc />
        public ObservableCollection<IDevice> Devices { get; }

        /// <inheritdoc cref="ICore.Library" />
        public ILibrary Library { get; }

        /// <inheritdoc cref="ICore.RecentlyPlayed" />
        public IRecentlyPlayed RecentlyPlayed { get; }

        /// <inheritdoc cref="ICore.Discoverables" />
        public IDiscoverables Discoverables { get; }

        /// <inheritdoc cref="ICore.Pins" />
        public ObservableCollection<IPlayable> Pins { get; }

        /// <inheritdoc cref="ICore.GetSearchAutoCompleteAsync" />
        public IAsyncEnumerable<string> GetSearchAutoCompleteAsync(string query) => _core.GetSearchAutoCompleteAsync(query);

        /// <inheritdoc cref="ICore.GetSearchResultsAsync" />
        public Task<ISearchResults> GetSearchResultsAsync(string query) => _core.GetSearchResultsAsync(query);

        /// <inheritdoc cref="ICore.InitAsync" />
        public Task InitAsync() => _core.InitAsync();

        /// <inheritdoc cref="IAsyncDisposable.DisposeAsync" />
        public async ValueTask DisposeAsync()
        {
            await _core.DisposeAsync().ConfigureAwait(true);
            DetachEvents();
        }

        /// <inheritdoc />
        public Task<bool> IsAddPinSupported(int index) => _core.IsAddPinSupported(index);

        /// <inheritdoc />
        public ObservableCollection<bool> IsRemovePinSupportedMap => _core.IsRemovePinSupportedMap;

        /// <inheritdoc cref="ICore.CoreStateChanged" />
        public event EventHandler<CoreState>? CoreStateChanged
        {
            add => _core.CoreStateChanged += value;

            remove => _core.CoreStateChanged -= value;
        }
    }
}
