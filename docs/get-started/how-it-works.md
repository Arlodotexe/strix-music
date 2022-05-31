# How it works

### Background

The Strix Music SDK was carefully planned for 3 things:

- **Easy of use**
- **Flexibility / extensibility**
- **Long term maintainability**

You can't build an application with no data to power it, so we treated data as a first-class citizen when designing the architecture.

We designed something that _fully_ utilized both the object-oriented language we were working in, and the object-oriented data we were working with.

### The Basics

No matter where it comes from (databases, APIs like [Spotify](https://developer.spotify.com/documentation/web-api/reference/#/operations/get-an-album) or [Deezer](https://developers.deezer.com/api/album), a file system, etc.), relational data has a few intrinsic properties.
- It's modular and reuseable
- It's relational (models can reference other models)
- It can be scoped to a context (login credentials, API keys, root folder, etc)

These properties are also intrinsic of nearly every object-oriented data structure (JSON, SQL, XML, C# objects, and many more).

In order to maintain these things throughout the entire SDK and all apps built on it, we had to conceptualize a new architectural pattern.

### Application Models
These are also known as "AppModels". These transform your backend's data structure into normal C# objects.

- Each model has properties for data, methods that returns other objects, and events that notify of changes, if needed.
- Models are always constructed by other models, so you only need to configure dependencies at the root object. Dependencies are transparently trickled down to other new objects as they're constructed.
- As normal C# objects, this has the bonus of making your application data completely portable. If you have an instance, that's all you need to read the data, update the data, and listen for changes to the data.

The result is an object graph of your app's data which keeps all the intrinsic properties of an object-oriented data structure. Plus, objects are easy to get and ready to use 🚀

### Application Models _in Strix_
In Strix, we don't simply pull in data from one source - we're turning data from _multiple_ sources into a single data structure.

This resulting combined data structure is what we call our [`AppModels`](../reference/api/StrixMusic.Sdk.AppModels.html), as this is what you should be building your application with.

We have 2 data structures that perfectly follow the Application Model pattern, as they maintain the scope, modularity, and relational nature of the data they represent.
- [`CoreModels`](../reference/api/StrixMusic.Sdk.CoreModels.html) - which standardize other music sources into a single API.
- [`AppModels`](../reference/api/StrixMusic.Sdk.AppModels.html) - which represent combined music sources, and is designed to build applications on top of.

While CoreModels are implemented by devs (e.g. LocalFilesCore, OneDriveCore), the SDK contains all the AppModel implementations you should need:

- `AdapterModels` - Adapts and merges one or more CoreModel into an AppModel.
- `PluginModels` - Uses [model plugins](../plugins/) to alter behavior of other AppModels as cleanly as possible.
- `ViewModels` - Provides INPC, ObservableCollections, Commands, etc., to facilitate the MVVM pattern for any AppModel in your apps.

