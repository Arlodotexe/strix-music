using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using StrixMusic.CoreInterfaces;
using StrixMusic.CoreInterfaces.Interfaces;
using StrixMusic.CoreInterfaces.Interfaces.CoreConfig;

namespace StrixMusic.ViewModels.Bindables
{
    /// <summary>
    /// Contains information about a <see cref="ICore"/>
    /// </summary>
    public class BindableCore : ObservableObject
    {
        private readonly ICore _core;

        /// <summary>
        /// Initializes a new instance of the <see cref="BindableCore"/> class.
        /// </summary>
        /// <param name="core"><inheritdoc cref="ICore"/></param>
        public BindableCore(ICore core)
        {
            _core = core;
            AttachEvents();
        }

        /// <inheritdoc cref="Core.CoreStateChanged" />
        public event EventHandler<CoreState>? CoreStateChanged
        {
            add
            {
                _core.CoreStateChanged += value;
            }

            remove
            {
                _core.CoreStateChanged -= value;
            }
        }

        /// <inheritdoc cref="Core.DevicesChanged" />
        public event EventHandler<CollectionChangedEventArgs<IDevice>>? DevicesChanged
        {
            add
            {
                _core.DevicesChanged += value;
            }

            remove
            {
                _core.DevicesChanged -= value;
            }
        }

        /// <inheritdoc cref="Core.SearchAutoCompleteChanged" />
        public event EventHandler<CollectionChangedEventArgs<IAsyncEnumerable<string>>>? SearchAutoCompleteChanged
        {
            add
            {
                _core.SearchAutoCompleteChanged += value;
            }

            remove
            {
                _core.SearchAutoCompleteChanged -= value;
            }
        }

        /// <inheritdoc cref="Core.SearchAutoCompleteChanged" />
        private void AttachEvents()
        {
            _core.CoreStateChanged += Core_CoreStateChanged;
        }

        private void DetachEvents()
        {
            _core.CoreStateChanged -= Core_CoreStateChanged;
        }

        /// <inheritdoc cref="Core.CoreState" />
        private void Core_CoreStateChanged(object sender, CoreState e) => CoreState = e;

        /// <inheritdoc cref="_core" />
        public IAsyncEnumerable<IDevice> GetDevicesAsync()
        {
            return _core.GetDevicesAsync();
        }

        /// <inheritdoc cref="Core.GetLibraryAsync" />
        public Task<ILibrary> GetLibraryAsync()
        {
            return _core.GetLibraryAsync();
        }

        /// <inheritdoc cref="Core.GetRecentlyPlayedAsync" />
        public Task<IRecentlyPlayed> GetRecentlyPlayedAsync()
        {
            return _core.GetRecentlyPlayedAsync();
        }

        /// <inheritdoc cref="Core.GetDiscoverablesAsync" />
        public IAsyncEnumerable<IPlayableCollectionGroup>? GetDiscoverablesAsync()
        {
            return _core.GetDiscoverablesAsync();
        }

        /// <inheritdoc cref="Core.GetSearchAutoCompleteAsync" />
        public Task<IAsyncEnumerable<string>> GetSearchAutoCompleteAsync(string query)
        {
            return _core.GetSearchAutoCompleteAsync(query);
        }

        /// <inheritdoc cref="Core.GetSearchResultsAsync" />
        public Task<ISearchResults> GetSearchResultsAsync(string query)
        {
            return _core.GetSearchResultsAsync(query);
        }

        /// <inheritdoc cref="Core.InitAsync" />
        public Task InitAsync()
        {
            return _core.InitAsync();
        }

        /// <inheritdoc cref="Core.DisposeAsync" />
        public ValueTask DisposeAsync()
        {
            return _core.DisposeAsync();
        }

        /// <inheritdoc cref="ICore.CoreState" />
        public CoreState CoreState
        {
            get => _core.CoreState;
            set => SetProperty(() => _core.CoreState, value);
        }

        /// <inheritdoc cref="Core.CoreConfig" />
        public ICoreConfig CoreConfig { get => _core.CoreConfig; set => _core.CoreConfig = value; }

        /// <inheritdoc cref="Core.Name" />
        public string Name => _core.Name;

        /// <inheritdoc cref="Core.User" />
        public IUser User => _core.User;
    }
}
