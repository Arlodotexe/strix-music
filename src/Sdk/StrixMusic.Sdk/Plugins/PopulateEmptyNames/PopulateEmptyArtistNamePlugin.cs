// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using StrixMusic.Sdk.Models;
using StrixMusic.Sdk.Plugins.Model;

namespace StrixMusic.Sdk.Plugins.PopulateEmptyNames;

/// <summary>
/// Provides a fallback name for Artists when missing or empty.
/// </summary>
internal class PopulateEmptyArtistNamePlugin : ArtistPluginBase
{
    private readonly string _artistName;

    /// <summary>
    /// Initializes a new instance of the <see cref="PopulateEmptyArtistNamePlugin"/> class.
    /// </summary>
    /// <param name="metadata">Contains metadata for a plugin. Used to identify a plugin before instantiation.</param>
    /// <param name="inner">An implementation which member access is delegated to, unless the member is overridden in a derived class which changes the behavior.</param>
    /// <param name="artistName">The name to use instead when the existing name is empty.</param>
    public PopulateEmptyArtistNamePlugin(ModelPluginMetadata metadata, IArtist inner, string artistName)
        : base(metadata, inner)
    {
        _artistName = artistName;
    }

    /// <inheritdoc/>
    public override string Name => string.IsNullOrWhiteSpace(base.Name) ? _artistName : base.Name;
}
