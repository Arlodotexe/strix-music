using StrixMusic.Shells.Groove.Controls.Pages.Abstract;
using StrixMusic.Shells.Groove.ViewModels.Pages;
using Windows.UI.Xaml;

namespace StrixMusic.Shells.Groove.Controls.Pages
{
    public partial class GrooveAlbumPage : GroovePageControl<AlbumViewViewModel>
    {
        /// <summary>
        /// The backing dependency property for <see cref="ViewModel"/>.
        /// </summary>
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(nameof(ViewModel), typeof(AlbumViewViewModel), typeof(GrooveAlbumPage), new PropertyMetadata(null));

        public GrooveAlbumPage()
        {
            this.DefaultStyleKey = typeof(GrooveAlbumPage);
        }

        /// <summary>
        /// The <see cref="AlbumViewViewModel"/> for the <see cref="GrooveAlbumPage"/> template.
        /// </summary>
        public AlbumViewViewModel? ViewModel
        {
            get { return (AlbumViewViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
    }
}
