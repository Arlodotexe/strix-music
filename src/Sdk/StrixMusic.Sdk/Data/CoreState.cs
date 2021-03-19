using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Sdk.Data
{
    /// <summary>
    /// The state of an <see cref="ICore"/>.
    /// </summary>
    public enum CoreState
    {
        /// <summary>
        /// The core is constructed but not initialized.
        /// </summary>
        Unloaded,

        /// <summary>
        /// The core need configuration data and has requested that the setup process be started.
        /// </summary>
        NeedsSetup,

        /// <summary>
        /// The setup process has finished and the core can be initialized.
        /// </summary>
        Configured,

        /// <summary>
        /// The core is currently loading.
        /// </summary>
        Loading,

        /// <summary>
        /// The core has finished initialization.
        /// </summary>
        Loaded,

        /// <summary>
        /// Something has gone wrong and the core may not function properly.
        /// </summary>
        Faulted,
    }
}
