// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.Models;
using StrixMusic.Sdk.Models.Base;

namespace StrixMusic.Sdk.MediaPlayback
{
    public partial class PlaybackHandlerService
    {
        /// <inheritdoc />
        public async Task PlayAsync(ITrackCollection trackCollection, IPlayableBase context, CancellationToken cancellationToken = default)
        {
            var apiTracks = await trackCollection.GetTracksAsync(1, 0, cancellationToken);
            Guard.HasSizeGreaterThan(apiTracks, 0, nameof(apiTracks));

            await PlayAsync(apiTracks[0], trackCollection, context, cancellationToken);
        }

        /// <inheritdoc />
        public async Task PlayAsync(ITrack track, ITrackCollection trackCollection, IPlayableBase context, CancellationToken cancellationToken = default)
        {
            Guard.IsNotNull(_strixDevice, nameof(_strixDevice));

            var canPlay = await PrepareToPlayCollection(cancellationToken);
            if (!canPlay)
                return;

            ClearPrevious();
            ClearNext();

            await AddTrackCollectionToQueue(track, trackCollection, cancellationToken: cancellationToken);

            if (ShuffleState)
                ShuffleOnInternal();

            var nextItem = _nextItems[0];
            await PlayFromNext(0, cancellationToken);

            Guard.IsNotNull(nextItem.MediaConfig, nameof(nextItem.MediaConfig));

            _strixDevice.SetPlaybackData(context, nextItem);
        }

        /// <inheritdoc />
        public async Task PlayAsync(IArtistCollection artistCollection, IPlayableBase context, CancellationToken cancellationToken = default)
        {
            var apiArtists = await artistCollection.GetArtistItemsAsync(1, 0, cancellationToken);
            Guard.HasSizeGreaterThan(apiArtists, 0, nameof(apiArtists));

            await PlayAsync(apiArtists[0], artistCollection, context, cancellationToken);
        }

        /// <inheritdoc />
        public async Task PlayAsync(IArtistCollectionItem artistCollectionItem, IArtistCollection artistCollection, IPlayableBase context, CancellationToken cancellationToken = default)
        {
            Guard.IsNotNull(_strixDevice, nameof(_strixDevice));

            var canPlay = await PrepareToPlayCollection(cancellationToken);
            if (!canPlay)
            {
                await artistCollection.PlayArtistCollectionAsync(artistCollectionItem, cancellationToken);
                return;
            }

            ClearPrevious();
            ClearNext();

            await AddArtistCollectionToQueue(artistCollectionItem, artistCollection, cancellationToken);

            if (ShuffleState)
                ShuffleOnInternal();

            var nextItem = _nextItems[0];
            await PlayFromNext(0, cancellationToken);

            Guard.IsNotNull(nextItem.MediaConfig, nameof(nextItem.MediaConfig));
            _strixDevice.SetPlaybackData(context, nextItem);
        }

        /// <inheritdoc />
        public async Task PlayAsync(IPlaylistCollection playlistCollection, IPlayableBase context, CancellationToken cancellationToken = default)
        {
            var apiPlaylists = await playlistCollection.GetPlaylistItemsAsync(1, 0, cancellationToken);
            Guard.HasSizeGreaterThan(apiPlaylists, 0, nameof(apiPlaylists));

            await PlayAsync(apiPlaylists[0], playlistCollection, context, cancellationToken);
        }

        /// <inheritdoc />
        public async Task PlayAsync(IAlbumCollection albumCollection, IPlayableBase context, CancellationToken cancellationToken = default)
        {
            var apiItems = await albumCollection.GetAlbumItemsAsync(1, 0, cancellationToken);
            Guard.HasSizeGreaterThan(apiItems, 0, nameof(apiItems));

            await PlayAsync(apiItems[0], albumCollection, context, cancellationToken);
        }

        /// <inheritdoc />
        public async Task PlayAsync(IAlbumCollectionItem albumCollectionItem, IAlbumCollection albumCollection, IPlayableBase context, CancellationToken cancellationToken = default)
        {
            Guard.IsNotNull(_strixDevice, nameof(_strixDevice));

            var canPlay = await PrepareToPlayCollection(cancellationToken);
            if (!canPlay)
            {
                await albumCollection.PlayAlbumCollectionAsync(albumCollectionItem, cancellationToken);
                return;
            }

            ClearPrevious();
            ClearNext();

            await AddAlbumCollectionToQueue(albumCollectionItem, albumCollection, cancellationToken);

            if (ShuffleState)
                ShuffleOnInternal();

            var nextItem = _nextItems[0];
            await PlayFromNext(0, cancellationToken);

            Guard.IsNotNull(nextItem.MediaConfig, nameof(nextItem.MediaConfig));

            _strixDevice.SetPlaybackData(context, nextItem);
        }

