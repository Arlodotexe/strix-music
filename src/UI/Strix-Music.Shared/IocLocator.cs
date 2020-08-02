using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Strix_Music.Services;
using StrixMusic.Services.Settings;
using StrixMusic.Services.StorageService;
using StrixMusix.ViewModels;

namespace StrixMusic.Shared
{
    /// <summary>
    /// A <see langword="class"/> to initialize the <see cref="MainViewModel"/>.
    /// </summary>
    public class IocLocator
    {
        static IocLocator()
        {
            Ioc.Default.ConfigureServices(services =>
            {
                services.AddSingleton<ISettingsService, SettingsService>();
                services.AddSingleton<IStorageService, StorageService>();
                services.AddSingleton<MainViewModel>();
            });
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IocLocator"/> class and the <see cref="MainViewModel"/>.
        /// </summary>
        public IocLocator()
        {
        }

        /// <summary>
        /// Gets the <see cref="MainViewModel"/>.
        /// </summary>
        public MainViewModel Main { get; } = Ioc.Default.GetService<MainViewModel>();
    }
}
