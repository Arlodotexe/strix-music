using OwlCore.Extensions;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.Groove.ViewModels.Pages;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.Groove.Controls.Pages
{
    /// <summary>
    /// A <see cref="Control"/> to display a <see cref="AlbumViewModel"/> on a page.
    /// </summary>
    public partial class GrooveAlbumPage : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GrooveAlbumPage"/> class.
        /// </summary>
        public GrooveAlbumPage()
        {
            DefaultStyleKey = typeof(GrooveAlbumPage);
            DataContext = new GrooveAlbumPageViewModel();
        }

        /// <summary>
        /// Backing property for <see cref="Album"/>.
        /// </summary>
        public static readonly DependencyProperty AlbumProperty =
            DependencyProperty.Register(nameof(Album), typeof(AlbumViewModel), typeof(GrooveAlbumPage), new PropertyMetadata(null, (d, e) => d.Cast<GrooveAlbumPage>().OnAlbumChanged()));

        /// <summary>
        /// The <see cref="GrooveAlbumPageViewModel"/> for the <see cref="GrooveAlbumPage"/> template.
        /// </summary>
        public GrooveAlbumPageViewModel ViewModel => (GrooveAlbumPageViewModel)DataContext;

        /// <summary>
        /// The album being displayed.
        /// </summary>
        public AlbumViewModel? Album
        {
            get => (AlbumViewModel)GetValue(AlbumProperty);
            set => SetValue(AlbumProperty, value);
        }

        private void OnAlbumChanged()
        {
            ViewModel.Album = Album;
        }
    }
}
