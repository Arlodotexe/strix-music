// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.Plugins.Model;

namespace StrixMusic.Sdk.Plugins.PopulateEmptyNames;

/// <summary>
/// A plugin that intercepts null or whitespaces names on artists and uses a value provided to the plugin instead.
/// </summary>
internal class PopulateEmptyArtistNamePlugin : ArtistPluginBase
{
    private readonly string _artistName;

    /// <summary>
    /// Initializes a new instance of the <see cref="PopulateEmptyArtistNamePlugin"/> class.
    /// </summary>
    /// <param name="metadata">Contains metadata for a plugin. Used to identify a plugin before instantiation.</param>
    /// <param name="inner">An implementation which member access is delegated to, unless the member is overridden in a derived class which changes the behavior.</param>
    /// <param name="pluginRoot">The plugin-enabled <see cref="IStrixDataRoot" /> which is responsible for creating this and all parent instances.</param>
    /// <param name="artistName">The name to use instead when the existing name is empty.</param>
    public PopulateEmptyArtistNamePlugin(ModelPluginMetadata metadata, IStrixDataRoot pluginRoot, IArtist inner, string artistName)
        : base(metadata, inner, pluginRoot)
    {
        _artistName = artistName;
    }

    /// <inheritdoc/>
    public override string Name => string.IsNullOrWhiteSpace(base.Name) ? _artistName : base.Name;
}
