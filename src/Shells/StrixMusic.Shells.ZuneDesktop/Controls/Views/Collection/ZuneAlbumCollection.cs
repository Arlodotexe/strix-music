using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI.Animations;
using OwlCore.Extensions;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Sdk.WinUI.Controls.Collections.Abstract;
using StrixMusic.Shells.ZuneDesktop.Controls.Views.Collections;
using StrixMusic.Shells.ZuneDesktop.Controls.Views.Items;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace StrixMusic.Shells.ZuneDesktop.Controls.Views.Collection
{
    /// <summary>
    /// The collection to perform zune specific behaviors.
    /// </summary>
    [INotifyPropertyChanged]
    public partial class ZuneAlbumCollection : CollectionControl<ZuneAlbumCollectionItem, ZuneAlbumItem>
    {
        private ResourceLoader _loacalizationService;
        private readonly SemaphoreSlim _collectionModifyMutex = new(1, 1);

        private ObservableCollection<ZuneAlbumCollectionItem> _albumItems = new();

        /// <summary>
        /// Creates a new instance of <see cref="ZuneAlbumCollection"/>.
        /// </summary>
        public ZuneAlbumCollection()
        {
            AlbumGroupedByCollection = new ObservableGroupedCollection<string, ZuneAlbumCollectionItem>();

            AlbumItems = new ReadOnlyObservableCollection<ZuneAlbumCollectionItem>(_albumItems);

            _loacalizationService = ResourceLoader.GetForCurrentView("StrixMusic.Shells.ZuneDesktop/ZuneSettings");
        }

        /// <summary>
        /// Holds the current state of the zune <see cref="CollectionContent"/>.
        /// </summary>
        public ObservableGroupedCollection<string, ZuneAlbumCollectionItem> AlbumGroupedByCollection
        {
            get { return (ObservableGroupedCollection<string, ZuneAlbumCollectionItem>)GetValue(AlbumGroupedByCollectionProperty); }
            set { SetValue(AlbumGroupedByCollectionProperty, value); }
        }

        /// <summary>
        /// Dependency property for <ses cref="CollectionContent" />.
        /// </summary>
        public static readonly DependencyProperty AlbumGroupedByCollectionProperty =
            DependencyProperty.Register(nameof(AlbumGroupedByCollection), typeof(ObservableGroupedCollection<string, List<IAlbumCollectionItem>>), typeof(ZuneAlbumCollection), new PropertyMetadata(null, null));

        /// <summary>
        /// Holds the current state of the zune <see cref="CollectionContent"/>.
        /// </summary>
        public CollectionContentType ZuneCollectionType
        {
            get { return (CollectionContentType)GetValue(ZuneCollectionTypeProperty); }
            set { SetValue(ZuneCollectionTypeProperty, value); }
        }

        /// <summary>
        /// Dependency property for <ses cref="CollectionContent" />.
        /// </summary>
        public static readonly DependencyProperty ZuneCollectionTypeProperty =
            DependencyProperty.Register(nameof(ZuneCollectionType), typeof(CollectionContent), typeof(ZuneAlbumCollection), new PropertyMetadata(CollectionContentType.Albums, null));

        /// <summary>
        /// The backing dependency property for <see cref="Collection" />.
        /// </summary>
        public static readonly DependencyProperty CollectionProperty =
            DependencyProperty.Register(nameof(Collection), typeof(IAlbumCollectionViewModel), typeof(ZuneAlbumCollection), new PropertyMetadata(null, (d, e) => _ = ((ZuneAlbumCollection)d).OnCollectionChangedAsync((IAlbumCollectionViewModel?)e.OldValue, (IAlbumCollectionViewModel?)e.NewValue)));

        /// <summary>
        /// The collection to display.
        /// </summary>
        public IAlbumCollectionViewModel? Collection
        {
            get => (IAlbumCollectionViewModel?)GetValue(CollectionProperty);
            set => SetValue(CollectionProperty, value);
        }

        /// <summary>
        /// Holds the instance of the sort textblock.
        /// </summary>
        public TextBlock? PART_SortLbl { get; private set; }

        /// <summary>
        /// Flag to determine if albums are already loaded.
        /// </summary>
        public bool AlbumsLoaded { get; private set; }

        /// <summary>
        /// The album items being displayed in the UI.
        /// </summary>
        public ReadOnlyObservableCollection<ZuneAlbumCollectionItem> AlbumItems { get; }

        /// <summary>
        /// The AlbumCollection GridView control.
        /// </summary>
        public GridView? PART_Selector { get; private set; }

        /// <summary>
        /// Dependency property for <see cref="SortState" />.
        /// </summary>
        public static readonly DependencyProperty SortStateProperty =
            DependencyProperty.Register(nameof(SortState), typeof(ZuneSortState), typeof(ZuneArtistCollection), new PropertyMetadata(ZuneSortState.AZ, null));

        /// <summary>
        /// Holds the current sort state of the zune <see cref="ZuneAlbumCollection"/>.
        /// </summary>
        public ZuneSortState SortState
        {
            get { return (ZuneSortState)GetValue(SortStateProperty); }
            set { SetValue(SortStateProperty, value); }
        }

        /// <inheritdoc />
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            PART_Selector = GetTemplateChild(nameof(PART_Selector)) as GridView;
            PART_SortLbl = GetTemplateChild(nameof(PART_SortLbl)) as TextBlock;

            Guard.IsNotNull(PART_SortLbl, nameof(PART_SortLbl));
            Guard.IsNotNull(PART_Selector, nameof(PART_Selector));

            PART_Selector.Loaded += PART_Selector_Loaded;
            Unloaded += ZuneAlbumCollection_Unloaded;

            PART_SortLbl.Tapped += PART_SortLbl_Tapped;
        }

        /// <inheritdoc />
        protected override void CheckAndToggleEmpty()
        {
            if (Collection is null)
                return;

            if (!Collection.PopulateMoreAlbumsCommand.IsRunning && Collection.TotalAlbumItemsCount == 0)
                EmptyContentVisibility = Visibility.Visible;
        }

        /// <inheritdoc/>
        protected override async Task LoadMore()
        {
            if (Collection is null)
                return;

            if (!Collection.PopulateMoreAlbumsCommand.IsRunning)
                await Collection.PopulateMoreAlbumsCommand.ExecuteAsync(25);
        }

        private void PART_SortLbl_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            Guard.IsNotNull(PART_Selector, nameof(PART_Selector));
            Guard.IsNotNull(PART_SortLbl, nameof(PART_SortLbl));
            Guard.IsNotNull(Collection, nameof(Collection));

            switch (SortState)
            {
                // State defines the next sort state in queue which differs from the current state.
                // Sort operation performed on the original Collection also updates the _albumItems using AlbumCollectionChanged event with NotifyCollectionChangedAction.Move.
                case ZuneSortState.AZ:
                    Collection.SortAlbumCollection(AlbumSortingType.Alphanumerical, SortDirection.Descending);
                    SortState = ZuneSortState.ZA;
                    PART_SortLbl.Text = _loacalizationService.GetString("Z-A");
                    PART_Selector.ItemsSource = _albumItems;
                    break;
                case ZuneSortState.ZA:
                    PopulateAlbumGroupedByArtists();
                    SortState = ZuneSortState.Artists;
                    PART_SortLbl.Text = _loacalizationService.GetString("By Artists");
                    break;
                case ZuneSortState.Artists:
                    PopulateAlbumGroupedByReleaseYear();
                    SortState = ZuneSortState.ReleaseYear;
                    PART_SortLbl.Text = _loacalizationService.GetString("By Release Year");
                    break;
                case ZuneSortState.ReleaseYear:
                    Collection.SortAlbumCollection(AlbumSortingType.DateAdded, SortDirection.Ascending);
                    SortState = ZuneSortState.DateAdded;
                    PART_SortLbl.Text = _loacalizationService.GetString("By Date Added");
                    PART_Selector.ItemsSource = _albumItems;
                    break;
                case ZuneSortState.DateAdded:
                    Collection.SortAlbumCollection(AlbumSortingType.Alphanumerical, SortDirection.Ascending);
                    SortState = ZuneSortState.AZ;
                    PART_SortLbl.Text = _loacalizationService.GetString("A-Z");
                    PART_Selector.ItemsSource = _albumItems;
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        /// <inheritdoc/>
        protected async Task OnCollectionChangedAsync(IAlbumCollectionViewModel? oldValue, IAlbumCollectionViewModel? newValue)
        {
            using (await _collectionModifyMutex.DisposableWaitAsync())
            {
                _albumItems.Clear();
                if (newValue is not null)
                {
                    if (oldValue is IAlbumCollectionViewModel oldCollection)
                    {
                        oldCollection.Albums.CollectionChanged -= Album_CollectionChanged;
                    }

                    if (newValue.InitAlbumCollectionAsyncCommand.IsRunning && newValue.InitAlbumCollectionAsyncCommand.ExecutionTask is not null)
                        await newValue.InitAlbumCollectionAsyncCommand.ExecutionTask;
                    else if (newValue.InitAlbumCollectionAsyncCommand.CanExecute(null))
                        await newValue.InitAlbumCollectionAsyncCommand.ExecuteAsync(null);

                    foreach (var item in newValue.Albums)
                    {
                        var newItems = new ZuneAlbumCollectionItem
                        {
                            ParentCollection = newValue,
                            Album = (AlbumViewModel)item,
                            ZuneCollectionType = ZuneCollectionType,
                        };

                        _albumItems.Add(newItems);
                    }

                    newValue.Albums.CollectionChanged += Album_CollectionChanged;
                }

                if (oldValue is not null)
                {
                    oldValue.Albums.CollectionChanged -= Album_CollectionChanged;
                }
            }

            if (Collection?.Albums.Count == 1)
            {
                if (PART_Selector != null)
                {
                    PART_Selector.SelectedItem = Collection.Albums[0];
                    var album = _albumItems.FirstOrDefault();

                    if (album != null)
                        album.DefaultSelectionState = true;
                }
            }
            else
            {
                if (PART_Selector != null)
                {
                    foreach (var item in _albumItems)
                    {
                        item.DefaultSelectionState = false;
                    }
                }
            }
        }

        private async void Album_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            using (await _collectionModifyMutex.DisposableWaitAsync())
            {
                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                {
                    if (e.NewItems is null)
                        return;

                    _albumItems.InsertOrAddRange(e.NewStartingIndex, e.NewItems.Cast<object>().Select(x =>
                    {
                        var newItem = new ZuneAlbumCollectionItem
                        {
                            Album = (AlbumViewModel)x,
                            ParentCollection = Collection,
                            DefaultSelectionState = false,
                            ZuneCollectionType = ZuneCollectionType
                        };

                        return newItem;
                    }));
                }

                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
                {
                    if (e.OldItems is null)
                        return;

                    foreach (var item in e.OldItems)
                    {
                        var target = AlbumItems.FirstOrDefault(x => x.Album == item);
                        if (target is not null)
                        {
                            _albumItems.Remove(target);
                        }
                    }
                }

                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Move)
                {
                    if (e.OldItems is null)
                        return;

                    for (var i = 0; i < e.OldItems.Count; i++)
                        _albumItems.Move(i + e.OldStartingIndex, i + e.NewStartingIndex);
                }

                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
                {
                    _albumItems.Clear();
                }

                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
                    throw new NotImplementedException();
            }
        }

        private void ZuneAlbumCollection_Unloaded(object sender, RoutedEventArgs e)
        {
            Guard.IsNotNull(PART_Selector, nameof(PART_Selector));
            Guard.IsNotNull(PART_SortLbl, nameof(PART_SortLbl));

            PART_Selector.Loaded -= PART_Selector_Loaded;
            Unloaded -= ZuneAlbumCollection_Unloaded;
            PART_SortLbl.Tapped -= PART_SortLbl_Tapped;
        }

        private void PART_Selector_Loaded(object sender, RoutedEventArgs e)
        {
            AnimateCollection();
        }

        /// <summary>
        /// Gets the list of the <see cref="UIElement"/> and animates it.
        /// </summary>
        public void AnimateCollection()
        {
            if (Collection is null)
                return;

            Guard.IsNotNull(PART_Selector, nameof(PART_Selector));

            var uiElments = new List<UIElement>();

            int itemIndex = 0;
            foreach (var item in Collection.Albums)
            {
                // NOTE: ContainerFromItem doesn't work in uno.
                var gridViewItem = (GridViewItem)PART_Selector.ContainerFromIndex(itemIndex);

                if (gridViewItem == null)
                    break;

                var uiElement = gridViewItem.ContentTemplateRoot;

                // This needs to be explicitly casted to UIElement to avoid a compiler error specific to android in uno.
                uiElments.Add((UIElement)uiElement);
                itemIndex++;
            }

            FadeInAlbumCollectionItems(uiElments);
        }

        private void PopulateAlbumGroupedByReleaseYear()
        {
            Guard.IsNotNull(PART_Selector, nameof(PART_Selector));
            Guard.IsNotNull(Collection, nameof(Collection));

            AlbumGroupedByCollection.Clear();

            AlbumGroupedByCollection = new ObservableGroupedCollection<string, ZuneAlbumCollectionItem>(
            _albumItems.GroupBy(c => c?.Album?.DatePublished?.Year.ToString() ?? "Unknown")
            .OrderBy(g => g.Key));

            // Set up the CollectionViewSource.
            var cvs = new CollectionViewSource
            {
                IsSourceGrouped = true,
                Source = AlbumGroupedByCollection,
            };

            PART_Selector.ItemsSource = cvs.View;
        }

        private void PopulateAlbumGroupedByArtists()
        {
            Guard.IsNotNull(PART_Selector, nameof(PART_Selector));
            Guard.IsNotNull(Collection, nameof(Collection));

            AlbumGroupedByCollection.Clear();

            AlbumGroupedByCollection = new ObservableGroupedCollection<string, ZuneAlbumCollectionItem>(
           _albumItems.GroupBy(c => c?.Album?.Artists.FirstOrDefault()?.Name ?? "Untitled")
           .OrderBy(g => g.Key));

            // Set up the CollectionViewSource.
            var cvs = new CollectionViewSource
            {
                IsSourceGrouped = true,
                Source = AlbumGroupedByCollection,
            };

            PART_Selector.ItemsSource = cvs.View;
        }

        private void FadeInAlbumCollectionItems(List<UIElement> uiElements)
        {
            double delay = 0;

            foreach (var item in uiElements)
            {
                var animationSet = new AnimationSet();
                var duration = 250;

                animationSet.Add(new OpacityAnimation()
                {
                    From = 0,
                    To = 1,
                    Duration = TimeSpan.FromMilliseconds(duration),
                    Delay = TimeSpan.FromMilliseconds(delay),
                    EasingMode = Windows.UI.Xaml.Media.Animation.EasingMode.EaseInOut,
                    EasingType = EasingType.Linear
                });
                delay += 75;

                animationSet.Start(item);
            }

            AlbumsLoaded = true;
        }
    }
}
