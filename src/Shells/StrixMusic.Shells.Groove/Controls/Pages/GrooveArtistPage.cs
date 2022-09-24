using StrixMusic.Sdk.ViewModels;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.Groove.Controls.Pages
{
    /// <summary>
    /// A <see cref="Control"/> to display an <see cref="ArtistViewModel"/>.
    /// </summary>
    public partial class GrooveArtistPage : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GrooveArtistPage"/> class.
        /// </summary>
        public GrooveArtistPage()
        {
            DefaultStyleKey = typeof(GrooveArtistPage);
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
        /// The artist to display in this control.
        /// </summary>
        public ArtistViewModel? Artist
        {
            get { return (ArtistViewModel?)GetValue(ArtistProperty); }
            set { SetValue(ArtistProperty, value); }
        }

        /// <summary>
        /// Backing dependency property for <see cref="Artist"/>.
        /// </summary>
        public static readonly DependencyProperty ArtistProperty = 
            DependencyProperty.Register(nameof(Artist), typeof(ArtistViewModel), typeof(GrooveArtistPage), new PropertyMetadata(null, (d, e) => ((GrooveArtistPage)d).OnArtistChanged()));

        private void OnArtistChanged()
        {
        }
    }
}
