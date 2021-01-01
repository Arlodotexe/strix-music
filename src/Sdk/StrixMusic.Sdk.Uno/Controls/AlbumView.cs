using System.Threading.Tasks;
using OwlCore.Extensions;
using StrixMusic.Sdk.ViewModels;
using Windows.UI.Xaml.Controls;

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

            LoadAsync().FireAndForget();
        }

        /// <summary>
        /// The <see cref="AlbumViewModel"/> for this control.
        /// </summary>
        public AlbumViewModel ViewModel => (AlbumViewModel)DataContext;

        private async Task LoadAsync()
        {
            if (!ViewModel.PopulateMoreArtistsCommand.IsRunning)
                await ViewModel.PopulateMoreArtistsCommand.ExecuteAsync(5);
        }
    }
}
