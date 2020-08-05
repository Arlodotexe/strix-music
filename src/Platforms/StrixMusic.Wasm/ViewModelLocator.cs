using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;

namespace StrixMusic.Services
{
    /// <summary>
    /// A <see langword="class"/> to initialize the <see cref="MainViewModel"/>.
    /// </summary>
    public class IocLocator
    {
        private static bool _hasRun = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="IocLocator"/> class and the <see cref="MainViewModel"/>.
        /// </summary>
        public IocLocator()
        {
            if (_hasRun)
                return;
            _hasRun = true;

            // TODO: Init ViewModel
        }

        /// <summary>
        /// Gets the <see cref="MainViewModel"/>.
        /// </summary>
        public object Main { get; } = Ioc.Default.GetService<object>();
    }
}
