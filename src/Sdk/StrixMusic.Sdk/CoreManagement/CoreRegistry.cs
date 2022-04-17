// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StrixMusic.Sdk.CoreModels;

namespace StrixMusic.Sdk.CoreManagement
{
    /// <summary>
    /// A central registry for tracking available cores.
    /// </summary>
    public static class CoreRegistry
    {
        private static readonly List<CoreMetadata> _metadataRegistry = new();
        private static readonly Dictionary<string, Func<string, Task<ICore>>> _coreFactories = new();

        /// <summary>
        /// Holds all registered core metadata.
        /// </summary>
        public static IReadOnlyList<CoreMetadata> MetadataRegistry => _metadataRegistry;

        /// <summary>
        /// Registers a core with the Strix SDK.
        /// </summary>
        /// <param name="coreFactory">A delegate that, given an instance ID, returns an instance of a core.</param>
        /// <param name="metadata">The metadata to register for this core.</param>
        public static void Register(Func<string, ICore> coreFactory, CoreMetadata metadata)
        {
            _metadataRegistry.Add(metadata);
            _coreFactories.Add(metadata.Id, x => Task.FromResult(coreFactory(x)));

            CoreRegistered?.Invoke(null, metadata);
        }

        /// <summary>
        /// Registers a core with the Strix SDK.
        /// </summary>
        /// <param name="coreFactory">A delegate that, given an instance ID, returns an instance of a core.</param>
        /// <param name="metadata">The metadata to register for this core.</param>
        public static void Register(CoreMetadata metadata, Func<string, Task<ICore>> coreFactory)
        {
            _metadataRegistry.Add(metadata);
            _coreFactories.Add(metadata.Id, coreFactory);

            CoreRegistered?.Invoke(null, metadata);
        }

        /// <summary>
        /// Raised when a new core is registered.
        /// </summary>
        public static event EventHandler<CoreMetadata>? CoreRegistered;

        /// <summary>
        /// Creates a core instance using a registered core factory.
        /// </summary>
        /// <param name="coreRegistryId">The core registry id of the core to create an instance of.</param>
        /// <param name="instanceId">A unique identifier for this core instance.</param>
        /// <returns>An new instance of <see cref="ICore"/> with the given <paramref name="instanceId"/>.</returns>
        public static Task<ICore> CreateCoreAsync(string coreRegistryId, string instanceId)
        {
            return _coreFactories[coreRegistryId](instanceId);
        }
    }
}
