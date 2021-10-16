namespace StrixMusic.Shells.Groove.Messages.Navigation.Pages.Abstract
{
    /// <summary>
    /// A base class for navigation requests.
    /// </summary>
    public abstract class PageNavigationRequestMessage
    {
        /// <summary>
        /// Create a new instance of <see cref="PageNavigationRequestMessage"/>.
        /// </summary>
        /// <param name="record">If true, navigation will be added to the navigation stack.</param>
        public PageNavigationRequestMessage(bool record = true)
        {
            RecordNavigation = record;
        }

        /// <summary>
        /// If true, navigation will be added to the navigation stack.
        /// </summary>
        public bool RecordNavigation { get; set; }

        /// <summary>
        /// If true, this page should use a large header style.
        /// </summary>
        public abstract bool ShowLargeHeader { get; }

        /// <summary>
        /// The translation resource for the page title.
        /// </summary>
        public abstract string PageTitleResource { get; }
    }
}
