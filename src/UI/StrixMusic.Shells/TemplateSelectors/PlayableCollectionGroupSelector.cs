using StrixMusic.Sdk.Interfaces;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.TemplateSelectors
{
    /// <summary>
    /// A <see cref="DataTemplateSelector"/> for the collections in a <see cref="IPlayableCollectionGroup"/> library.
    /// </summary>
    public class PlayableCollectionGroupSelector : DataTemplateSelector
    {
        /// <summary>
        /// The <see cref="DataTemplate"/> for track lists.
        /// </summary>
        public DataTemplate? AlbumTemplate { get; set; }

        /// <summary>
        /// The <see cref="DataTemplate"/> for any <see cref="IPlayableCollectionGroup"/>.
        /// </summary>
        public DataTemplate? ArtistTemplate { get; set; }

        /// <inheritdoc/>
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            switch (item)
            {
                case IAlbum _:
                    return AlbumTemplate!;
                case IArtist _:
                    return ArtistTemplate!;
                default:
                    return ArtistTemplate!;
            }
        }
    }
}
