// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using StrixMusic.Sdk.Models;
using StrixMusic.Sdk.Plugins.Model;

namespace StrixMusic.Sdk.Plugins.PopulateEmptyNames;

/// <summary>
/// Provides a fallback name for all <see cref="IPlayable"/> items when missing or empty.
/// </summary>
internal class PopulateEmptyPlayableNamePlugin : PlayablePluginBase
{
    private readonly string _playableName;

    /// <summary>
    /// Initializes a new instance of the <see cref="PopulateEmptyPlayableNamePlugin"/> class.
    /// </summary>
    /// <param name="metadata">Contains metadata for a plugin. Used to identify a plugin before instantiation.</param>
    /// <param name="inner">An implementation which member access is delegated to, unless the member is overridden in a derived class which changes the behavior.</param>
    /// <param name="playableName">The name to use instead when the existing name is empty.</param>
    public PopulateEmptyPlayableNamePlugin(ModelPluginMetadata metadata, IPlayable inner, string playableName)
        : base(metadata, inner)
    {
        _playableName = playableName;
    }

    /// <inheritdoc/>
    public override string Name => string.IsNullOrWhiteSpace(base.Name) ? _playableName : base.Name;
}
