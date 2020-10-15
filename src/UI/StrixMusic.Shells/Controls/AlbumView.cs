using StrixMusic.Sdk.Observables;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shell.Default.Controls
{
    /// <summary>
    /// A Templated <see cref="Control"/> for showing an <see cref="ObservableAlbum"/> as a page.
    /// </summary>
    public sealed partial class AlbumView : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AlbumView"/> class.
        /// </summary>
        /// <param name="album">The Album in view.</param>
        public AlbumView(ObservableAlbum album)
        {
            this.DefaultStyleKey = typeof(AlbumView);
            DataContext = album;
            LoadTracksAsync();
        }

        /// <summary>
        /// The ViewModel for this page item
        /// </summary>
        public ObservableAlbum ViewModel => (DataContext as ObservableAlbum) !;

        private async void LoadTracksAsync()
        {
            await ViewModel.PopulateMoreTracksAsync(25);
        }
    }
}
