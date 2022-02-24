// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

namespace StrixMusic.Sdk.Plugins.Model;

/// <summary>
///         A model plugin is one or more implementations of <see cref="IModelPlugin"/>
///         that modifies the behavior of an interface implementation
///         by wrapping around an existing instance and selectively overriding members.
/// <para/> This class groups together several individual model plugin implementations under a
///         single <see cref="IModelPlugin"/> that can be easily consumed.
/// </summary>
/// <remarks>
///         Contains a chainable plugin builder for every relevant model interface used
///         in the Strix Music SDK. 
/// <para/> When the chain is built, the first added Plugin is returned, with the next Plugin provided during construction for 
///         proxying, which also had the next item passed into it during construction, and so on.
/// <para/> When accessing a member, a plugin may retrieve data from the next plugin (or, if none, the actual implementation), and
///         transform or replace it as needed.
///         A plugin may ignore the inner implementation entirely and supply new data.
///         Or, a plugin might not override that member and simply relay data from the actual implementation.
/// </remarks>
public class SdkModelPlugin : SdkModelPlugins, IModelPlugin
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SdkModelPlugin"/> class.
    /// </summary>
    /// <param name="metadata">Contains metadata for a plugin. Used to identify a plugin before instantiation.</param>
    public SdkModelPlugin(ModelPluginMetadata metadata)
    {
        Metadata = metadata;
    }

    /// <inheritdoc/>
    public ModelPluginMetadata Metadata { get; }
}
