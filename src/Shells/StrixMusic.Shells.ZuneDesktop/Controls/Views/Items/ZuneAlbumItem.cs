using System;
using CommunityToolkit.Diagnostics;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Sdk.WinUI.Controls.Items;
using StrixMusic.Shells.ZuneDesktop.Controls.Views.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.ZuneDesktop.Controls.Views.Items
{
    /// <summary>
    /// Zune custom implemation for <see cref="AlbumItem"/>.
    /// </summary>
    public partial class ZuneAlbumItem : AlbumItem
    {
        private const int DEFAULT_ALBUM_WIDTH = 98;
        private const int DEFAULT_ALBUM_HEIGHT = 88;
        private const int ALBUM_VIEW_ALBUM_WIDTH = 150;
        private const int ALBUM_VIEW_ALBUM_HEIGHT = 150;

        /// <summary>
        /// Holds the current state of the zune <see cref="CollectionContent"/>.
        /// </summary>
        public CollectionContentType ZuneCollectionType
        {
            get => (CollectionContentType)GetValue(ZuneCollectionTypeProperty);
            set => SetValue(ZuneCollectionTypeProperty, value);
        }

        /// <summary>
        /// Dependency property for <ses cref="CollectionContentType" />.
        /// </summary>
        public static readonly DependencyProperty ZuneCollectionTypeProperty =
            DependencyProperty.Register(nameof(ZuneCollectionType), typeof(CollectionContentType), typeof(ZuneAlbumItem), new PropertyMetadata(CollectionContentType.Albums, (s, e) => OnZuneCollectionTypeChanged(s, e)));

        /// <summary>
        /// Flag to determine if the collection item should be selected by default or not.
        /// </summary>
        public bool DefaultSelectionState
        {
            get => (bool)GetValue(DefaultSelectionStateProperty);
            set => SetValue(DefaultSelectionStateProperty, value);
        }

        /// <summary>
        /// Dependency property for <ses cref="CollectionContent" />.
        /// </summary>
        public static readonly DependencyProperty DefaultSelectionStateProperty =
            DependencyProperty.Register(nameof(DefaultSelectionState), typeof(bool), typeof(ZuneAlbumItem), new PropertyMetadata(false, (s, e) => OnDefaultSelectionStateChanged(s, e)));

        /// <summary>
        /// The AlbumCollection GridView control.
        /// </summary>
        public Button? PART_PlayIcon { get; private set; }

        /// <summary>
        /// The Root Grid of AlbumItem.
        /// </summary>
        public Grid? PART_RootGrid { get; private set; }

        /// <summary>
        /// The Image Grid of AlbumItem.
        /// </summary>
        public Grid? PART_AlbumGrid { get; private set; }

        /// <summary>
        /// The ArtistName TextBlock.
        /// </summary>
        public TextBlock? PART_ArtistName { get; private set; }

        /// <summary>
        /// The LabelGrid Grid of AlbumItem.
        /// </summary>
        public StackPanel? PART_LabelPnl { get; private set; }

        /// <summary>
        /// Emits the <see cref="AlbumItem"/> whose collection needs to be played.
        /// </summary>
        public event EventHandler<AlbumViewModel>? AlbumPlaybackTriggered;

        /// <summary>
        /// Creates a new instance of <see cref="ZuneAlbumItem"/>.
        /// </summary>
        public ZuneAlbumItem()
        {
        }

        /// <inheritdoc/>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            PART_PlayIcon = GetTemplateChild(nameof(PART_PlayIcon)) as Button;
            PART_RootGrid = GetTemplateChild(nameof(PART_RootGrid)) as Grid;
            PART_AlbumGrid = GetTemplateChild(nameof(PART_AlbumGrid)) as Grid;
            PART_LabelPnl = GetTemplateChild(nameof(PART_LabelPnl)) as StackPanel;
            PART_ArtistName = GetTemplateChild("ArtistName") as TextBlock;

            if (ZuneCollectionType == CollectionContentType.Albums)
            {
                ResizeAlbumItem(isAlbumCollectionView: true);
            }
            else
            {
                ResizeAlbumItem();
            }

            Guard.IsNotNull(PART_PlayIcon, nameof(PART_PlayIcon));
            Guard.IsNotNull(PART_RootGrid, nameof(PART_RootGrid));
            PART_PlayIcon.Tapped += PART_PlayIcon_Tapped;
            Unloaded += ZuneAlbumItem_Unloaded;
        }

        private static void OnDefaultSelectionStateChanged(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is bool b)
            {
                if (b)
                {
                    var zuneAlbumItem = s as ZuneAlbumItem;
                    VisualStateManager.GoToState(zuneAlbumItem, "Selected", true);
                }
                else
                {
                    var zuneAlbumItem = s as ZuneAlbumItem;
                    VisualStateManager.GoToState(zuneAlbumItem, "Unselected", true);
                }
            }
        }

        private static void OnZuneCollectionTypeChanged(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            var zuneAlbumItem = s as ZuneAlbumItem;

            Guard.IsNotNull(zuneAlbumItem, nameof(zuneAlbumItem));

            var newValue = (CollectionContentType)e.NewValue;

            if (newValue == CollectionContentType.Albums)
            {
                zuneAlbumItem.ResizeAlbumItem(isAlbumCollectionView: true);

                zuneAlbumItem.UpdateLayout();
            }
            else
            {
                zuneAlbumItem.ResizeAlbumItem();
            }
        }

        private void ResizeAlbumItem(bool isAlbumCollectionView = false)
        {
            if (PART_RootGrid != null && PART_AlbumGrid != null && PART_LabelPnl != null)
            {
                PART_RootGrid.Width = isAlbumCollectionView ? ALBUM_VIEW_ALBUM_WIDTH : DEFAULT_ALBUM_WIDTH;
                PART_LabelPnl.Width = isAlbumCollectionView ? ALBUM_VIEW_ALBUM_WIDTH - 10 : DEFAULT_ALBUM_WIDTH - 10;
                PART_AlbumGrid.Width = isAlbumCollectionView ? ALBUM_VIEW_ALBUM_WIDTH : DEFAULT_ALBUM_WIDTH;
                PART_AlbumGrid.Height = isAlbumCollectionView ? ALBUM_VIEW_ALBUM_HEIGHT : DEFAULT_ALBUM_HEIGHT;
            }
        }

        private void ZuneAlbumItem_Unloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Guard.IsNotNull(PART_PlayIcon, nameof(PART_PlayIcon));
            PART_PlayIcon.Tapped -= PART_PlayIcon_Tapped;
        }

        private void PART_PlayIcon_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            if (Album is not null)
            {
                AlbumPlaybackTriggered?.Invoke(this, Album as AlbumViewModel ?? new AlbumViewModel(Album));
            }
        }

        /// <inheritdoc/>
        protected override void OnAlbumChanged(IAlbum? oldValue, IAlbum? newValue)
        {
            base.OnAlbumChanged(oldValue, newValue);
            UpdateArtistName();

            if (AlbumVm != null)
                AlbumVm.Artists.CollectionChanged += (_, _) => UpdateArtistName();
        }

        private void UpdateArtistName()
        {
            if (PART_ArtistName != null && AlbumVm?.Artists.Count > 0 && AlbumVm.Artists[0] is IPlayable playable)
                PART_ArtistName.Text = playable.Name;
        }
    }
}
