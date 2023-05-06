// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OwlCore.ComponentModel;
using OwlCore.Extensions;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Plugins.Model;

namespace StrixMusic.Sdk.Plugins.PlaybackHandler;

/// <summary>
/// Adds a playback device using the provided <see cref="IPlaybackHandlerService"/>
/// </summary>
internal class StrixDataRootPlaybackHandlerPlugin : StrixDataRootPluginBase
{
    private readonly IPlaybackHandlerService _playbackHandler;
    private readonly List<IDevice> _devices = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="PlaybackHandlerPlugin"/> class.
    /// </summary>
    /// <param name="registration">Contains metadata for a plugin. Used to identify a plugin before instantiation.</param>
    /// <param name="inner">An implementation which member access is delegated to, unless the member is overridden in a derived class which changes the behavior.</param>
    /// <param name="playbackHandler">An instance of <see cref="IPlaybackHandlerService"/> that should be used when playback commands are issued.</param>
    protected internal StrixDataRootPlaybackHandlerPlugin(ModelPluginMetadata registration, IStrixDataRoot inner, IPlaybackHandlerService playbackHandler)
        : base(registration, inner)
    {
        _playbackHandler = playbackHandler;

        _devices.Add(_playbackHandler.LocalDevice);
        _devices.AddRange(base.Devices);
        DevicesChanged?.Invoke(this, new List<CollectionChangedItem<IDevice>>(new CollectionChangedItem<IDevice>(_playbackHandler.LocalDevice, 0).IntoList()), new List<CollectionChangedItem<IDevice>>());
        AttachEvents();
    }
    
    /// <inheritdoc/>
    public override event CollectionChangedEventHandler<IDevice>? DevicesChanged;

    /// <inheritdoc/>
    public override IReadOnlyList<IDevice> Devices => _devices;

    private void AttachEvents() => Inner.DevicesChanged += OnDevicesChanged;

    private void DetachEvents() => Inner.DevicesChanged -= OnDevicesChanged;

    private void OnDevicesChanged(object sender, IReadOnlyList<CollectionChangedItem<IDevice>> addedItems, IReadOnlyList<CollectionChangedItem<IDevice>> removedItems)
    {
        // We're injecting a single device, so shift indices +1
        var wrappedAdded = addedItems.Select(x => new CollectionChangedItem<IDevice>(x.Data, x.Index + 1)).ToList();
        var wrappedRemoved = removedItems.Select(x => new CollectionChangedItem<IDevice>(x.Data, x.Index + 1)).ToList();

        _devices.ChangeCollection(wrappedAdded, wrappedRemoved);
        DevicesChanged?.Invoke(sender, addedItems, removedItems);
    }

    /// <inheritdoc/>
    public override ValueTask DisposeAsync()
    {
        DetachEvents();
        return base.DisposeAsync();
    }
}
