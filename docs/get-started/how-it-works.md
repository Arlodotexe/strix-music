# How it works

---

## Background

The architecture in the Strix Music SDK had to be very carefully planned for 3 things:
- Easy of use
- Flexibility / extensibility
- Long term maintainability

Data from databases, APIs (like [Spotify](https://developer.spotify.com/documentation/web-api/reference/#/operations/get-an-album) or [Deezer](https://developers.deezer.com/api/album)), file systems, and most backend in general, are always:
- Modular / Reuseable
- Relational (models can reference other models)
- Scope-able (login credentials, API keys, etc)

These properties are intrinsic of _every_ object-oriented data structure (JSON, SQL, XML, C# objects, and many more).

In order to maintain these things throughout the entire SDK and all apps built on it, we had to conceptualize a new architectural pattern.

---

## Application Models
In any normal application, these are also known as "AppModels".
  - These transform your backend's data structure into normal C# objects.
  - Each model has properties for data, methods that returns other C# objects, or events that notify of changes, if needed. 
  - Since objects always return objects, it maintains the same scope, structure and reusability of your data.
  - As normal C# objects, this has the bonus of making your application data completely portable. 
    - Models are always constructed by other models, so you only need to configure dependencies at the root object. 
    - Dependencies are transparently trickled down to other new objects before being returned or emitted by an event.
    - The result is an object tree of your app's data, where objects are easy to get and ready to use 🚀

---

## Application Models _in Strix_
Strix isn't simply pulling in data from one source - we're combining data from _multiple_ sources into one data structure.

This resulting combined data structure is what we call our [`AppModels`](../reference/api/StrixMusic.Sdk.AppModels.html), as this is what you should be building your application with.

We have 2 data structures that perfectly follow the Application Model pattern, as they maintain the scope, modularity, and relational nature of the data they represent.
- [`CoreModels`](../reference/api/StrixMusic.Sdk.CoreModels.html) - which standardize other music sources into a single API.
- [`AppModels`](../reference/api/StrixMusic.Sdk.AppModels.html) - which represent combined music sources, and is designed to build applications on top of.

While CoreModels are implemented by devs (e.g. LocalFilesCore, OneDriveCore), the SDK contains all the AppModel implementations you should need:

- `AdapterModels` - Adapts and merges one or more CoreModel into an AppModel.
- `PluginModels` - Uses [model plugins](../plugins/) to alter behavior of other AppModels as cleanly as possible.
- `ViewModels` - Provides INPC, ObservableCollections, Commands, etc., to facilitate the MVVM pattern for any AppModel in your apps.

