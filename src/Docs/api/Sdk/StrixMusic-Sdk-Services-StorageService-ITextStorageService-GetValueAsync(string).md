#### [StrixMusic.Sdk](./index.md 'index')
### [StrixMusic.Sdk.Services.StorageService](./StrixMusic-Sdk-Services-StorageService.md 'StrixMusic.Sdk.Services.StorageService').[ITextStorageService](./StrixMusic-Sdk-Services-StorageService-ITextStorageService.md 'StrixMusic.Sdk.Services.StorageService.ITextStorageService')
## ITextStorageService.GetValueAsync(string) Method
Returns a stored setting, deserialized into a type.  
```csharp
System.Threading.Tasks.Task<string> GetValueAsync(string filename);
```
#### Parameters
<a name='StrixMusic-Sdk-Services-StorageService-ITextStorageService-GetValueAsync(string)-filename'></a>
`filename` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')  
The name of the file to get.  
  
#### Returns
[System.Threading.Tasks.Task&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.Task-1 'System.Threading.Tasks.Task`1')[System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.Task-1 'System.Threading.Tasks.Task`1')  
String representation of the stored value. Null if file isn't found.  
