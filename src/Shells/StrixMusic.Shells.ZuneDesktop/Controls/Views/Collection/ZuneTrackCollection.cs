using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Sdk.WinUI.Controls.Collections;
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
            Guard.IsNotNull(PART_ArtistColumn, nameof(PART_ArtistColumn));

            PART_ArtistColumn.Visibility = Visibility.Collapsed;
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
        /// Dependency property for <ses cref="ITrackCollectionViewModel" />.
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
