# Create a core
This tutorial will show you the basics on creating a core, and provide helpful advice for common pitfalls.

To follow current recommendations, these instructions are for creating a new core in Visual Studio in the [Strix Music repo](https://github.com/Arlodotexe/strix-music).

> [!WARNING] 
>
> Until we reach our golden release `1.0.0-sdk-stable`:
> - We are in a rapid release cycle. Builds are automatically published every other weekday.
> - Breaking changes may be introduced as we address feedback from the community.
> - Comprehensive [changelogs](../reference/changelogs/) are included with every release.
> - We advise keeping development (e.g. creating cores) for the SDK in the [main repo](https://github.com/Arlodotexe/strix-music) so we can automatically migrate breaking changes for you.

> [!NOTE]
> #### Notes for core authors
> - Cores do not handle playback, unless the playback is done remotely on an external device. To enable playback, use a model plugin.
> 
> - Properties should be initialized before being returned or emitted. For example, if `TotalTracksCount` is zero, it'll appear as though there are no tracks to get, and some code may never try to get them.
> 
> - The SDK has built-in support for collection folders. Album, Artist and Playlist collections contain collection _items_, which are an interface inherited by both Album/Artist/Playlist, and AlbumCollection/ArtistCollection/PlaylistCollection. 

# Create a project

## New project

Start by opening the StrixMusic solution in Visual Studio.

![The StrixMusic solution open, showing the solution explorer](../assets/cores/create/open-vs.png)

Right click the "Cores" folder -> Add -> New Project

![The expanded context menu for a folder in the solution explorer](../assets/cores/create/context-menu-new-project.png)

In the list of project templates, search for "Class Library" and select the C# class library capable of target .NET Standard.

![The add new project dialog box with "class library" in the search box](../assets/cores/create/add-new-project-select-template.png)

Click "Next". Name the project `StrixMusic.Cores.MyMusicSource`, replacing `MyMusicSource` with your desired name.

Then, change the location to `.\src\Cores\` and click "Next".

![The project configuration screen with the recommended settings](../assets/cores/create/configure-new-project.png)

Change the Framework to `.NET Standard 2.0` and click "Create".

![The framework dropdown with .NET Standard 2.0 selected](../assets/cores/create/additional-information.png)

You'll be greeted with an empty project and a starter class file. You can delete the default `Class1.cs` or repurpose it for your core in the next section.

![](../assets/cores/create/empty-core-project.png)

## Enable nullables and C# 10
Open up the `.csproj` for your new core and add the following lines before the closing `</Project>`:

```xml
	<PropertyGroup>
		<Nullable>enable</Nullable>
		<LangVersion>10.0</LangVersion>
		<WarningsAsErrors>nullable</WarningsAsErrors>
	</PropertyGroup>
```

This tells the compiler to use C# 10, enable nullable reference types, and treat nullable warnings as errors.

> [!TIP]
> This is _highly_ recommended to help avoid sneaky `NullReferenceException`s in your code. 

## Add the SDK

Finally, add a reference to the Strix Music SDK to your project.

Right click the new project -> Add -> Project Reference.

![](../assets/cores/create/new-project-context-menu-add-projectreference.png)

Check the `StrixMusic.Sdk` box and click "OK".

![](../assets/cores/create/reference-manager-new-core.png)

# Set up your models.

All music services are different, therefore you should implement each CoreModel interface in a way that makes sense for _your_ backing API.

## Implement ICore

Start by creating a class called `MyMusicSourceCore` (again, replacing `MyMusicSource` with the desired name) and implement the [`ICore`](../reference/api/StrixMusic.Sdk.CoreModels.ICore.html) interface:

![](../assets/cores/create/core-unimplemented.png)

![](../assets/cores/create/core-unimplemented-autofilled.png)

### Create registration metadata
Cores need a way to identify themselves before and after construction.

Create a public, static property containing `CoreMetadata`, and point the instance `Registration` to it:

![](../assets/cores/create/core-registration.png)

This allows consumers of your core to identify it before creating an instance.

### Replace implementation defaults
Visual Studio autofilled each property and method with `=> throw new NotImplementedException();`.

This is useful to track which things we haven't implemented yet, but bad if we actually want to _use_ this core.

Start by giving the methods at the bottom a proper body, instead of throwing:

```csharp
public ValueTask DisposeAsync()
{
    return default;
}

public Task<ICoreMember?> GetContextByIdAsync(string id, CancellationToken cancellationToken = default)
{
    return Task.FromResult<ICoreMember?>(null);
}

public Task<IMediaSourceConfig?> GetMediaSourceAsync(ICoreTrack track, CancellationToken cancellationToken = default)
{
    return Task.FromResult<IMediaSourceConfig?>(null);
}

public Task InitAsync(CancellationToken cancellationToken = default)
{
    return Task.CompletedTask;
}
```

Then fix properties by doing a replace all, trading `=> throw new NotImplementedException();` for `{ get; private set; }`. The end result should look a bit like this:

![](../assets/cores/create/core-unimplemented-fixed-bodies.png)


### Setting storage
Before continuing, we should set up a place for our core to persist settings. For the sake of simplicity, let's say the only setting for this core is an API token.

We highly recommend using `SettingsBase` from OwlCore:

```csharp
using OwlCore.AbstractStorage;
using OwlCore.Services;
using StrixMusic.Sdk.Services;

namespace StrixMusic.Cores.MyMusicSource
{
    /// <summary>
    /// A container for <see cref="MyMusicSourceCore"/> settings.
    /// </summary>
    public sealed class MyMusicSourceSettings : SettingsBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="MyMusicSourceSettings"/>.
        /// </summary>
        public MyMusicSourceSettings(IFolderData folder)
            : base(folder, NewtonsoftStreamSerializer.Singleton)
        {
        }

        /// <summary>
        /// The API key used for login, if any.
        /// </summary>
        public string ApiKey
        {
            get => GetSetting(() => string.Empty);
            set => SetSetting(value);
        }
    }
}
```

- To change a setting, simply set the property.
- To change the default value, change the return value in `GetSetting(() => "some default value");`. This should never be null.
- To save or load all settings from the provided folder abstraction, simple call `await settings.LoadAsync()` or `await settings.SaveAsync()`.
- To listen for changes to settings, subscribe to the `settings.PropertyChanged` event (easy bindings in XAML).
- and more

### Constructors and dependencies

> [!NOTE]
> Any dependencies required for your core to function should be exposed in the constructor.

The constructor for a core should be _very_ lightweight, only doing enough to satisfy dependencies and non-nullable properties. Cores should also always be multi-instantiable, using the [`InstanceId`](../reference/api/StrixMusic.Sdk.CoreModels.ICore.html#StrixMusic_Sdk_CoreModels_ICore_InstanceId) to identify each instance, passed in via the constructor along with any dependencies.

We'll create a `Settings` property and a constructor which takes an instance of the settings container, where data can be stored.

![](../assets/cores/create/core-unimplemented-constructors.png)

> [!TIP]
> This approach opens the door to configuring a core before creating it, bypassing any user-interactive setup.

Then, we'll set up an example API service. This is where our music service's data will come from:

![](../assets/cores/create/core-unimplemented-with-api-prop.png)

## Implement the library
> [!WARNING]
> Library, RecentlyPlayed, Discoverables, etc., are all an [`ICorePlayableCollectionGroup`](../reference/api/StrixMusic.Sdk.CoreModels.ICorePlayableCollectionGroup.html). This interface is a combination of most collection types (album, artist, track, playlist, image).
> <br/> <br/>
> It's recommended to create an abstract base class with virtual, overrideable members to reduce the work needed to implement these. 

### Implement a base class

We'll be following the above advice, and creating a `MyMusicSourcePlayableCollectionGroupBase`.

![](../assets/cores/create/playablecollectiongroup-base.png)

After implementing, replace the implementation defaults the same [as above](#replace-implementation-defaults).

All core implementations are required to have a `SourceCore` property which points to the `ICore` instance that created it. This should be trickled down via constructors:

![](../assets/cores/create/playablecollectiongroup-base-constructor.png)

This also allows us to access our already configured backend service, ready to use:

![](../assets/cores/create/playablecollectiongroup-base-api-access.png)

For now, we'll only focus on albums, so that's all we're going to set up. You can implement the rest outside of this tutorial.

Search the document for "AlbumItems", and make each member you find `virtual`. Close this file for now.

### Implement ICoreLibrary

Create a new class called `MyMusicSourceLibrary`. Inherit from `ICoreLibrary` and the new `MyMusicSourcePlayableCollectionGroupBase`:

![](../assets/cores/create/core-unimplemented-library.png)

Then, we can override the `GetAlbumItemsAsync` method and use our API to return albums in the user's library:

![](../assets/cores/create/core-implemented-library-precorealbum.png)

You'll notice there's still an error. That's because we're trying to return our API's `Album` type, when we need to return a CoreModel.

### Implement item CoreModels

To solve this, let's implement `ICoreAlbum` and use our API's `Album` type to power it.

Our backend service also has a method to get an album's tracks, so we can put some skeleton code in place for that while we're at it.

![](../assets/cores/create/album-implementation.png)

Now, we can return to our Library and finish implementing getting albums:

![](../assets/cores/create/core-implemented-library-postcorealbum.png)

Great! There's just one more issue to solve.

## Initializing properties
Properties should _always_ be initialized before being returned or emitted outside the core.

For example, if `TotalTracksCount` is zero, it'll appear as though there are no tracks to get, and some code may never try to get them.

Some APIs, especially "pagination" style APIs, don't supply this information until you try getting some items.

The example we've provided so far suffers from this problem for demonstration purposes: our backend API doesn't tell us how many items there are, it just allows us to get them.

To remedy this, you'll need to get this information asynchronously while iterating through the parent collection.

To fix this for our `Album`s in the Library:

```csharp
public class MyMusicSourceLibrary : MyMusicSourcePlayableCollectionGroupBase, ICoreLibrary
{
    public MyMusicSourceLibrary(ICore sourceCore)
        : base(sourceCore)
    {
    }

    public override int TotalAlbumItemsCount { get; }

    public override async IAsyncEnumerable<ICoreAlbumCollectionItem> GetAlbumItemsAsync(int limit, int offset, CancellationToken cancellationToken = default)
    {
        var api = ((MyMusicSourceCore)SourceCore).Api;

        // TODO: handle limit and offset
        foreach(var item in await api.GetSavedAlbums())
        {
            var tracks = await api.GetAlbumTracks(item.Id);

            yield return new MyMusicSourceAlbum(SourceCore, item)
            {
                TotalTrackCount = tracks.Count(),
            };
        }
    }
}
```

This has the obvious drawback of increasing the time it takes to get each album, but since the SDK using `IAsyncEnumerable`, each item is asynchronously returned as soon as it possibly can.

Cores which have this issue can use caching mechanisms to further mitigate the issue.

Small issues like this tend to appear when you try to adapt one API to another. If your API does this, this is the only workaround that's guaranteed to work as expected.

We caught this problem early in development, and made 100% sure you could realistically create a core with a backend that does this.

## Initializing the core
Before your core can be used, consumers are required to call `InitAsync` on your core.

This is your opportunity to perform any async setup needed before usage, such as login, database init, user configuration, etc.

This is also the place to make sure you _have_ all the data you need to operate. If any data is missing, you can change the CoreState to `NeedsConfiguration` and assign some AbstractUI to the [`AbstractConfigPanel`](../reference/api/StrixMusic.Sdk.CoreModels.ICore.html#StrixMusic_Sdk_CoreModels_ICore_AbstractConfigPanel).

The consuming application should display this UI to the user, and when the user has provided an indication that they're finished (via an event on one of your components), async initialization can continue as normal.

For our example core, the `InitAsync` method would look a bit like this:

```csharp

public async Task InitAsync(CancellationToken cancellationToken = default)
{
    CoreState = CoreState.Loading;
    CoreStateChanged?.Invoke(this, CoreState);

    await Settings.LoadAsync();

CheckConfig:
    if (string.IsNullOrWhiteSpace(Settings.ApiKey))
    {
        CoreState = CoreState.NeedsConfiguration;
        CoreStateChanged?.Invoke(this, CoreState);
        
        var textBox = new AbstractTextBox(id: "api-key", value: Settings.ApiKey)
        {
            Title = "Enter an API key",
        };

        var doneButton = new AbstractButton(id: "config-done", "Done", type: AbstractButtonType.Confirm);
        var doneButtonClickedTaskCompletionSource = new TaskCompletionSource<object?>();

        textBox.ValueChanged += TextBox_ValueChanged;
        doneButton.Clicked += DoneButton_Clicked;

        AbstractConfigPanel = new AbstractUICollection(id: "panel-id")
        {
            textBox,
            doneButton,
        };

        AbstractConfigPanelChanged?.Invoke(this, EventArgs.Empty);

        await doneButtonClickedTaskCompletionSource.Task;
        await Settings.SaveAsync();
        
        textBox.ValueChanged -= TextBox_ValueChanged;
        doneButton.Clicked -= DoneButton_Clicked;

        goto CheckConfig;

        void TextBox_ValueChanged(object sender, string e) => Settings.ApiKey = e;
        void DoneButton_Clicked(object sender, EventArgs e) => doneButtonClickedTaskCompletionSource.SetResult(null);
    }

    CoreState = CoreState.Configured;
    CoreStateChanged?.Invoke(this, CoreState);

    Api = new MusicBackendService(Settings.ApiKey);

    await Api.Login();

    CoreState = CoreState.Loaded;
    CoreStateChanged?.Invoke(this, CoreState);
}
```
