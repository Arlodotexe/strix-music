using Windows.UI.Xaml.Controls;
using StrixMusic.Sdk.ViewModels;

namespace StrixMusic.Sdk.Uno.Controls
{
    /// <summary>
    /// A Templated <see cref="Control"/> for showing an <see cref="AlbumViewModel"/> as a page.
    /// </summary>
    public sealed partial class AlbumView : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AlbumView"/> class.
        /// </summary>
        /// <param name="albumViewModel">The Album in view.</param>
        public AlbumView(AlbumViewModel albumViewModel)
        {
            this.DefaultStyleKey = typeof(AlbumView);
            DataContext = albumViewModel;
            LoadTracksAsync();
        }

        /// <summary>
        /// The ViewModel for this page item
        /// </summary>
        public AlbumViewModel ViewModel => (DataContext as AlbumViewModel) !;

        private async void LoadTracksAsync()
        {
            await ViewModel.PopulateMoreTracksCommand.ExecuteAsync(25);
        }
    }
}
