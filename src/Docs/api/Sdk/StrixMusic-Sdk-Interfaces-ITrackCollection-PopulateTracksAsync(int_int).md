#### [StrixMusic.Sdk](./index.md 'index')
### [StrixMusic.Sdk.Interfaces](./StrixMusic-Sdk-Interfaces.md 'StrixMusic.Sdk.Interfaces').[ITrackCollection](./StrixMusic-Sdk-Interfaces-ITrackCollection.md 'StrixMusic.Sdk.Interfaces.ITrackCollection')
## ITrackCollection.PopulateTracksAsync(int, int) Method
Populates a set of [ITrack](./StrixMusic-Sdk-Interfaces-ITrack.md 'StrixMusic.Sdk.Interfaces.ITrack') into [Tracks](./StrixMusic-Sdk-Interfaces-ITrackCollection-Tracks.md 'StrixMusic.Sdk.Interfaces.ITrackCollection.Tracks').  
```csharp
System.Threading.Tasks.Task<System.Collections.Generic.IReadOnlyList<StrixMusic.Sdk.Interfaces.ITrack>> PopulateTracksAsync(int limit, int offset=0);
```
#### Parameters
<a name='StrixMusic-Sdk-Interfaces-ITrackCollection-PopulateTracksAsync(int_int)-limit'></a>
`limit` [System.Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32 'System.Int32')  
  
<a name='StrixMusic-Sdk-Interfaces-ITrackCollection-PopulateTracksAsync(int_int)-offset'></a>
`offset` [System.Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32 'System.Int32')  
  
#### Returns
[System.Threading.Tasks.Task&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.Task-1 'System.Threading.Tasks.Task`1')[System.Collections.Generic.IReadOnlyList&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IReadOnlyList-1 'System.Collections.Generic.IReadOnlyList`1')[ITrack](./StrixMusic-Sdk-Interfaces-ITrack.md 'StrixMusic.Sdk.Interfaces.ITrack')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IReadOnlyList-1 'System.Collections.Generic.IReadOnlyList`1')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.Task-1 'System.Threading.Tasks.Task`1')  
A [System.Threading.Tasks.Task](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.Task 'System.Threading.Tasks.Task') representing the asynchronous operation.  
