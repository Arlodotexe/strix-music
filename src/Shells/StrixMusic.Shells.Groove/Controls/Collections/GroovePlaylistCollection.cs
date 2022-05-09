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
        }

        /// <summary>
        /// The backing dependency property for <see cref="Collection"/>.
        /// </summary>
        public static readonly DependencyProperty CollectionProperty =
            DependencyProperty.Register(nameof(Collection), typeof(IPlaylistCollectionViewModel), typeof(GroovePlaylistCollection), new PropertyMetadata(null, (d, e) => d.Cast<GroovePlaylistCollection>().OnPlaylistCollectionChanged()));

        /// <summary>
        /// A view model for this control.
        /// </summary>
        public GroovePlaylistCollectionViewModel ViewModel
        {
            get => (GroovePlaylistCollectionViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        /// <summary>
        /// The backing Dependency Property for <see cref="ViewModel"/>.
        /// </summary>
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(GroovePlaylistCollectionViewModel), typeof(GroovePlaylistCollection), new PropertyMetadata(new GroovePlaylistCollectionViewModel()));

        /// <summary>
        /// The playlist collection to display.
        /// </summary>
        public IPlaylistCollectionViewModel Collection
        {
            get => (IPlaylistCollectionViewModel)GetValue(CollectionProperty);
            set => SetValue(CollectionProperty, value);
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
            ViewModel.SelectedPlaylist = null!;
        }
    }
}
