using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using Microsoft.Toolkit.Uwp.UI.Animations;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Sdk.WinUI.Controls.Collections;
using StrixMusic.Shells.ZuneDesktop.Controls.Views.Collections;
using StrixMusic.Shells.ZuneDesktop.Controls.Views.Items;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.ZuneDesktop.Controls.Views.Collection
{
    /// <summary>
    /// The collection to perform zune specific behaviors.
    /// </summary>
    public class ZuneAlbumCollection : AlbumCollection
    {
        /// <summary>
        /// Creates a new instance of <see cref="ZuneAlbumCollection"/>.
        /// </summary>
        public ZuneAlbumCollection()
        {
        }

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
        /// Flag to determine if albums are already loaded.
        /// </summary>
        public bool AlbumsLoaded { get; private set; }

        /// <summary>
        /// The AlbumCollection GridView control.
        /// </summary>
        public GridView? PART_Selector { get; private set; }

        /// <inheritdoc />
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            PART_Selector = GetTemplateChild(nameof(PART_Selector)) as GridView;

            Guard.IsNotNull(PART_Selector, nameof(PART_Selector));

            PART_Selector.Loaded += PART_Selector_Loaded;
            Unloaded += ZuneAlbumCollection_Unloaded;
        }

        private async void Albums_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                // Intentional delay (for safe side), this doesn't freeze anything as event attachment can be done silently, it waits for the emitted album to load in Visual Tree.
                // There is no event on GridView that tells when a UI Element is added to Items list.
                // ------------
                // TODO: We shouldn't be using a delay to do this.
                // ------------
                // TODO: this method of getting a loaded item does not work with virtualization.
                // Instead
                // - Create a custom ViewModel that wraps around AlbumViewModel
                // - Give it a "RequestPlaybackCommand" we can invoke from XAML, and a "PlaybackRequested" event that's raised by the command.
                // - Put it in a new ObservableCollection<TheNewViewModel> property. Bind to that in the UI instead of what we use now.
                // - Use behaviors in XAML to invoke this new command when the "ZuneAlbumItem.AlbumPlaybackTriggered" event fires.
                // - Sync the new ObservableCollection with the source, wrapping the data with our new ViewModel.
                // - Listen to the event on each item in this new ViewModel and invoke playback when it's called
                // - Clean up all the code from the existing approach.
                await Task.Delay(1000);

                Guard.IsNotNull(PART_Selector);
                Guard.IsNotNull(Collection);

                foreach (var item in e.NewItems)
                {
                    Guard.IsNotNull(item, nameof(item));

                    var index = Collection.Albums.IndexOf((IAlbumCollectionItem)item);
                    var gridViewItem = (GridViewItem)PART_Selector.ContainerFromIndex(index);

                    // PATCH! HACK! See above TODO.
                    if (gridViewItem is null)
                    {
                        continue;
                    }

                    var uiElement = gridViewItem.ContentTemplateRoot;
                    if (uiElement is ZuneAlbumItem zuneAlbumItem)
                    {
                        zuneAlbumItem.AlbumPlaybackTriggered += ZuneAlbumItem_AlbumPlaybackTriggered;
                        zuneAlbumItem.Unloaded += ZuneAlbumItem_Unloaded;
                        zuneAlbumItem.ZuneCollectionType = ZuneCollectionType;
                    }
                }
            }
        }

        /// <inheritdoc/>
        protected override void OnCollectionChanged(IAlbumCollectionViewModel? oldValue, IAlbumCollectionViewModel? newValue)
        {
            if (oldValue is IAlbumCollectionViewModel oldCollection)
            {
                oldCollection.Albums.CollectionChanged -= Albums_CollectionChanged;
            }

            if (newValue is IAlbumCollectionViewModel newCollection)
            {
                newCollection.Albums.CollectionChanged += Albums_CollectionChanged;
            }

            base.OnCollectionChanged(oldValue, newValue);
        }

        private void ZuneAlbumItem_Unloaded(object sender, RoutedEventArgs e)
        {
            if (sender is ZuneAlbumItem zuneAlbum)
                zuneAlbum.AlbumPlaybackTriggered -= ZuneAlbumItem_AlbumPlaybackTriggered;
        }

        private void ZuneAlbumCollection_Unloaded(object sender, RoutedEventArgs e)
        {
            Guard.IsNotNull(PART_Selector, nameof(PART_Selector));

            PART_Selector.Loaded -= PART_Selector_Loaded;
            Unloaded -= ZuneAlbumCollection_Unloaded;
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

                if (uiElement is ZuneAlbumItem zuneAlbumItem)
                {
                    zuneAlbumItem.AlbumPlaybackTriggered += ZuneAlbumItem_AlbumPlaybackTriggered;
                    zuneAlbumItem.ZuneCollectionType = ZuneCollectionType;
                }

                // This needs to be explicitly casted to UIElement to avoid a compiler error specific to android in uno.
                uiElments.Add((UIElement)uiElement);
                itemIndex++;
            }

            FadeInAlbumCollectionItems(uiElments);
        }

        private async void ZuneAlbumItem_AlbumPlaybackTriggered(object sender, AlbumViewModel e)
        {
            Guard.IsNotNull(Collection);
            await Collection.PlayAlbumCollectionAsync(e);
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
