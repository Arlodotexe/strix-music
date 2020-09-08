#### [StrixMusic.Sdk](./index.md 'index')
### [StrixMusic.Sdk.Services.StorageService](./StrixMusic-Sdk-Services-StorageService.md 'StrixMusic.Sdk.Services.StorageService').[IFileSystemService](./StrixMusic-Sdk-Services-StorageService-IFileSystemService.md 'StrixMusic.Sdk.Services.StorageService.IFileSystemService')
## IFileSystemService.RevokeAccess(StrixMusic.Sdk.Interfaces.Storage.IFolderData) Method
Called when the user wants to revoke access to a folder.  
```csharp
System.Threading.Tasks.Task RevokeAccess(StrixMusic.Sdk.Interfaces.Storage.IFolderData folder);
```
#### Parameters
<a name='StrixMusic-Sdk-Services-StorageService-IFileSystemService-RevokeAccess(StrixMusic-Sdk-Interfaces-Storage-IFolderData)-folder'></a>
`folder` [IFolderData](./StrixMusic-Sdk-Interfaces-Storage-IFolderData.md 'StrixMusic.Sdk.Interfaces.Storage.IFolderData')  
The folder to be revoked.  
  
#### Returns
[System.Threading.Tasks.Task](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.Task 'System.Threading.Tasks.Task')  
A [System.Threading.Tasks.Task&lt;&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.Task-1 'System.Threading.Tasks.Task`1') representing the asyncronous operation.  
