// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System.Threading;
using System.Threading.Tasks;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Plugins.Model;

namespace StrixMusic.Sdk.Plugins.PlaybackHandler;

/// <summary>
/// Integrates an instance of <see cref="IPlaybackHandlerService"/> into all instances of <see cref="ITrackCollection"/>.
/// </summary>
internal class TrackCollectionPlaybackHandlerPlugin : TrackCollectionPluginBase
{
    private readonly IPlaybackHandlerService _playbackHandler;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlaybackHandlerPlugin"/> class.
    /// </summary>
    /// <param name="metadata">Contains metadata for a plugin. Used to identify a plugin before instantiation.</param>
    /// <param name="pluginRoot">The plugin-enabled <see cref="IStrixDataRoot" /> which is responsible for creating this and all parent instances.</param>
    /// <param name="inner">An implementation which member access is delegated to, unless the member is overridden in a derived class which changes the behavior.</param>
    /// <param name="playbackHandler">An instance of <see cref="IPlaybackHandlerService"/> that should be used when playback commands are issued.</param>
    public TrackCollectionPlaybackHandlerPlugin(ModelPluginMetadata metadata, IStrixDataRoot pluginRoot, ITrackCollection inner, IPlaybackHandlerService playbackHandler)
        : base(metadata, inner, pluginRoot)
    {
        _playbackHandler = playbackHandler;
    }

    /// <inheritdoc/>
    public override Task PlayTrackCollectionAsync(CancellationToken cancellationToken = default) => _playbackHandler.PlayAsync(this, Inner, cancellationToken);

    /// <inheritdoc/>
    public override Task PlayTrackCollectionAsync(ITrack track, CancellationToken cancellationToken = default) => _playbackHandler.PlayAsync(track, this, Inner, cancellationToken);
}
