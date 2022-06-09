using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OwlCore.Extensions;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Sdk.WinUI.Controls.Items;
using Windows.UI.Xaml;

namespace StrixMusic.Shells.ZuneDesktop.Controls.Views.Items
{
    /// <summary>
    /// Zune implemenation for <see cref="ZuneTrackItem"/>.
    /// </summary>
    public class ZuneTrackItem : TrackItem
    {
        /// <summary>
        /// Holds the list of artists.
        /// </summary>
        public string ArtistString
        {
            get { return (string)GetValue(ArtistStringProperty); }
            set { SetValue(ArtistStringProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="ArtistString" />.
        /// </summary>
        public static readonly DependencyProperty ArtistStringProperty =
            DependencyProperty.Register(nameof(ArtistString), typeof(string), typeof(ZuneTrackItem), new PropertyMetadata(null));

        /// <summary>
        /// Holds the current state of the zune <see cref="ITrackCollectionViewModel"/>.
        /// </summary>
        public ITrackCollectionViewModel ParentCollection
        {
            get { return (ITrackCollectionViewModel)GetValue(ParentCollectionProperty); }
            set { SetValue(ParentCollectionProperty, value); }
        }

        /// <summary>
        /// Dependency property for <ses cref="ITrackCollectionViewModel" />.
        /// </summary>
        public static readonly DependencyProperty ParentCollectionProperty =
            DependencyProperty.Register(nameof(ParentCollection), typeof(ITrackCollectionViewModel), typeof(ZuneTrackItem), new PropertyMetadata(null));

        /// <summary>
        /// Holds the current state of the zune <see cref="ParentAlbumArtistCollection"/>.
        /// </summary>
        public IAlbumCollectionViewModel ParentAlbumArtistCollection
        {
            get { return (IAlbumCollectionViewModel)GetValue(ParentAlbumArtistCollectionProperty); }
            set { SetValue(ParentAlbumArtistCollectionProperty, value); }
        }

        /// <summary>
        /// Dependency property for <ses cref="IAlbumCollectionViewModel" />.
        /// </summary>
        public static readonly DependencyProperty ParentAlbumArtistCollectionProperty =
            DependencyProperty.Register(nameof(ParentAlbumArtistCollection), typeof(IAlbumCollectionViewModel), typeof(ZuneTrackItem), new PropertyMetadata(null));

        /// <summary>
        /// Creates a new instance of <see cref="ZuneTrackItem"/>.
        /// </summary>
        public ZuneTrackItem()
        {
        }

        /// <inheritdoc />
        public override async void OnTrackChanged(TrackViewModel? oldValue, TrackViewModel newValue)
        {
            base.OnTrackChanged(oldValue, newValue);

            if (Track == null)
                return;

            var artists = await Track.GetArtistItemsAsync(Track.TotalArtistItemsCount, 0).ToListAsync();

            if (ParentCollection is AlbumViewModel album)
            {
                foreach (var artist in artists)
                {
                    if (ParentAlbumArtistCollection != null && ParentAlbumArtistCollection is ArtistViewModel aVm)
                    {
                        // Ignore the artist name from which the track originated.
                        if (aVm.Name == artist.Name)
                            continue;
                    }

                    ArtistString += $"{artist.Name},";
                }

                if (ArtistString != null)
                    ArtistString = ArtistString.TrimEnd(',').TrimStart();
            }
        }

        /// <inheritdoc />
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }
    }
}
