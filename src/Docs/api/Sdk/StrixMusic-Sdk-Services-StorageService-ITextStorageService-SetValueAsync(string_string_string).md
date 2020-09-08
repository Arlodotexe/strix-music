#### [StrixMusic.Sdk](./index.md 'index')
### [StrixMusic.Sdk.Services.StorageService](./StrixMusic-Sdk-Services-StorageService.md 'StrixMusic.Sdk.Services.StorageService').[ITextStorageService](./StrixMusic-Sdk-Services-StorageService-ITextStorageService.md 'StrixMusic.Sdk.Services.StorageService.ITextStorageService')
## ITextStorageService.SetValueAsync(string, string, string) Method
Stores data locally.  
```csharp
System.Threading.Tasks.Task SetValueAsync(string filename, string value, string path);
```
#### Parameters
<a name='StrixMusic-Sdk-Services-StorageService-ITextStorageService-SetValueAsync(string_string_string)-filename'></a>
`filename` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')  
The name of the file (including the file extension).  
  
<a name='StrixMusic-Sdk-Services-StorageService-ITextStorageService-SetValueAsync(string_string_string)-value'></a>
`value` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')  
The value to be stored.  
  
<a name='StrixMusic-Sdk-Services-StorageService-ITextStorageService-SetValueAsync(string_string_string)-path'></a>
`path` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')  
A relative path (separated by forward slashes), to save the file in a subfolder.  
  
#### Returns
[System.Threading.Tasks.Task](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.Task 'System.Threading.Tasks.Task')  
The [System.Threading.Tasks.Task](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.Task 'System.Threading.Tasks.Task') representing the asyncronous operation.  
