using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.Groove.Messages.Navigation.Pages.Abstract;

namespace StrixMusic.Shells.Groove.Messages.Navigation.Pages
{
    /// <summary>
    /// Represents a request to navigate to a playlist collection view.
    /// </summary>
    public class PlaylistsViewNavigationRequestMessage : PageNavigationRequestMessage<IPlaylistCollectionViewModel>
    {
        /// <summary>
        /// Creates a new instance of <see cref="PlaylistsViewNavigationRequestMessage"/>.
        /// </summary>
        /// <param name="playlistCollection">The playlist collection to display.</param>
        /// <param name="record">If true, navigation will be added to the navigation stack.</param>
        public PlaylistsViewNavigationRequestMessage(IPlaylistCollectionViewModel playlistCollection, bool record = true)
            : base(playlistCollection, record)
        {
        }

        /// <inheritdoc/>
        public override bool ShowLargeHeader => true;

        /// <inheritdoc/>
        public override string PageTitleResource => "Playlists";
    }
}
