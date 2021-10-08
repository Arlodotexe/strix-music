using StrixMusic.Shells.Groove.Controls.Pages.Abstract;
using StrixMusic.Shells.Groove.ViewModels.Pages;
using Windows.UI.Xaml;

namespace StrixMusic.Shells.Groove.Controls.Pages
{
    public partial class GroovePlaylistPage : GroovePageControl<PlaylistViewViewModel>
    {
        /// <summary>
        /// The backing dependency property for <see cref="ViewModel"/>.
        /// </summary>
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(nameof(ViewModel), typeof(PlaylistViewViewModel), typeof(GroovePlaylistPage), new PropertyMetadata(null));

        public GroovePlaylistPage()
        {
            this.DefaultStyleKey = typeof(GroovePlaylistPage);
        }

        /// <summary>
        /// The <see cref="PlaylistViewViewModel"/> for the <see cref="GroovePlaylistPage"/> template.
        /// </summary>
        public PlaylistViewViewModel? ViewModel
        {
            get { return (PlaylistViewViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
    }
}
