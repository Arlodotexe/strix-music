using StrixMusic.Sdk.Uno.Controls.Items.Abstract;
using StrixMusic.Sdk.ViewModels;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Sdk.Uno.Controls.Items
{
    /// <summary>
    /// A Templated <see cref="Control"/> for displaying an <see cref="AlbumViewModel"/> in a list.
    /// </summary>
    public partial class AlbumItem : ItemControl
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

        private void AlbumItem_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Loaded -= AlbumItem_Loaded;
        }

        /// <summary>
        /// The <see cref="ArtistViewModel"/> for the control.
        /// </summary>
        public AlbumViewModel ViewModel => (AlbumViewModel)DataContext;
    }
}
