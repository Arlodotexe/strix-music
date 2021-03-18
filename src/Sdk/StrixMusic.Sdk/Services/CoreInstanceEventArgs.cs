using System;
using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Sdk.Services
{
    /// <summary>
    /// Event args used to identify a specific core instance before it's created.
    /// </summary>
    public class CoreInstanceEventArgs : EventArgs
    {
        /// <summary>
        /// Creates a new instance of <see cref="CoreInstanceEventArgs"/>.
        /// </summary>
        /// <param name="instanceId">Unique instance identifier for the core instance.</param>
        /// <param name="assemblyInfo">The assembly info for the core instance.</param>
        public CoreInstanceEventArgs(string instanceId, CoreAssemblyInfo assemblyInfo)
        {
            InstanceId = instanceId;
            AssemblyInfo = assemblyInfo;
        }

        /// <summary>
        /// Unique instance identifier for the core instance.
        /// </summary>
        public string InstanceId { get; }

        /// <summary>
        /// The assembly info for the core instance.
        /// </summary>
        public CoreAssemblyInfo AssemblyInfo { get; }
    }
}