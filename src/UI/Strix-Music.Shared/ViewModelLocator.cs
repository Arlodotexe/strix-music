using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using StrixMusic.Services.Settings;
using StrixMusix.ViewModels;

namespace StrixMusic.Services
{
    /// <summary>
    /// A <see langword="class"/> to initialize the <see cref="MainViewModel"/>.
    /// </summary>
    public class ViewModelLocator
    {
        private static bool _hasRun = false;

        static ViewModelLocator()
        {
            Ioc.Default.ConfigureServices(services =>
            {
                services.AddSingleton<ISettingsService, SettingsService>();
                services.AddSingleton<MainViewModel>();
            });
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelLocator"/> class and the <see cref="MainViewModel"/>.
        /// </summary>
        public ViewModelLocator()
        {

        }

        /// <summary>
        /// Gets the <see cref="MainViewModel"/>.
        /// </summary>
        public MainViewModel Main { get; } = Ioc.Default.GetService<MainViewModel>();
    }
}
