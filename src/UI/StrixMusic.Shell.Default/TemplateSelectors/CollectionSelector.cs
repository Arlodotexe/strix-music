using StrixMusic.Sdk.Interfaces;
using StrixMusic.Sdk.Observables;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shell.Default.TemplateSelectors
{
    /// <summary>
    /// A <see cref="DataTemplateSelector"/> for the collections in a <see cref="IPlayableCollectionGroup"/> library.
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
        /// The <see cref="DataTemplate"/> for any <see cref="IPlayableCollectionGroup"/>.
        /// </summary>
        public DataTemplate? PlayableCollectionGroupTemplate { get; set; }

        /// <inheritdoc/>
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            switch (item)
            {
                case ObservableTrack _:
                    return TrackListTemplate!;
                case ObservableAlbum _:
                    return AlbumListTemplate!;
                default:
                    return PlayableCollectionGroupTemplate!;
            }
        }
    }
}
