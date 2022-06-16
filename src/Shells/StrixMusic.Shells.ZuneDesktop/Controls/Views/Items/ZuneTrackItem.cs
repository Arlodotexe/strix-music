using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OwlCore.Extensions;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Sdk.WinUI.Controls.Items;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace StrixMusic.Shells.ZuneDesktop.Controls.Views.Items
{
    /// <summary>
    /// Zune implemenation for <see cref="ZuneTrackItem"/>.
    /// </summary>
    public class ZuneTrackItem : TrackItem
    {
        private bool _isSelected = false;

        /// <summary>
        /// The top most grid around the track item.
        /// </summary>
        public Grid? PART_MainGrid { get; private set; }

        /// <summary>
        /// GradientStop for the artist column.
        /// </summary>
        public GradientStop? PART_GradientStopArtist { get; private set; }

        /// <summary>
        /// GradientStop for the track column.
        /// </summary>
        public GradientStop? PART_GradientStopTrack { get; private set; }

        /// <summary>
        /// Artist column textblock.
        /// </summary>
        public TextBlock? PART_Tb { get; private set; }

        /// <summary>
        /// Gradient that holds tracks column gradient. 
        /// </summary>
        public Rectangle? PART_GradientRect { get; private set; }

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
            Loaded += ZuneTrackItem_Loaded;
        }

        /// <inheritdoc />
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            PART_MainGrid = GetTemplateChild(nameof(PART_MainGrid)) as Grid;
            PART_GradientRect = GetTemplateChild(nameof(PART_GradientRect)) as Rectangle;
            PART_Tb = GetTemplateChild(nameof(PART_Tb)) as TextBlock;

            if (PART_Tb != null && PART_GradientRect != null)
            {
                Grid.SetColumnSpan(PART_Tb, 3);
                Grid.SetColumnSpan(PART_GradientRect, 3);
            }

            if (PART_MainGrid != null)
            {
                PART_MainGrid.PointerEntered += PART_MainGrid_PointerEntered;
                PART_MainGrid.PointerExited += PART_MainGrid_PointerExited;
            }

            PART_GradientStopArtist = GetTemplateChild(nameof(PART_GradientStopArtist)) as GradientStop;
            PART_GradientStopTrack = GetTemplateChild(nameof(PART_GradientStopTrack)) as GradientStop;
        }

        /// <summary>
        /// Triggers whenever the item is unselected.
        /// </summary>
        internal void ZuneTrackItemUnselected()
        {
            _isSelected = false;
            SetGradientColor("#FFFFFF");
        }

        /// <summary>
        /// Triggers whenever the item is selected.
        /// </summary>
        internal void ZuneTrackItemSelected()
        {
            _isSelected = true;
            SetGradientColor("#ECECED");
        }

        private void PART_MainGrid_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (!_isSelected)
                SetGradientColor("#FFFFFF");
        }

        private void PART_MainGrid_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (!_isSelected)
                SetGradientColor("#F3F4F4");
        }

        private void SetGradientColor(string color)
        {
            var winUIColor = (Windows.UI.Color)XamlBindingHelper.ConvertValue(typeof(Windows.UI.Color), color);

            if (PART_GradientStopArtist != null)
                PART_GradientStopArtist.Color = winUIColor;

            if (PART_GradientStopTrack != null)
                PART_GradientStopTrack.Color = winUIColor;
        }

        private void ZuneTrackItem_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= ZuneTrackItem_Loaded;
            Unloaded += ZuneTrackItem_Unloaded;
        }

        private void ZuneTrackItem_Unloaded(object sender, RoutedEventArgs e)
        {
            Unloaded -= ZuneTrackItem_Unloaded;

            if (Track != null)
                Track.ArtistItemsCountChanged -= Track_ArtistItemsCountChanged;

            if (PART_MainGrid != null)
            {
                PART_MainGrid.PointerEntered -= PART_MainGrid_PointerEntered;
                PART_MainGrid.PointerExited -= PART_MainGrid_PointerExited;
            }
        }

        /// <inheritdoc />
        public override async void OnTrackChanged(TrackViewModel? oldValue, TrackViewModel newValue)
        {
            base.OnTrackChanged(oldValue, newValue);

            if (Track == null)
                return;

            if (ParentCollection is AlbumViewModel album)
            {
                var albumArtists = await album.GetArtistItemsAsync(album.TotalArtistItemsCount, 0).ToListAsync();

                if (album.TotalArtistItemsCount == 0)
                    album.ArtistItemsCountChanged += Album_ArtistItemsCountChanged;

                if (albumArtists.Count > 1)
                {
                    Track.ArtistItemsCountChanged += Track_ArtistItemsCountChanged;

                    if (Track.TotalArtistItemsCount == 0)
                        return;

                    var artists = await Track.GetArtistItemsAsync(Track.TotalArtistItemsCount, 0).ToListAsync();
                    PopulateArtists(artists);
                }
                else
                {
                    if (PART_Tb != null && PART_GradientRect != null)
                    {
                        Grid.SetColumnSpan(PART_Tb, 3);
                        Grid.SetColumnSpan(PART_GradientRect, 3);
                    }
                }
            }
            else
            {
                if (PART_Tb != null && PART_GradientRect != null)
                {
                    Grid.SetColumnSpan(PART_Tb, 3);
                    Grid.SetColumnSpan(PART_GradientRect, 3);
                }
            }
        }

        private async void Album_ArtistItemsCountChanged(object sender, int e)
        {
            if (Track == null)
                return;

            Track.ArtistItemsCountChanged -= Track_ArtistItemsCountChanged;

            Track.ArtistItemsCountChanged += Track_ArtistItemsCountChanged;

            if (Track.TotalArtistItemsCount == 0)
                return;

            var artists = await Track.GetArtistItemsAsync(Track.TotalArtistItemsCount, 0).ToListAsync();

            PopulateArtists(artists);

            Track.ArtistItemsCountChanged += Track_ArtistItemsCountChanged;
        }

        private async void Track_ArtistItemsCountChanged(object sender, int e)
        {
            if (Track == null)
                return;

            // Unsubsribing the event here because GetArtistsItemsAsync again triggers ArtistItemsCountChanged putting the app in an infinite stack causing StackOverflow Exception.
            Track.ArtistItemsCountChanged -= Track_ArtistItemsCountChanged;

            var artists = await Track.GetArtistItemsAsync(Track.TotalArtistItemsCount, 0).ToListAsync();
            PopulateArtists(artists.ToList());

            Track.ArtistItemsCountChanged += Track_ArtistItemsCountChanged;
        }

        private void PopulateArtists(List<Sdk.AppModels.IArtistCollectionItem> artists)
        {
            // Clear existing list.
            ArtistString = string.Empty;

            artists = artists.Distinct().ToList();
            foreach (var artist in artists)
                ArtistString += $"{artist.Name},";

            if (ArtistString != null)
                ArtistString = ArtistString.TrimEnd(',').TrimStart();


            if (PART_Tb != null && PART_GradientRect != null)
            {
                if (ArtistString == null)
                {
                    Grid.SetColumnSpan(PART_Tb, 3);
                    Grid.SetColumnSpan(PART_GradientRect, 3);
                }
                else
                {
                    Grid.SetColumnSpan(PART_Tb, 1);
                    Grid.SetColumnSpan(PART_GradientRect, 1);
                }
            }
        }
    }
}
