using System;

namespace StrixMusic.Services.Navigation
{
    /// <summary>
    /// The arguments for navigating to a new page
    /// </summary>
    /// <typeparam name="T">The type of the navigation objects</typeparam>
    public class NavigateEventArgs<T> : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NavigateEventArgs{T}"/> class.
        /// </summary>
        /// <param name="page">The page to navigate to.</param>
        /// <param name="overlay">Whether or not the page is an overlay</param>
        public NavigateEventArgs(T page, bool overlay = false)
        {
            Page = page;
            IsOverlay = overlay;
        }

        /// <summary>
        /// Gets the page being navigated to.
        /// </summary>
        public T Page { get; }

        /// <summary>
        /// Gets whether or not the page is an overlay.
        /// </summary>
        public bool IsOverlay { get; }
    }
}
