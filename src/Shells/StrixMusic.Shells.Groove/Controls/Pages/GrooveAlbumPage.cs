using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using OwlCore.Extensions;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.Groove.Helper;
using Windows.UI;
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
        }

        /// <summary>
        /// Backing property for <see cref="BackgroundColor"/>.
        /// </summary>
        public static readonly DependencyProperty BackgroundColorProperty =
            DependencyProperty.Register(nameof(BackgroundColor), typeof(Color?), typeof(GrooveAlbumPage), new PropertyMetadata(null, null));

        /// <summary>
        /// Gets or sets the color of the background for the <see cref="Controls.Pages.GrooveAlbumPage"/>.
        /// </summary>
        public Color? BackgroundColor
        {
            get => (Color)GetValue(BackgroundColorProperty);
            set => SetValue(BackgroundColorProperty, value);
        }

        /// <summary>
        /// Backing property for <see cref="Album"/>.
        /// </summary>
        public static readonly DependencyProperty AlbumProperty =
            DependencyProperty.Register(nameof(Album), typeof(AlbumViewModel), typeof(GrooveAlbumPage), new PropertyMetadata(null, (d, e) => d.Cast<GrooveAlbumPage>().OnAlbumChanged()));

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
            if (!(Album is null))
                _ = ProcessAlbumArtColorAsync(Album);
        }

        private async Task ProcessAlbumArtColorAsync(AlbumViewModel album)
        {
            // Load images if there aren't images loaded.
            await album.InitImageCollectionAsync();

            if (album.Images.Count > 0)
                BackgroundColor = await Task.Run(() => DynamicColorHelper.GetImageAccentColorAsync(album.Images[0].Uri));
        }
    }
}
