using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.Groove.Messages.Navigation.Pages.Abstract;

namespace StrixMusic.Shells.Groove.Messages.Navigation.Pages
{
    /// <summary>
    /// Represents a request to navigate to the an home view.
    /// </summary>
    public class HomeViewNavigationRequestMessage : PageNavigationRequestMessage<LibraryViewModel>
    {
        /// <summary>
        /// Creates a new instance of <see cref="HomeViewNavigationRequestMessage"/>.
        /// </summary>
        /// <param name="library">The library to display in the home view.</param>
        /// <param name="record">If true, navigation will be added to the navigation stack.</param>
        public HomeViewNavigationRequestMessage(LibraryViewModel library, bool record = true)
            : base(library, record)
        {
        }

        /// <inheritdoc/>
        public override bool ShowLargeHeader => true;

        /// <inheritdoc/>
        public override string PageTitleResource => "MyMusic";
    }
}
