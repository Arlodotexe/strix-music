using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.Data.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Sdk.Uno.TemplateSelectors
{
    /// <summary>
    /// A <see cref="DataTemplateSelector"/> for the collections in a <see cref="IPlayableCollectionGroupBase"/> library.
    /// </summary>
    public class PlayableCollectionGroupSelector : DataTemplateSelector
    {
        /// <summary>
        /// The <see cref="DataTemplate"/> for track lists.
        /// </summary>
        public DataTemplate? AlbumTemplate { get; set; }

        /// <summary>
        /// The <see cref="DataTemplate"/> for any <see cref="IPlayableCollectionGroupBase"/>.
        /// </summary>
        public DataTemplate? ArtistTemplate { get; set; }

        /// <inheritdoc/>
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            switch (item)
            {
                case ICoreAlbum _:
                    return AlbumTemplate!;
                case ICoreArtist _:
                    return ArtistTemplate!;
                default:
                    return ArtistTemplate!;
            }
        }
    }
}
