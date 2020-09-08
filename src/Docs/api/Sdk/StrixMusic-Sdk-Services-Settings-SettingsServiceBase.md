#### [StrixMusic.Sdk](./index.md 'index')
### [StrixMusic.Sdk.Services.Settings](./StrixMusic-Sdk-Services-Settings.md 'StrixMusic.Sdk.Services.Settings')
## SettingsServiceBase Class
A https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/class that handles the local app settings.  
```csharp
public abstract class SettingsServiceBase :
ISettingsService
```
Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; SettingsServiceBase  

Derived  
&#8627; [DefaultSettingsService](./StrixMusic-Sdk-Services-Settings-DefaultSettingsService.md 'StrixMusic.Sdk.Services.Settings.DefaultSettingsService')  

Implements [ISettingsService](./StrixMusic-Sdk-Services-Settings-ISettingsService.md 'StrixMusic.Sdk.Services.Settings.ISettingsService')  
### Constructors
- [SettingsServiceBase()](./StrixMusic-Sdk-Services-Settings-SettingsServiceBase-SettingsServiceBase().md 'StrixMusic.Sdk.Services.Settings.SettingsServiceBase.SettingsServiceBase()')
- [SettingsServiceBase(StrixMusic.Sdk.Services.StorageService.ITextStorageService)](./StrixMusic-Sdk-Services-Settings-SettingsServiceBase-SettingsServiceBase(StrixMusic-Sdk-Services-StorageService-ITextStorageService).md 'StrixMusic.Sdk.Services.Settings.SettingsServiceBase.SettingsServiceBase(StrixMusic.Sdk.Services.StorageService.ITextStorageService)')
### Properties
- [Id](./StrixMusic-Sdk-Services-Settings-SettingsServiceBase-Id.md 'StrixMusic.Sdk.Services.Settings.SettingsServiceBase.Id')
- [SettingsKeysType](./StrixMusic-Sdk-Services-Settings-SettingsServiceBase-SettingsKeysType.md 'StrixMusic.Sdk.Services.Settings.SettingsServiceBase.SettingsKeysType')
### Methods
- [GetValue&lt;T&gt;(string)](./StrixMusic-Sdk-Services-Settings-SettingsServiceBase-GetValue-T-(string).md 'StrixMusic.Sdk.Services.Settings.SettingsServiceBase.GetValue&lt;T&gt;(string)')
- [GetValue&lt;T&gt;(string, string)](./StrixMusic-Sdk-Services-Settings-SettingsServiceBase-GetValue-T-(string_string).md 'StrixMusic.Sdk.Services.Settings.SettingsServiceBase.GetValue&lt;T&gt;(string, string)')
- [ResetToDefaults()](./StrixMusic-Sdk-Services-Settings-SettingsServiceBase-ResetToDefaults().md 'StrixMusic.Sdk.Services.Settings.SettingsServiceBase.ResetToDefaults()')
- [ResetToDefaults(string)](./StrixMusic-Sdk-Services-Settings-SettingsServiceBase-ResetToDefaults(string).md 'StrixMusic.Sdk.Services.Settings.SettingsServiceBase.ResetToDefaults(string)')
- [SetValue&lt;T&gt;(string, object?, bool)](./StrixMusic-Sdk-Services-Settings-SettingsServiceBase-SetValue-T-(string_object-_bool).md 'StrixMusic.Sdk.Services.Settings.SettingsServiceBase.SetValue&lt;T&gt;(string, object?, bool)')
- [SetValue&lt;T&gt;(string, object?, string, bool)](./StrixMusic-Sdk-Services-Settings-SettingsServiceBase-SetValue-T-(string_object-_string_bool).md 'StrixMusic.Sdk.Services.Settings.SettingsServiceBase.SetValue&lt;T&gt;(string, object?, string, bool)')
### Events
- [SettingChanged](./StrixMusic-Sdk-Services-Settings-SettingsServiceBase-SettingChanged.md 'StrixMusic.Sdk.Services.Settings.SettingsServiceBase.SettingChanged')
