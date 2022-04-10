// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using OwlCore.AbstractUI.Models;
using OwlCore.AbstractUI.ViewModels;
using OwlCore.Events;
using OwlCore.Extensions;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Models;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Models.Merged;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// A ViewModel for <see cref="ICore"/>.
    /// </summary>
    public sealed class CoreViewModel : ObservableObject, ISdkViewModel, ICore
    {
        private readonly ICore _core;
        private readonly SynchronizationContext _syncContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreViewModel"/> class.
        /// </summary>
        /// <param name="core">The <see cref="ICore"/> to wrap around.</param>
        /// <param name="coreMetadata">The metadata that was used to construct this core instance.</param>
        /// <param name="root">The <see cref="MainViewModel"/> that this or the object that created this originated from.</param>
        /// <remarks>
        /// Creating a new <see cref="CoreViewModel"/> will register itself into <see cref="MainViewModel.Cores"/> on <paramref name="root"/>.
        /// </remarks>
        internal CoreViewModel(MainViewModel root, ICore core, CoreMetadata coreMetadata)
        {
            _syncContext = SynchronizationContext.Current;

            Root = root;
            _core = core;

            root.Cores.Add(this);

            DisplayName = coreMetadata.DisplayName;
            LogoUri = coreMetadata.LogoUri;

            Library = new LibraryViewModel(root, new MergedLibrary(_core.Library.IntoList(), root.MergeConfig));

            if (_core.RecentlyPlayed != null)
                RecentlyPlayed = new RecentlyPlayedViewModel(root, new MergedRecentlyPlayed(_core.RecentlyPlayed.IntoList(), root.MergeConfig));

            if (_core.Discoverables != null)
                Discoverables = new DiscoverablesViewModel(root, new MergedDiscoverables(_core.Discoverables.IntoList(), root.MergeConfig));

            if (_core.Pins != null)
                Pins = new PlayableCollectionGroupViewModel(root, new MergedPlayableCollectionGroup(_core.Pins.IntoList(), root.MergeConfig));

            if (_core.Search != null)
                Search = new SearchViewModel(root, new MergedSearch(_core.Search.IntoList(), root.MergeConfig));

            Devices = new ObservableCollection<DeviceViewModel>();

            CoreState = _core.CoreState;

            AbstractConfigPanel = new AbstractUICollectionViewModel(_core.AbstractConfigPanel);

            AttachEvents();
        }

        private void AttachEvents()
        {
            _core.CoreStateChanged += Core_CoreStateChanged;
            _core.DevicesChanged += Core_DevicesChanged;
            _core.InstanceDescriptorChanged += Core_InstanceDescriptorChanged;
            _core.AbstractConfigPanelChanged += OnAbstractConfigPanelChanged;
        }

        private void OnAbstractConfigPanelChanged(object sender, EventArgs e) => _syncContext.Post(_ =>
        {
            AbstractConfigPanel = new AbstractUICollectionViewModel(_core.AbstractConfigPanel);
            OnPropertyChanged(nameof(AbstractConfigPanel));
        }, null);

        private void DetachEvents()
        {
            _core.CoreStateChanged -= Core_CoreStateChanged;
            _core.DevicesChanged -= Core_DevicesChanged;
            _core.InstanceDescriptorChanged -= Core_InstanceDescriptorChanged;
            _core.AbstractConfigPanelChanged -= OnAbstractConfigPanelChanged;
        }

        private void Core_DevicesChanged(object sender, IReadOnlyList<CollectionChangedItem<ICoreDevice>> addedItems, IReadOnlyList<CollectionChangedItem<ICoreDevice>> removedItems) => _syncContext.Post(_ =>
        {
            Devices.ChangeCollection(addedItems, removedItems, item => new DeviceViewModel(Root, new CoreDeviceProxy(item.Data)));
        }, null);

        private void Core_InstanceDescriptorChanged(object sender, string e) => _syncContext.Post(_ =>
        {
            OnPropertyChanged(nameof(InstanceDescriptor));
            InstanceDescriptorChanged?.Invoke(sender, e);
        }, null);

        /// <inheritdoc cref="ICore.CoreState" />
        private void Core_CoreStateChanged(object sender, CoreState e)
        {
            CoreState = e;

            _syncContext.Post(_ =>
            {
                OnPropertyChanged(nameof(CoreState));

                OnPropertyChanged(nameof(IsCoreStateUnloaded));
                OnPropertyChanged(nameof(IsCoreStateConfiguring));
                OnPropertyChanged(nameof(IsCoreStateConfigured));
                OnPropertyChanged(nameof(IsCoreStateLoading));
                OnPropertyChanged(nameof(IsCoreStateLoaded));
            }, null);
        }

        /// <inheritdoc />
        public string InstanceId => _core.InstanceId;

        /// <inheritdoc />
        public string InstanceDescriptor => _core.InstanceDescriptor;

        /// <inheritdoc />
        AbstractUICollection ICore.AbstractConfigPanel => _core.AbstractConfigPanel;

        /// <inheritdoc cref="ICore.AbstractConfigPanel"/>
        public AbstractUICollectionViewModel AbstractConfigPanel { get; private set; }

        /// <inheritdoc />
        public MediaPlayerType PlaybackType => _core.PlaybackType;

        /// <summary>
        /// A local path or url pointing to a SVG file containing the logo for this core.
        /// </summary>
        public Uri LogoUri { get; }

        /// <summary>
        /// The user-friendly name of the core.
        /// </summary>
        public string DisplayName { get; }

        /// <inheritdoc cref="ICore.User" />
        public ICoreUser? User => _core.User;

        /// <inheritdoc cref="ICore.CoreState" />
        public CoreState CoreState { get; internal set; }

        /// <inheritdoc />
        public ICore SourceCore => _core.SourceCore;

        /// <inheritdoc/>
        public MainViewModel Root { get; }

        /// <summary>
        /// True when <see cref="CoreState"/> is <see cref="Models.CoreState.Unloaded"/>.
        /// </summary>
        public bool IsCoreStateUnloaded => CoreState == CoreState.Unloaded;

        /// <summary>
        /// True when <see cref="CoreState"/> is <see cref="Models.CoreState.NeedsConfiguration"/>.
        /// </summary>
        public bool IsCoreStateConfiguring => CoreState == CoreState.NeedsConfiguration;

        /// <summary>
        /// True when <see cref="CoreState"/> is <see cref="Models.CoreState.Configured"/>.
        /// </summary>
        public bool IsCoreStateConfigured => CoreState == CoreState.Configured;

        /// <summary>
        /// True when <see cref="CoreState"/> is <see cref="Models.CoreState.Loading"/>.
        /// </summary>
        public bool IsCoreStateLoading => CoreState == CoreState.Loading;

        /// <summary>
        /// True when <see cref="CoreState"/> is <see cref="Models.CoreState.Loaded"/>.
        /// </summary>
        public bool IsCoreStateLoaded => CoreState == CoreState.Loaded;

        /// <inheritdoc cref="ICore.CoreStateChanged" />
        public event EventHandler<CoreState>? CoreStateChanged
        {
            add => _core.CoreStateChanged += value;
            remove => _core.CoreStateChanged -= value;
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ICoreDevice>? DevicesChanged;

        /// <inheritdoc />
        public event EventHandler? AbstractConfigPanelChanged;

        /// <inheritdoc />
        public event EventHandler<string>? InstanceDescriptorChanged;

        /// <inheritdoc />
        IReadOnlyList<ICoreDevice> ICore.Devices => _core.Devices;

        /// <inheritdoc cref="ICore.Devices" />
        public ObservableCollection<DeviceViewModel> Devices { get; }

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

        /// <inheritdoc cref="CoreMetadata"/>
        public CoreMetadata Registration => _core.Registration;

        /// <inheritdoc cref="IAsyncDisposable.DisposeAsync" />
        public async ValueTask DisposeAsync()
        {
            DetachEvents();

            await Library.DisposeAsync();

            if (RecentlyPlayed != null)
                await RecentlyPlayed.DisposeAsync();

            if (Discoverables != null)
                await Discoverables.DisposeAsync();

            if (Pins != null)
                await Pins.DisposeAsync();

            await _core.DisposeAsync();
        }

        /// <inheritdoc/>
        public Task<ICoreMember?> GetContextByIdAsync(string id, CancellationToken cancellationToken = default) => _core.GetContextByIdAsync(id, cancellationToken);

        /// <inheritdoc />
        public Task<IMediaSourceConfig?> GetMediaSourceAsync(ICoreTrack track, CancellationToken cancellationToken = default) => _core.GetMediaSourceAsync(track, cancellationToken);

        /// <inheritdoc />
        public Task InitAsync(CancellationToken cancellationToken = default) => _core.InitAsync(cancellationToken);

        /// <inheritdoc />
        public bool IsInitialized => _core.IsInitialized;
    }
}
