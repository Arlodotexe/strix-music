using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.MediaPlayback.LocalDevice;
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
            var mainViewModel = MainViewModel.Singleton;

            Guard.IsTrue(mainViewModel?.Library?.IsInitialized ?? false, nameof(mainViewModel.Library.IsInitialized));

            var core = await GetPlaybackCore(track);
            var activeDevice = mainViewModel?.ActiveDevice;
            var localDevice = mainViewModel?.LocalDevice?.Model as StrixDevice;

            Guard.IsNotNull(localDevice, nameof(localDevice));

            // Pause the active player first.
            if (!(activeDevice is null))
            {
                _ = activeDevice.PauseAsync();
            }

            // If there is no active device, activate the associated local device.
            if (activeDevice is null)
            {
                await localDevice.SwitchToAsync();
                activeDevice ??= mainViewModel?.ActiveDevice;
            }

            Guard.IsNotNull(activeDevice, nameof(activeDevice));

            // Don't continue if playback isn't supported by the core.
            if (activeDevice.SourceCore?.CoreConfig.PlaybackType == MediaPlayerType.None)
                return;

            // If the active device is controlled remotely, the rest is handled there.
            if (activeDevice.Type == DeviceType.Remote)
            {
                // TODO 
                //await context.PlayAsync();
                return;
            }

            await trackCollection.InitAsync();

            // Setup for local playback
            var trackPlaybackIndex = 0;

            for (var i = 0; i < trackCollection.Tracks.Count; i++)
            {
                var item = trackCollection.Tracks[i];
                var coreTrack = item.GetSources<ICoreTrack>().First(x => x.Id == item.Id);

                var mediaSource = await core.GetMediaSource(coreTrack);
                if (mediaSource is null)
                    continue;

                if (item.Id == track.Id)
                    trackPlaybackIndex = i;

                InsertNext(i, mediaSource);
            }

            localDevice.SetPlaybackData(context, track);
            await PlayFromNext(trackPlaybackIndex);
        }

        /// <inheritdoc />
        public Task PlayAsync(IAlbum track, IAlbumCollectionViewModel albumCollection, IPlayableBase context)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task PlayAsync(IAlbumCollectionViewModel albumCollection, IPlayableBase context)
        {
            throw new NotImplementedException();
        }
    }
}