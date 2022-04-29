// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Collections.Generic;
using OwlCore.Events;
using OwlCore.Provisos;
using StrixMusic.Sdk.AdapterModels;
using StrixMusic.Sdk.CoreModels;

namespace StrixMusic.Sdk.AppModels
{
    /// <summary>
    /// Implementations provide a root entry point for interaction with the SDK. Allows for interfacing with multiple
    /// merged <see cref="ICore"/>s, configuring plugins, and more. 
    /// </summary>
    /// <remarks>Instances of this class may contain data merged from one or more sources.</remarks>
    public interface IStrixDataRoot : IAppModel, IMerged<ICore>, IAsyncInit, IAsyncDisposable
    {
        /// <summary>
        /// Configuration options for merging collections items together.
        /// </summary>
        public MergedCollectionConfig MergeConfig { get; }
        
        /// <summary>
        /// The available devices.
        /// </summary>
        public IReadOnlyList<IDevice> Devices { get; }

        /// <summary>
        /// Gets the library for the user on this core.
        /// </summary>
        public ILibrary Library { get; }

        /// <summary>
        /// A list of pinned playable items.
        /// </summary>
        public IPlayableCollectionGroup? Pins { get; }

        /// <summary>
        /// Contains various search-related data and activities.
        /// </summary>
        public ISearch? Search { get; }

        /// <summary>
        /// Gets the recently played items for this .
        /// </summary>
        public IRecentlyPlayed? RecentlyPlayed { get; }

        /// <summary>
        /// Used to browse and discover new music.
        /// </summary>
        public IDiscoverables? Discoverables { get; }

        /// <summary>
        /// Raised when <see cref="Pins"/> is changed.
        /// </summary>
        public event EventHandler<IPlayableCollectionGroup>? PinsChanged;

        /// <summary>
        /// Raised when <see cref="Search"/> is changed.
        /// </summary>
        public event EventHandler<ISearch>? SearchChanged;

        /// <summary>
        /// Raised when <see cref="RecentlyPlayed"/> is changed.
        /// </summary>
        public event EventHandler<IRecentlyPlayed>? RecentlyPlayedChanged;

        /// <summary>
        /// Raised when <see cref="Discoverables"/> is changed.
        /// </summary>
        public event EventHandler<IDiscoverables>? DiscoverablesChanged;

        /// <summary>
        /// Raised when the contents of <see cref="Devices"/> is changed.
        /// </summary>
        public event CollectionChangedEventHandler<IDevice>? DevicesChanged;
    }
}
