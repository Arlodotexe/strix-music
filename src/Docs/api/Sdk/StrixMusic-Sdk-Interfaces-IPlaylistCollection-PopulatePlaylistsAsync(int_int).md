#### [StrixMusic.Sdk](./index.md 'index')
### [StrixMusic.Sdk.Interfaces](./StrixMusic-Sdk-Interfaces.md 'StrixMusic.Sdk.Interfaces').[IPlaylistCollection](./StrixMusic-Sdk-Interfaces-IPlaylistCollection.md 'StrixMusic.Sdk.Interfaces.IPlaylistCollection')
## IPlaylistCollection.PopulatePlaylistsAsync(int, int) Method
Populates the [IPlaylist](./StrixMusic-Sdk-Interfaces-IPlaylist.md 'StrixMusic.Sdk.Interfaces.IPlaylist') in the collection.  
```csharp
System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyList<StrixMusic.Sdk.Interfaces.IPlaylist>> PopulatePlaylistsAsync(int limit, int offset=0);
```
#### Parameters
<a name='StrixMusic-Sdk-Interfaces-IPlaylistCollection-PopulatePlaylistsAsync(int_int)-limit'></a>
`limit` [System.Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32 'System.Int32')  
  
<a name='StrixMusic-Sdk-Interfaces-IPlaylistCollection-PopulatePlaylistsAsync(int_int)-offset'></a>
`offset` [System.Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32 'System.Int32')  
  
#### Returns
[System.Threading.Tasks.Task&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.Task-1 'System.Threading.Tasks.Task`1')[System.Collections.Generic.IReadOnlyList&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IReadOnlyList-1 'System.Collections.Generic.IReadOnlyList`1')[IPlaylist](./StrixMusic-Sdk-Interfaces-IPlaylist.md 'StrixMusic.Sdk.Interfaces.IPlaylist')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IReadOnlyList-1 'System.Collections.Generic.IReadOnlyList`1')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.Task-1 'System.Threading.Tasks.Task`1')  
A [System.Threading.Tasks.Task](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.Task 'System.Threading.Tasks.Task') representing the asynchronous operation.  
