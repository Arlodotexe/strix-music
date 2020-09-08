#### [StrixMusic.Sdk](./index.md 'index')
### [StrixMusic.Sdk.Interfaces](./StrixMusic-Sdk-Interfaces.md 'StrixMusic.Sdk.Interfaces')
## IPlayableCollectionGroup Interface
Represents multiple playable collections that are grouped together under a single context.  
```csharp
public interface IPlayableCollectionGroup :
IPlayableCollectionBase,
IPlayable,
IPlaylistCollection,
ITrackCollection,
IAlbumCollection,
IArtistCollection
```
Derived  
&#8627; [ILibrary](./StrixMusic-Sdk-Interfaces-ILibrary.md 'StrixMusic.Sdk.Interfaces.ILibrary')  
&#8627; [IRecentlyPlayed](./StrixMusic-Sdk-Interfaces-IRecentlyPlayed.md 'StrixMusic.Sdk.Interfaces.IRecentlyPlayed')  
&#8627; [ISearchResults](./StrixMusic-Sdk-Interfaces-ISearchResults.md 'StrixMusic.Sdk.Interfaces.ISearchResults')  
&#8627; [MergedSearchResults](./StrixMusic-Sdk-MergedWrappers-MergedSearchResults.md 'StrixMusic.Sdk.MergedWrappers.MergedSearchResults')  

Implements [IPlayableCollectionBase](./StrixMusic-Sdk-Interfaces-IPlayableCollectionBase.md 'StrixMusic.Sdk.Interfaces.IPlayableCollectionBase'), [IPlayable](./StrixMusic-Sdk-Interfaces-IPlayable.md 'StrixMusic.Sdk.Interfaces.IPlayable'), [IPlaylistCollection](./StrixMusic-Sdk-Interfaces-IPlaylistCollection.md 'StrixMusic.Sdk.Interfaces.IPlaylistCollection'), [ITrackCollection](./StrixMusic-Sdk-Interfaces-ITrackCollection.md 'StrixMusic.Sdk.Interfaces.ITrackCollection'), [IAlbumCollection](./StrixMusic-Sdk-Interfaces-IAlbumCollection.md 'StrixMusic.Sdk.Interfaces.IAlbumCollection'), [IArtistCollection](./StrixMusic-Sdk-Interfaces-IArtistCollection.md 'StrixMusic.Sdk.Interfaces.IArtistCollection')  
### Properties
- [Children](./StrixMusic-Sdk-Interfaces-IPlayableCollectionGroup-Children.md 'StrixMusic.Sdk.Interfaces.IPlayableCollectionGroup.Children')
- [TotalChildrenCount](./StrixMusic-Sdk-Interfaces-IPlayableCollectionGroup-TotalChildrenCount.md 'StrixMusic.Sdk.Interfaces.IPlayableCollectionGroup.TotalChildrenCount')
### Methods
- [PopulateChildrenAsync(int, int)](./StrixMusic-Sdk-Interfaces-IPlayableCollectionGroup-PopulateChildrenAsync(int_int).md 'StrixMusic.Sdk.Interfaces.IPlayableCollectionGroup.PopulateChildrenAsync(int, int)')
### Events
- [ChildrenChanged](./StrixMusic-Sdk-Interfaces-IPlayableCollectionGroup-ChildrenChanged.md 'StrixMusic.Sdk.Interfaces.IPlayableCollectionGroup.ChildrenChanged')
