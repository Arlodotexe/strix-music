using OwlCore.Extensions;
using StrixMusic.Sdk.Uno.Controls.Items.Abstract;
using StrixMusic.Sdk.ViewModels;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Sdk.Uno.Controls.Items
{
    /// <summary>
    /// A Templated <see cref="Control"/> for displaying an <see cref="PlaylistViewModel"/> in a list.
    /// </summary>
    public partial class PlaylistItem : ItemControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlaylistItem"/> class.
        /// </summary>
        public PlaylistItem()
        {
            this.DefaultStyleKey = typeof(PlaylistItem);
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
            if (!ViewModel.PopulateMoreTracksCommand.IsRunning)
                await ViewModel.PopulateMoreTracksCommand.ExecuteAsync(5);
        }

        /// <summary>
        /// The <see cref="PlaylistViewModel"/> for the control.
        /// </summary>
        public PlaylistViewModel ViewModel => (PlaylistViewModel)DataContext;
    }
}
