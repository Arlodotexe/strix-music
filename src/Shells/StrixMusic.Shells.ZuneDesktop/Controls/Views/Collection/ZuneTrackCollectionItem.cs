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
                    ArtistNamesMetadata.Clear();
                }

                if (newValue is not null)
                {
                    if (newValue.Artists.Count == 0)
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
                if (newValue is null)
                    return;

                await newValue.InitTrackCollectionAsync();

                // If any track in the parent collection has more than 2 artists,
                // ALL track items should display their artist list, including this instance.
                ShouldShowArtistList = newValue.Tracks.Any(x => x.TotalArtistItemsCount > 1) && newValue is IAlbum or IArtist;
            }
        }
    }
}
