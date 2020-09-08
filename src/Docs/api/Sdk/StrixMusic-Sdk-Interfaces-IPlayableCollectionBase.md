#### [StrixMusic.Sdk](./index.md 'index')
### [StrixMusic.Sdk.Interfaces](./StrixMusic-Sdk-Interfaces.md 'StrixMusic.Sdk.Interfaces')
## IPlayableCollectionBase Interface
Interface representing a collection of playable items.  
```csharp
public interface IPlayableCollectionBase :
IPlayable
```
Derived  
&#8627; [IAlbum](./StrixMusic-Sdk-Interfaces-IAlbum.md 'StrixMusic.Sdk.Interfaces.IAlbum')  
&#8627; [IAlbumCollection](./StrixMusic-Sdk-Interfaces-IAlbumCollection.md 'StrixMusic.Sdk.Interfaces.IAlbumCollection')  
&#8627; [IArtist](./StrixMusic-Sdk-Interfaces-IArtist.md 'StrixMusic.Sdk.Interfaces.IArtist')  
&#8627; [IArtistCollection](./StrixMusic-Sdk-Interfaces-IArtistCollection.md 'StrixMusic.Sdk.Interfaces.IArtistCollection')  
&#8627; [ILibrary](./StrixMusic-Sdk-Interfaces-ILibrary.md 'StrixMusic.Sdk.Interfaces.ILibrary')  
&#8627; [IPlayableCollectionGroup](./StrixMusic-Sdk-Interfaces-IPlayableCollectionGroup.md 'StrixMusic.Sdk.Interfaces.IPlayableCollectionGroup')  
&#8627; [IPlaylist](./StrixMusic-Sdk-Interfaces-IPlaylist.md 'StrixMusic.Sdk.Interfaces.IPlaylist')  
&#8627; [IPlaylistCollection](./StrixMusic-Sdk-Interfaces-IPlaylistCollection.md 'StrixMusic.Sdk.Interfaces.IPlaylistCollection')  
&#8627; [IRecentlyPlayed](./StrixMusic-Sdk-Interfaces-IRecentlyPlayed.md 'StrixMusic.Sdk.Interfaces.IRecentlyPlayed')  
&#8627; [ISearchResults](./StrixMusic-Sdk-Interfaces-ISearchResults.md 'StrixMusic.Sdk.Interfaces.ISearchResults')  
&#8627; [ITrackCollection](./StrixMusic-Sdk-Interfaces-ITrackCollection.md 'StrixMusic.Sdk.Interfaces.ITrackCollection')  
&#8627; [MergedSearchResults](./StrixMusic-Sdk-MergedWrappers-MergedSearchResults.md 'StrixMusic.Sdk.MergedWrappers.MergedSearchResults')  

Implements [IPlayable](./StrixMusic-Sdk-Interfaces-IPlayable.md 'StrixMusic.Sdk.Interfaces.IPlayable')  
### Remarks
No https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/class should ever directly implement this interface. The items in this collection, the count, and the method for getting them are defined in a child https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/interface.  
