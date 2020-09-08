#### [StrixMusic.Sdk](./index.md 'index')
### [StrixMusic.Sdk.Interfaces](./StrixMusic-Sdk-Interfaces.md 'StrixMusic.Sdk.Interfaces').[IArtistCollection](./StrixMusic-Sdk-Interfaces-IArtistCollection.md 'StrixMusic.Sdk.Interfaces.IArtistCollection')
## IArtistCollection.PopulateArtistsAsync(int, int) Method
Populates a set of [Artists](./StrixMusic-Sdk-Interfaces-IArtistCollection-Artists.md 'StrixMusic.Sdk.Interfaces.IArtistCollection.Artists') into the collection.  
```csharp
System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyList<StrixMusic.Sdk.Interfaces.IArtist>> PopulateArtistsAsync(int limit, int offset=0);
```
#### Parameters
<a name='StrixMusic-Sdk-Interfaces-IArtistCollection-PopulateArtistsAsync(int_int)-limit'></a>
`limit` [System.Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32 'System.Int32')  
  
<a name='StrixMusic-Sdk-Interfaces-IArtistCollection-PopulateArtistsAsync(int_int)-offset'></a>
`offset` [System.Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32 'System.Int32')  
  
#### Returns
[System.Threading.Tasks.Task&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.Task-1 'System.Threading.Tasks.Task`1')[System.Collections.Generic.IReadOnlyList&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IReadOnlyList-1 'System.Collections.Generic.IReadOnlyList`1')[IArtist](./StrixMusic-Sdk-Interfaces-IArtist.md 'StrixMusic.Sdk.Interfaces.IArtist')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IReadOnlyList-1 'System.Collections.Generic.IReadOnlyList`1')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.Task-1 'System.Threading.Tasks.Task`1')  
A [System.Threading.Tasks.Task](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.Task 'System.Threading.Tasks.Task') representing the asynchronous operation.  
