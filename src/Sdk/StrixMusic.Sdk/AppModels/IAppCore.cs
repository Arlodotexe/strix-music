// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System.Collections.Generic;
using OwlCore.Events;
using StrixMusic.Sdk.AdapterModels;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Sdk.Plugins;
using StrixMusic.Sdk.Plugins.Model;

namespace StrixMusic.Sdk.AppModels
{
    /// <summary>
    /// Implementations provide a root entry point for interaction with the SDK. Allows for interfacing with multiple
    /// merged <see cref="ICore"/>s, configuring plugins, and more. 
    /// </summary>
    /// <remarks>Instances of this class may contain data merged from one or more sources.</remarks>
    public interface IAppCore : ISdkMember, IMerged<ICore>
    {
        /// <summary>
        /// All available and configured plugins for this instance.
        /// </summary>
        /// <remarks>
        ///         NOTICE:
        /// <para/> Model plugins cannot be applied automatically to classes which merge core data, as it would hide members 
        ///         of <see cref="IMergedMutable{T}"/> and prevent you from adding or removing sources. Instead, create your
        ///         instance of <see cref="IMerged{T}"/>, then pass it to the corresponding plugin builder.
        ///         
        /// <para/> Once built, the returned instance will have plugins applied on top of the <see cref="IMergedMutable{T}"/> instance.
        ///         If no plugins override functionality when accessing a member, the provided <see cref="IMergedMutable{T}"/> will be used instead.
        /// <para/> See <see cref="SdkModelPlugins"/> for more info.
        /// </remarks>
        /// <seealso cref="SdkModelPlugins"/>
        /// <seealso cref="SdkModelPlugin"/>
        /// <seealso cref="GlobalModelPluginConnector"/>
        public PluginManager Plugins { get; }

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
        /// Raised when the contents of <see cref="Devices"/> is changed.
        /// </summary>
        public event CollectionChangedEventHandler<IDevice>? DevicesChanged;
    }
}
