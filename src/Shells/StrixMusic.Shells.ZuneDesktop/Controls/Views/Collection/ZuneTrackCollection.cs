using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Uwp.UI.Controls;
using OwlCore.ComponentModel;
using OwlCore.Extensions;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Sdk.WinUI.Controls.Collections.Abstract;
using StrixMusic.Shells.ZuneDesktop.Controls.Views.Items;
using Windows.UI.Xaml;

namespace StrixMusic.Shells.ZuneDesktop.Controls.Views.Collection
{
    /// <summary>
    /// Zune implementation of a track collection.
    /// </summary>
    [INotifyPropertyChanged]
    public partial class ZuneTrackCollection : CollectionControl<ZuneTrackCollectionItem, ZuneTrackItem>
    {
        private readonly SemaphoreSlim _collectionModifyMutex = new(1, 1);
        private readonly ObservableCollection<ZuneTrackCollectionItem> _trackItems = new();

        // Cache artist item values
        // Keeping a minimum cache that updates with events allows us to
        // avoid checking all items when a single item updates.
        private readonly ConcurrentDictionary<string, int> _lastKnownTrackArtistsCount = new();

        /// <summary>
        /// Creates a new instance for <see cref="ZuneTrackCollection"/>.
        /// </summary>
        public ZuneTrackCollection()
        {
            TrackItems = new ReadOnlyObservableCollection<ZuneTrackCollectionItem>(_trackItems);

            // Some events are always attached (even if the control is never loaded in XAML)
            // Using the unloaded event gives us a chance to detach events manually in case the GC doesn't
            Unloaded += ZuneTrackCollection_Unloaded;
        }

        private void AttachEvents(ZuneTrackCollectionItem item)
        {
            item.PropertyChanged += Item_PropertyChanged;
            if (item.Track is not null)
            {
                item.Track.ArtistItemsChanged += Track_ArtistItemsChanged;
            }
        }

        private void DetachEvents(ZuneTrackCollectionItem item)
        {
            item.PropertyChanged -= Item_PropertyChanged;

            if (item.Track is not null)
            {
                item.Track.ArtistItemsChanged -= Track_ArtistItemsChanged;
            }
        }

        /// <summary>
        /// The backing dependency property for <see cref="Collection" />.
        /// </summary>
        public static readonly DependencyProperty CollectionProperty =
            DependencyProperty.Register(nameof(Collection), typeof(ITrackCollectionViewModel), typeof(ZuneTrackCollection), new PropertyMetadata(null, (d, e) => _ = ((ZuneTrackCollection)d).OnCollectionChangedAsync((ITrackCollectionViewModel?)e.OldValue, (ITrackCollectionViewModel?)e.NewValue)));

        /// <summary>
        /// The collection to display.
        /// </summary>
        public ITrackCollectionViewModel? Collection
        {
            get => (ITrackCollectionViewModel?)GetValue(CollectionProperty);
            set => SetValue(CollectionProperty, value);
        }

        /// <summary>
        /// The track items being displayed in the UI. Contains additional functionality specific to this view.
        /// </summary>
        public ReadOnlyObservableCollection<ZuneTrackCollectionItem> TrackItems { get; }

        /// <summary>
        /// If true, there is only 1 distinct track artist for all tracks in the provided <see cref="Collection"/>.
        /// </summary>
        public bool AllTrackArtistsAreTheSame => TrackItems.Any(x => x.ShouldShowArtistList);

        /// <inheritdoc/>
        protected override async Task LoadMore()
        {
            if (Collection is null)
                return;

            if (!Collection.PopulateMoreTracksCommand.IsRunning)
                await Collection.PopulateMoreTracksCommand.ExecuteAsync(25);
        }

        /// <inheritdoc/>
        protected override void CheckAndToggleEmpty()
        {
            if (Collection is null)
                return;

            if (!Collection.PopulateMoreTracksCommand.IsRunning && Collection.TotalTrackCount == 0)
                EmptyContentVisibility = Visibility.Visible;
        }

