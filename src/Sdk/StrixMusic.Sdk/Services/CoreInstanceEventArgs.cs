using StrixMusic.Sdk.Models.Core;
using System;

namespace StrixMusic.Sdk.Services
{
    /// <summary>
    /// Event args used to identify a specific core instance before it's created.
    /// </summary>
    public sealed class CoreInstanceEventArgs : EventArgs
    {
        /// <summary>
        /// Creates a new instance of <see cref="CoreInstanceEventArgs"/>.
        /// </summary>
        /// <param name="instanceId">Unique instance identifier for the core instance.</param>
        /// <param name="coreMetadata">The metadata for the core instance.</param>
        public CoreInstanceEventArgs(string instanceId, CoreMetadata coreMetadata)
        {
            InstanceId = instanceId;
            CoreMetadata = coreMetadata;
        }

        /// <summary>
        /// Unique instance identifier for the core instance.
        /// </summary>
        public string InstanceId { get; }

        /// <summary>
        /// The assembly info for the core instance.
        /// </summary>
        public CoreMetadata CoreMetadata { get; }
    }
}