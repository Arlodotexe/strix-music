#### [StrixMusic.Sdk](./index.md 'index')
### [StrixMusic.Sdk.Services.StorageService](./StrixMusic-Sdk-Services-StorageService.md 'StrixMusic.Sdk.Services.StorageService').[ITextStorageService](./StrixMusic-Sdk-Services-StorageService-ITextStorageService.md 'StrixMusic.Sdk.Services.StorageService.ITextStorageService')
## ITextStorageService.FileExistsAsync(string) Method
Checks if the file exists.  
```csharp
System.Threading.Tasks.Task<bool> FileExistsAsync(string filename);
```
#### Parameters
<a name='StrixMusic-Sdk-Services-StorageService-ITextStorageService-FileExistsAsync(string)-filename'></a>
`filename` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')  
The name of the file (including the file extension).  
  
#### Returns
[System.Threading.Tasks.Task&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.Task-1 'System.Threading.Tasks.Task`1')[System.Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean 'System.Boolean')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.Task-1 'System.Threading.Tasks.Task`1')  
True if the file exists, otherwise false.  
