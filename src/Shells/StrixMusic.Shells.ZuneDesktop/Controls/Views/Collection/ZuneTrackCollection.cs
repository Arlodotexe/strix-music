using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Diagnostics;
using OwlCore.Extensions;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Sdk.WinUI.Controls.Collections;
using StrixMusic.Sdk.WinUI.Controls.Collections.Abstract;
using StrixMusic.Shells.ZuneDesktop.Controls.Views.Items;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using StrixMusic.Sdk.ViewModels.Helpers;

namespace StrixMusic.Shells.ZuneDesktop.Controls.Views.Collection
{
    /// <summary>
    /// Zune implmenation for the <see cref="ZuneTrackCollection"/>.
    /// </summary>
    [INotifyPropertyChanged]
    public partial class ZuneTrackCollection : CollectionControl<ZuneTrackCollectionItem, ZuneTrackItem>
    {
        private ObservableCollection<ZuneTrackCollectionItem> _trackItems = new();

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
        }

        private void DetachEvents(ZuneTrackCollectionItem item)
        {
            item.PropertyChanged -= Item_PropertyChanged;
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
                SetEmptyVisibility(Visibility.Visible);
        }

        private async Task OnCollectionChangedAsync(ITrackCollectionViewModel? oldValue, ITrackCollectionViewModel? newValue)
        {
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
        }

        private async void Tracks_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
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
