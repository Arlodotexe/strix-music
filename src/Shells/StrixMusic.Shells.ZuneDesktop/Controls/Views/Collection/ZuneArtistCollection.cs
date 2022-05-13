using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using OwlCore;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Sdk.WinUI.Controls.Collections;
using StrixMusic.Shells.ZuneDesktop.Converters;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.ZuneDesktop.Controls.Views.Collection
{
    /// <summary>
    /// Zune implementation of the <see cref="ArtistCollection"/>.
    /// </summary>
    public class ZuneArtistCollection : ArtistCollection
    {
        private object _lockObj = new object();

        /// <summary>
        /// Creates a new instance of <see cref="ArtistCollection" />.
        /// </summary>
        public ZuneArtistCollection()
        {
            Unloaded += ZuneArtistCollection_Unloaded;
        }

        /// <summary>
        /// Holds the instance of the sort textblock.
        /// </summary>
        public TextBlock? PART_SortLbl { get; private set; }

        /// <summary>
        /// Holds the current sort state of the zune <see cref="ArtistCollection"/>.
        /// </summary>
        public ZuneSortState SortState
        {
            get { return (ZuneSortState)GetValue(SortStateProperty); }
            set { SetValue(SortStateProperty, value); }
        }

        /// <summary>
        /// Dependency property for <ses cref="SortState" />.
        /// </summary>
        public static readonly DependencyProperty SortStateProperty =
            DependencyProperty.Register(nameof(SortState), typeof(ZuneSortState), typeof(ZuneArtistCollection), new PropertyMetadata(ZuneSortState.AZ, null));

        /// <summary>
        /// Backing dependency property for <see cref="ArtistCollection" />.
        /// </summary>
        public new IArtistCollectionViewModel? Collection
        {
            get { return (IArtistCollectionViewModel)GetValue(CollectionProperty); }
            set { SetValue(CollectionProperty, value); }
        }

        /// <summary>
        /// Dependency property for <ses cref="IArtistCollectionViewModel" />.
        /// </summary>
        public static readonly new DependencyProperty CollectionProperty =
            DependencyProperty.Register(nameof(Collection), typeof(IArtistCollectionViewModel), typeof(ZuneArtistCollection), new PropertyMetadata(null, (s, e) =>
            {
                if (s is ZuneArtistCollection zc)
                {
                    zc.OnArtistCollectionChanged(s, e);
                }
            }));

        /// <inheritdoc/>
        protected override async Task LoadMore()
        {
            if (Collection != null && !Collection.PopulateMoreArtistsCommand.IsRunning)
                await Collection.PopulateMoreArtistsCommand.ExecuteAsync(25);
        }

        /// <inheritdoc />
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            PART_SortLbl = GetTemplateChild(nameof(PART_SortLbl)) as TextBlock;
            Guard.IsNotNull(PART_SortLbl, nameof(PART_SortLbl));

            PART_SortLbl.Tapped += PART_SortLbl_Tapped;
        }

        private void ZuneArtistCollection_Unloaded(object sender, RoutedEventArgs e)
        {
            Guard.IsNotNull(PART_SortLbl, nameof(PART_SortLbl));
            PART_SortLbl.Tapped -= PART_SortLbl_Tapped;

            Unloaded -= ZuneArtistCollection_Unloaded;

            if (Collection is not null)
                Collection.Artists.CollectionChanged -= Artists_CollectionChanged;
        }

        private void PART_SortLbl_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            Guard.IsNotNull(Collection);
            Guard.IsNotNull(PART_SortLbl, nameof(PART_SortLbl));

            switch (SortState)
            {
                case ZuneSortState.AZ:
                    Collection.SortArtistCollection(Sdk.ViewModels.ArtistSortingType.Alphanumerical, Sdk.ViewModels.SortDirection.Descending);
                    SortState = ZuneSortState.ZA;
                    PART_SortLbl.Text = "Z-A";
                    break;
                case ZuneSortState.ZA:
                    Collection.SortArtistCollection(Sdk.ViewModels.ArtistSortingType.Alphanumerical, Sdk.ViewModels.SortDirection.Ascending);
                    SortState = ZuneSortState.AZ;
                    PART_SortLbl.Text = "A-Z";
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        private void OnArtistCollectionChanged(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is IArtistCollectionViewModel oldCol)
            {
                oldCol.Artists.CollectionChanged -= Artists_CollectionChanged;
            }

            if (e.NewValue is IArtistCollectionViewModel newCol)
            {
                newCol.Artists.CollectionChanged += Artists_CollectionChanged;
            }
        }

        private async void Artists_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (Collection is null)
                return;

            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                // Define default sort state of the albums on load.
                try
                {
                    // Gives breathing space to the processor and reduces he odds the where observable collection changed during CollectionChanged event to minimum. 
                    await Task.Delay(100);

                    lock (_lockObj)
                        Collection.SortArtistCollection(Sdk.ViewModels.ArtistSortingType.Alphanumerical, Sdk.ViewModels.SortDirection.Ascending);
                }
                catch (InvalidOperationException)
                {
                    // It handles a rare case where observable collection changed during CollectionChanged event. 
                    // More precisely "Cannot change ObservableCollection during a CollectionChanged event."
                }
            }
        }
    }
}
