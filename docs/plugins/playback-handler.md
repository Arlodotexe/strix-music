# Playback Handler

Playback operations in Cores only exist to facilitate remote device control. Cores are not responsible for playback.

Instead, playback requests should be intercepted by a model plugin, which can then play audio locally, or delegate to a core for remote playback if needed.

## PlaybackHandlerPlugin

We provide a [`PlaybackHandlerPlugin`](../reference/api/StrixMusic.Sdk.Plugins.PlaybackHandler.PlaybackHandlerPlugin.html#StrixMusic_Sdk_Plugins_PlaybackHandler_PlaybackHandlerPlugin__ctor_StrixMusic_Sdk_MediaPlayback_IPlaybackHandlerService_) in the SDK which does this for you. This takes an implementation of `IPlaybackHandlerService`, detailed below.

```csharp
// Create the AppModels with one or more sources
var mergedLayer = new MergedCore(mergedConfig, onedrive, spotify, youtube, sideloaded);

// Add plugins
var dataRoot = new StrixDataRootPluginWrapper(mergedLayer,

    // Handle playback locally, add start/stop flair, bring your own shuffle logic, whatever you want.
    new PlaybackHandlerPlugin(_playbackHandler),
);

await dataRoot.InitAsync();

// Get albums in all libraries
var albums = await dataRoot.Library.GetAlbumItemsAsync(limit: 20, offset: 0).ToListAsync();

// and play one
await dataRoot.Library.PlayAlbumCollectionAsync(startWith: albums[5]);
```

## IPlaybackHandlerService

A playback handler service is responsible for:
- Playing track collections, or multiple track collections back to back (e.g. queueing many albums in an album collection).
- Player queue management
- Shuffle/unshuffle
- Repeat state
- Exposing a standard [`IDevice`](../reference/api/StrixMusic.Sdk.AppModels.IDevice.html) that reflects the current playback state.
 

You can either use the inbox implementation, or create your own and completely customize how playback is handled.

#### Inbox PlaybackHandlerSerivce

We provide an implementation of [`PlaybackHandlerService`](../reference/api/StrixMusic.Sdk.MediaPlayback.PlaybackHandlerService.html) which does all the hard parts (fisher-yates shuffle, unshuffle, queue management, etc) for you.

If you go with this approach, you can skip creation of `IPlaybackHandlerService` and instead move on to the next step.

## IAudioPlayerService
Once you have a working implementation of `IPlaybackHandlerService`, you need to set up your audio player.

[`IAudioPlayerService`](../reference/api/StrixMusic.Sdk.MediaPlayback.IAudioPlayerService.html) is the simplest form of an audio player. When [`IAudioPlayerService.Play(PlaybackItem, CancellationToken)`](../reference/api/StrixMusic.Sdk.MediaPlayback.IAudioPlayerService.html#StrixMusic_Sdk_MediaPlayback_IAudioPlayerService_Play_StrixMusic_Sdk_MediaPlayback_PlaybackItem_System_Threading_CancellationToken_) is called, you're given a PlaybackItem which contains track data and a media source, and you're expected to play it however you can.

Implementors also need to provide basic things like pause/resume, seeking, reporting position and playback state, etc. 

Generally, this interface is implemented to interact with a platform-specific media player such as the WinUI[`MediaPlayerElement`](https://docs.microsoft.com/en-us/uwp/api/Windows.UI.Xaml.Controls.MediaPlayerElement?view=winrt-22621), the web-based [`<audio>`](https://developer.mozilla.org/en-US/docs/Web/HTML/Element/audio), or even CLI executables like `vlc`, `aplay` on linux, or `wmplayer.exe` on Windows.

### Register your audio player
To tell your PlaybackHandler how to play audio for a certain core, call [`IPlaybackHandlerService.RegisterAudioPlayer(IAudioPlayerService, string)`](../reference/api/StrixMusic.Sdk.MediaPlayback.IPlaybackHandlerService.html#StrixMusic_Sdk_MediaPlayback_IPlaybackHandlerService_RegisterAudioPlayer_StrixMusic_Sdk_MediaPlayback_IAudioPlayerService_System_String_)

 The state of the current audio player is monitored by the playback handler, which relays the data back into a standard `IDevice` implementation which represents all local playback done by your playback handler. This Device is exposed as [`IPlaybackHandlerService.LocalDevice`](../reference/api/StrixMusic.Sdk.MediaPlayback.IPlaybackHandlerService.html#StrixMusic_Sdk_MediaPlayback_IPlaybackHandlerService_LocalDevice).
