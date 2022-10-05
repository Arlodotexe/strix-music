using System.Threading.Tasks;
using OwlCore.Extensions;
using StrixMusic.Sdk.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using StrixMusic.Sdk.WinUI.Controls.Items;

namespace StrixMusic.Sdk.WinUI.Controls.Views.Secondary
{
    /// <summary>
    /// A Templated <see cref="Control"/> for showing an <see cref="AlbumViewModel"/> as a page.
    /// </summary>
    public sealed partial class AlbumView : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AlbumView"/> class.
        /// </summary>
        /// <param name="albumViewModel">The Album in view.</param>
        public AlbumView(AlbumViewModel albumViewModel)
        {
            this.DefaultStyleKey = typeof(AlbumView);
            Album = albumViewModel;

            LoadAsync().Forget();
        }

        /// <summary>
        /// ViewModel holding the data for <see cref="AlbumItem" />
        /// </summary>
        public AlbumViewModel Album
        {
            get { return (AlbumViewModel)GetValue(AlbumProperty); }
            set { SetValue(AlbumProperty, value); }
        }

        /// <summary>
        /// Dependency property for <ses cref="AlbumViewModel" />.
        /// </summary>
        // Using a DependencyProperty as the backing store for ViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AlbumProperty =
            DependencyProperty.Register(nameof(Album), typeof(ArtistViewModel), typeof(AlbumView), new PropertyMetadata(0));

        private async Task LoadAsync()
        {
            if (!Album.PopulateMoreArtistsCommand.IsRunning)
                await Album.PopulateMoreArtistsCommand.ExecuteAsync(25);

            if (!Album.PopulateMoreTracksCommand.IsRunning)
                await Album.PopulateMoreTracksCommand.ExecuteAsync(25);
        }
    }
}
