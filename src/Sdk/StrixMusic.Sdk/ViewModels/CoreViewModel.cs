using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using OwlCore;
using OwlCore.Collections;
using OwlCore.Extensions;
using OwlCore.Provisos;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Data.Merged;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// Contains information about an <see cref="ICore"/>
    /// </summary>
    public class CoreViewModel : ObservableObject, ICore
    {
        private readonly ICore _core;

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreViewModel"/> class.
        /// </summary>
        /// <param name="core">The base <see cref="ICore"/></param>
        /// <remarks>
        /// Creating a new <see cref="CoreViewModel"/> will register itself into <see cref="MainViewModel.Cores"/>.
        /// </remarks>
        public CoreViewModel(ICore core)
        {
            _core = core;

            MainViewModel.Singleton?.Cores.Add(this);

            Library = new LibraryViewModel(new MergedLibrary(_core.Library.IntoList()));

            if (_core.RecentlyPlayed != null)
                RecentlyPlayed = new RecentlyPlayedViewModel(new MergedRecentlyPlayed(_core.RecentlyPlayed.IntoList()));

            if (_core.Discoverables != null)
                Discoverables = new DiscoverablesViewModel(new MergedDiscoverables(_core.Discoverables.IntoList()));

            if (_core.Pins != null)
                Pins = new PlayableCollectionGroupViewModel(new MergedPlayableCollectionGroup(_core.Pins.IntoList()));

            if (_core.Search != null)
                Search = new SearchViewModel(new MergedSearch(_core.Search.IntoList()));
            
            CoreState = _core.CoreState;

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
        private void Core_CoreStateChanged(object sender, CoreState e)
        {
            CoreState = e;

            _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(CoreState)));
        }

        /// <inheritdoc />
        public string InstanceId => _core.InstanceId;

        /// <inheritdoc cref="ICore.Name" />
        public string Name => _core.Name;

        /// <inheritdoc cref="ICore.User" />
        public ICoreUser? User => _core.User;

        /// <inheritdoc cref="ICore.CoreConfig" />
        public ICoreConfig CoreConfig => _core.CoreConfig;

        /// <inheritdoc cref="ICore.CoreState" />
        public CoreState CoreState { get; internal set; }

        /// <inheritdoc />
        public SynchronizedObservableCollection<ICoreDevice> Devices => _core.Devices;

        /// <inheritdoc cref="ICore.Library" />
        ICoreLibrary ICore.Library => _core.Library;

        /// <inheritdoc cref="LibraryViewModel"/>
        public LibraryViewModel Library { get; }

        /// <inheritdoc />
        ICoreSearch? ICore.Search { get; }

        /// <inheritdoc cref="SearchViewModel"/>
        public SearchViewModel? Search { get; }

        /// <inheritdoc cref="ICore.RecentlyPlayed" />
        ICoreRecentlyPlayed? ICore.RecentlyPlayed => _core.RecentlyPlayed;

        /// <inheritdoc cref="RecentlyPlayed"/>
        public RecentlyPlayedViewModel? RecentlyPlayed { get; }

        /// <inheritdoc cref="ICore.Discoverables" />
        ICoreDiscoverables? ICore.Discoverables => _core.Discoverables;

        /// <inheritdoc cref="DiscoverablesViewModel" />
        public DiscoverablesViewModel? Discoverables { get; }

        /// <inheritdoc cref="ICore.Pins" />
        ICorePlayableCollectionGroup? ICore.Pins => _core.Pins;

        /// <inheritdoc cref="ICore.Pins" />
        public PlayableCollectionGroupViewModel? Pins { get; }

        /// <inheritdoc cref="IAsyncInit.InitAsync" />
        public Task InitAsync(IServiceCollection services) => _core.InitAsync(services);

        /// <inheritdoc cref="IAsyncDisposable.DisposeAsync" />
        public async ValueTask DisposeAsync()
        {
            await _core.DisposeAsync().ConfigureAwait(false);
            DetachEvents();
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<ICoreMember> GetContextById(string id) => _core.GetContextById(id);

        /// <inheritdoc />
        public Task<IMediaSourceConfig?> GetMediaSource(ICoreTrack track) => _core.GetMediaSource(track);

        /// <inheritdoc cref="ICore.CoreStateChanged" />
        public event EventHandler<CoreState>? CoreStateChanged
        {
            add => _core.CoreStateChanged += value;

            remove => _core.CoreStateChanged -= value;
        }

        /// <inheritdoc />
        public ICore SourceCore => _core.SourceCore;
    }
}
