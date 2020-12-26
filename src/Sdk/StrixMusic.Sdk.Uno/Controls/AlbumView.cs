using System.Threading.Tasks;
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
            _ = LoadAsync();
        }

        /// <summary>
        /// The <see cref="AlbumViewModel"/> for this control.
        /// </summary>
        public AlbumViewModel ViewModel => (AlbumViewModel)DataContext;

        private async Task LoadAsync()
        {
            if (!ViewModel.PopulateMoreTracksCommand.IsRunning)
                await ViewModel.PopulateMoreTracksCommand.ExecuteAsync(25);

            if (!ViewModel.PopulateMoreImagesCommand.IsRunning)
                await ViewModel.PopulateMoreImagesCommand.ExecuteAsync(25);
        }
    }
}
