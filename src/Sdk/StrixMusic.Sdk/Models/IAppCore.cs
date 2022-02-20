// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System.Collections.Generic;
using OwlCore.Events;
using StrixMusic.Sdk.Models.Base;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Models.Merged;
using StrixMusic.Sdk.Plugins;

namespace StrixMusic.Sdk.Models
{
    /// <summary>
    /// Implementations provide a root entrypoint for interaction with the SDK. Allows for interfacing with multiple merged <see cref="ICore"/>s, configuring plugins, and more. 
    /// </summary>
    /// <remarks>Instances of this class may contain data merged from one or more sources.</remarks>
    public interface IAppCore : ICoreBase, ISdkMember, IMerged<ICore>
    {
        /// <summary>
        /// All available and configured plugins for this instance.
        /// </summary>
        public PluginManager Plugins { get; }
        
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
        /// Raised when the contents of <see cref="Devices"/> is changed.
        /// </summary>
        public event CollectionChangedEventHandler<IDevice>? DevicesChanged;
    }
}
