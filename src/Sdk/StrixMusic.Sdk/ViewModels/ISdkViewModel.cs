namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// Properties which must be implemented by all view models in the SDK.
    /// </summary>
    public interface ISdkViewModel
    {
        /// <summary>
        /// The <see cref="MainViewModel"/> that this or the object that created this originated from.
        /// </summary>
        public MainViewModel Root { get; }
    }
}
