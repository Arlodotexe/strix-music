using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Uwp.UI.Controls;
using OwlCore.Extensions;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.ViewModels;

namespace StrixMusic.Shells.ZuneDesktop.Controls.Views.Collection
{
    /// <summary>
    /// An container for items in a <see cref="ZuneTrackCollection"/>, with added functionality and observable properties.
    /// </summary>
    public sealed partial class ZuneTrackCollectionItem : ObservableObject
    {
        // Cache artist item values
        // Keeping a minimum cache that updates with events allows us to
        // avoid checking all items when a single item updates.
        private ConcurrentDictionary<string, int> _lastKnownTrackArtistsCount = new();

        [ObservableProperty]
        private ITrackCollectionViewModel? _parentCollection;

        [ObservableProperty]
        private TrackViewModel? _track;

        [ObservableProperty]
        private bool _shouldShowArtistList;

        [ObservableProperty]
        private ObservableCollection<MetadataItem> _artistNamesMetadata = new ObservableCollection<MetadataItem>();

        partial void OnTrackChanging(TrackViewModel? newValue)
        {
            _ = ExecuteAsync(newValue);

            async Task ExecuteAsync(TrackViewModel? newValue)
            {
                if (Track is not null)
                {
                    DetachEvents(Track);
                    ArtistNamesMetadata.Clear();
                }

                if (newValue is not null)
                {
                    AttachEvents(newValue);

                    await newValue.InitArtistCollectionAsync();

                    foreach (var artist in newValue.Artists)
                        ArtistNamesMetadata.Add(new MetadataItem { Label = artist.Name });
                }
            }
        }

        partial void OnParentCollectionChanging(ITrackCollectionViewModel? newValue)
        {
            _ = ExecuteAsync(newValue);

            async Task ExecuteAsync(ITrackCollectionViewModel? newValue)
            {
                if (ParentCollection is not null)
                {
                    DetachEvents(ParentCollection);

                    foreach (var item in ParentCollection.Tracks)
                        DetachEvents(item);
                }

                if (newValue is null)
                    return;

                await newValue.InitTrackCollectionAsync();

                // If any track in the parent collection has more than 2 artists,
                // ALL track items should display their artist list, including this instance.
                ShouldShowArtistList = newValue.Tracks.Any(x => x.TotalArtistItemsCount > 1) && newValue is IAlbum or IArtist;

                lock (_lastKnownTrackArtistsCount)
                {
                    _lastKnownTrackArtistsCount.Clear();

                    foreach (var item in newValue.Tracks)
                    {
                        _lastKnownTrackArtistsCount[item.Id] = item.TotalArtistItemsCount;
                        AttachEvents(item);
                    }

                    AttachEvents(newValue);
                }
            }
        }

        private void AttachEvents(ITrackCollectionViewModel trackCollection)
        {
            trackCollection.TracksChanged += TrackCollection_TracksChanged;
        }

        private void DetachEvents(ITrackCollectionViewModel trackCollection)
        {
            trackCollection.TracksChanged -= TrackCollection_TracksChanged;
        }

        private void AttachEvents(ITrack track)
        {
            track.ArtistItemsChanged += Track_ArtistItemsChanged;
        }

        private void DetachEvents(ITrack track)
        {
            track.ArtistItemsChanged -= Track_ArtistItemsChanged;
        }

        private void TrackCollection_TracksChanged(object sender, IReadOnlyList<OwlCore.Events.CollectionChangedItem<ITrack>> addedItems, IReadOnlyList<OwlCore.Events.CollectionChangedItem<ITrack>> removedItems)
        {
            // Handle newly added/removed tracks
            lock (_lastKnownTrackArtistsCount)
            {
                foreach (var item in removedItems)
                    Guard.IsTrue(_lastKnownTrackArtistsCount.TryRemove(item.Data.Id, out _));

                foreach (var item in addedItems)
                    Guard.IsTrue(_lastKnownTrackArtistsCount.TryAdd(item.Data.Id, item.Data.TotalArtistItemsCount));

                ShouldShowArtistList = _lastKnownTrackArtistsCount.Any(x => x.Value > 1) && _parentCollection is IAlbum or IArtist;
            }
        }

        private void Track_ArtistItemsChanged(object sender, IReadOnlyList<OwlCore.Events.CollectionChangedItem<IArtistCollectionItem>> addedItems, IReadOnlyList<OwlCore.Events.CollectionChangedItem<IArtistCollectionItem>> removedItems)
        {
            // If any track in the parent collection has more than 2 artists,
            // ALL track items should display their artist list, including this instance.
            var track = (ITrack)sender;

            _lastKnownTrackArtistsCount[track.Id] += addedItems.Count - removedItems.Count;
            Guard.IsGreaterThanOrEqualTo(_lastKnownTrackArtistsCount[track.Id], 0);

            if (track.Id == Track?.Id)
                ArtistNamesMetadata.ChangeCollection(addedItems, removedItems, x => new MetadataItem { Label = x.Data.Name });

            // Fast path
            if (track.TotalArtistItemsCount >= 2)
            {
                ShouldShowArtistList = _parentCollection is IAlbum or IArtist;
                return;
            }

            lock (_lastKnownTrackArtistsCount)
            {
                // Keeping a minimum cache that updates with events allows us to
                // avoid checking all items asynchronously when a single item updates.
                ShouldShowArtistList = _lastKnownTrackArtistsCount.Any(x => x.Value > 1) && _parentCollection is IAlbum or IArtist;
            }
        }
    }
}
