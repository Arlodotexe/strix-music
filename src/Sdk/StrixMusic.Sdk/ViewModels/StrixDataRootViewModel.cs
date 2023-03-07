// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using OwlCore.ComponentModel;
using OwlCore.Extensions;
using StrixMusic.Sdk.AdapterModels;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;

namespace StrixMusic.Sdk.ViewModels;

/// <summary>
/// A ViewModel wrapper for an instance of <see cref="IStrixDataRoot"/>.
/// </summary>
public class StrixDataRootViewModel : ObservableObject, IStrixDataRoot, IDelegatable<IStrixDataRoot>
{
    private readonly IStrixDataRoot _dataRoot;
    private readonly ObservableCollection<IDevice> _devices;
    private IReadOnlyList<ICore> _knownSources;

    /// <summary>
    /// Initializes a new instance of the <see cref="StrixDataRootViewModel"/> class.
    /// </summary>
    public StrixDataRootViewModel(IStrixDataRoot dataRoot)
    {
        _dataRoot = dataRoot;
        _knownSources = dataRoot.Sources.ToList();

        Library = new LibraryViewModel(dataRoot.Library);

        if (dataRoot.Pins is not null)
            Pins = new PlayableCollectionGroupViewModel(dataRoot.Pins);

        if (dataRoot.RecentlyPlayed is not null)
            RecentlyPlayed = new RecentlyPlayedViewModel(dataRoot.RecentlyPlayed);

        if (dataRoot.Discoverables is not null)
            Discoverables = new DiscoverablesViewModel(dataRoot.Discoverables);

        if (dataRoot.Search is not null)
            Search = new SearchViewModel(dataRoot.Search);

        _devices = new ObservableCollection<IDevice>(dataRoot.Devices.Select(x => new DeviceViewModel(x)));

        Sources = new ObservableCollection<CoreViewModel>(dataRoot.Sources.Select(x => new CoreViewModel(x)));

        AttachEvents(dataRoot);
    }

    private void AttachEvents(IStrixDataRoot dataRoot)
    {
        dataRoot.DevicesChanged += OnDevicesChanged;
        dataRoot.PinsChanged += OnPinsChanged;
        dataRoot.RecentlyPlayedChanged += OnRecentlyPlayedChanged;
        dataRoot.DiscoverablesChanged += OnDiscoverablesChanged;
        dataRoot.SearchChanged += OnSearchChanged;
        dataRoot.SourcesChanged += DataRootOnSourcesChanged;
    }

    private void DetachEvents(IStrixDataRoot dataRoot)
    {
        dataRoot.DevicesChanged -= OnDevicesChanged;
        dataRoot.PinsChanged -= OnPinsChanged;
        dataRoot.RecentlyPlayedChanged -= OnRecentlyPlayedChanged;
        dataRoot.DiscoverablesChanged -= OnDiscoverablesChanged;
        dataRoot.SearchChanged -= OnSearchChanged;
        dataRoot.SourcesChanged -= DataRootOnSourcesChanged;
    }

    private void DataRootOnSourcesChanged(object? sender, EventArgs e)
    {
        var previousSources = _knownSources;
        var newSources = _dataRoot.Sources;

        var addedSources = newSources.Except(previousSources).ToArray();
        var removedSources = previousSources.Except(newSources);

        foreach (var item in removedSources)
            Sources.Remove(Sources.First(x => x.InstanceId == item.InstanceId));

        foreach (var item in addedSources)
            Sources.Add(new CoreViewModel(item));

        _knownSources = newSources;
    }

    private void OnDevicesChanged(object sender, IReadOnlyList<CollectionChangedItem<IDevice>> addedItems, IReadOnlyList<CollectionChangedItem<IDevice>> removedItems)
    {
        var wrappedAdded = addedItems.Select(x => new CollectionChangedItem<IDevice>(new DeviceViewModel(x.Data), x.Index)).ToList();
        var wrappedRemoved = removedItems.Select(x => new CollectionChangedItem<IDevice>(new DeviceViewModel(x.Data), x.Index)).ToList();

        _devices.ChangeCollection(wrappedAdded, wrappedRemoved);
        DevicesChanged?.Invoke(this, wrappedAdded, wrappedRemoved);
    }

    private void OnDiscoverablesChanged(object? sender, IDiscoverables e)
    {
        Discoverables = new DiscoverablesViewModel(e);
        DiscoverablesChanged?.Invoke(this, Discoverables);
    }

    private void OnRecentlyPlayedChanged(object? sender, IRecentlyPlayed e)
    {
        RecentlyPlayed = new RecentlyPlayedViewModel(e);
        RecentlyPlayedChanged?.Invoke(this, RecentlyPlayed);
    }

    private void OnPinsChanged(object? sender, IPlayableCollectionGroup e)
    {
        Pins = new PlayableCollectionGroupViewModel(e);
        PinsChanged?.Invoke(this, Pins);
    }

    private void OnSearchChanged(object? sender, ISearch e)
    {
        Search = new SearchViewModel(e);
        SearchChanged?.Invoke(this, Search);
    }

    /// <inheritdoc/>
    IStrixDataRoot IDelegatable<IStrixDataRoot>.Inner => _dataRoot;

    /// <inheritdoc/>
    public event EventHandler? SourcesChanged
    {
        add => _dataRoot.SourcesChanged += value;
        remove => _dataRoot.SourcesChanged -= value;
    }

    /// <inheritdoc/>
    public event CollectionChangedEventHandler<IDevice>? DevicesChanged;

    /// <inheritdoc/>
    IReadOnlyList<ICore> IMerged<ICore>.Sources => _dataRoot.Sources;

    /// <summary>
    /// The sources used to create this data root.
    /// </summary>
    public ObservableCollection<CoreViewModel> Sources { get; }

    /// <inheritdoc/>
    public Task InitAsync(CancellationToken cancellationToken = new CancellationToken()) => _dataRoot.InitAsync(cancellationToken);

    /// <inheritdoc/>
    public bool IsInitialized => _dataRoot.IsInitialized;

    /// <inheritdoc/>
    public MergedCollectionConfig MergeConfig => _dataRoot.MergeConfig;

    /// <inheritdoc/>
    public IReadOnlyList<IDevice> Devices => _devices;

    /// <inheritdoc/>
    ILibrary IStrixDataRoot.Library => Library;

    /// <summary>
    /// A ViewModel wrapper for <see cref="IStrixDataRoot.Library"/>.
    /// </summary>
    public LibraryViewModel Library { get; }

    /// <inheritdoc/>
    public IPlayableCollectionGroup? Pins { get; private set; }

    /// <inheritdoc/>
    public ISearch? Search { get; private set; }

    /// <inheritdoc/>
    public IRecentlyPlayed? RecentlyPlayed { get; private set; }

    /// <inheritdoc/>
    public IDiscoverables? Discoverables { get; private set; }

    /// <inheritdoc/>
    public event EventHandler<IPlayableCollectionGroup>? PinsChanged;

    /// <inheritdoc/>
    public event EventHandler<IDiscoverables>? DiscoverablesChanged;

    /// <inheritdoc/>
    public event EventHandler<ISearch>? SearchChanged;

    /// <inheritdoc/>
    public event EventHandler<IRecentlyPlayed>? RecentlyPlayedChanged;

    /// <inheritdoc/>
    public bool Equals(ICore? other) => _dataRoot.Equals(other!);

    /// <inheritdoc/>
    public ValueTask DisposeAsync()
    {
        DetachEvents(_dataRoot);
        return _dataRoot.DisposeAsync();
    }
}
