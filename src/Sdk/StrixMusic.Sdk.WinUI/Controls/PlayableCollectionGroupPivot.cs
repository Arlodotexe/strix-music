using System.Collections.Generic;
using System.Linq;
using System.Threading;
using StrixMusic.Sdk.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.WinUI.Controls.Views;

namespace StrixMusic.Sdk.WinUI.Controls
{
    /// <summary>
    /// Displays the content of a PlayableCollectionGroupViewModel in a Pivot.
    /// </summary>
    [ObservableObject]
    public sealed partial class PlayableCollectionGroupPivot : Control
    {
        private static readonly Dictionary<string, int> _pivotItemPositionMemo = new();
        private SynchronizationContext _synchronizationContext;
        [ObservableProperty] private PlayableCollectionGroupViewModel? _viewModel = null;

        /// <summary>
        /// The backing <see cref="DependencyProperty"/> for the <see cref="RestoreSelectedPivot"/> property.
        /// </summary>
        public static readonly DependencyProperty RestoreSelectedPivotProperty =
            DependencyProperty.Register(
                nameof(RestoreSelectedPivot),
                typeof(bool),
                typeof(PlayableCollectionGroupPivot),
                new PropertyMetadata(false));

        /// <summary>
        /// The backing <see cref="DependencyProperty"/> for the <see cref="PivotTitle"/> property.
        /// </summary>
        public static readonly DependencyProperty PivotTitleProperty =
            DependencyProperty.Register(
                nameof(PivotTitle),
                typeof(string),
                typeof(PlayableCollectionGroupPivot),
                new PropertyMetadata(string.Empty, (d, e) => ((PlayableCollectionGroupPivot)d).SetPivotTitle((string)e.NewValue)));

        /// <summary>
        /// If true, remember and restore the last pivot that the user had selected when the control is loaded.
        /// </summary>
        public bool RestoreSelectedPivot
        {
            get => (bool)GetValue(RestoreSelectedPivotProperty);
            set => SetValue(RestoreSelectedPivotProperty, value);
        }

        /// <summary>
        /// If true, remember and restore the last pivot that the user had selected when the control is loaded.
        /// </summary>
        public string PivotTitle
        {
            get => (string)GetValue(PivotTitleProperty);
            set => SetValue(PivotTitleProperty, value);
        }

        /// <summary>
        /// The backing <see cref="DependencyProperty"/> for the <see cref="AllEmptyContent"/> property.
        /// </summary>
        public static readonly DependencyProperty AllEmptyContentProperty =
            DependencyProperty.Register(
                nameof(AllEmptyContent),
                typeof(string),
                typeof(PlayableCollectionGroupPivot),
                new PropertyMetadata(null, (d, e) => ((PlayableCollectionGroupPivot)d).SetNoContentTemplate((FrameworkElement)e.NewValue)));

        /// <summary>
        /// The content to show when all the collections in this <see cref="IPlaylistCollectionViewModel"/> are empty.
        /// </summary>
        public FrameworkElement AllEmptyContent
        {
            get => (FrameworkElement)GetValue(AllEmptyContentProperty);
            set => SetValue(AllEmptyContentProperty, value);
        }

