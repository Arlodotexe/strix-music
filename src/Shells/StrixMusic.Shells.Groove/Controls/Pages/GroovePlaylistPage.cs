using OwlCore.Extensions;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.Groove.ViewModels.Pages;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.Groove.Controls.Pages
{
    /// <summary>
    /// A <see cref="Control"/> to display a <see cref="Sdk.ViewModels.PlaylistViewModel"/> on a page.
    /// </summary>
    public partial class GroovePlaylistPage : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GroovePlaylistPage"/> class.
        /// </summary>
        public GroovePlaylistPage()
        {
            DefaultStyleKey = typeof(GroovePlaylistPage);
            DataContext = new GroovePlaylistPageViewModel();
        }

        /// <summary>
        /// The backing depenency property for <see cref="Playlist"/>.s
        /// </summary>
        public static readonly DependencyProperty PlaylistProperty =
            DependencyProperty.Register(nameof(Playlist), typeof(PlaylistViewModel), typeof(GroovePlaylistPage), new PropertyMetadata(null, (d, e) => d.Cast<GroovePlaylistPage>().OnPlaylistChanged()));

        /// <summary>
        /// The <see cref="GroovePlaylistPageViewModel"/> for the <see cref="GrooveHomePage"/> template.
        /// </summary>
        public GroovePlaylistPageViewModel ViewModel => (GroovePlaylistPageViewModel)DataContext;

        /// <summary>
        /// The playlist to display.
        /// </summary>
        public PlaylistViewModel Playlist
        {
            get { return (PlaylistViewModel)GetValue(PlaylistProperty); }
            set { SetValue(PlaylistProperty, value); }
        }

        private void OnPlaylistChanged()
        {
            ViewModel.Playlist = Playlist;
        }
    }
}