        private async Task OnCollectionChangedAsync(ITrackCollectionViewModel? oldValue, ITrackCollectionViewModel? newValue)
        {
            await _collectionModifyMutex.WaitAsync();

            foreach (var item in TrackItems)
                DetachEvents(item);

            _trackItems.Clear();
            OnPropertyChanged(nameof(AllTrackArtistsAreTheSame));

            if (newValue is not null)
            {
                if (newValue.InitTrackCollectionAsyncCommand.IsRunning && newValue.InitTrackCollectionAsyncCommand.ExecutionTask is not null)
                    await newValue.InitTrackCollectionAsyncCommand.ExecutionTask;
                else if (newValue.InitTrackCollectionAsyncCommand.CanExecute(null))
                    await newValue.InitTrackCollectionAsyncCommand.ExecuteAsync(null);

                foreach (var item in newValue.Tracks)
                {
                    var newItems = new ZuneTrackCollectionItem
                    {
                        ParentCollection = newValue,
                        Track = item,
                    };

                    _trackItems.Add(newItems);
                    AttachEvents(newItems);
                }

                newValue.Tracks.CollectionChanged += Tracks_CollectionChanged;
                OnPropertyChanged(nameof(AllTrackArtistsAreTheSame));
            }

            if (oldValue is not null)
            {
                oldValue.Tracks.CollectionChanged -= Tracks_CollectionChanged;
            }

            _collectionModifyMutex.Release();
        }

        private async void Tracks_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            await _collectionModifyMutex.WaitAsync();

            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                var tracks = e.NewItems.Cast<TrackViewModel>();
                foreach (var item in tracks)
                {
                    await item.InitArtistCollectionAsync();
                }

                var data = e.NewItems.Cast<TrackViewModel>().Select(x =>
                {
                    var newItem = new ZuneTrackCollectionItem
                    {
                        Track = x,
                        ParentCollection = Collection,
                        ShouldShowArtistList = x.Artists.Count > 1 && (Collection is IArtist or IAlbum)
                    };

                    AttachEvents(newItem);
                    return newItem;
                });
                _trackItems.InsertOrAddRange(e.NewStartingIndex, data);
            }

            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                foreach (var item in e.OldItems)
                {
                    var target = TrackItems.FirstOrDefault(x => x.Track == item);
                    if (target is not null)
                    {
                        _trackItems.Remove(target);
                        DetachEvents(target);
                    }
                }
            }

            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Move)
            {
                for (var i = 0; i < e.OldItems.Count; i++)
                    _trackItems.Move(i + e.OldStartingIndex, i + e.NewStartingIndex);
            }

            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
                foreach (var item in TrackItems)
                    DetachEvents(item);

                _trackItems.Clear();
            }

            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
                throw new NotImplementedException();

            _collectionModifyMutex.Release();
        }

        private async void Track_ArtistItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IArtistCollectionItem>> addedItems, IReadOnlyList<CollectionChangedItem<IArtistCollectionItem>> removedItems)
        {
            await _collectionModifyMutex.WaitAsync();
            // If any track in the parent collection has more than 2 artists,
            // ALL track items should display their artist list, including this instance.
            var track = (ITrack)sender;

            lock (_lastKnownTrackArtistsCount)
            {
                _lastKnownTrackArtistsCount[track.Id] += addedItems.Count - removedItems.Count;
                Guard.IsGreaterThanOrEqualTo(_lastKnownTrackArtistsCount[track.Id], 0);
            }

            foreach (var item in _trackItems)
            {
                if (track.Id == item.Track?.Id)
                {
                    item.ArtistNamesMetadata.ChangeCollection(addedItems, removedItems, x => new MetadataItem { Label = x.Data.Name });
                }
            }

            bool showArtists;

            // Fast path
            if (track.TotalArtistItemsCount > 2)
            {
                showArtists = Collection is IAlbum or IArtist;

                goto MAKE_ARTIST_DECISION;
            }

            lock (_lastKnownTrackArtistsCount)
            {
                // Keeping a minimum cache that updates with events allows us to
                // avoid checking all items asynchronously when a single item updates.
                showArtists = _lastKnownTrackArtistsCount.Any(x => x.Value > 1) && Collection is IAlbum or IArtist;
            }

        MAKE_ARTIST_DECISION:
            if (showArtists)
                _trackItems.All(x => x.ShouldShowArtistList = true);

            _collectionModifyMutex.Release();
        }

        private void ZuneTrackCollection_Unloaded(object sender, RoutedEventArgs e)
        {
            Unloaded -= ZuneTrackCollection_Unloaded;

            foreach (var item in TrackItems)
                DetachEvents(item);
        }

        private void Item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ZuneTrackCollectionItem.ShouldShowArtistList))
                OnPropertyChanged(nameof(AllTrackArtistsAreTheSame));
        }
    }
}
