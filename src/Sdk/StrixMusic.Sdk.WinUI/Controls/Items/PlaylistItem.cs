using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Sdk.WinUI.Controls.Items.Abstract;
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
            Loaded += PlaylistItem_Loaded;
            Unloaded += PlaylistItem_Unloaded;
        }

        private void DetachEvents()
        {
            Unloaded -= PlaylistItem_Unloaded;
        }

        private void PlaylistItem_Unloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            DetachEvents();
        }

        private void PlaylistItem_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Loaded -= PlaylistItem_Loaded;
        }

        /// <summary>
        /// Backing dependency property for <see cref="Playlist"/>.
        /// </summary>
        public static readonly DependencyProperty PlaylistProperty = DependencyProperty.Register(nameof(Playlist), typeof(IPlaylist), typeof(PlaylistItem),
                                                                   new PropertyMetadata(null, (s, e) => ((PlaylistItem)s).OnPlaylistChanged()));

        /// <summary>
        /// Backing dependency property for <see cref="PlaylistVm"/>.
        /// </summary>
        public static readonly DependencyProperty PlaylistVmProperty = DependencyProperty.Register(nameof(PlaylistVm), typeof(PlaylistViewModel), typeof(PlaylistItem),
                                                                   new PropertyMetadata(null));

        /// <summary>
        /// The playlist to display.
        /// </summary>
        public IPlaylist? Playlist
        {
            get => (IPlaylist?)GetValue(PlaylistProperty);
            set => SetValue(PlaylistProperty, value);
        }

        /// <summary>
        /// The Playlist view model to display.
        /// </summary>
        public PlaylistViewModel? PlaylistVm
        {
            get => (PlaylistViewModel?)GetValue(PlaylistVmProperty);
            set => SetValue(PlaylistVmProperty, value);
        }

        private void OnPlaylistChanged()
        {
            if (Playlist is null)
            {
                PlaylistVm = null;
                return;
            }

            if (Playlist is not PlaylistViewModel pvm)
                pvm = new PlaylistViewModel(Playlist);

            PlaylistVm = pvm;
            _ = PlaylistVm.InitAsync();
        }
    }
}
