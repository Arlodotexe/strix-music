using OwlCore.Extensions;
using StrixMusic.Sdk;
using StrixMusic.Sdk.Data.Merged;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.Groove.ViewModels.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.Groove.Controls.Collections
{
    public partial class GrooveAlbumCollection : Control
    {
        public static readonly DependencyProperty AlbumCollectionProperty =
            DependencyProperty.Register(nameof(AlbumCollection), typeof(IAlbumCollectionViewModel), typeof(GrooveAlbumCollection), new PropertyMetadata(null, (d, e) => d.Cast<GrooveAlbumCollection>().OnAlbumCollectionChanged()));

        public GrooveAlbumCollection()
        {
            this.DefaultStyleKey = typeof(GrooveAlbumCollection);
            DataContext = new GrooveAlbumCollectionViewModel();
        }

        public IAlbumCollectionViewModel? AlbumCollection
        {
            get => (IAlbumCollectionViewModel)GetValue(AlbumCollectionProperty);
            set => SetValue(AlbumCollectionProperty, value);
        }

        public GrooveAlbumCollectionViewModel ViewModel => (GrooveAlbumCollectionViewModel)DataContext;

        private void OnAlbumCollectionChanged()
        {
            if (AlbumCollection != null && ViewModel != null)
                ViewModel.ViewModel = AlbumCollection;
        }
    }
}
