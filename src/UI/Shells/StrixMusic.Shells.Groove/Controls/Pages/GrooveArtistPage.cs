using StrixMusic.Shells.Groove.Controls.Pages.Abstract;
using StrixMusic.Shells.Groove.ViewModels.Pages;
using Windows.UI.Xaml;

namespace StrixMusic.Shells.Groove.Controls.Pages
{
    public partial class GrooveArtistPage : GroovePageControl<ArtistViewViewModel>
    {
        /// <summary>
        /// The backing dependency property for <see cref="ViewModel"/>.
        /// </summary>
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(nameof(ViewModel), typeof(ArtistViewViewModel), typeof(GrooveArtistPage), new PropertyMetadata(null));

        public GrooveArtistPage()
        {
            this.DefaultStyleKey = typeof(GrooveArtistPage);
        }

        /// <summary>
        /// The <see cref="ArtistViewViewModel"/> for the <see cref="GrooveArtistPage"/> template.
        /// </summary>
        public ArtistViewViewModel? ViewModel
        {
            get { return (ArtistViewViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
    }
}
