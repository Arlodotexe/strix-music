using StrixMusic.Sdk.Uno.Controls.Collections.Abstract;
using StrixMusic.Sdk.Uno.Controls.Items;
using StrixMusic.Sdk.ViewModels;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Sdk.Uno.Controls.Collections
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
        }

        /// <summary>
        /// The <see cref="IPlaylistCollectionViewModel"/> for the control.
        /// </summary>
        public IPlaylistCollectionViewModel ViewModel => (IPlaylistCollectionViewModel)DataContext;

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
            if (!ViewModel.PopulateMorePlaylistsCommand.IsRunning)
                await ViewModel.PopulateMorePlaylistsCommand.ExecuteAsync(25);
        }

        /// <inheritdoc/>
        protected override void CheckAndToggleEmpty()
        {
            if (!ViewModel.PopulateMorePlaylistsCommand.IsRunning &&
                ViewModel.TotalPlaylistItemsCount == 0)
                SetEmptyVisibility(Visibility.Visible);
        }

        private void AttachHandlers()
        {
            Unloaded += PlaylistCollection_Unloaded;
        }

        private void PlaylistCollection_Unloaded(object sender, RoutedEventArgs e)
        {
            DetachHandlers();
        }

        private void DetachHandlers()
        {
            Unloaded -= PlaylistCollection_Unloaded;
        }
    }
}
