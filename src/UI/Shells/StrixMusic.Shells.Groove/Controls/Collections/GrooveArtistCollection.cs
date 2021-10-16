using OwlCore.Extensions;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.Groove.ViewModels.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.Groove.Controls.Collections
{
    /// <summary>
    /// A <see cref="Control"/> for displaying <see cref="ArtistCollectionViewModel"/>s in the Groove Shell.
    /// </summary>
    public partial class GrooveArtistCollection : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GrooveTrackCollection"/> class.
        /// </summary>
        public GrooveArtistCollection()
        {
            DefaultStyleKey = typeof(GrooveArtistCollection);
            DataContext = new GrooveArtistCollectionViewModel();
        }

        /// <summary>
        /// The backing dependency property for <see cref="ArtistCollection"/>.F
        /// </summary>
        public static readonly DependencyProperty ArtistCollectionProperty =
            DependencyProperty.Register(nameof(ArtistCollection), typeof(IArtistCollectionViewModel), typeof(GrooveArtistCollection), new PropertyMetadata(null, (d, e) => d.Cast<GrooveArtistCollection>().OnArtistCollectionChanged()));

        /// <summary>
        /// The artist collection to display.
        /// </summary>
        public IArtistCollectionViewModel ArtistCollection
        {
            get { return (IArtistCollectionViewModel)GetValue(ArtistCollectionProperty); }
            set { SetValue(ArtistCollectionProperty, value); }
        }

        /// <summary>
        /// The ViewModel for a <see cref="GrooveArtistCollection"/>
        /// </summary>
        public GrooveArtistCollectionViewModel ViewModel => (GrooveArtistCollectionViewModel)DataContext;

        private void OnArtistCollectionChanged()
        {
            ViewModel.ArtistCollection = ArtistCollection;
        }
    }
}
