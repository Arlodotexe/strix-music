# Build with the SDK

> [!TIP]
> For a better understanding of terms like "CoreModels" and "AppModels", see [How it works](./how-it-works.md).

#### Collect your cores

Cores can be distributed as Nuget packages, or created and included directly in your project.

Once you have your cores, instantiate them:

```csharp
var onedrive = new OneDriveCore(id, config);
var youtube = new YouTubeCore(id, config);
var sideloaded = new RemoteCore(id, config);
```

The actual method signature is decided by each core, dependending on what it needs to function.

> [!WARNING]
> When possible, avoid building your application on top of CoreModels directly. These are intended as data sources only, and are not interchangable with AppModels or the tools/plugins built with them.

#### Create your AppModels
This is the easy part. To turn one or more CoreModel into an AppModel, simply pass it to a new MergedCore along with some config:
```csharp
var prefs = new MergedCollectionConfig();
var mergedLayer = new MergedCore(prefs, onedrive, youtube, localFiles);
```

If this is all you needed, then you're done! You can start using this `mergedLayer` object in your application to interact with all the music services at once:
```csharp
// Perform any async initialization needed. Authenticating, connecting to database, etc.
await mergedLayer.InitAsync(); 

// Get the first 100 tracks from all sources.
var tracks = await mergedLayer.Library.GetTracksAsync(limit: 100, offset: 0).ToListAsync();
```


#### Add Model Plugins (optional)
[Model plugins](../plugins/index.md) are an _extremely_ modular and flexible way to customize the SDK. 

In short, a model plugin modifies data or behavior by wrapping around any [AppModel](../docs/reference/api/StrixMusic.Sdk.AppModels.html) in the SDK and selectively overriding members, then taking the place of the original model.

That means applying plugins is this easy:

```csharp
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
```

Then, simply use `pluginLayer` instead of `mergedLayer`.

```csharp
// Perform any async initialization needed. Authenticating, connecting to database, etc.
await pluginLayer.InitAsync(); 

// Get the first 100 tracks from all sources.
var tracks = await pluginLayer.Library.GetTracksAsync(limit: 100, offset: 0).ToListAsync();
```

#### Create View Models (optional)
If you intend to create an application with the MVVM pattern, the SDK provides another AppModel layer that does creates ViewModels around the entire data structure.

In code-behind:
```csharp
var vmLayer = new StrixDataRootViewModel(mergedLayer); // without plugins
var vmLayer = new StrixDataRootViewModel(pluginLayer); // with plugins

// Perform any async initialization needed. Authenticating, connecting to database, etc.
await vmLayer.InitAsync();

DataRoot = vmLayer;
```

In XAML:
```xml
<Button Command="{Binding DataRoot.Library.PopulateMoreTracksCommand}" Content="Load 100 more tracks" />

<ItemsControl ItemsSource="{Binding DataRoot.Library.Tracks}">
  ...
</ItemsControl>
```
