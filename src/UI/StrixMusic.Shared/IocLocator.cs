using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using StrixMusic.Core.Mock;
using StrixMusic.CoreInterfaces.Interfaces;
using StrixMusic.Services;
using StrixMusic.Services.Navigation;
using StrixMusic.Services.Settings;
using StrixMusic.Services.StorageService;
using StrixMusic.Services.SuperShell;
using StrixMusic.ViewModels;
using Windows.UI.Xaml.Controls;

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
                services.AddSingleton<ITextStorageService, TextStorageService>();
                services.AddSingleton<ISuperShellService, SuperShellService>();
                services.AddSingleton<IFileSystemService, FileSystemService>();
                services.AddSingleton<INavigationService<Control>, NavigationService>();

                //services.AddSingleton<ICore, FileCore>();
                services.AddSingleton<ICore, MockCore>();
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
