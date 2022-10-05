using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.Groove.ViewModels.Pages;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.Groove.Controls.Pages
{
    /// <summary>
    /// A <see cref="Control"/> to display an <see cref="IPlaylistCollectionViewModel"/>.
    /// </summary>
    public partial class GroovePlaylistsPage : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GroovePlaylistPage"/> class.
        /// </summary>
        public GroovePlaylistsPage()
        {
            DefaultStyleKey = typeof(GroovePlaylistsPage);
            DataContext = new GroovePlaylistsPageViewModel();
        }

        /// <summary>
        /// The <see cref="GroovePlaylistsPageViewModel"/> for the <see cref="GroovePlaylistsPage"/> template.
        /// </summary>
        public GroovePlaylistsPageViewModel ViewModel => (GroovePlaylistsPageViewModel)DataContext;

        /// <summary>
        /// The playlist collection being displayed.
        /// </summary>
        public IPlaylistCollectionViewModel? PlaylistCollection
        {
            get { return (IPlaylistCollectionViewModel)GetValue(PlaylistCollectionProperty); }
            set { SetValue(PlaylistCollectionProperty, value); }
        }

        /// <summary>
        /// Backing dependency property for <see cref="PlaylistCollection"/>.
        /// </summary>
        public static readonly DependencyProperty PlaylistCollectionProperty =
            DependencyProperty.Register(nameof(PlaylistCollection), typeof(IPlaylistCollectionViewModel), typeof(GroovePlaylistsPage), new PropertyMetadata(null, (d,e) => ((GroovePlaylistsPage)d).OnPlaylistCollectionChanged()));

        private void OnPlaylistCollectionChanged()
        {
            ViewModel.PlaylistCollection = PlaylistCollection;
        }
    }
}
