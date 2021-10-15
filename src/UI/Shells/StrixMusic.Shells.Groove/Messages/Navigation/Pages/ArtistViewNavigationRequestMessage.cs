using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.Groove.Messages.Navigation.Pages.Abstract;

namespace StrixMusic.Shells.Groove.Messages.Navigation.Pages
{
    /// <summary>
    /// Represents a request to navigate to the an artist view.
    /// </summary>
    public sealed class ArtistViewNavigationRequestMessage : PageNavigationRequestMessage<ArtistViewModel>
    {
        /// <summary>
        /// Creates a new instance of <see cref="ArtistViewNavigationRequestMessage"/>.
        /// </summary>
        /// <param name="artist">The artist to display in the artist view.</param>
        /// <param name="record">If true, navigation will be added to the navigation stack.</param>
        public ArtistViewNavigationRequestMessage(ArtistViewModel artist, bool record = true) 
            : base(artist, record)
        {
        }

        /// <inheritdoc/>
        public override bool ShowLargeHeader => false;

        /// <inheritdoc/>
        public override string PageTitleResource => "Artist";
    }
}
