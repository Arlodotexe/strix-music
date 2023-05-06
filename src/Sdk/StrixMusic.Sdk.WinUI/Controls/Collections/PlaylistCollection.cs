using System.Threading.Tasks;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Sdk.WinUI.Controls.Collections.Abstract;
using StrixMusic.Sdk.WinUI.Controls.Items;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Sdk.WinUI.Controls.Collections
{
    /// <summary>
    /// A templated <see cref="Control"/> for displaying an <see cref="IPlaylistCollectionViewModel"/>.
    /// </summary>
    /// <remarks>
    /// This class temporarily only displays <see cref="PlaylistViewModel"/>s.
    /// </remarks>
    public partial class PlaylistCollection : CollectionControl<PlaylistViewModel, PlaylistItem>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlaylistCollection"/> class.
        /// </summary>
        public PlaylistCollection()
        {
            this.DefaultStyleKey = typeof(PlaylistCollection);
            DataContext = this;
        }

        /// <summary>
        /// Backing dependency property for <see cref="Collection"/>.
        /// </summary>
        public IPlaylistCollectionViewModel Collection
        {
            get => (IPlaylistCollectionViewModel)GetValue(CollectionProperty);
            set => SetValue(CollectionProperty, value);
        }

        /// <summary>
        /// Dependency property for <ses cref="IPlayableCollectionViewModel" />.
        /// </summary>
        public static readonly DependencyProperty CollectionProperty =
            DependencyProperty.Register(nameof(Collection), typeof(IPlaylistCollectionViewModel), typeof(PlaylistCollection), new PropertyMetadata(0));

        /// <inheritdoc />
        protected override void OnApplyTemplate()
        {
            // OnApplyTemplate is often a more appropriate point to deal with
            // adjustments to the template-created visual tree than is the Loaded event.
            // The Loaded event might occur before the template is applied,
            // and the visual tree might be incomplete as of Loaded.
            base.OnApplyTemplate();

            AttachHandlers();
        }

        /// <inheritdoc/>
        protected override async Task LoadMore()
        {
            if (!Collection.PopulateMorePlaylistsCommand.IsRunning)
                await Collection.PopulateMorePlaylistsCommand.ExecuteAsync(25);
        }

        /// <inheritdoc/>
        protected override void CheckAndToggleEmpty()
        {
            if (!Collection.PopulateMorePlaylistsCommand.IsRunning && Collection.TotalPlaylistItemsCount == 0)
                EmptyContentVisibility = Visibility.Visible;
        }

        private void AttachHandlers() => Unloaded += PlaylistCollection_Unloaded;

        private void PlaylistCollection_Unloaded(object sender, RoutedEventArgs e) => DetachHandlers();

        private void DetachHandlers() => Unloaded -= PlaylistCollection_Unloaded;
    }
}
