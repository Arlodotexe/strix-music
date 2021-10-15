using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.Groove.Messages.Navigation.Pages.Abstract;

namespace StrixMusic.Shells.Groove.Messages.Navigation.Pages
{
    /// <summary>
    /// Represents a request to navigate to the an album view.
    /// </summary>
    public sealed class AlbumViewNavigationRequestMessage
        : PageNavigationRequestMessage<AlbumViewModel>
    {
        /// <summary>
        /// Creates a new instance of <see cref="AlbumViewNavigationRequestMessage"/>.
        /// </summary>
        /// <param name="album">The album to display in the album view.</param>
        /// <param name="record">If true, navigation will be added to the navigation stack.</param>
        public AlbumViewNavigationRequestMessage(AlbumViewModel album, bool record = true)
            : base(album, record)
        {
        }

        /// <inheritdoc/>
        public override bool ShowLargeHeader => false;

        /// <inheritdoc/>
        public override string PageTitleResource => "Album";
    }
}
