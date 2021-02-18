using StrixMusic.Sdk.Uno.Controls.Collections.Abstract;
using StrixMusic.Sdk.Uno.Controls.Items;
using StrixMusic.Sdk.ViewModels;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Sdk.Uno.Controls.Collections
{
    /// <summary>
    /// A templated <see cref="Control"/> for displaying an <see cref="IArtistCollectionViewModel"/>.
    /// </summary>
    /// <remarks>
    /// This class temporarily only displays <see cref="ArtistViewModel"/>s.
    /// </remarks>
    public sealed partial class ArtistCollection : CollectionControl<ArtistViewModel, ArtistItem>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArtistCollection"/> class.
        /// </summary>
        public ArtistCollection()
        {
            this.DefaultStyleKey = typeof(ArtistCollection);
        }

        /// <summary>
        /// The <see cref="IArtistCollectionViewModel"/> for the control.
        /// </summary>
        public IArtistCollectionViewModel ViewModel => (IArtistCollectionViewModel)DataContext;

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
            if (!ViewModel.PopulateMoreArtistsCommand.IsRunning)
                await ViewModel.PopulateMoreArtistsCommand.ExecuteAsync(25);
        }

        /// <inheritdoc/>
        protected override void CheckAndToggleEmpty()
        {
            if (!ViewModel.PopulateMoreArtistsCommand.IsRunning &&
                ViewModel.TotalArtistItemsCount == 0)
            
            SetEmptyVisibility(Visibility.Visible);
        }

        private void AttachHandlers()
        {
            Unloaded += ArtistCollection_Unloaded;
        }

        private void DetachHandlers()
        {
            Unloaded -= ArtistCollection_Unloaded;
        }

        private void ArtistCollection_Unloaded(object sender, RoutedEventArgs e)
        {
            DetachHandlers();
        }
    }
}
