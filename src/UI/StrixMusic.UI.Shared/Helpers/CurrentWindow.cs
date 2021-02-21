using System;
using StrixMusic.Sdk;
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
        /// Get the <see cref="AppFrame"/> residing in the current window content.
        /// </summary>
        /// <returns>The <see cref="AppFrame"/> residing in the current window, or <see langword="null"/> if not found.</returns>
        public static AppFrame AppFrame => App.AppFrame;

        /// <summary>
        /// Gets the app-level navigation service.
        /// </summary>
        /// <returns>A <see cref="NavigationService{Control}"/> if found, otherwise <see langword="null"/>.</returns>
        public static INavigationService<Control> NavigationService => AppFrame.NavigationService;

        /// <inheritdoc cref="MainViewModel"/>
        /// TODO fix me and remove the singleton
        public static MainViewModel MainViewModel => MainViewModel.Singleton ?? throw new NullReferenceException();
    }
}
