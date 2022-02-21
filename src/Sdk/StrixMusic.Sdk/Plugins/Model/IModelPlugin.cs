namespace StrixMusic.Sdk.Plugins.Model
{
    /// <summary>
    /// A model plugin is an implementation of an interface that proxies an existing implementation.
    /// It may delegate all or some member access to the inner implementation, consumes and alter the data,
    /// or ignore and provide custom data.
    /// </summary>
    public interface IModelPlugin
    {
        /// <summary>
        /// The plugin metadata that was provided during registration.
        /// </summary>
        public ModelPluginMetadata Metadata { get; }
    }
}
