using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using StrixMusic.Sdk.Core.Data;
using StrixMusic.Sdk.Core.ViewModels;

namespace StrixMusic.Sdk.Uno.TemplateSelectors
{
    /// <summary>
    /// A <see cref="DataTemplateSelector"/> for the collections in a <see cref="IPlayableCollectionGroupBase"/> library.
    /// </summary>
    public class CollectionSelector : DataTemplateSelector
    {
        /// <summary>
        /// The <see cref="DataTemplate"/> for track lists.
        /// </summary>
        public DataTemplate? TrackListTemplate { get; set; }

        /// <summary>
        /// The <see cref="DataTemplate"/> for album lists.
        /// </summary>
        public DataTemplate? AlbumListTemplate { get; set; }

        /// <summary>
        /// The <see cref="DataTemplate"/> for any <see cref="IPlayableCollectionGroupBase"/>.
        /// </summary>
        public DataTemplate? PlayableCollectionGroupTemplate { get; set; }

        /// <inheritdoc/>
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            switch (item)
            {
                case TrackViewModel _:
                    return TrackListTemplate!;
                case AlbumViewModel _:
                    return AlbumListTemplate!;
                default:
                    return PlayableCollectionGroupTemplate!;
            }
        }
    }
}