        /// <summary>
        /// The backing <see cref="DependencyProperty"/> for the <see cref="HideEmptyPivots"/> property.
        /// </summary>
        public static readonly DependencyProperty HideEmptyPivotsProperty =
            DependencyProperty.Register(
                nameof(HideEmptyPivots),
                typeof(bool),
                typeof(PlayableCollectionGroupPivot),
                new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets whether or not to hide pivots with no content.
        /// </summary>
        public bool HideEmptyPivots
        {
            get => (bool)GetValue(HideEmptyPivotsProperty);
            set => SetValue(HideEmptyPivotsProperty, value);
        }

        /// <summary>
        /// The primary pivot displayed by this control.
        /// </summary>
        public Pivot? PART_Pivot { get; private set; }

        /// <summary>
        /// The pivot item that displays an <see cref="ITrackCollectionViewModel" />
        /// </summary>
        public PivotItem? PART_SongsPivotItem { get; private set; }

        /// <summary>
        /// The pivot item that displays an <see cref="IAlbumCollectionViewModel" />
        /// </summary>
        public PivotItem? PART_AlbumsPivotItem { get; private set; }

        /// <summary>
        /// The pivot item that displays an <see cref="IArtistCollectionViewModel" />
        /// </summary>
        public PivotItem? PART_ArtistsPivotItem { get; private set; }

        /// <summary>
        /// The pivot item that displays an <see cref="IPlaylistCollectionViewModel" />
        /// </summary>
        public PivotItem? PART_PlaylistsPivotItem { get; private set; }

        /// <inheritdoc cref="AllEmptyContent"/>
        public ContentPresenter? PART_AllEmptyContentPresenter { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayableCollectionGroupPivot"/> class.
        /// </summary>
        public PlayableCollectionGroupPivot()
        {
            _synchronizationContext = SynchronizationContext.Current ?? new();
            DataContext = this;
        }

        /// <inheritdoc />
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            PART_Pivot = GetTemplateChild(nameof(PART_Pivot)) as Pivot;

            PART_SongsPivotItem = GetTemplateChild(nameof(PART_SongsPivotItem)) as PivotItem;
            PART_AlbumsPivotItem = GetTemplateChild(nameof(PART_AlbumsPivotItem)) as PivotItem;
            PART_ArtistsPivotItem = GetTemplateChild(nameof(PART_ArtistsPivotItem)) as PivotItem;
            PART_PlaylistsPivotItem = GetTemplateChild(nameof(PART_PlaylistsPivotItem)) as PivotItem;
            PART_AllEmptyContentPresenter = GetTemplateChild(nameof(PART_AllEmptyContentPresenter)) as ContentPresenter;

            RestoreMostRecentSelectedPivot();

            AttachEvents();

            ToggleAnyEmptyPivotItems();
            SetNoContentTemplate(AllEmptyContent);
        }

        private void AttachEvents()
        {
            Unloaded += PlayableCollectionGroupPivot_Unloaded;

            if (PART_Pivot != null)
            {
                PART_Pivot.SelectionChanged += PivotSelectionChanged;
            }

            if (Collection != null)
            {
                Collection.AlbumItemsCountChanged += AnyItemCountChanged;
                Collection.ArtistItemsCountChanged += AnyItemCountChanged;
                Collection.AlbumItemsCountChanged += AnyItemCountChanged;
                Collection.PlaylistItemsCountChanged += AnyItemCountChanged;
            }
        }

        private void AnyItemCountChanged(object sender, int e)=> _synchronizationContext.Post(_ => ToggleAnyEmptyPivotItems(), null);

        private void DetachEvents()
        {
            Unloaded -= PlayableCollectionGroupPivot_Unloaded;

            if (PART_Pivot != null)
            {
                PART_Pivot.SelectionChanged -= PivotSelectionChanged;
            }

            if (Collection != null)
            {
                Collection.AlbumItemsCountChanged -= AnyItemCountChanged;
                Collection.ArtistItemsCountChanged -= AnyItemCountChanged;
                Collection.AlbumItemsCountChanged -= AnyItemCountChanged;
                Collection.PlaylistItemsCountChanged -= AnyItemCountChanged;
            }
        }

        private void PlayableCollectionGroupPivot_Unloaded(object sender, RoutedEventArgs e) => DetachEvents();

        /// <summary>
        /// Used to handle saving of most recently selected pivot.
        /// </summary>
        public void PivotSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PART_Pivot == null || Collection is null)
                return;

            _pivotItemPositionMemo[Collection.Id] = PART_Pivot.SelectedIndex;
        }

        /// <summary>
        /// Changes the <see cref="Pivot.Title"/> on the controls' primary <see cref="PART_Pivot"/>.
        /// </summary>
        /// <param name="title">The title to set. </param>
        public void SetPivotTitle(string title)
        {
            if (PART_Pivot != null)
            {
                PART_Pivot.Title = title;
            }
        }

