# Add a new music source
This guide will help you add a new music source to the Strix Music App.

In the app, this is all handled in the `StrixMusic.Shared` project.

> [!NOTE]
> 1. If you do not have a core that is ready to use, see [Create a core](../cores/create.md) first. 
> 2. We created this guide by documenting how we added OneDrive as a music source. Substitute any OneDrive-specific code with your own.

You'll need to:
1. Create the settings class for your core
2. Create your CoreFactory
3. Register your music source
4. Tell AppRoot how to turn settings into core instances
5. Create and link the first time setup UI for your core
6. Create and link the "Options" control for your core.

## 1. Create your core settings
Settings are used to store the data that's required to construct a core. We'll be turning a settings class into an `ICore` instance, so this up.

### Initial setup

To speed up setup of serialization and storage, duplicate an existing settings file instead of creating a new one.

1. Duplicate and rename an existing settings class (anything that inherits from `CoreSettingsBase`). 
2. Perform a replace-all on it, trading the existing class name for your own.
3. Remove all properties except for `InstanceId`
4. In `IsSettingValidForCoreCreation` and `GetSettingByName`, remove handling of any removed properties.
5. Update all existing comments to reflect your new class.

You should now have a settings class with all the plumbing and requirements set up. 

Here, we've copied `LocalStorageCoreSettings.cs` and created `OneDriveCoreSettings.cs`:
![](../assets/app/add-music-source/empty-settings-class-ready-to-use.png)

### Add your settings

1. Create a public property
2. In the getter, use `GetSetting(() => ...);`, replacing `...` with the fallback value to use when unset.
3. In the setter, add `SetSetting(value);`.
4. Add code comments explaining what this setting is or what it's for.
5. If the property is required for core creation, add validation in `IsSettingValidForCoreCreation` and handle the property in `GetSettingByName`.
6. Add the property's type to `AppSettingsSerializerContext` if needed.

Here, we've added what we need to create a OneDrive folder that we can give to `StorageCore`:
![](../assets/app/add-music-source/onedrive-core-settings-with-properties.png)

## Create your CoreFactory
In the Shared project, open the [`CoreFactory`](../reference/api/StrixMusic.Sdk.Plugins.PlaybackHandler.PlaybackHandlerPlugin.html#StrixMusic_Sdk_Plugins_PlaybackHandler_PlaybackHandlerPlugin__ctor_StrixMusic_Sdk_MediaPlayback_IPlaybackHandlerService_)

TODO 

## Register your music source

TODO 

## Tell AppRoot how to turn settings into core instances

TODO 


## Create and link the first time setup UI for your core

TODO 


## Create and link the "Options" control for your core.

TODO
