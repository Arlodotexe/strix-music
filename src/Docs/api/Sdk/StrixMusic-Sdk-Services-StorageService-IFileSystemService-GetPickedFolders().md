#### [StrixMusic.Sdk](./index.md 'index')
### [StrixMusic.Sdk.Services.StorageService](./StrixMusic-Sdk-Services-StorageService.md 'StrixMusic.Sdk.Services.StorageService').[IFileSystemService](./StrixMusic-Sdk-Services-StorageService-IFileSystemService.md 'StrixMusic.Sdk.Services.StorageService.IFileSystemService')
## IFileSystemService.GetPickedFolders() Method
Returns the folders that the user has granted access to.  
```csharp
System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyList<StrixMusic.Sdk.Interfaces.Storage.IFolderData?>> GetPickedFolders();
```
#### Returns
[System.Threading.Tasks.Task&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.Task-1 'System.Threading.Tasks.Task`1')[System.Collections.Generic.IReadOnlyList&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IReadOnlyList-1 'System.Collections.Generic.IReadOnlyList`1')[IFolderData](./StrixMusic-Sdk-Interfaces-Storage-IFolderData.md 'StrixMusic.Sdk.Interfaces.Storage.IFolderData')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IReadOnlyList-1 'System.Collections.Generic.IReadOnlyList`1')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.Task-1 'System.Threading.Tasks.Task`1')  
A [System.Collections.Generic.IReadOnlyList&lt;&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IReadOnlyList-1 'System.Collections.Generic.IReadOnlyList`1') containing paths pointing to the folders the user has granted access to.  
