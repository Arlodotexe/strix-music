using OwlCore.Extensions;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.Groove.ViewModels.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.Groove.Controls.Collections
{
    /// <summary>
    /// A <see cref="Control"/> for displaying <see cref="PlaylistCollectionViewModel"/>s in the Groove Shell.
    /// </summary>
    public partial class GroovePlaylistCollection : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GroovePlaylistCollection"/> class.
        /// </summary>
        public GroovePlaylistCollection()
        {
            this.DefaultStyleKey = typeof(GroovePlaylistCollection);
            DataContext = new GroovePlaylistCollectionViewModel();
        }

        /// <summary>
        /// The backing dependency property for <see cref="Collection"/>.
        /// </summary>
        public static readonly DependencyProperty CollectionProperty =
            DependencyProperty.Register(nameof(Collection), typeof(IPlaylistCollectionViewModel), typeof(GroovePlaylistCollection), new PropertyMetadata(null, (d, e) => d.Cast<GroovePlaylistCollection>().OnPlaylistCollectionChanged()));

        /// <summary>
        /// The ViewModel for a <see cref="GroovePlaylistCollection"/>.
        /// </summary>
        public GroovePlaylistCollectionViewModel ViewModel => (GroovePlaylistCollectionViewModel)DataContext;

        /// <summary>
        /// The playlist collection to display.
        /// </summary>
        public IPlaylistCollectionViewModel Collection
        {
            get { return (IPlaylistCollectionViewModel)GetValue(CollectionProperty); }
            set { SetValue(CollectionProperty, value); }
        }

        private void OnPlaylistCollectionChanged()
        {
            ViewModel.PlaylistCollection = Collection;
        }

        /// <summary>
        /// Clears the selected item in the <see cref="GroovePlaylistCollection"/>.
        /// </summary>
        public void ClearSelected()
        {
            if (!(ViewModel is null))
                ViewModel.SelectedPlaylist = null!;
        }
    }
}
