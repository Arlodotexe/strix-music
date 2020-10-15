using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using StrixMusic.Sdk.Services.Navigation;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.Default
{
    /// <summary>
    /// A static class containing the Ioc for the DefaultShell
    /// </summary>
    internal static class DefaultShellIoc
    {
        /// <summary>
        /// An Ioc for only the DefaultShell.
        /// </summary>
        internal static Ioc Ioc { get; private set; } = new Ioc();

        /// <summary>
        /// Initializes the services for the DefaultShell.
        /// </summary>
        internal static void Initialize()
        {
            Ioc = new Ioc();
            Ioc.ConfigureServices(services =>
            {
                services.AddSingleton<INavigationService<Control>, NavigationService<Control>>();
            });
        }
    }
}
