using OwlCore.Extensions;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.Groove.ViewModels.Pages;
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
            DependencyProperty.Register(nameof(Artist), typeof(ArtistViewModel), typeof(GrooveArtistPage), new PropertyMetadata(null, (d, e) => d.Cast<GrooveArtistPage>().OnArtistChanged()));

        private void OnArtistChanged()
        {
        }
    }
}
