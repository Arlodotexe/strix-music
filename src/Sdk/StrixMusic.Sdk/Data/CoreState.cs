namespace StrixMusic.Sdk.Data
{
    /// <summary>
    /// The state of a <see cref="ICore"/>.
    /// </summary>
    public enum CoreState
    {
        /// <summary>
        /// The core constructed but not initialized.
        /// </summary>
        Unloaded,

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
