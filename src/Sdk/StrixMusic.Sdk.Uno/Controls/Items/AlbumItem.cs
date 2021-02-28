using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using StrixMusic.Sdk.Uno.Controls.Items.Abstract;
using StrixMusic.Sdk.ViewModels;

namespace StrixMusic.Sdk.Uno.Controls.Items
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
            AttachEvents();
        }

        private void AttachEvents()
        {
            Loaded += AlbumItem_Loaded;
            Unloaded += AlbumItem_Unloaded;
        }

        private void DetachEvents()
        {
            Unloaded -= AlbumItem_Unloaded;
        }

        private void AlbumItem_Unloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            DetachEvents();
        }

        private async void AlbumItem_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Loaded -= AlbumItem_Loaded;

            await InitAsync();
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
