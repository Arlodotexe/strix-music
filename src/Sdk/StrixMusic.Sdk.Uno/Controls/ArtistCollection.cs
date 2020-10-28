using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using StrixMusic.Sdk.Core.ViewModels;
using Windows.UI.Xaml.Controls.Primitives;
using Uno.Extensions.Specialized;
using StrixMusic.Sdk.Core.Data;
using System;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Uno.Controls
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

        public IArtistCollectionViewModel? ViewModel => (DataContext as IArtistCollectionViewModel);

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

        protected override async Task LoadMore()
        {
            if (!ViewModel!.PopulateMoreArtistsCommand!.IsRunning)
                await ViewModel!.PopulateMoreArtistsCommand!.ExecuteAsync(25);
        }

        private void AttachHandlers()
        {
            Unloaded += ArtistCollection_Unloaded;

        }

        private void DetachHandlers()
        {
        }

        private void ArtistCollection_Unloaded(object sender, RoutedEventArgs e)
        {
            DetachHandlers();
        }
    }
}
