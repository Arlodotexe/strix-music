using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.Groove.Messages.Navigation.Pages.Abstract;

namespace StrixMusic.Shells.Groove.Messages.Navigation.Pages
{
    /// <summary>
    /// Represents a request to navigate to a playlist view.
    /// </summary>
    public class PlaylistViewNavigationRequestMessage : PageNavigationRequestMessage<PlaylistViewModel>
    {
        /// <summary>
        /// Creates a new instance of <see cref="PlaylistsViewNavigationRequestMessage"/>.
        /// </summary>
        /// <param name="playlist">The playlist to display in the playlist view.</param>
        /// <param name="record">If true, navigation will be added to the navigation stack.</param>
        public PlaylistViewNavigationRequestMessage(PlaylistViewModel playlist, bool record = true)
            : base(playlist, record)
        {
        }

        /// <inheritdoc/>
        public override bool ShowLargeHeader => false;

        /// <inheritdoc/>
        public override string PageTitleResource => "Playlist";
    }
}
