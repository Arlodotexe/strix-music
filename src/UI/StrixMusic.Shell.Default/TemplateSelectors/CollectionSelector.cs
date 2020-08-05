using StrixMusic.CoreInterfaces.Interfaces;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Strix_Music.Shell.Default.TemplateSelectors
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
        /// The <see cref="DataTemplate"/> for any <see cref="IPlayableCollectionGroup"/>.
        /// </summary>
        public DataTemplate? PlayableCollectionGroupTemplate { get; set; }

        /// <inheritdoc/>
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            switch (item)
            {
                case IPlaylist _:
                    return TrackListTemplate!;
                default:
                    return PlayableCollectionGroupTemplate!;
            }
        }
    }
}
