using StrixMusic.Sdk.Observables;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.Controls
{
    /// <summary>
    /// A Templated <see cref="Control"/> for displaying an <see cref="ObservableArtist"/> as a page.
    /// </summary>
    public sealed partial class ArtistView : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArtistView"/> class.
        /// </summary>
        /// <param name="artist">The Artist in view.</param>
        public ArtistView(ObservableArtist artist)
        {
            this.DefaultStyleKey = typeof(ArtistView);
            DataContext = artist;
            LoadTracksAsync();
        }

        /// <summary>
        /// The ViewModel for this page item
        /// </summary>
        public ObservableArtist ViewModel => (DataContext as ObservableArtist) !;

        private async void LoadTracksAsync()
        {
            await ViewModel.PopulateMoreTracksAsync(25);
        }
    }
}
