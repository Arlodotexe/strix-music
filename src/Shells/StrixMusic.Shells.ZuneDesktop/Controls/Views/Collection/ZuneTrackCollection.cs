using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Sdk.WinUI.Controls.Collections;
using StrixMusic.Shells.ZuneDesktop.Controls.Views.Items;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.ZuneDesktop.Controls.Views.Collection
{
    /// <summary>
    /// Zune implmenation for the <see cref="ZuneTrackCollection"/>.
    /// </summary>
    public class ZuneTrackCollection : TrackCollection
    {
        /// <summary>
        /// Holds the instance of a artist column textblock.
        /// </summary>
        public TextBlock? PART_ArtistColumn { get; private set; }

        /// <summary>
        /// Holds the <see cref="ZuneTrackCollection"/> listview.
        /// </summary>
        public ListView? PART_Selector { get; private set; }

        /// <summary>
        /// Creates a new instace for <see cref="ZuneTrackCollection"/>.
        /// </summary>
        public ZuneTrackCollection()
        {
        }

        /// <inheritdoc/>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            PART_ArtistColumn = GetTemplateChild(nameof(PART_ArtistColumn)) as TextBlock;
            PART_Selector = GetTemplateChild(nameof(PART_Selector)) as ListView;

            Guard.IsNotNull(PART_ArtistColumn, nameof(PART_ArtistColumn));

            if (PART_Selector != null)
                PART_Selector.SelectionChanged += PART_Selector_SelectionChanged;

            PART_ArtistColumn.Visibility = Visibility.Collapsed;
        }

        private void PART_Selector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PART_Selector != null)
            {
                foreach (var removed in e.RemovedItems)
                {
                    var index = Collection.Tracks.IndexOf((TrackViewModel)removed);
                    var listViewItem = PART_Selector.ContainerFromIndex(index) as ListViewItem;

                    if (listViewItem == null)
                        return;

                    var uiElement = listViewItem.ContentTemplateRoot;
                    if (uiElement is ZuneTrackItem zuneTrackItem)
                    {
                        zuneTrackItem.ZuneTrackItemUnselected();
                    }
                }

                foreach (var added in e.AddedItems)
                {
                    var index = Collection.Tracks.IndexOf((TrackViewModel)added);
                    var listViewItem = PART_Selector.ContainerFromIndex(index) as ListViewItem;

                    if (listViewItem == null)
                        return;

                    var uiElement = listViewItem.ContentTemplateRoot;
                    if (uiElement is ZuneTrackItem zuneTrackItem)
                    {
                        zuneTrackItem.ZuneTrackItemSelected();
                    }
                }
            }
        }

        /// <summary>
        /// Backing dependency property for <see cref="Collection"/>.
        /// </summary>
        public new ITrackCollectionViewModel Collection
        {
            get { return (ITrackCollectionViewModel)GetValue(CollectionProperty); }
            set { SetValue(CollectionProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="ITrackCollectionViewModel" />.
        /// </summary>
        public static readonly new DependencyProperty CollectionProperty =
            DependencyProperty.Register(nameof(Collection), typeof(ITrackCollectionViewModel), typeof(ZuneTrackCollection), new PropertyMetadata(null, (s, e) =>
            {
                if (s is ZuneTrackCollection trackCollection)
                {
                    if (e.NewValue is ITrackCollectionViewModel zt)
                    {
                        zt.Tracks.CollectionChanged += trackCollection.Tracks_CollectionChanged;
                    }

                    if (e.OldValue is ITrackCollectionViewModel zte)
                    {
                        zte.Tracks.CollectionChanged -= trackCollection.Tracks_CollectionChanged;
                    }
                }
            }));

        /// <summary>
        /// Backing dependency property for <see cref="AlbumArtistCollection"/>.
        /// </summary>
        public IAlbumCollectionViewModel? AlbumArtistCollection
        {
            get { return (IAlbumCollectionViewModel)GetValue(AlbumArtistCollectionProperty); }
            set { SetValue(AlbumArtistCollectionProperty, value); }
        }

        /// <summary>
        /// Dependency property for <ses cref="IAlbumCollectionViewModel" />.
        /// </summary>
        public static readonly DependencyProperty AlbumArtistCollectionProperty =
            DependencyProperty.Register(nameof(AlbumArtistCollection), typeof(ITrackCollectionViewModel), typeof(ZuneTrackCollection), new PropertyMetadata(null, null));

        private async void Tracks_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (PART_ArtistColumn == null)
                return;

            if (Collection is AlbumViewModel)
            {
                foreach (var track in Collection.Tracks)
                {
                    var artists = await track.GetArtistItemsAsync(track.TotalArtistItemsCount, 0).ToListAsync();

                    if (artists.Count > 1)
                    {
                        PART_ArtistColumn.Visibility = Visibility.Visible;
                        return;
                    }
                }
                PART_ArtistColumn.Visibility = Visibility.Collapsed;
            }
            else
            {
                PART_ArtistColumn.Visibility = Visibility.Collapsed;
            }
        }

        /// <inheritdoc/>
        protected override async Task LoadMore()
        {
            if (Collection == null)
                return;

            if (!Collection.PopulateMoreTracksCommand.IsRunning)
                await Collection.PopulateMoreTracksCommand.ExecuteAsync(25);
        }
    }
}
