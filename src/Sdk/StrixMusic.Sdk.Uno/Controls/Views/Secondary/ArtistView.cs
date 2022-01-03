using StrixMusic.Sdk.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Sdk.Uno.Controls.Views.Secondary
{
    /// <summary>
    /// A Templated <see cref="Control"/> for displaying an <see cref="ArtistViewModel"/> as a page.
    /// </summary>
    public sealed partial class ArtistView : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArtistView"/> class.
        /// </summary>
        /// <param name="artistViewModel">The Artist in view.</param>
        public ArtistView(ArtistViewModel artistViewModel)
        {
            this.DefaultStyleKey = typeof(ArtistView);
            Artist = artistViewModel;
        }

        /// <summary>
        /// ViewModel holding the data for <see cref="ArtistItem" />
        /// </summary>
        public ArtistViewModel Artist
        {
            get { return (ArtistViewModel)GetValue(ArtistProperty); }
            set { SetValue(ArtistProperty, value); }
        }

        /// <summary>
        /// Dependency property for <ses cref="ArtistViewModel" />.
        /// </summary>
        // Using a DependencyProperty as the backing store for ViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ArtistProperty =
            DependencyProperty.Register(nameof(Artist), typeof(ArtistViewModel), typeof(ArtistView), new PropertyMetadata(0));
    }
}
