using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Ipfs;
using Ipfs.CoreApi;
using OwlCore.Kubo;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Plugins.Model;
using StrixMusic.Sdk.Plugins.PlaybackHandler;

namespace StrixMusic.Plugins
{
    /// <summary>
    /// A plugin that uses IPFS to check how many users are listening to a playable item.
    /// </summary>
    public class GlobalPlaybackStateCountPlugin : SdkModelPlugin
    {
        private static readonly ModelPluginMetadata _metadata = new(
            id: typeof(GlobalPlaybackStateCountPlugin).FullName,
            displayName: "Playback handler",
            description: "Uses IPFS to check how many users are listening to a playable item.",
            new Version(0, 0, 0));

        /// <summary>
        /// Initializes a new instance of the <see cref="PlaybackHandlerPlugin"/> class.
        /// </summary>
        public GlobalPlaybackStateCountPlugin(Ipfs.Peer thisPeer, IPubSubApi pubSub)
            : base(_metadata)
        {
            Playable.Add(x => new GlobalPlaybackStateCountPlayablePlugin(thisPeer, pubSub, _metadata, x));
        }
    }

    /// <summary>
    /// When the playback state is changed to playing, paused, failed or loading, the user joins a room with other users in the same playback state for this item.
    /// </summary>
    [ObservableObject]
    public partial class GlobalPlaybackStateCountPlayablePlugin : PlayablePluginBase
    {
        private readonly Peer _thisPeer;
        private readonly IPubSubApi _pubSub;
        
        [ObservableProperty] private PeerRoom? _playingRoom;
        [ObservableProperty] private PeerRoom? _pausedRoom;
        [ObservableProperty] private PeerRoom? _failedRoom;
        [ObservableProperty] private PeerRoom? _loadingRoom;

        /// <summary>
        /// Creates a new instance of <see cref="GlobalPlaybackStateCountPlayablePlugin"/>.
        /// </summary>
        internal protected GlobalPlaybackStateCountPlayablePlugin(Ipfs.Peer thisPeer, IPubSubApi pubSub, ModelPluginMetadata registration, IPlayable inner)
            : base(registration, inner)
        {
            _thisPeer = thisPeer;
            _pubSub = pubSub;

            Inner.PlaybackStateChanged += InnerOnPlaybackStateChanged;
        }

        private async void InnerOnPlaybackStateChanged(object sender, PlaybackState e)
        {
            PlayingRoom?.Dispose();
            PlayingRoom = null;

            PausedRoom?.Dispose();
            PausedRoom = null;

            FailedRoom?.Dispose();
            FailedRoom = null;

            LoadingRoom?.Dispose();
            LoadingRoom = null;

            if (e is PlaybackState.Playing)
                PlayingRoom = new PeerRoom(_thisPeer, _pubSub, $"{Metadata.Id}.{this.Id}.{nameof(PlayingRoom)}");

            if (e is PlaybackState.Paused)
                PausedRoom = new PeerRoom(_thisPeer, _pubSub, $"{Metadata.Id}.{this.Id}.{nameof(PausedRoom)}");

            if (e is PlaybackState.Failed)
                FailedRoom = new PeerRoom(_thisPeer, _pubSub, $"{Metadata.Id}.{this.Id}.{nameof(FailedRoom)}");

            if (e is PlaybackState.Loading)
                LoadingRoom = new PeerRoom(_thisPeer, _pubSub, $"{Metadata.Id}.{this.Id}.{nameof(LoadingRoom)}");
        }
    }
}
