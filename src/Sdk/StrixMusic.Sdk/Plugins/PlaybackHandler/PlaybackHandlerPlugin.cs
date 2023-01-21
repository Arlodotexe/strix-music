// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Plugins.Model;

namespace StrixMusic.Sdk.Plugins.PlaybackHandler;

/// <summary>
/// Adds playback functionality using a provided <see cref="IPlaybackHandlerService"/>.
/// </summary>
public class PlaybackHandlerPlugin : SdkModelPlugin
{
    private static readonly ModelPluginMetadata _metadata = new(
        id: nameof(PlaybackHandlerPlugin),
        displayName: "Playback handler",
        description: "Intercepts playback requests to play them locally on your device",
        new Version(0, 0, 0));

    /// <summary>
    /// Initializes a new instance of the <see cref="PlaybackHandlerPlugin"/> class.
    /// </summary>
    /// <param name="playbackHandler">An instance of <see cref="IPlaybackHandlerService"/> that should be used when playback commands are issued.</param>
    public PlaybackHandlerPlugin(IPlaybackHandlerService playbackHandler)
        : base(_metadata)
    {
        // Inject local playback device
        StrixDataRoot.Add(x => new StrixDataRootPlaybackHandlerPlugin(_metadata, x, playbackHandler));

        // Redirect playback requests to playback handler.
        ArtistCollection.Add(x => new ArtistCollectionPlaybackHandlerPlugin(_metadata, x, playbackHandler));
        AlbumCollection.Add(x => new AlbumCollectionPlaybackHandlerPlugin(_metadata, x, playbackHandler));
        TrackCollection.Add(x => new TrackCollectionPlaybackHandlerPlugin(_metadata, x, playbackHandler));
        PlaylistCollection.Add(x => new PlaylistCollectionPlaybackHandlerPlugin(_metadata, x, playbackHandler));
    }
}
