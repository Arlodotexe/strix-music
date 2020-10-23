using Windows.UI.Xaml.Controls;
using StrixMusic.Sdk.Core.ViewModels;

namespace StrixMusic.Sdk.Uno.Controls
{
    /// <summary>
    /// A Templated <see cref="Control"/> for displaying an <see cref="ArtistViewModel"/> as a page.
    /// </summary>
    public sealed partial class ArtistView : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArtistView"/> class.
        /// </summary>
        /// <param name="artistViewModel">The Artist in view.</param>
        public ArtistView(ArtistViewModel artistViewModel)
        {
            this.DefaultStyleKey = typeof(ArtistView);
            DataContext = artistViewModel;
            LoadTracksAsync();
            LoadAlbumsAsync();
        }

        /// <summary>
        /// The ViewModel for this page item
        /// </summary>
        public ArtistViewModel ViewModel => (DataContext as ArtistViewModel) !;

        private async void LoadTracksAsync()
        {
            await ViewModel.PopulateMoreTracksCommand.ExecuteAsync(25);
        }

        private async void LoadAlbumsAsync()
        {
            await ViewModel.PopulateMoreAlbumsCommand.ExecuteAsync(25);
        }
    }
}
