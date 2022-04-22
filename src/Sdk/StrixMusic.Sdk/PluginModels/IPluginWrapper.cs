// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using StrixMusic.Sdk.AdapterModels;
using StrixMusic.Sdk.Plugins.Model;

namespace StrixMusic.Sdk.PluginModels;

/// <summary>
/// Indicates a type which wraps around an existing implementation and provides plugins.
/// </summary>
public interface IPluginWrapper
{
    /// <summary>
    /// All plugins that were imported and activated for this instance.
    /// </summary>
    /// <remarks>
    ///         Once built, the returned instance will have plugins applied on top of the <see cref="IMergedMutable{T}"/> instance.
    ///         If no plugins override functionality when accessing a member, the provided <see cref="IMergedMutable{T}"/> will be used instead.
    /// 
    /// <para/> See <see cref="SdkModelPlugin"/> for more info.
    /// </remarks>
    /// <seealso cref="SdkModelPlugin"/>
    /// <seealso cref="SdkModelPlugin"/>
    /// <seealso cref="GlobalModelPluginConnector"/>
    public SdkModelPlugin ActivePlugins { get; }
}
