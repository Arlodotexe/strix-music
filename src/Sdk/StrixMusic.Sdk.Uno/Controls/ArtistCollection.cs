using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Uno.Extensions.Specialized;
using System;
using StrixMusic.Sdk.ViewModels;

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
        }

        private void ArtistCollection_Unloaded(object sender, RoutedEventArgs e)
        {
            DetachHandlers();
        }
    }
}
