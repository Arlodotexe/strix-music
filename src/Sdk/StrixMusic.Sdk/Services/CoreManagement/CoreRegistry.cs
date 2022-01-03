using System;
using System.Collections.Generic;
using StrixMusic.Sdk.Models.Core;

namespace StrixMusic.Sdk.Services
{
    /// <summary>
    /// Represents a single registered core.
    /// </summary>
    public static class CoreRegistry
    {
        private static readonly List<CoreMetadata> _metadataRegistry = new();
        private static readonly Dictionary<string, Func<string, ICore>> _coreFactories = new Dictionary<string, Func<string, ICore>>();

        /// <summary>
        /// Holds all registered core metadata.
        /// </summary>
        public static IReadOnlyList<CoreMetadata> MetadataRegistry => _metadataRegistry;

        /// <summary>
        /// Registers a core with the Strix SDK.
        /// </summary>
        /// <param name="coreFactory">A <see cref="Func{T, TResult}"/> that, given an instance ID, returns an instance of a core.</param>
        /// <param name="metadata">The metadata to register for this core.</param>
        public static void Register(Func<string, ICore> coreFactory, CoreMetadata metadata)
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
        public static ICore CreateCore(string coreRegistryId, string instanceId)
        {
            return _coreFactories[coreRegistryId](instanceId);
        }
    }
}