        /// <summary>
        /// Sets the content to show when all the collections in this <see cref="IPlaylistCollectionViewModel"/> are empty.
        /// </summary>
        /// <param name="frameworkElement"></param>
        public void SetNoContentTemplate(FrameworkElement frameworkElement)
        {
            if (PART_AllEmptyContentPresenter != null)
            {
                PART_AllEmptyContentPresenter.Content = frameworkElement;
            }
        }

        private void RestoreMostRecentSelectedPivot()
        {
            if (!RestoreSelectedPivot || PART_Pivot == null || Collection is null)
                return;

            var pivotSelectionMemo = _pivotItemPositionMemo;

            if (pivotSelectionMemo != null && pivotSelectionMemo.TryGetValue(Collection.Id, out var value))
            {
                PART_Pivot.SelectedIndex = value;
            }
        }

        private void ToggleAnyEmptyPivotItems()
        {
            if (!HideEmptyPivots)
                return;

            TogglePivotItemViaCollectionCount(nameof(PART_SongsPivotItem), PART_SongsPivotItem, Collection?.TotalTrackCount ?? 0);
            TogglePivotItemViaCollectionCount(nameof(PART_AlbumsPivotItem), PART_AlbumsPivotItem, Collection?.TotalAlbumItemsCount ?? 0);
            TogglePivotItemViaCollectionCount(nameof(PART_ArtistsPivotItem), PART_ArtistsPivotItem, Collection?.TotalArtistItemsCount ?? 0);
            TogglePivotItemViaCollectionCount(nameof(PART_PlaylistsPivotItem), PART_PlaylistsPivotItem, Collection?.TotalPlaylistItemsCount ?? 0);

            if (PART_AllEmptyContentPresenter != null)
            {
                var allEmpty = (Collection?.TotalTrackCount ?? 0) == 0 &&
                               (Collection?.TotalAlbumItemsCount ?? 0) == 0 &&
                               (Collection?.TotalArtistItemsCount ?? 0) == 0 &&
                               (Collection?.TotalPlaylistItemsCount ?? 0) == 0;

                PART_AllEmptyContentPresenter.Visibility = allEmpty ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void TogglePivotItemViaCollectionCount(string pivotItemName, PivotItem? pivotItem, int itemCount)
        {
            if (PART_Pivot == null || pivotItem == null)
                return;

            var pivotPositionExists = _pivotItemPositionMemo.TryGetValue(pivotItemName, out var position);

            if (itemCount > 0 && PART_Pivot.Items.Contains(pivotItem))
            {
                if (!pivotPositionExists)
                {
                    _pivotItemPositionMemo.Add(pivotItemName, PART_Pivot.Items.IndexOf(pivotItem));
                }

                PART_Pivot.Items.Remove(pivotItem);
            }

            if (!PART_Pivot.Items.Contains(pivotItem) && pivotPositionExists && itemCount > 0)
            {
                PART_Pivot.Items.Insert(position, pivotItem);
            }
        }

        /// <summary>
        /// The root object containing all data needed to power strix.
        /// </summary>
        public IPlayableCollectionGroup? Collection
        {
            get => (IPlayableCollectionGroup?)GetValue(CollectionProperty);
            set => SetValue(CollectionProperty, value);
        }

        /// <summary>
        /// Backing dependency property for <see cref="Collection"/>.
        /// </summary>
        public static readonly DependencyProperty CollectionProperty =
            DependencyProperty.Register(nameof(Collection), typeof(IPlayableCollectionGroup), typeof(PlayableCollectionGroupPivot), new PropertyMetadata(null, (d, e) => ((PlayableCollectionGroupPivot)d).OnPlayableCollectionGroupChanged(e.OldValue as IPlayableCollectionGroup, e.NewValue as IPlayableCollectionGroup)));

        private void OnPlayableCollectionGroupChanged(IPlayableCollectionGroup? oldValue, IPlayableCollectionGroup? newValue)
        {
            if (newValue is not null)
            {
                if (newValue is PlayableCollectionGroupViewModel vm)
                    ViewModel = vm;
                else
                    ViewModel = new PlayableCollectionGroupViewModel(newValue);
            }
            else
            {
                ViewModel = null;
            }
        }
    }
}
