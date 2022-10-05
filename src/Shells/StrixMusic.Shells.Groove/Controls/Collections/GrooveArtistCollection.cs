using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.Groove.Messages.Navigation.Pages;
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
            NavigateToArtistCommand = new RelayCommand<ArtistViewModel>(NavigateToArtist);
        }

        /// <summary>
        /// The backing dependency property for <see cref="ArtistCollection"/>.F
        /// </summary>
        public static readonly DependencyProperty ArtistCollectionProperty =
            DependencyProperty.Register(nameof(ArtistCollection), typeof(IArtistCollectionViewModel), typeof(GrooveArtistCollection), new PropertyMetadata(null, (d, e) => ((GrooveArtistCollection)d).OnArtistCollectionChanged()));

        /// <summary>
        /// The artist collection to display.
        /// </summary>
        public IArtistCollectionViewModel ArtistCollection
        {
            get { return (IArtistCollectionViewModel)GetValue(ArtistCollectionProperty); }
            set { SetValue(ArtistCollectionProperty, value); }
        }

        /// <summary>
        /// A Command that requests a navigation to an artist page.
        /// </summary>
        public RelayCommand<ArtistViewModel> NavigateToArtistCommand { get; private set; }

        private void NavigateToArtist(ArtistViewModel? viewModel)
        {
            if (viewModel != null)
                WeakReferenceMessenger.Default.Send(new ArtistViewNavigationRequestMessage(viewModel));
        }

        private void OnArtistCollectionChanged()
        {
        }
    }
}
