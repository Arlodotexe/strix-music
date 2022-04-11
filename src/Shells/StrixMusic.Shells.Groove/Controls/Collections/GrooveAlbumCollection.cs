using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using OwlCore.Extensions;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.Groove.Messages.Navigation.Pages;
using StrixMusic.Shells.Groove.ViewModels.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.Groove.Controls.Collections
{
    /// <summary>
    /// A <see cref="Control"/> for displaying <see cref="Sdk.ViewModels.AlbumCollectionViewModel"/>s in the Groove Shell.
    /// </summary>
    public partial class GrooveAlbumCollection : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GrooveAlbumCollection"/> class.
        /// </summary>
        public GrooveAlbumCollection()
        {
            DefaultStyleKey = typeof(GrooveAlbumCollection);
            NavigateToAlbumCommand = new RelayCommand<AlbumViewModel>(NavigateToAlbum);
        }

        /// <summary>
        /// A Command that requests a navigation to an album page.
        /// </summary>
        public RelayCommand<AlbumViewModel> NavigateToAlbumCommand { get; private set; }

        /// <summary>
        /// The ViewModel for a <see cref="GrooveAlbumCollection"/>.
        /// </summary>
        public GrooveAlbumCollectionViewModel ViewModel => (GrooveAlbumCollectionViewModel)DataContext;

        /// <summary>
        /// The album collection to display.
        /// </summary>
        public IAlbumCollectionViewModel? AlbumCollection
        {
            get { return (IAlbumCollectionViewModel)GetValue(AlbumCollectionProperty); }
            set { SetValue(AlbumCollectionProperty, value); }
        }

        /// <summary>
        /// The backing dependency property for <see cref="AlbumCollection"/>.
        /// </summary>
        public static readonly DependencyProperty AlbumCollectionProperty =
            DependencyProperty.Register(nameof(AlbumCollection), typeof(IAlbumCollectionViewModel), typeof(GrooveAlbumCollection), new PropertyMetadata(null, (d, e) => d.Cast<GrooveAlbumCollection>().OnAlbumCollectionChanged()));

        private void OnAlbumCollectionChanged()
        {
        }

        private void NavigateToAlbum(AlbumViewModel? viewModel)
        {
            if (viewModel != null)
                WeakReferenceMessenger.Default.Send(new AlbumViewNavigationRequestMessage(viewModel));
        }
    }
}
