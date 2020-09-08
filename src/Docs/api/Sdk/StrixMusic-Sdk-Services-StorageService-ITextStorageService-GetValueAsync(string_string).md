#### [StrixMusic.Sdk](./index.md 'index')
### [StrixMusic.Sdk.Services.StorageService](./StrixMusic-Sdk-Services-StorageService.md 'StrixMusic.Sdk.Services.StorageService').[ITextStorageService](./StrixMusic-Sdk-Services-StorageService-ITextStorageService.md 'StrixMusic.Sdk.Services.StorageService.ITextStorageService')
## ITextStorageService.GetValueAsync(string, string) Method
Returns a stored setting, deserialized into a type.  
```csharp
System.Threading.Tasks.Task<string> GetValueAsync(string filename, string path);
```
#### Parameters
<a name='StrixMusic-Sdk-Services-StorageService-ITextStorageService-GetValueAsync(string_string)-filename'></a>
`filename` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')  
The name of the file to get.  
  
<a name='StrixMusic-Sdk-Services-StorageService-ITextStorageService-GetValueAsync(string_string)-path'></a>
`path` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')  
A relative path (separated by forward slashes), to save the file in a subfolder.  
  
#### Returns
[System.Threading.Tasks.Task&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.Task-1 'System.Threading.Tasks.Task`1')[System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.Task-1 'System.Threading.Tasks.Task`1')  
String representation of the stored value. Null if file isn't found.  
