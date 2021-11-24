using OwlCore.Extensions;
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
    public partial class ArtistCollection : CollectionControl<ArtistViewModel, ArtistItem>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArtistCollection"/> class.
        /// </summary>
        public ArtistCollection()
        {
            this.DefaultStyleKey = typeof(ArtistCollection);
        }

        /// <summary>
        /// Backing dependency property for <see cref="Collection"/>.
        /// </summary>
        public IArtistCollectionViewModel Collection
        {
            get { return (IArtistCollectionViewModel)GetValue(CollectionProperty); }
            set { SetValue(CollectionProperty, value); }
        }

        /// <summary>
        /// Dependency property for <ses cref="IArtistCollectionViewModel" />.
        /// </summary>
        // Using a DependencyProperty as the backing store for ViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CollectionProperty =
            DependencyProperty.Register(nameof(Collection), typeof(IArtistCollectionViewModel), typeof(ArtistCollection), new PropertyMetadata(0));

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

        private void AttachHandlers()
        {
            Unloaded += ArtistCollection_Unloaded;
        }

        private void DetachHandlers()
        {
            Unloaded -= ArtistCollection_Unloaded;
        }

        /// <inheritdoc/>
        protected override async Task LoadMore()
        {
            if (Collection != null && !Collection.PopulateMoreArtistsCommand.IsRunning)
                await Collection.PopulateMoreArtistsCommand.ExecuteAsync(25);
        }

        /// <inheritdoc/>
        protected override void CheckAndToggleEmpty()
        {
            if (Collection != null && !Collection.PopulateMoreArtistsCommand.IsRunning && Collection.TotalArtistItemsCount == 0)
                SetEmptyVisibility(Visibility.Visible);
        }

        private void ArtistCollection_Unloaded(object sender, RoutedEventArgs e)
        {
            DetachHandlers();
        }
    }
}
