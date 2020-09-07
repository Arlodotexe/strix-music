using System;

namespace StrixMusic.Sdk.Services.Navigation
{
    /// <summary>
    /// Handles events in the UI that will usually result in navigating between pages.
    /// </summary>
    /// <typeparam name="T">The type of the UI Elements for the app.</typeparam>
    public interface INavigationService<T>
    {
        /// <summary>
        /// Raised when a navigation is requested.
        /// </summary>
        event EventHandler<NavigateEventArgs<T>> NavigationRequested;

        /// <summary>
        /// Raised when a back navigation is requested
        /// </summary>
        event EventHandler BackRequested;

        /// <summary>
        /// Registers a page to have its state cached.
        /// </summary>
        /// <remarks>
        /// <paramref name="type"/> must inherit <typeparamref name="T"/>.
        /// A registered page cannot contain arguments.
        /// </remarks>
        /// <param name="type">The <see cref="Type"/> of the page cached.</param>
        void RegisterCommonPage(Type type);

        /// <summary>
        /// Raises the <see cref="NavigationRequested"/> event based on the arguments
        /// </summary>
        /// <param name="type">The type of the page to navigate to.</param>
        /// <param name="overlay">Whether or not the page is an overlay.</param>
        /// <param name="args">The arguments for creating the page object.</param>
        void NavigateTo(Type type, bool overlay = false, object? args = null);

        /// <summary>
        /// Raises the <see cref="NavigationRequested"/> event based on the arguments
        /// </summary>
        /// <param name="type">The page object to navigate to.</param>
        /// <param name="overlay">Whether or not the page is an overlay.</param>
        void NavigateTo(T type, bool overlay = false);

        /// <summary>
        /// Raises the <see cref="BackRequested"/> event
        /// </summary>
        void GoBack();
    }
}
