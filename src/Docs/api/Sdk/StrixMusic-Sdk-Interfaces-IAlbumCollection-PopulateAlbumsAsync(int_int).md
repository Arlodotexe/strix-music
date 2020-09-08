#### [StrixMusic.Sdk](./index.md 'index')
### [StrixMusic.Sdk.Interfaces](./StrixMusic-Sdk-Interfaces.md 'StrixMusic.Sdk.Interfaces').[IAlbumCollection](./StrixMusic-Sdk-Interfaces-IAlbumCollection.md 'StrixMusic.Sdk.Interfaces.IAlbumCollection')
## IAlbumCollection.PopulateAlbumsAsync(int, int) Method
Populates the [Albums](./StrixMusic-Sdk-Interfaces-IAlbumCollection-Albums.md 'StrixMusic.Sdk.Interfaces.IAlbumCollection.Albums') in the collection.  
```csharp
System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyList<StrixMusic.Sdk.Interfaces.IAlbum>> PopulateAlbumsAsync(int limit, int offset=0);
```
#### Parameters
<a name='StrixMusic-Sdk-Interfaces-IAlbumCollection-PopulateAlbumsAsync(int_int)-limit'></a>
`limit` [System.Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32 'System.Int32')  
  
<a name='StrixMusic-Sdk-Interfaces-IAlbumCollection-PopulateAlbumsAsync(int_int)-offset'></a>
`offset` [System.Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32 'System.Int32')  
  
#### Returns
[System.Threading.Tasks.Task&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.Task-1 'System.Threading.Tasks.Task`1')[System.Collections.Generic.IReadOnlyList&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IReadOnlyList-1 'System.Collections.Generic.IReadOnlyList`1')[IAlbum](./StrixMusic-Sdk-Interfaces-IAlbum.md 'StrixMusic.Sdk.Interfaces.IAlbum')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IReadOnlyList-1 'System.Collections.Generic.IReadOnlyList`1')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.Task-1 'System.Threading.Tasks.Task`1')  
A [System.Threading.Tasks.Task](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.Task 'System.Threading.Tasks.Task') representing the asynchronous operation.  
