// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using StrixMusic.Sdk.Models;
using StrixMusic.Sdk.Plugins.Model;

namespace StrixMusic.Sdk.Plugins.PopulateEmptyNames;

/// <summary>
/// A plugin that intercepts null or whitespaces names on tracks and uses a value provided to the plugin instead.
/// </summary>
internal class PopulateEmptyTrackNamePlugin : TrackPluginBase
{
    private readonly string _trackName;

    /// <summary>
    /// Initializes a new instance of the <see cref="PopulateEmptyTrackNamePlugin"/> class.
    /// </summary>
    /// <param name="metadata">Contains metadata for a plugin. Used to identify a plugin before instantiation.</param>
    /// <param name="inner">An implementation which member access is delegated to, unless the member is overridden in a derived class which changes the behavior.</param>
    /// <param name="trackName">The name to use instead when the existing name is empty.</param>
    public PopulateEmptyTrackNamePlugin(ModelPluginMetadata metadata, ITrack inner, string trackName)
        : base(metadata, inner)
    {
        _trackName = trackName;
    }

    /// <inheritdoc/>
    public override string Name => string.IsNullOrWhiteSpace(base.Name) ? _trackName : base.Name;
}
