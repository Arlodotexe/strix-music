using StrixMusic.Sdk.ViewModels;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Sdk.Uno.Controls
{
    /// <summary>
    /// A templated <see cref="Control"/> for displaying an <see cref="IAlbumCollectionViewModel"/>.
    /// </summary>
    /// <remarks>
    /// This class temporarily only displays <see cref="AlbumViewModel"/>s.
    /// </remarks>
    public sealed partial class AlbumCollection : CollectionControl<AlbumViewModel, AlbumItem>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AlbumCollection"/> class.
        /// </summary>
        public AlbumCollection()
        {
            this.DefaultStyleKey = typeof(ArtistCollection);
        }

        /// <summary>
        /// The <see cref="IAlbumCollectionViewModel"/> for the control.
        /// </summary>
        public IAlbumCollectionViewModel ViewModel => (IAlbumCollectionViewModel)DataContext;

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
            if (!ViewModel.PopulateMoreAlbumsCommand.IsRunning)
                await ViewModel.PopulateMoreAlbumsCommand.ExecuteAsync(25);
        }

        private void AttachHandlers()
        {
            Unloaded += AlbumCollection_Unloaded;
        }

        private void AlbumCollection_Unloaded(object sender, RoutedEventArgs e)
        {
            DetachHandlers();
        }

        private void DetachHandlers()
        {
            Unloaded -= AlbumCollection_Unloaded;
        }
    }
}
