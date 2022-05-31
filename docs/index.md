---
uid: home
title: Strix Music SDK
---

<div style="text-align:center">

![Strix Music SDK](assets/logos/sdk.svg)

</div>

<h2 style="text-align: center;">
 The universal music development kit
</h2>


<div style="text-align:center">
A highly flexible and standardized development kit for music apps.

Rapidly interface with any music service to build new apps and tools. 
</div>

<div style="text-align:center;">

<img height="225px" src="assets/services.png" />

</div>

> [!WARNING] 
>
> Until we reach our golden release `1.0.0-sdk-stable`:
> - We are in a rapid release cycle. Builds are automatically published every other weekday.
> - Breaking changes may be introduced as we address feedback from the community.
> - Comprehensive [changelogs](reference/changelogs/) are included with every release.
> - We advise keeping development (e.g. creating cores) for the SDK in the [main repo](https://github.com/Arlodotexe/strix-music) so we can automatically migrate breaking changes for you.


<!-- Example for embedding code directly in docs.  -->
<!-- [!code[Main](../src/Sdk/StrixMusic.Sdk/Plugins/PlaybackHandler/PlaybackHandlerPlugin.cs#L26-L34)] -->

# Welcome to the open source alpha

We spent the last 2 years designing and refining the SDK, making sure it could cleanly handle anything we threw at it.

The foundation has been laid, and we've entered the next stage in development: **open source alpha**.


- ✅ Use the SDK in your own project to easily interface with any music service.

- ✅ Extend any strix-compatible app with new music sources and plugins.

- ✅ Provide feedback, feature suggestions or bug reports to improve the SDK for everyone.

- ❌ Build WinUI applications using our inbox control library (not ready)

- ❌ Download the Strix Music App, our incubation and demo project for the SDK (not ready)

---

# **Cores**
#### Interchangeable music sources

A core is any music source that implements the [CoreModel interfaces](reference/api/StrixMusic.Sdk.CoreModels.html). These are a standardized and highly flexible data structure designed to fit the needs of different types of music sources.

## Basic usage
```csharp
// Core method signature may vary.
var localFilesCore = new LocalFilesCore(id, folderAbstraction);

// Perform any async setup needed before usage, such as login or database init.
await localFilesCore.InitAsync();

// Get tracks from the library
var tracks = await localFilesCore.Library.GetTracksAsync(limit: 20, offset: 0).ToListAsync();
```

## Merge them together!
The Strix Music SDK allows you to merge multiple sources together. Libraries, search results, recently played, devices, etc. are all seamelessly combined under a single data structure. 

```csharp
// Assumes that `config` contains all the info needed to function without user interaction.
// Spotify and YouTube cores are examples only.

// Create a few cores.
var onedrive = new OneDriveCore(id, config);
var youtube = new YouTubeCore(id, config);
var sideloaded = new RemoteCore(id, config);

// Merge them together
var mergedLayer = new MergedCore(mergedConfig, onedrive, spotify, youtube, sideloaded);
await mergedLayer.InitAsync();

// Get albums in all libraries
var albums = await mergedLayer.Library.GetAlbumItemsAsync(limit: 20, offset: 0).ToListAsync();

// and play one
await mergedLayer.Library.PlayAlbumCollectionAsync(startWith: albums[5]);

// Search everywhere
var searchResults = await mergedLayer.Search.GetSearchResultsAsync("Zombie by Jamie T").ToListAsync();
var tracks = await searchResults.GetTracksAsync(limit: 100, offset: 0).ToListAsync();
var artists = await searchResults.GetArtistItemsAsync(limit: 10, offset: 0).ToListAsync();

// Get all the sources that were combined to create the track
var sourceTracks = tracks[0].Sources; // FilesCoreTrack, YouTubeTrack
```

## The sky is the limit

Anyone can create a core, and a core can be anything that satisfies the CoreModel interfaces.

Here are some ideas for cores we'd like to see join the Strix ecosystem (click to expand):

<details>
  <summary><span>> File based</span> (90%+ code sharing)</summary>

  - ✅ Local Files
  - ✅ OneDrive
  - 🔳 Google Drive
  - 🔳 Dropbox
  - 🔳 [IPFS](https://ipfs.io/)
  - 🔳 FTP
  
  </details>

<details>
  <summary><span>> Streaming based</span> (all possible but not promised)</summary>

  - 🔳 YouTube
  - 🔳 SoundCloud
  - 🔳 Spotify
  - 🔳 Pandora
  - 🔳 Audible
  - 🔳 Deezer
  
  </details>

<details>
  <summary><span>> Hardware based</span></summary>

  - 🔳 CDs
  - 🔳 Zune

  </details>
  
<details>
  <summary><span>> Remote </span> (out of process / on another machine)</summary>

  - 🔳 [OwlCore.Remoting](https://arlo.site/owlcore/articles/OwlCore.Remoting/index.html) (see [#103](https://github.com/Arlodotexe/strix-music/issues/103))
  - 🔳 gRPC
  - 🔳 Standardized REST API

  </details>


# Model Plugins
#### Infinite customization

[Model plugins](../docs/reference/api/StrixMusic.Sdk.Plugins.Model.html) are an _extremely_ modular and flexible way to customize the SDK. 

In short, a model plugin modifies data or behavior by wrapping around any [AppModel](../docs/reference/api/StrixMusic.Sdk.AppModels.html) in the SDK and selectively overriding members, then taking the place of the original model.

Once you have at least one model plugin, use the [PluginModels](../docs/reference/api/StrixMusic.Sdk.PluginModels.html) layer to wrap around an existing [data root](..//docs/reference/api/StrixMusic.Sdk.AppModels.IStrixDataRoot.html), and provide plugins that you want applied to all interface implementations in the data structure. For example, an [ImageCollection](../docs/reference/api/StrixMusic.Sdk.Plugins.Model.ImageCollectionPluginBase.html) plugin is applied to `IAlbum`, `IArtist`, `IPlaylist`, etc..

Then, simply take the place of the original data root.

## Applying model plugins

```csharp
// Create the AppModels with one or more sources
var mergedLayer = new MergedCore(mergedConfig, onedrive, spotify, youtube, sideloaded);

// Add plugins
var pluginLayer = new StrixDataRootPluginWrapper(mergedLayer,
    new FallbackImagePlugin(fallbackImage),

    // Handle playback locally, add start/stop flair, bring your own shuffle logic, whatever you want.
    new PlaybackHandlerPlugin(_playbackHandler),

    // Other ideas that are possible
    new LastFmPlugin(),
    new MissingMetadataFromMusicBrainzPlugin(),
    new MusixmatchSyncedLyricsPlugin(),
    new CacheEverythingOfflinePlugin(),
);

// Optionally wrap with ViewModels for MVVM.
var viewModel = new StrixDataRootViewModel(mergedLayer); // without plugins
var viewModel = new StrixDataRootViewModel(pluginLayer); // with plugins

```

### Example: fallback images
In this example, we create a plugin that can inject a fallback image into any empty `IImageCollection`.

```csharp
// Implement the PluginBase that corresponds to the interface you want to affect.
public class AddFallbackToImageCollectionPlugin : ImageCollectionPluginBase
{
    private readonly IImage _fallbackImage;

    public AddFallbackToImageCollectionPlugin(ModelPluginMetadata metadata, IImageCollection inner, IImage fallbackImage)
      : base(metadata, inner)
    {
        _fallbackImage = fallbackImage;
    }

    public override int TotalImageCount => base.TotalImageCount > 0 ? base.TotalImageCount : 1;

    public override async IAsyncEnumerable<IImage> GetImagesAsync(int limit, int offset, CancellationToken cancellationToken = default)
    {
        if (base.TotalImageCount == 0)
            yield return _fallbackImage;

        await foreach (var item in base.GetImagesAsync(limit, offset, cancellationToken))
            yield return item;
    }
}

// Create a container that identifies your plugin and tells us how to construct the pieces:
public class FallbackImagePlugin : SdkModelPlugin
{
    private static readonly ModelPluginMetadata _metadata = new(
        id: nameof(FallbackImagePlugin),
        displayName: "Fallback images",
        description: "When an image collection is empty, this plugin injects a fallback image.",
        new Version(0, 0, 0));

    public FallbackImagePlugin(IImage fallbackImage)
        : base(_metadata)
    {
        ImageCollection.Add(x => new StrixDataRootPlaybackHandlerPlugin(_metadata, x, fallbackImage));
    }
}
```


# Philosophy of Strix Music
#### Our promises to the community

## A free and open standard
That means community focused and no paywalls. You'll never be charged to build with or use the Strix Music standard, Strix Music SDK or the Strix Music App. 
 
To drive our efforts, we rely on donations and contributions from users like you.

## Privacy focused
No logs are generated and no servers are contacted unless you say so. Your data is exclusively put into your hands.

## Perpetually preserved
Apps that work standalone and offline are a lost art. We want to build software where a given version will always work, whether on Day 1 or Day 10,000

The entire project (docs, website, source code, build scripts, dependencies and releases) are perpetually preserved in every release, and hosted on [IPFS](https://ipfs.io/). If _anyone_ has these things on their local node, you'll be able to access it.

Our project will never break from a server outage, and cannot be taken down or censored.

Not even an apocalypse could ruin our hard work.
