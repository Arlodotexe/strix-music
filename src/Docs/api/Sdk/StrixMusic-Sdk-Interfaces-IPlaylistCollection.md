#### [StrixMusic.Sdk](./index.md 'index')
### [StrixMusic.Sdk.Interfaces](./StrixMusic-Sdk-Interfaces.md 'StrixMusic.Sdk.Interfaces')
## IPlaylistCollection Interface
A playlist collection.  
```csharp
public interface IPlaylistCollection :
IPlayableCollectionBase,
IPlayable
```
Derived  
&#8627; [ILibrary](./StrixMusic-Sdk-Interfaces-ILibrary.md 'StrixMusic.Sdk.Interfaces.ILibrary')  
&#8627; [IPlayableCollectionGroup](./StrixMusic-Sdk-Interfaces-IPlayableCollectionGroup.md 'StrixMusic.Sdk.Interfaces.IPlayableCollectionGroup')  
&#8627; [IRecentlyPlayed](./StrixMusic-Sdk-Interfaces-IRecentlyPlayed.md 'StrixMusic.Sdk.Interfaces.IRecentlyPlayed')  
&#8627; [ISearchResults](./StrixMusic-Sdk-Interfaces-ISearchResults.md 'StrixMusic.Sdk.Interfaces.ISearchResults')  
&#8627; [MergedSearchResults](./StrixMusic-Sdk-MergedWrappers-MergedSearchResults.md 'StrixMusic.Sdk.MergedWrappers.MergedSearchResults')  

Implements [IPlayableCollectionBase](./StrixMusic-Sdk-Interfaces-IPlayableCollectionBase.md 'StrixMusic.Sdk.Interfaces.IPlayableCollectionBase'), [IPlayable](./StrixMusic-Sdk-Interfaces-IPlayable.md 'StrixMusic.Sdk.Interfaces.IPlayable')  
### Properties
- [Playlists](./StrixMusic-Sdk-Interfaces-IPlaylistCollection-Playlists.md 'StrixMusic.Sdk.Interfaces.IPlaylistCollection.Playlists')
- [TotalPlaylistCount](./StrixMusic-Sdk-Interfaces-IPlaylistCollection-TotalPlaylistCount.md 'StrixMusic.Sdk.Interfaces.IPlaylistCollection.TotalPlaylistCount')
### Methods
- [PopulatePlaylistsAsync(int, int)](./StrixMusic-Sdk-Interfaces-IPlaylistCollection-PopulatePlaylistsAsync(int_int).md 'StrixMusic.Sdk.Interfaces.IPlaylistCollection.PopulatePlaylistsAsync(int, int)')
### Events
- [PlaylistsChanged](./StrixMusic-Sdk-Interfaces-IPlaylistCollection-PlaylistsChanged.md 'StrixMusic.Sdk.Interfaces.IPlaylistCollection.PlaylistsChanged')
