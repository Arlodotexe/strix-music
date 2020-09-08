#### [StrixMusic.Sdk](./index.md 'index')
### [StrixMusic.Sdk.Interfaces](./StrixMusic-Sdk-Interfaces.md 'StrixMusic.Sdk.Interfaces')
## ITrackCollection Interface
A track collection.  
```csharp
public interface ITrackCollection :
IPlayableCollectionBase,
IPlayable
```
Derived  
&#8627; [IAlbum](./StrixMusic-Sdk-Interfaces-IAlbum.md 'StrixMusic.Sdk.Interfaces.IAlbum')  
&#8627; [IArtist](./StrixMusic-Sdk-Interfaces-IArtist.md 'StrixMusic.Sdk.Interfaces.IArtist')  
&#8627; [ILibrary](./StrixMusic-Sdk-Interfaces-ILibrary.md 'StrixMusic.Sdk.Interfaces.ILibrary')  
&#8627; [IPlayableCollectionGroup](./StrixMusic-Sdk-Interfaces-IPlayableCollectionGroup.md 'StrixMusic.Sdk.Interfaces.IPlayableCollectionGroup')  
&#8627; [IPlaylist](./StrixMusic-Sdk-Interfaces-IPlaylist.md 'StrixMusic.Sdk.Interfaces.IPlaylist')  
&#8627; [IRecentlyPlayed](./StrixMusic-Sdk-Interfaces-IRecentlyPlayed.md 'StrixMusic.Sdk.Interfaces.IRecentlyPlayed')  
&#8627; [ISearchResults](./StrixMusic-Sdk-Interfaces-ISearchResults.md 'StrixMusic.Sdk.Interfaces.ISearchResults')  
&#8627; [MergedSearchResults](./StrixMusic-Sdk-MergedWrappers-MergedSearchResults.md 'StrixMusic.Sdk.MergedWrappers.MergedSearchResults')  

Implements [IPlayableCollectionBase](./StrixMusic-Sdk-Interfaces-IPlayableCollectionBase.md 'StrixMusic.Sdk.Interfaces.IPlayableCollectionBase'), [IPlayable](./StrixMusic-Sdk-Interfaces-IPlayable.md 'StrixMusic.Sdk.Interfaces.IPlayable')  
### Properties
- [TotalTracksCount](./StrixMusic-Sdk-Interfaces-ITrackCollection-TotalTracksCount.md 'StrixMusic.Sdk.Interfaces.ITrackCollection.TotalTracksCount')
- [Tracks](./StrixMusic-Sdk-Interfaces-ITrackCollection-Tracks.md 'StrixMusic.Sdk.Interfaces.ITrackCollection.Tracks')
### Methods
- [PopulateTracksAsync(int, int)](./StrixMusic-Sdk-Interfaces-ITrackCollection-PopulateTracksAsync(int_int).md 'StrixMusic.Sdk.Interfaces.ITrackCollection.PopulateTracksAsync(int, int)')
### Events
- [TracksChanged](./StrixMusic-Sdk-Interfaces-ITrackCollection-TracksChanged.md 'StrixMusic.Sdk.Interfaces.ITrackCollection.TracksChanged')
