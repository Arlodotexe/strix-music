using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using StrixMusic.Sdk.Services.Navigation;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Sdk.Uno
{
    /// <summary>
    /// A static class containing the Ioc for the DefaultShell
    /// </summary>
    /// TODO: 
    public static class DefaultShellIoc
    {
        /// <summary>
        /// An Ioc for only the DefaultShell.
        /// </summary>
        public static Ioc Ioc { get; private set; } = new Ioc();

        /// <summary>
        /// Initializes the services for the DefaultShell.
        /// </summary>
        public static void Initialize()
        {
            Ioc = new Ioc();

            var services = new ServiceCollection();
            services.AddSingleton<INavigationService<Control>, NavigationService<Control>>();

            Ioc.ConfigureServices(services.BuildServiceProvider());
        }
    }
}
