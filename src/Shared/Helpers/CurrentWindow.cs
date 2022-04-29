using System;
using Windows.UI.Xaml;
using StrixMusic.Sdk.Services.Navigation;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shared
{
    /// <summary>
    /// Helpers related to Window.Current.
    /// </summary>
    public static class CurrentWindow
    {
        /// <summary>
        /// Gets the <see cref="AppFrame"/> instance in the current window.
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        public static AppFrame GetAppFrame(this Window window) => (AppFrame)window.Content;
        
        /// <summary>
        /// Gets the app-level navigation service.
        /// </summary>
        /// <returns>A <see cref="NavigationService{Control}"/> if found, otherwise <see langword="null"/>.</returns>
        [Obsolete]
        public static INavigationService<Control> NavigationService { get; } = new NavigationService<Control>();
    }
}
