using System;
using System.Collections.Generic;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Sdk.WinUI.Controls;

namespace StrixMusic.Sdk.WinUI.Services.ShellManagement
{
    /// <summary>
    /// Manages registered shells that the SDK can consume and display to the user.
    /// </summary>
    public static class ShellRegistry
    {
        private static readonly List<ShellMetadata> _metadataRegistry = new();
        private static readonly Dictionary<ShellMetadata, Func<StrixDataRootViewModel, Shell>> _coreFactories = new();

        /// <summary>
        /// Holds all registered core metadata.
        /// </summary>
        public static IReadOnlyList<ShellMetadata> MetadataRegistry => _metadataRegistry;

        /// <summary>
        /// Registers a shell with the Strix SDK.
        /// </summary>
        /// <param name="coreFactory">A <see cref="Func{T, TResult}"/> that, given an instance ID, returns an instance of a core.</param>
        /// <param name="metadata">The metadata to register for this core.</param>
        public static void Register(ShellMetadata metadata, Func<StrixDataRootViewModel, Shell> coreFactory)
        {
            _metadataRegistry.Add(metadata);
            _coreFactories.Add(metadata, coreFactory);

            ShellRegistered?.Invoke(null, metadata);
        }

        /// <summary>
        /// Raised when a new shell is registered.
        /// </summary>
        public static event EventHandler<ShellMetadata>? ShellRegistered;

        /// <summary>
        /// Creates a shell instance using a registered factory.
        /// </summary>
        /// <param name="metadata">The metadata for the core to create an instance of.</param>
        /// <param name="dataRoot">The application model root to use when constructing this shell.</param>
        /// <returns>An new instance of <see cref="Shell"/>.</returns>
        public static Shell CreateShell(ShellMetadata metadata, StrixDataRootViewModel dataRoot)
        {
            return _coreFactories[metadata](dataRoot);
        }
    }
}