        /// <inheritdoc />
        public async Task PlayAsync(IPlaylistCollectionItem playlistCollectionItem, IPlaylistCollection playlistCollection, IPlayableBase context, CancellationToken cancellationToken = default)
        {
            Guard.IsNotNull(_strixDevice, nameof(_strixDevice));

            var canPlay = await PrepareToPlayCollection(cancellationToken);
            if (!canPlay)
            {
                await playlistCollection.PlayPlaylistCollectionAsync(playlistCollectionItem, cancellationToken);
                return;
            }

            ClearPrevious();
            ClearNext();

            await AddPlaylistCollectionToQueue(playlistCollectionItem, playlistCollection, cancellationToken);

            if (ShuffleState)
                ShuffleOnInternal();

            var nextItem = _nextItems[0];
            await PlayFromNext(0, cancellationToken);

            Guard.IsNotNull(nextItem.MediaConfig, nameof(nextItem.MediaConfig));

            _strixDevice.SetPlaybackData(context, nextItem);
        }

        /// <summary>
        /// Common tasks done by all "Play" methods when playback is requested.
        /// </summary>
        /// <returns>True if playback should continue locally, false if playback should continue remotely.</returns>
        private async Task<bool> PrepareToPlayCollection(CancellationToken cancellationToken = default)
        {
            Guard.IsNotNull(_strixDevice, nameof(_strixDevice));

            // Pause the active player first.
            if (ActiveDevice is not null)
                await ActiveDevice.PauseAsync(cancellationToken);

            // If there is no active device, activate the device used for local playback.
            if (ActiveDevice is null)
            {
                // Switching to the device should activate it and set it as our active device via events,
                // via the same mechanism used for switching to remote devices.
                await _strixDevice.SwitchToAsync(cancellationToken);
            }

            Guard.IsNotNull(ActiveDevice, nameof(ActiveDevice));

            // If the active device is controlled remotely, the rest is handled there.
            return ActiveDevice.Type != DeviceType.Remote;
        }

        /// <summary>
        /// Adds the tracks in the collection to the queue.
        /// </summary>
        /// <param name="track">A target track to return an index for.</param>
        /// <param name="trackCollection">The collection to iterate for adding to the queue.</param>
        /// <param name="pushTarget">When adding to the queue, specify where to push the items to.</param>
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns>The index of the given <paramref name="track"/> in the <paramref name="trackCollection"/>.</returns>
        private Task<int> AddTrackCollectionToQueue(ITrack track, ITrackCollection trackCollection, AddTrackPushTarget pushTarget = AddTrackPushTarget.Normal, CancellationToken cancellationToken = default) => AddTrackCollectionToQueue(track, trackCollection, 0, pushTarget, cancellationToken);

