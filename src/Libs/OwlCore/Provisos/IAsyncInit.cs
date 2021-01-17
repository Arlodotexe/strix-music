using System.Threading.Tasks;

namespace OwlCore.Provisos
{
    /// <summary>
    /// Specifies that the implementation should be initialized asynchronously.
    /// </summary>
    public interface IAsyncInit
    {
        /// <summary>
        /// Runs the asynchronous initialization.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task InitAsync();

        /// <summary>
        /// Indicated whether or not the instance has been initialized.
        /// </summary>
        bool IsInitialized { get; }
    }
}