using System;
using StrixMusic.Sdk.Data.Base;

namespace StrixMusic.Sdk.Data.Core
{
    /// <summary>
    /// Provides various methods of configuring a core.
    /// </summary>
    public interface ICoreConfig : ICoreConfigBase, ICoreMember
    {
        /// <summary>
        /// The services for this instance of the core.
        /// </summary>
        IServiceProvider? Services { get; }
    }
}
