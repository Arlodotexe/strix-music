#### [StrixMusic.Sdk](./index.md 'index')
### [StrixMusic.Sdk.Interfaces](./StrixMusic-Sdk-Interfaces.md 'StrixMusic.Sdk.Interfaces')
## IArtistCollection Interface
A collection of artists.  
```csharp
public interface IArtistCollection :
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
- [Artists](./StrixMusic-Sdk-Interfaces-IArtistCollection-Artists.md 'StrixMusic.Sdk.Interfaces.IArtistCollection.Artists')
- [TotalArtistsCount](./StrixMusic-Sdk-Interfaces-IArtistCollection-TotalArtistsCount.md 'StrixMusic.Sdk.Interfaces.IArtistCollection.TotalArtistsCount')
### Methods
- [PopulateArtistsAsync(int, int)](./StrixMusic-Sdk-Interfaces-IArtistCollection-PopulateArtistsAsync(int_int).md 'StrixMusic.Sdk.Interfaces.IArtistCollection.PopulateArtistsAsync(int, int)')
### Events
- [ArtistsChanged](./StrixMusic-Sdk-Interfaces-IArtistCollection-ArtistsChanged.md 'StrixMusic.Sdk.Interfaces.IArtistCollection.ArtistsChanged')
