namespace StrixMusic.Shells.Groove.Messages.Navigation.Pages.Abstract
{
    /// <summary>
    /// A base class for navigation requests with parameter data.
    /// </summary>
    /// <typeparam name="T">The type of the data being held.</typeparam>
    public abstract class PageNavigationRequestMessage<T> : PageNavigationRequestedMessage
    {
        /// <summary>
        /// Creates a new instance of <see cref="PageNavigationRequestedMessage"/>
        /// </summary>
        /// <param name="data">Parameter data used for page navigation.</param>
        /// <param name="record">If true, navigation will be added to the navigation stack.</param>
        protected PageNavigationRequestMessage(T data, bool record = true)
            : base(record)
        {
            PageData = data;
        }

        /// <summary>
        /// Parameter data used for constructing the page.
        /// </summary>
        public T PageData { get; }
    }
}
