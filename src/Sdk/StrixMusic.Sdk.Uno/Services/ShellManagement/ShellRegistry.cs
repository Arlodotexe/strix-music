using System;
using System.Collections.Generic;
using StrixMusic.Sdk.Uno.Controls.Shells;

namespace StrixMusic.Sdk.Uno.Services.ShellManagement
{
    /// <summary>
    /// Manages registered shells that the SDK can consume and display to the user.
    /// </summary>
    public static class ShellRegistry
    {
        private static readonly List<ShellMetadata> _metadataRegistry = new();
        private static readonly Dictionary<ShellMetadata, Func<Shell>> _coreFactories = new Dictionary<ShellMetadata, Func<Shell>>();

        /// <summary>
        /// Holds all registered core metadata.
        /// </summary>
        public static IReadOnlyList<ShellMetadata> MetadataRegistry => _metadataRegistry;

        /// <summary>
        /// Registers a shell with the Strix SDK.
        /// </summary>
        /// <param name="coreFactory">A <see cref="Func{T, TResult}"/> that, given an instance ID, returns an instance of a core.</param>
        /// <param name="metadata">The metadata to register for this core.</param>
        public static void Register(Func<Shell> coreFactory, ShellMetadata metadata)
        {
            _metadataRegistry.Add(metadata);
            _coreFactories.Add(metadata, coreFactory);
        }

        /// <summary>
        /// Creates a shell instance using a registered factory.
        /// </summary>
        /// <param name="metadata">The metadata for the core to create an instance of.</param>
        /// <returns>An new instance of <see cref="Shell"/>..</returns>
        public static Shell CreateShell(ShellMetadata metadata)
        {
            return _coreFactories[metadata]();
        }
    }
}
