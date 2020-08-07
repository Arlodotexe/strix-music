namespace StrixMusic.CoreInterfaces.Interfaces.CoreConfig
{
    /// <summary>
    /// Provides various methods of configuring a core.
    /// </summary>
    public interface ICoreConfig
    {
        /// <summary>
        /// <inheritdoc cref="IFileConfig"/>
        /// </summary>
        IFileConfig? FileConfig { get; set; }
    }
}
