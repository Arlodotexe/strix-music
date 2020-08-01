using OwlCore.ArchTools;

namespace StrixMusic.Services
{
    /// <summary>
    /// Initializes the ViewModel .
    /// </summary>
    public class ViewModelLocator
    {
        private static bool _hasRun = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelLocator"/> class and the <see cref="MainViewModel"/>.
        /// Creates and registers all the services with <see cref="SimpleIoc.Default"/>.
        /// </summary>
        public ViewModelLocator()
        {
            if (_hasRun)
                return;
            _hasRun = true;

            // TODO: Init ViewModel
        }

        /// <summary>
        /// Gets the <see cref="MainViewModel"/>.
        /// </summary>
        public object Main { get; } = ServiceLocator.Instance.Resolve<object>();
    }
}
