# Strix Music: Cores

A core is an interchangeable music source that implements the [`CoreModel`](../reference/api/StrixMusic.Sdk.CoreModels.html) interfaces.

Anything that can implement these interfaces can be used as a music source, regardless of what APIs, protocols or technologies are used behind the scenes.

> [!TIP]
> For a better understanding of terms like "CoreModels" and "AppModels", see [How it works](../get-started/how-it-works.md).


> [!WARNING] 
>
> Until we reach our golden release `1.0.0-sdk-stable`:
> - We are in a rapid release cycle. Builds are automatically published every other weekday.
> - Breaking changes may be introduced as we address feedback from the community.
> - Comprehensive [changelogs](../reference/changelogs/) are included with every release.
> - We advise keeping development (e.g. creating cores) for the SDK in the [main repo](https://github.com/Arlodotexe/strix-music) so we can automatically migrate breaking changes for you.

## Basic usage


#### Set up your cores

Cores can be distributed as Nuget packages, or created and included directly in your project.

Once you have your cores, instantiate them:

```csharp
var onedrive = new OneDriveCore(id, config);
var youtube = new YouTubeCore(id, config);
var sideloaded = new RemoteCore(id, config);
```

The actual method signature is decided by each core, dependingon what it needs to function.

Cores have an [AbstractUI panel](../reference/api/StrixMusic.Sdk.CoreModels.ICore.html#StrixMusic_Sdk_CoreModels_ICore_AbstractConfigPanel) that can be presented to the user for login, configuration, etc., when the core is first loaded. This API is a part of OwlCore, and is data abstraction of standardized, simple UI elements. It includes buttons, boolean toggles, multichoice, text box, item lists/grids, and more. 

Anything that can interact with these models can render an interactive UI capable of displaying settings, login, and even interactive components such as filesystem browsers.

Some noteworthy events here:
- [`ICore.CoreStateChanged`](../reference/api/StrixMusic.Sdk.CoreModels.ICore.html#StrixMusic_Sdk_CoreModels_ICore_CoreStateChanged) - When the state is changed to `NeedsConfiguration`, the config panel should be displayed to the user.
- [`ICore.AbstractConfigPanelChanged`](reference/api/StrixMusic.Sdk.CoreModels.ICore.html#StrixMusic_Sdk_CoreModels_ICore_AbstractConfigPanelChanged) - When raised, the config panel has been updated and should be re-rendered. 

Cores should also provide a constructor overload that exposes all the necessary information to use the core headless, without user interaction.

> [!WARNING]
> When possible, avoid building your application on top of CoreModels directly. These are intended as data sources only, and are not interchangable with AppModels or the tools/plugins built with them.
> ​
> ​<br/><br/>
> If you decide to do this, you'll need to call `InitAsync` on the core itself:
> 
> ```csharp
> // Only if using a core standalone (not recommended)
> // Perform any async initialization needed. Authenticating, connecting to database, etc.
> await onedrive.InitAsync();
> ```

#### Merge them together
This is the easy part. To turn one or more CoreModel into an AppModel, simply pass it to an AdapterModel along with some config:
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

