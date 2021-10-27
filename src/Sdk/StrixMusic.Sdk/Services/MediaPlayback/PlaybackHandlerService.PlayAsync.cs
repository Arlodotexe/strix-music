using System.Linq;
using System.Linq.Expressions;
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
            Guard.IsNotNull(_strixDevice, nameof(_strixDevice));
            await trackCollection.InitAsync();

            var canPlay = await PrepareToPlayCollection();
            if (!canPlay)
                return;

            ClearPrevious();
            ClearNext();

            await AddTrackCollectionToQueue(track, trackCollection);

            if (ShuffleState)
                ShuffleOnInternal();

            await PlayFromNext(0);
            _strixDevice.SetPlaybackData(context, track);
        }

        /// <inheritdoc />
        public async Task PlayAsync(IArtistCollectionViewModel artistCollection, IPlayableBase context)
        {
            await artistCollection.InitAsync();

            var firstArtist = artistCollection.Artists.FirstOrDefault();
            if (firstArtist is null)
            {
                var apiArtists = await artistCollection.GetArtistItemsAsync(1, 0);
                Guard.HasSizeGreaterThan(apiArtists, 0, nameof(apiArtists));

                firstArtist = apiArtists[0];
            }

            await PlayAsync(firstArtist, artistCollection, context);
        }

        /// <inheritdoc />
        public async Task PlayAsync(IArtistCollectionItem artistCollectionItem, IArtistCollectionViewModel artistCollection, IPlayableBase context)
        {
            Guard.IsNotNull(_strixDevice, nameof(_strixDevice));

            await artistCollection.InitAsync();

            var canPlay = await PrepareToPlayCollection();
            if (!canPlay)
            {
                await artistCollection.PlayArtistCollectionAsync(artistCollectionItem);
                return;
            }

            ClearPrevious();
            ClearNext();

            var trackInfo = await AddArtistCollectionToQueue(artistCollectionItem, artistCollection);

            if (ShuffleState)
                ShuffleOnInternal();

            await PlayFromNext(0);
            _strixDevice.SetPlaybackData(context, trackInfo.PlaybackTrack);
        }

        /// <inheritdoc />
        public async Task PlayAsync(IPlaylistCollectionViewModel playlistCollection, IPlayableBase context)
        {
            await playlistCollection.InitAsync();

            var firstPlaylist = playlistCollection.Playlists.FirstOrDefault();
            if (firstPlaylist is null)
            {
                var apiPlaylists = await playlistCollection.GetPlaylistItemsAsync(1, 0);
                Guard.HasSizeGreaterThan(apiPlaylists, 0, nameof(apiPlaylists));

                firstPlaylist = apiPlaylists[0];
            }

            await PlayAsync(firstPlaylist, playlistCollection, context);
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
            Guard.IsNotNull(_strixDevice, nameof(_strixDevice));

            await albumCollection.InitAsync();

            var canPlay = await PrepareToPlayCollection();
            if (!canPlay)
            {
                await albumCollection.PlayAlbumCollectionAsync(albumCollectionItem);
                return;
            }

            ClearPrevious();
            ClearNext();

            var trackInfo = await AddAlbumCollectionToQueue(albumCollectionItem, albumCollection);

            if (ShuffleState)
                ShuffleOnInternal();

            await PlayFromNext(0);
            _strixDevice.SetPlaybackData(context, trackInfo.PlaybackTrack);
        }

        /// <inheritdoc />
        public async Task PlayAsync(IPlaylistCollectionItem playlistCollectionItem, IPlaylistCollectionViewModel playlistCollection, IPlayableBase context)
        {
            Guard.IsNotNull(_strixDevice, nameof(_strixDevice));

            await playlistCollection.InitAsync();

            var canPlay = await PrepareToPlayCollection();
            if (!canPlay)
            {
                await playlistCollection.PlayPlaylistCollectionAsync(playlistCollectionItem);
                return;
            }

            ClearPrevious();
            ClearNext();

            var trackInfo = await AddPlaylistCollectionToQueue(playlistCollectionItem, playlistCollection);

            if (ShuffleState)
                ShuffleOnInternal();

            await PlayFromNext(0);
            _strixDevice.SetPlaybackData(context, trackInfo.PlaybackTrack);
        }

        /// <summary>
        /// Common tasks done by all "Play" methods when playback is requested.
        /// </summary>
        /// <returns>True if playback should continue locally, false if playback should continue remotely.</returns>
        private async Task<bool> PrepareToPlayCollection()
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

            Guard.IsTrue(reachedTargetTrack, nameof(reachedTargetTrack));

            return trackPlaybackIndex;
        }

        /// <summary>
        /// Adds the tracks in the collection to the queue.
        /// </summary>
        /// <param name="track">A target track to return an index for.</param>
        /// <param name="trackCollection">The collection to iterate for adding to the queue.</param>
        /// <param name="offset">The offset to add when adding tracks to <see cref="NextItems"/></param>.
        /// <param name="pushTarget">When adding to the queue, specify where to push the items to.</param>
        /// <returns>The index of the given <paramref name="track"/> in the <paramref name="trackCollection"/>.</returns>
        private async Task<int> AddTrackCollectionToQueue(ITrack track, ITrackCollectionViewModel trackCollection, int offset, AddTrackPushTarget pushTarget = AddTrackPushTarget.Normal)
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
                        InsertNext(i + offset, mediaSource);
                        break;
                    default:
                        return ThrowHelper.ThrowArgumentOutOfRangeException<int>(nameof(pushTarget));
                }
            }

            Guard.IsTrue(reachedTargetTrack, nameof(reachedTargetTrack));

            return trackPlaybackIndex;
        }

        /// <summary>
        /// Adds all albums in the given collection to the queue.
        /// </summary>
        /// <returns>The instance and index of the first playable track in the selected <paramref name="albumCollectionItem"/> items within the entire <paramref name="albumCollection"/>.</returns>
        private async Task<(ITrack PlaybackTrack, int Index)> AddAlbumCollectionToQueue(IAlbumCollectionItem? albumCollectionItem, IAlbumCollectionViewModel albumCollection)
        {
            await albumCollection.InitAsync();

            var itemIndex = 0;
            ITrack? playbackTrack = null;
            var foundItemTarget = false;
            var offset = 0;

            foreach (var albumItem in albumCollection.Albums)
            {
                if (albumItem is IAlbum album)
                {
                    var albumVm = (AlbumViewModel)album;

                    await albumVm.InitAsync();

                    // We expect an album to have at least 1 track.
                    Guard.IsGreaterThan(album.TotalTrackCount, 0, nameof(album.TotalTrackCount));

                    var firstTrack = albumVm.Tracks[0].Model;

                    if (albumItem.Id == albumCollectionItem?.Id && !foundItemTarget)
                    {
                        // Tracks are added to the queue of previous items until we reach the item the user wants to play.
                        itemIndex = _prevItems.Count;
                        foundItemTarget = true;

                        playbackTrack = firstTrack;
                    }

                    if (foundItemTarget)
                    {
                        _ = await AddTrackCollectionToQueue(firstTrack, albumVm, offset,
                            foundItemTarget ? AddTrackPushTarget.AllNext : AddTrackPushTarget.AllPrevious);
                        offset = _nextItems.Count;
                    }
                    else
                    {
                        _ = await AddTrackCollectionToQueue(firstTrack, albumVm,
                            foundItemTarget ? AddTrackPushTarget.AllNext : AddTrackPushTarget.AllPrevious);
                    }
                }

                if (albumItem is IAlbumCollection albumCol)
                {
                    var albumColVm = (AlbumCollectionViewModel)albumCol;
                    await albumColVm.InitAsync();

                    _ = await AddAlbumCollectionToQueue(null, albumColVm);
                }
            }

            Guard.IsTrue(foundItemTarget, nameof(foundItemTarget));
            Guard.IsNotNull(playbackTrack, nameof(playbackTrack));

            return (playbackTrack, itemIndex);
        }

        /// <summary>
        /// Adds all artists in the given collection to the queue.
        /// </summary>
        /// <returns>The instance and index of the first playable track in the selected <paramref name="artistCollectionItem"/> items within the entire <paramref name="artistCollection"/>.</returns>
        private async Task<(ITrack PlaybackTrack, int Index)> AddArtistCollectionToQueue(IArtistCollectionItem? artistCollectionItem, IArtistCollectionViewModel artistCollection)
        {
            await artistCollection.InitAsync();

            var itemIndex = 0;
            ITrack? playbackTrack = null;
            var foundItemTarget = false;
            var offset = 0;

            foreach (var artistItem in artistCollection.Artists)
            {
                if (artistItem is IArtist artist)
                {
                    var artistVm = (ArtistViewModel)artist;

                    await artistVm.InitAsync();

                    // We expect an artist to have at least 1 track.
                    Guard.IsGreaterThan(artist.TotalTrackCount, 0, nameof(artist.TotalTrackCount));

                    var firstTrack = artistVm.Tracks[0].Model;

                    if (artistItem.Id == artistCollectionItem?.Id && !foundItemTarget)
                    {
                        // Tracks are added to the queue of previous items until we reach the item the user wants to play.
                        itemIndex = _prevItems.Count;
                        foundItemTarget = true;

                        playbackTrack = firstTrack;
                    }

                    if (foundItemTarget)
                    {
                        _ = await AddTrackCollectionToQueue(firstTrack, artistVm, offset,
                            foundItemTarget ? AddTrackPushTarget.AllNext : AddTrackPushTarget.AllPrevious);
                        offset = _nextItems.Count;
                    }
                    else
                    {
                        _ = await AddTrackCollectionToQueue(firstTrack, artistVm,
                            foundItemTarget ? AddTrackPushTarget.AllNext : AddTrackPushTarget.AllPrevious);
                    }
                }

                if (artistItem is IArtistCollection artistCol)
                {
                    var artistColVm = (ArtistCollectionViewModel)artistCol;
                    await artistColVm.InitAsync();

                    _ = await AddArtistCollectionToQueue(null, artistColVm);
                }
            }

            Guard.IsTrue(foundItemTarget, nameof(foundItemTarget));
            Guard.IsNotNull(playbackTrack, nameof(playbackTrack));

            return (playbackTrack, itemIndex);
        }

        /// <summary>
        /// Adds all playlists in the given collection to the queue.
        /// </summary>
        /// <returns>The instance and index of the first playable track in the selected <paramref name="playlistCollectionItem"/> items within the entire <paramref name="playlistCollection"/>.</returns>
        private async Task<(ITrack PlaybackTrack, int Index)> AddPlaylistCollectionToQueue(IPlaylistCollectionItem? playlistCollectionItem, IPlaylistCollectionViewModel playlistCollection)
        {
            await playlistCollection.InitAsync();

            var itemIndex = 0;
            ITrack? playbackTrack = null;
            var foundItemTarget = false;
            var offset = 0;

            foreach (var playlistItem in playlistCollection.Playlists)
            {
                if (playlistItem is IPlaylist playlist)
                {
                    var playlistVm = (PlaylistViewModel)playlist;

                    await playlistVm.InitAsync();

                    // We expect an playlist to have at least 1 track.
                    Guard.IsGreaterThan(playlist.TotalTrackCount, 0, nameof(playlist.TotalTrackCount));

                    var firstTrack = playlistVm.Tracks[0].Model;

                    if (playlistItem.Id == playlistCollectionItem?.Id && !foundItemTarget)
                    {
                        // Tracks are added to the queue of previous items until we reach the item the user wants to play.
                        itemIndex = _prevItems.Count;
                        foundItemTarget = true;

                        playbackTrack = firstTrack;
                    }

                    if (foundItemTarget)
                    {
                        _ = await AddTrackCollectionToQueue(firstTrack, playlistVm, offset,
                            foundItemTarget ? AddTrackPushTarget.AllNext : AddTrackPushTarget.AllPrevious);
                        offset = _nextItems.Count;
                    }
                    else
                    {
                        _ = await AddTrackCollectionToQueue(firstTrack, playlistVm,
                            foundItemTarget ? AddTrackPushTarget.AllNext : AddTrackPushTarget.AllPrevious);
                    }
                }

                if (playlistItem is IPlaylistCollection playlistCol)
                {
                    var playlistColVm = (PlaylistCollectionViewModel)playlistCol;
                    await playlistColVm.InitAsync();

                    _ = await AddPlaylistCollectionToQueue(null, playlistColVm);
                }
            }

            Guard.IsTrue(foundItemTarget, nameof(foundItemTarget));
            Guard.IsNotNull(playbackTrack, nameof(playbackTrack));

            return (playbackTrack, itemIndex);
        }

        private enum AddTrackPushTarget : byte
        {
            Normal = 0,
            AllPrevious = 1,
            AllNext = 2,
        }
    }
}