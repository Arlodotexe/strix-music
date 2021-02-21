using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.ViewModels;

namespace StrixMusic.Sdk.Services.MediaPlayback
{
    public partial class PlaybackHandlerService
    {
        /// <inheritdoc />
        public async Task PlayAsync(ITrackCollectionViewModel trackCollection, IPlayableBase context)
        {
            await trackCollection.InitAsync();

            var firstTrack = trackCollection.Tracks.FirstOrDefault();
            if (firstTrack is null)
            {
                var apiTracks = await trackCollection.GetTracksAsync(1, 0);
                Guard.HasSizeGreaterThan(apiTracks, 0, nameof(apiTracks));
                firstTrack = new TrackViewModel(apiTracks[0]);
            }

            await PlayAsync(firstTrack, trackCollection, context);
        }

        /// <inheritdoc />
        public async Task PlayAsync(ITrack track, ITrackCollectionViewModel trackCollection, IPlayableBase context)
        {
            await trackCollection.InitAsync();

            var playerEntry = await PlayCollectionEntry();
            if (!playerEntry)
                return;

            await AddTrackCollectionToQueue(track, trackCollection);

            await PlayFromNext(0);
        }

        /// <inheritdoc />
        public async Task PlayAsync(IAlbumCollectionViewModel albumCollection, IPlayableBase context)
        {
            await albumCollection.InitAsync();

            var firstAlbum = albumCollection.Albums.FirstOrDefault();
            if (firstAlbum is null)
            {
                var apiItems = await albumCollection.GetAlbumItemsAsync(1, 0);
                Guard.HasSizeGreaterThan(apiItems, 0, nameof(apiItems));

                firstAlbum = apiItems[0];
            }

            await PlayAsync(firstAlbum, albumCollection, context);
        }

        /// <inheritdoc />
        public async Task PlayAsync(IAlbumCollectionItem albumCollectionItem, IAlbumCollectionViewModel albumCollection, IPlayableBase context)
        {
            await albumCollection.InitAsync();

            var playerEntry = await PlayCollectionEntry();
            if (!playerEntry)
            {
                await albumCollection.PlayAlbumCollectionAsync(albumCollectionItem);
                return;
            }

            await AddAlbumCollectionToQueue(albumCollectionItem, albumCollection);

            await PlayFromNext(0);
        }

        /// <summary>
        /// Common tasks done by all "Play" methods.
        /// </summary>
        /// <returns>True if playback should continue locally, false if playback should continue remotely.</returns>
        private async Task<bool> PlayCollectionEntry()
        {
            var mainViewModel = MainViewModel.Singleton;

            Guard.IsNotNull(_strixDevice, nameof(_strixDevice));
            Guard.IsTrue(mainViewModel?.Library?.IsInitialized ?? false, nameof(mainViewModel.Library.IsInitialized));

            var activeDevice = mainViewModel?.ActiveDevice;

            // Pause the active player first.
            if (!(activeDevice is null))
            {
                _ = activeDevice.PauseAsync();
            }

            // If there is no active device, activate the device used for local playback.
            if (activeDevice is null)
            {
                await _strixDevice.SwitchToAsync();
                activeDevice ??= mainViewModel?.ActiveDevice;
            }

            // If the active device is controlled remotely, the rest is handled there.
            if (activeDevice?.Type == DeviceType.Remote)
            {
                return false;
            }

            Guard.IsNotNull(activeDevice, nameof(activeDevice));

            return true;
        }

        /// <summary>
        /// Adds the tracks in the collection to the queue.
        /// </summary>
        /// <param name="track">A target track to return an index for.</param>
        /// <param name="trackCollection">The collection to iterate for adding to the queue.</param>
        /// <param name="pushTarget">When adding to the queue, specify where to push the items to.</param>
        /// <returns>The index of the given <paramref name="track"/> in the <paramref name="trackCollection"/>.</returns>
        private async Task<int> AddTrackCollectionToQueue(ITrack track, ITrackCollectionViewModel trackCollection, AddTrackPushTarget pushTarget = AddTrackPushTarget.Normal)
        {
            // Setup for local playback
            await trackCollection.InitAsync();
            var trackPlaybackIndex = 0;
            var reachedTargetTrack = false;

            for (var i = 0; i < trackCollection.Tracks.Count; i++)
            {
                var item = trackCollection.Tracks[i];

                var coreTrack = item.GetSources<ICoreTrack>().First(x => x.Id == item.Id);

                var mediaSource = await coreTrack.SourceCore.GetMediaSource(coreTrack);
                if (mediaSource is null)
                    continue;

                if (item.Id == track.Id)
                {
                    reachedTargetTrack = true;
                    trackPlaybackIndex = i;
                }

                switch (pushTarget)
                {
                    case AddTrackPushTarget.Normal when reachedTargetTrack:
                        InsertNext(i - trackPlaybackIndex, mediaSource);
                        break;
                    case AddTrackPushTarget.Normal:
                    case AddTrackPushTarget.AllPrevious:
                        PushPrevious(mediaSource);
                        break;
                    case AddTrackPushTarget.AllNext:
                        InsertNext(i, mediaSource);
                        break;
                    default:
                        return ThrowHelper.ThrowArgumentOutOfRangeException<int>(nameof(pushTarget));
                }
            }

            return trackPlaybackIndex;
        }

        /// <summary>
        /// Adds all albums in the given collection to the queue.
        /// </summary>
        /// <returns>The index of the first playable track in the selected <paramref name="albumCollectionItem"/> items within the entire <paramref name="albumCollection"/>.</returns>
        private async Task<int> AddAlbumCollectionToQueue(IAlbumCollectionItem? albumCollectionItem, IAlbumCollectionViewModel albumCollection)
        {
            await albumCollection.InitAsync();

            var itemIndex = 0;
            var foundItemTarget = false;

            foreach (var albumItem in albumCollection.Albums)
            {
                if (albumItem is IAlbum album)
                {
                    var albumVm = new AlbumViewModel(album);

                    if (albumItem.Id == albumCollectionItem?.Id)
                    {
                        // Tracks are added to the queue of previous items until we reach the item the user wants to play.
                        itemIndex = _prevItems.Count + 1;
                        foundItemTarget = true;
                    }

                    await albumVm.InitAsync();
                    _ = await AddTrackCollectionToQueue(albumVm.Tracks.First(), albumVm, foundItemTarget ? AddTrackPushTarget.AllNext : AddTrackPushTarget.AllPrevious);
                }

                if (albumItem is IAlbumCollection albumCol)
                {
                    var albumColVm = new AlbumCollectionViewModel(albumCol);
                    await albumColVm.InitAsync();

                    _ = await AddAlbumCollectionToQueue(null, albumColVm);
                }
            }

            return itemIndex;
        }

        private enum AddTrackPushTarget : byte
        {
            Normal = 0,
            AllPrevious = 1,
            AllNext = 2,
        }
    }
}