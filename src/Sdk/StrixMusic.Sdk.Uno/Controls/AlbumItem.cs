using System.Threading.Tasks;
using StrixMusic.Sdk.ViewModels;
using Windows.UI.Xaml.Controls;
using OwlCore.Extensions;

namespace StrixMusic.Sdk.Uno.Controls
{
    /// <summary>
    /// A Templated <see cref="Control"/> for displaying an <see cref="AlbumViewModel"/> in a list.
    /// </summary>
    public sealed partial class AlbumItem : ItemControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AlbumItem"/> class.
        /// </summary>
        public AlbumItem()
        {
            this.DefaultStyleKey = typeof(AlbumItem);

            InitAsync().FireAndForget();
        }

        private async Task InitAsync()
        {
            if (!ViewModel.PopulateMoreArtistsCommand.IsRunning)
                await ViewModel.PopulateMoreArtistsCommand.ExecuteAsync(5);
        } 

        /// <summary>
        /// The <see cref="ArtistViewModel"/> for the control.
        /// </summary>
        public AlbumViewModel ViewModel => (AlbumViewModel)DataContext;
    }
}
