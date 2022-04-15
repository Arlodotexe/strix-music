using OwlCore.Extensions;
using StrixMusic.Sdk.WinUI.Controls.Items.Abstract;
using StrixMusic.Sdk.ViewModels;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Sdk.WinUI.Controls.Items
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
            if (!Playlist.PopulateMoreTracksCommand.IsRunning)
                await Playlist.PopulateMoreTracksCommand.ExecuteAsync(5);
        }

        /// <summary>
        /// Backing dependency property for <see cref="Playlist"/>.
        /// </summary>
        public static readonly DependencyProperty PlaylistProperty = DependencyProperty.Register(nameof(Playlist), typeof(PlaylistViewModel), typeof(PlaylistItem),
                                                                   new PropertyMetadata(null, null));

        /// <summary>
        /// The playlist to display.
        /// </summary>
        public PlaylistViewModel Playlist
        {
            get { return (PlaylistViewModel)GetValue(PlaylistProperty); }
            set { SetValue(PlaylistProperty, value); }
        }
    }
}
