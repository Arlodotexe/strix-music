using System.Threading.Tasks;
using OwlCore.Extensions;
using StrixMusic.Sdk.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Sdk.WinUI.Controls.Views.Secondary
{
    /// <summary>
    /// A Templated <see cref="Control"/> for showing an <see cref="PlaylistViewModel"/> as a page.
    /// </summary>
    public sealed partial class PlaylistView : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlaylistView"/> class.
        /// </summary>
        /// <param name="playlistViewModel">The Playlist in view.</param>
        public PlaylistView(PlaylistViewModel playlistViewModel)
        {
            this.DefaultStyleKey = typeof(PlaylistView);
            DataContext = playlistViewModel;
        }

        /// <summary>
        /// Backing dependency property for <see cref="Playlist"/>.
        /// </summary>
        public static readonly DependencyProperty PlaylistProperty = 
            DependencyProperty.Register(nameof(Playlist),
                                        typeof(PlaylistViewModel),
                                        typeof(PlaylistView),
                                        new PropertyMetadata(null, (s, e) => ((PlaylistView)s).OnPlaylistChanged()));

        /// <summary>
        /// The playlist to display.
        /// </summary>
        public PlaylistViewModel? Playlist
        {
            get { return (PlaylistViewModel)GetValue(PlaylistProperty); }
            set { SetValue(PlaylistProperty, value); }
        }

        private void OnPlaylistChanged()
        {
            if (Playlist is null)
                return;

            if (!Playlist.InitTrackCollectionAsyncCommand.IsRunning)
                _ = Playlist.InitTrackCollectionAsyncCommand.ExecuteAsync(null);
        }
    }
}
