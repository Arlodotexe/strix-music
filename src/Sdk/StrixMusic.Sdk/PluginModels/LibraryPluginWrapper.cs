// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System.Collections.Generic;
using StrixMusic.Sdk.AdapterModels;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Sdk.Plugins.Model;

namespace StrixMusic.Sdk.PluginModels;

/// <summary>
/// Wraps an instance of <see cref="ILibrary"/> with the provided plugins.
/// </summary>
public class LibraryPluginWrapper : PlayableCollectionGroupPluginWrapperBase, ILibrary, IPluginWrapper
{
    private readonly ILibrary _library;

    /// <summary>
    /// Initializes a new instance of the <see cref="LibraryPluginWrapper"/> class.
    /// </summary>
    /// <param name="library">An existing instance to wrap around and provide plugins on top of.</param>
    /// <param name="plugins">The plugins to import and apply to everything returned from this wrapper.</param>
    internal LibraryPluginWrapper(ILibrary library, params SdkModelPlugin[] plugins)
        : base(GlobalModelPluginConnector.Create(new SdkModelPlugin(PluginModelWrapperInfo.Metadata, plugins)).Library.Execute(library), plugins)
    {
        foreach (var plugin in plugins)
            ActivePlugins.Import(plugin);

        ActivePlugins = GlobalModelPluginConnector.Create(ActivePlugins);

        _library = ActivePlugins.Library.Execute(library);
    }

    /// <inheritdoc/>
    public SdkModelPlugin ActivePlugins { get; } = new(PluginModelWrapperInfo.Metadata);

    /// <inheritdoc/>
    public IReadOnlyList<ICoreLibrary> Sources => ((IMerged<ICoreLibrary>)_library).Sources;

    /// <inheritdoc/>
    public bool Equals(ICoreLibrary other) => _library.Equals(other);
}
