using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using StrixMusic.Sdk.Services.Navigation;
using StrixMusic.Sdk.Uno.Services;
using StrixMusic.Shells.ZuneDesktop.Settings;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.ZuneDesktop
{
    /// <summary>
    /// A static class containing the Ioc for the ZuneDesktopShell.
    /// </summary>
    internal static class ZuneDesktopShellIoc
    {
        /// <summary>
        /// An Ioc for only the ZuneDesktopShell.
        /// </summary>
        internal static Ioc Ioc { get; private set; } = new Ioc();

        /// <summary>
        /// Initializes the services for the ZuneDesktopShell.
        /// </summary>
        internal static void Initialize()
        {
            Ioc = new Ioc();
            var services = new ServiceCollection();

            var textStorageService = new TextStorageService();
            var settingsService = new ZuneDesktopSettingsService(textStorageService);

            services.AddSingleton<INavigationService<Control>, NavigationService<Control>>();
            services.AddSingleton(settingsService);

            Ioc.ConfigureServices(services.BuildServiceProvider());
        }
    }
}
