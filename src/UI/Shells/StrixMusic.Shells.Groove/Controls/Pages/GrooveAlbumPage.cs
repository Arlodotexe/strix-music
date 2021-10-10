using Microsoft.Toolkit.Diagnostics;
using OwlCore.Extensions;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.Groove.ViewModels.Pages;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.Groove.Controls.Pages
{
    public partial class GrooveAlbumPage : Control
    {
        /// <summary>
        /// The backing dependency property for <see cref="ViewModel"/>.
        /// </summary>
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(nameof(ViewModel), typeof(GrooveAlbumPageViewModel), typeof(GrooveAlbumPage), new PropertyMetadata(null));

        /// <summary>
        /// The backing dependency property for <see cref="ViewModel"/>.
        /// </summary>
        public static readonly DependencyProperty AlbumProperty =
            DependencyProperty.Register(nameof(Album), typeof(AlbumViewModel), typeof(GrooveAlbumPage), new PropertyMetadata(null, (d, e) => d.Cast<GrooveAlbumPage>().OnAlbumChanged()));

        public GrooveAlbumPage()
        {
            this.DefaultStyleKey = typeof(GrooveAlbumPage);
        }

        /// <summary>
        /// The <see cref="AlbumViewViewModel"/> for the <see cref="GrooveAlbumPage"/> template.
        /// </summary>
        public AlbumViewModel? Album
        {
            get { return (AlbumViewModel)GetValue(AlbumProperty); }
            set { SetValue(AlbumProperty, value); }
        }

        /// <summary>
        /// The <see cref="AlbumViewViewModel"/> for the <see cref="GrooveAlbumPage"/> template.
        /// </summary>
        public GrooveAlbumPageViewModel? ViewModel
        {
            get { return (GrooveAlbumPageViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        private void OnAlbumChanged()
        {
            Guard.IsNotNull(Album, nameof(Album));
            ViewModel = new GrooveAlbumPageViewModel(Album);
        }
    }
}
