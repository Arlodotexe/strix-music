using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using StrixMusic.Sdk.Core.ViewModels;
using StrixMusic.Sdk.Core.Data;

namespace StrixMusic.Sdk.Uno.Controls
{
    /// <summary>
    /// A templated <see cref="Control"/> for displaying an <see cref="IAlbumCollectionViewModel"/>.
    /// </summary>
    /// <remarks>
    /// This class temporarily only displays <see cref="AlbumViewModel"/>s.
    /// </remarks>
    public sealed partial class AlbumCollection : CollectionControl<IAlbum, AlbumItem>
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
            Unloaded += AlbumCollection_Unloaded;
        }

        private void AlbumCollection_Unloaded(object sender, RoutedEventArgs e)
        {
            DetachHandlers();
        }

        private void DetachHandlers()
        {
        }
    }
}
