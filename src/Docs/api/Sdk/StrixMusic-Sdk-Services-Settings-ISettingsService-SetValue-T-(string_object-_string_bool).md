#### [StrixMusic.Sdk](./index.md 'index')
### [StrixMusic.Sdk.Services.Settings](./StrixMusic-Sdk-Services-Settings.md 'StrixMusic.Sdk.Services.Settings').[ISettingsService](./StrixMusic-Sdk-Services-Settings-ISettingsService.md 'StrixMusic.Sdk.Services.Settings.ISettingsService')
## ISettingsService.SetValue&lt;T&gt;(string, object?, string, bool) Method
Assigns a value to a settings key.  
```csharp
void SetValue<T>(string key, object? value, string identifier, bool overwrite=true);
```
#### Type parameters
<a name='StrixMusic-Sdk-Services-Settings-ISettingsService-SetValue-T-(string_object-_string_bool)-T'></a>
`T`  
The type of the object bound to the key.  
  
#### Parameters
<a name='StrixMusic-Sdk-Services-Settings-ISettingsService-SetValue-T-(string_object-_string_bool)-key'></a>
`key` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')  
The key to check.  
  
<a name='StrixMusic-Sdk-Services-Settings-ISettingsService-SetValue-T-(string_object-_string_bool)-value'></a>
`value` [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object')  
The value to assign to the setting key.  
  
<a name='StrixMusic-Sdk-Services-Settings-ISettingsService-SetValue-T-(string_object-_string_bool)-identifier'></a>
`identifier` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')  
Identifies a unique version of this settings key.  
  
<a name='StrixMusic-Sdk-Services-Settings-ISettingsService-SetValue-T-(string_object-_string_bool)-overwrite'></a>
`overwrite` [System.Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean 'System.Boolean')  
Indicates whether or not to overwrite the setting, if it already exists.  
  