        /// <summary>
        /// Adds the tracks in the collection to the queue.
        /// </summary>
        /// <param name="track">A target track to return an index for.</param>
        /// <param name="trackCollection">The collection to iterate for adding to the queue.</param>
        /// <param name="offset">The offset to add when adding tracks to <see cref="NextItems"/></param>.
        /// <param name="pushTarget">When adding to the queue, specify where to push the items to.</param>
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns>The index of the given <paramref name="track"/> in the <paramref name="trackCollection"/>.</returns>
        private async Task<int> AddTrackCollectionToQueue(ITrack track, ITrackCollection trackCollection, int offset, AddTrackPushTarget pushTarget = AddTrackPushTarget.Normal, CancellationToken cancellationToken = default)
        {
            // Setup for local playback
            var trackPlaybackIndex = 0;
            var reachedTargetTrack = false;

            var tracks = await trackCollection.GetTracksAsync(trackCollection.TotalTrackCount, 0, cancellationToken);

            for (var i = 0; i < trackCollection.TotalTrackCount; i++)
            {
                var item = tracks[i];

                var coreTrack = item.GetSources<ICoreTrack>().First(x => x.Id == item.Id);

                var mediaSource = await coreTrack.SourceCore.GetMediaSourceAsync(coreTrack, cancellationToken);
                if (mediaSource is null)
                    continue;

                if (item.Id == track.Id)
                {
                    reachedTargetTrack = true;
                    trackPlaybackIndex = i;
                }

                var playbackItem = new PlaybackItem()
                {
                    MediaConfig = mediaSource,
                    Track = item
                };
                
                switch (pushTarget)
                {
                    case AddTrackPushTarget.Normal when reachedTargetTrack:
                        InsertNext(i - trackPlaybackIndex, playbackItem);
                        break;
                    case AddTrackPushTarget.Normal:
                    case AddTrackPushTarget.AllPrevious:
                        PushPrevious(playbackItem);
                        break;
                    case AddTrackPushTarget.AllNext:
                        InsertNext(i + offset, playbackItem);
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
        private async Task<(ITrack PlaybackTrack, int Index)> AddAlbumCollectionToQueue(IAlbumCollectionItem? albumCollectionItem, IAlbumCollection albumCollection, CancellationToken cancellationToken = default)
        {
            var itemIndex = 0;
            ITrack? playbackTrack = null;
            var foundItemTarget = false;
            var offset = 0;

            var albums = await albumCollection.GetAlbumItemsAsync(albumCollection.TotalAlbumItemsCount, offset, cancellationToken);

            foreach (var albumItem in albums)
            {
                if (albumItem is IAlbum album)
                {
                    // Ignore albums without any tracks
                    if (album.TotalTrackCount < 1)
                        continue;

                    var tracks = await album.GetTracksAsync(1, 0, cancellationToken);
                    var firstTrack = tracks[0];

                    if (albumItem.Id == albumCollectionItem?.Id && !foundItemTarget)
                    {
                        // Tracks are added to the queue of previous items until we reach the item the user wants to play.
                        itemIndex = _prevItems.Count;
                        foundItemTarget = true;

                        playbackTrack = firstTrack;
                    }

                    if (foundItemTarget)
                    {
                        _ = await AddTrackCollectionToQueue(firstTrack, album, offset,
                            foundItemTarget ? AddTrackPushTarget.AllNext : AddTrackPushTarget.AllPrevious, cancellationToken);
                        offset = _nextItems.Count;
                    }
                    else
                    {
                        _ = await AddTrackCollectionToQueue(firstTrack, album,
                            foundItemTarget ? AddTrackPushTarget.AllNext : AddTrackPushTarget.AllPrevious, cancellationToken);
                    }
                }

                if (albumItem is IAlbumCollection albumCol)
                {
                    _ = await AddAlbumCollectionToQueue(null, albumCol, cancellationToken);
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
        private async Task<(ITrack PlaybackTrack, int Index)> AddArtistCollectionToQueue(IArtistCollectionItem? artistCollectionItem, IArtistCollection artistCollection, CancellationToken cancellationToken = default)
        {
            var itemIndex = 0;
            ITrack? playbackTrack = null;
            var foundItemTarget = false;
            var offset = 0;

            var artists = await artistCollection.GetArtistItemsAsync(artistCollection.TotalArtistItemsCount, offset, cancellationToken);

            foreach (var artistItem in artists)
            {
                switch (artistItem)
                {
                    // Ignore artist without any tracks
                    case IArtist { TotalTrackCount: < 1 }:
                        continue;
                    case IArtist artist:
                        var tracks = await artist.GetTracksAsync(1, 0, cancellationToken);
                        var firstTrack = tracks[0];

                        if (artistItem.Id == artistCollectionItem?.Id && !foundItemTarget)
                        {
                            // Tracks are added to the queue of previous items until we reach the item the user wants to play.
                            itemIndex = _prevItems.Count;
                            foundItemTarget = true;

                            playbackTrack = firstTrack;
                        }

                        if (foundItemTarget)
                        {
                            _ = await AddTrackCollectionToQueue(firstTrack, artist, offset,
                                foundItemTarget ? AddTrackPushTarget.AllNext : AddTrackPushTarget.AllPrevious, cancellationToken);
                            offset = _nextItems.Count;
                        }
                        else
                        {
                            _ = await AddTrackCollectionToQueue(firstTrack, artist,
                                foundItemTarget ? AddTrackPushTarget.AllNext : AddTrackPushTarget.AllPrevious, cancellationToken);
                        }

                        break;
                    case IArtistCollection artistCol:
                        _ = await AddArtistCollectionToQueue(null, artistCol, cancellationToken);
                        break;
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
        private async Task<(ITrack PlaybackTrack, int Index)> AddPlaylistCollectionToQueue(IPlaylistCollectionItem? playlistCollectionItem, IPlaylistCollection playlistCollection, CancellationToken cancellationToken = default)
        {
            var itemIndex = 0;
            ITrack? playbackTrack = null;
            var foundItemTarget = false;
            var offset = 0;

            var playlists = await playlistCollection.GetPlaylistItemsAsync(playlistCollection.TotalPlaylistItemsCount, offset, cancellationToken);

            foreach (var playlistItem in playlists)
            {
                if (playlistItem is IPlaylist playlist)
                {
                    // Ignore artist without any tracks
                    if (playlist.TotalTrackCount < 1)
                        continue;

                    var tracks = await playlist.GetTracksAsync(1, 0, cancellationToken);
                    var firstTrack = tracks[0];

                    if (playlistItem.Id == playlistCollectionItem?.Id && !foundItemTarget)
                    {
                        // Tracks are added to the queue of previous items until we reach the item the user wants to play.
                        itemIndex = _prevItems.Count;
                        foundItemTarget = true;

                        playbackTrack = firstTrack;
                    }

                    if (foundItemTarget)
                    {
                        _ = await AddTrackCollectionToQueue(firstTrack, playlist, offset,
                            foundItemTarget ? AddTrackPushTarget.AllNext : AddTrackPushTarget.AllPrevious, cancellationToken);
                        offset = _nextItems.Count;
                    }
                    else
                    {
                        _ = await AddTrackCollectionToQueue(firstTrack, playlist,
                            foundItemTarget ? AddTrackPushTarget.AllNext : AddTrackPushTarget.AllPrevious, cancellationToken);
                    }
                }

                if (playlistItem is IPlaylistCollection playlistCol)
                {
                    _ = await AddPlaylistCollectionToQueue(null, playlistCol, cancellationToken);
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
