// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StrixMusic.Sdk.AdapterModels;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Sdk.Plugins.Model;

namespace StrixMusic.Sdk.PluginModels;

/// <summary>
/// Wraps an instance of <see cref="IGenre"/> with the provided plugins.
/// </summary>
public class GenrePluginWrapper : IGenre, IPluginWrapper
{
    private readonly IGenre _genre;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlayableCollectionGroupPluginWrapperBase"/> class.
    /// </summary>
    /// <param name="genre">The instance to wrap around and apply plugins to.</param>
    /// <param name="plugins">The plugins that are applied to items returned from or emitted by this collection.</param>
    internal GenrePluginWrapper(IGenre genre, params SdkModelPlugin[] plugins)
    {
        foreach (var item in plugins)
            ActivePlugins.Import(item);

        ActivePlugins = GlobalModelPluginConnector.Create(ActivePlugins);

        _genre = genre;
        AttachEvents(_genre);
    }
    
    /// <inheritdoc/>
    public SdkModelPlugin ActivePlugins { get; } = new(PluginModelWrapperInfo.Metadata);

    private void AttachEvents(IGenre genre)
    {
        genre.SourcesChanged += OnSourcesChanged;
    }

    private void DetachEvents(IGenre genre)
    {
        genre.SourcesChanged -= OnSourcesChanged;
    }
    
    private void OnSourcesChanged(object sender, EventArgs e) => SourcesChanged?.Invoke(sender, e);
    
    /// <inheritdoc cref="IMerged.SourcesChanged"/>
    public event EventHandler? SourcesChanged;

    /// <inheritdoc/>
    public bool Equals(ICoreGenre other) => _genre.Equals(other);

    /// <inheritdoc/>
    public IReadOnlyList<ICoreGenre> Sources => _genre.Sources;

    /// <inheritdoc/>
    public string Name => _genre.Name;

    /// <inheritdoc/>
    public ValueTask DisposeAsync()
    {
        DetachEvents(_genre);
        return _genre.DisposeAsync();
    }
}
