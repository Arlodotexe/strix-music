using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Sdk.WinUI.Controls.Items.Abstract;

namespace StrixMusic.Sdk.WinUI.Controls.Items
{
    /// <summary>
    /// A Templated <see cref="Control"/> for displaying an <see cref="PlaylistViewModel"/> in a list.
    /// </summary>
    public class PlaylistItem : ItemControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlaylistItem"/> class.
        /// </summary>
        public PlaylistItem()
        {
            DefaultStyleKey = typeof(PlaylistItem);
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

        private void PlaylistItem_Unloaded(object sender, RoutedEventArgs e)
        {
            DetachEvents();
        }

        private void PlaylistItem_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= PlaylistItem_Loaded;
        }

        /// <summary>
        /// The playlist to display.
        /// </summary>
        public IPlaylist? Playlist
        {
            get => (IPlaylist?)GetValue(PlaylistProperty);
            set => SetValue(PlaylistProperty, value);
        }

        /// <summary>
        /// The playlist to display.
        /// </summary>
        public PlaylistViewModel PlaylistVm => (PlaylistViewModel)GetValue(PlaylistViewModelProperty);

        /// <summary>
        /// Dependency property for <see cref="Playlist"/>.
        /// </summary>
        public static readonly DependencyProperty PlaylistProperty =
            DependencyProperty.Register(nameof(Playlist), typeof(IPlaylist), typeof(PlaylistItem), new PropertyMetadata(null, (d, e) => ((PlaylistItem)d).OnPlaylistChanged(e.OldValue as IPlaylist, e.NewValue as IPlaylist)));

        /// <summary>
        /// Dependency property for <see cref="PlaylistVm"/>.
        /// </summary>
        public static readonly DependencyProperty PlaylistViewModelProperty =
            DependencyProperty.Register(nameof(Playlist), typeof(PlaylistViewModel), typeof(PlaylistItem), new PropertyMetadata(null));

        /// <summary>
        /// Fires when the <see cref="Playlist"/> is changed.
        /// </summary>
        protected virtual void OnPlaylistChanged(IPlaylist? oldValue, IPlaylist? newValue)
        {
            if (newValue is not null)
                SetValue(PlaylistViewModelProperty, Playlist as PlaylistViewModel ?? new PlaylistViewModel(newValue));

            _ = PlaylistVm.InitAsync();
        }
    }
}
