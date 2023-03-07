// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using StrixMusic.Sdk.AdapterModels;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Sdk.Plugins.Model;

namespace StrixMusic.Sdk.PluginModels;

/// <summary>
/// Wraps an instance of <see cref="IPlayableCollectionGroup"/> with the provided plugins.
/// </summary>
public class PlayableCollectionGroupPluginWrapper : PlayableCollectionGroupPluginWrapperBase, IPlayableCollectionGroup, IPluginWrapper
{
    private readonly IPlayableCollectionGroup _playableCollectionGroup;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlayableCollectionGroupPluginWrapper"/> class.
    /// </summary>
    /// <param name="playableCollectionGroup">An existing instance to wrap around and provide plugins on top of.</param>
    /// <param name="plugins">The plugins to import and apply to everything returned from this wrapper.</param>
    internal PlayableCollectionGroupPluginWrapper(IPlayableCollectionGroup playableCollectionGroup, params SdkModelPlugin[] plugins)
        : base(GlobalModelPluginConnector.Create(plugins.Aggregate((x, y) =>
        {
            x.Import(y);
            return x;
        })).PlayableCollectionGroup.Execute(playableCollectionGroup), plugins)
    {
        foreach(var plugin in plugins)
            ActivePlugins.Import(plugin);

        ActivePlugins = GlobalModelPluginConnector.Create(ActivePlugins);

        _playableCollectionGroup = ActivePlugins.PlayableCollectionGroup.Execute(playableCollectionGroup);
    }
    
    /// <inheritdoc/>
    public SdkModelPlugin ActivePlugins { get; } = new(PluginModelWrapperInfo.Metadata);

    /// <inheritdoc/>
    public IReadOnlyList<ICorePlayableCollectionGroup> Sources => ((IMerged<ICorePlayableCollectionGroup>)_playableCollectionGroup).Sources;

    /// <inheritdoc/>
    public new bool Equals(ICorePlayableCollectionGroup? other) => _playableCollectionGroup.Equals(other!);
}
