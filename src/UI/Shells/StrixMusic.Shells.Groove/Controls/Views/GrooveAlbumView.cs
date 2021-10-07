using StrixMusic.Sdk.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using OwlCore.Extensions;
using StrixMusic.Shells.Groove.ViewModels;

namespace StrixMusic.Shells.Groove.Controls.Views
{
    /// <summary>
    /// An AlbumView control for Groove Shell.
    /// </summary>
    public class GrooveAlbumView : Control
    {
        /// <summary>
        /// The backing dependency property for <see cref="Album"/>.
        /// </summary>
        public static readonly DependencyProperty AlbumProperty =
            DependencyProperty.Register(nameof(Album), typeof(AlbumViewModel), typeof(GrooveAlbumView), new PropertyMetadata(null, (s, e) => e.Cast<GrooveAlbumView>().OnAlbumChanged()));

        /// <summary>
        /// The backing dependency property for <see cref="ViewModel"/>.
        /// </summary>
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(nameof(ViewModel), typeof(AlbumViewViewModel), typeof(GrooveAlbumView), new PropertyMetadata(null));

        /// <summary>
        /// The <see cref="AlbumViewModel"/> being displayed.
        /// </summary>
        public AlbumViewModel? Album
        {
            get { return (AlbumViewModel)GetValue(AlbumProperty); }
            set { SetValue(AlbumProperty, value); }
        }

        /// <summary>
        /// The <see cref="AlbumViewViewModel"/> for the <see cref="GrooveAlbumView"/> template.
        /// </summary>
        public AlbumViewViewModel? ViewModel { get; private set; }

        private void OnAlbumChanged()
        {
            if (Album == null)
                return;

            ViewModel = new AlbumViewViewModel(Album);
        }
    }
}
