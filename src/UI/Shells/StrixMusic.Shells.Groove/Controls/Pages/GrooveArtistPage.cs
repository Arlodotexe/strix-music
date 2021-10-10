using StrixMusic.Shells.Groove.ViewModels.Pages;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.Groove.Controls.Pages
{
    public partial class GrooveArtistPage : Control
    {
        /// <summary>
        /// The backing dependency property for <see cref="ViewModel"/>.
        /// </summary>
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(nameof(ViewModel), typeof(GrooveArtistPageViewModel), typeof(GrooveArtistPage), new PropertyMetadata(null));

        public GrooveArtistPage()
        {
            this.DefaultStyleKey = typeof(GrooveArtistPage);
        }

        /// <summary>
        /// The <see cref="GrooveArtistPageViewModel"/> for the <see cref="GrooveArtistPage"/> template.
        /// </summary>
        public GrooveArtistPageViewModel? ViewModel
        {
            get { return (GrooveArtistPageViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
    }
}
