using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using StrixMusic.Sdk.Services.Navigation;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shell.Strix
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
            Ioc.ConfigureServices(services =>
            {
                services.AddSingleton<INavigationService<Control>, NavigationService<Control>>();
            });
        }
    }
}
