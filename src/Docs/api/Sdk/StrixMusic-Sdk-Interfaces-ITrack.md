#### [StrixMusic.Sdk](./index.md 'index')
### [StrixMusic.Sdk.Interfaces](./StrixMusic-Sdk-Interfaces.md 'StrixMusic.Sdk.Interfaces')
## ITrack Interface
Metadata about a track.  
```csharp
public interface ITrack :
IPlayable
```
Derived  
&#8627; [MergedTrack](./StrixMusic-Sdk-MergedWrappers-MergedTrack.md 'StrixMusic.Sdk.MergedWrappers.MergedTrack')  

Implements [IPlayable](./StrixMusic-Sdk-Interfaces-IPlayable.md 'StrixMusic.Sdk.Interfaces.IPlayable')  
### Properties
- [Album](./StrixMusic-Sdk-Interfaces-ITrack-Album.md 'StrixMusic.Sdk.Interfaces.ITrack.Album')
- [Artists](./StrixMusic-Sdk-Interfaces-ITrack-Artists.md 'StrixMusic.Sdk.Interfaces.ITrack.Artists')
- [DatePublished](./StrixMusic-Sdk-Interfaces-ITrack-DatePublished.md 'StrixMusic.Sdk.Interfaces.ITrack.DatePublished')
- [Genres](./StrixMusic-Sdk-Interfaces-ITrack-Genres.md 'StrixMusic.Sdk.Interfaces.ITrack.Genres')
- [IsChangeAlbumAsyncSupported](./StrixMusic-Sdk-Interfaces-ITrack-IsChangeAlbumAsyncSupported.md 'StrixMusic.Sdk.Interfaces.ITrack.IsChangeAlbumAsyncSupported')
- [IsChangeArtistsAsyncSupported](./StrixMusic-Sdk-Interfaces-ITrack-IsChangeArtistsAsyncSupported.md 'StrixMusic.Sdk.Interfaces.ITrack.IsChangeArtistsAsyncSupported')
- [IsChangeDatePublishedAsyncSupported](./StrixMusic-Sdk-Interfaces-ITrack-IsChangeDatePublishedAsyncSupported.md 'StrixMusic.Sdk.Interfaces.ITrack.IsChangeDatePublishedAsyncSupported')
- [IsChangeGenresAsyncSupported](./StrixMusic-Sdk-Interfaces-ITrack-IsChangeGenresAsyncSupported.md 'StrixMusic.Sdk.Interfaces.ITrack.IsChangeGenresAsyncSupported')
- [IsChangeIsExplicitAsyncSupported](./StrixMusic-Sdk-Interfaces-ITrack-IsChangeIsExplicitAsyncSupported.md 'StrixMusic.Sdk.Interfaces.ITrack.IsChangeIsExplicitAsyncSupported')
- [IsChangeLanguageAsyncSupported](./StrixMusic-Sdk-Interfaces-ITrack-IsChangeLanguageAsyncSupported.md 'StrixMusic.Sdk.Interfaces.ITrack.IsChangeLanguageAsyncSupported')
- [IsChangeLyricsAsyncSupported](./StrixMusic-Sdk-Interfaces-ITrack-IsChangeLyricsAsyncSupported.md 'StrixMusic.Sdk.Interfaces.ITrack.IsChangeLyricsAsyncSupported')
- [IsChangeTrackNumberAsyncSupported](./StrixMusic-Sdk-Interfaces-ITrack-IsChangeTrackNumberAsyncSupported.md 'StrixMusic.Sdk.Interfaces.ITrack.IsChangeTrackNumberAsyncSupported')
- [IsExplicit](./StrixMusic-Sdk-Interfaces-ITrack-IsExplicit.md 'StrixMusic.Sdk.Interfaces.ITrack.IsExplicit')
- [Language](./StrixMusic-Sdk-Interfaces-ITrack-Language.md 'StrixMusic.Sdk.Interfaces.ITrack.Language')
- [Lyrics](./StrixMusic-Sdk-Interfaces-ITrack-Lyrics.md 'StrixMusic.Sdk.Interfaces.ITrack.Lyrics')
- [PlayCount](./StrixMusic-Sdk-Interfaces-ITrack-PlayCount.md 'StrixMusic.Sdk.Interfaces.ITrack.PlayCount')
- [RelatedItems](./StrixMusic-Sdk-Interfaces-ITrack-RelatedItems.md 'StrixMusic.Sdk.Interfaces.ITrack.RelatedItems')
- [TrackNumber](./StrixMusic-Sdk-Interfaces-ITrack-TrackNumber.md 'StrixMusic.Sdk.Interfaces.ITrack.TrackNumber')
- [Type](./StrixMusic-Sdk-Interfaces-ITrack-Type.md 'StrixMusic.Sdk.Interfaces.ITrack.Type')
### Methods
- [ChangeAlbumAsync(StrixMusic.Sdk.Interfaces.IAlbum?)](./StrixMusic-Sdk-Interfaces-ITrack-ChangeAlbumAsync(StrixMusic-Sdk-Interfaces-IAlbum-).md 'StrixMusic.Sdk.Interfaces.ITrack.ChangeAlbumAsync(StrixMusic.Sdk.Interfaces.IAlbum?)')
- [ChangeArtistsAsync(System.Collections.Generic.IReadOnlyList&lt;StrixMusic.Sdk.Interfaces.IArtist&gt;?)](./StrixMusic-Sdk-Interfaces-ITrack-ChangeArtistsAsync(System-Collections-Generic-IReadOnlyList-StrixMusic-Sdk-Interfaces-IArtist--).md 'StrixMusic.Sdk.Interfaces.ITrack.ChangeArtistsAsync(System.Collections.Generic.IReadOnlyList&lt;StrixMusic.Sdk.Interfaces.IArtist&gt;?)')
- [ChangeDatePublishedAsync(System.DateTime)](./StrixMusic-Sdk-Interfaces-ITrack-ChangeDatePublishedAsync(System-DateTime).md 'StrixMusic.Sdk.Interfaces.ITrack.ChangeDatePublishedAsync(System.DateTime)')
- [ChangeGenresAsync(System.Collections.Generic.IReadOnlyList&lt;string&gt;?)](./StrixMusic-Sdk-Interfaces-ITrack-ChangeGenresAsync(System-Collections-Generic-IReadOnlyList-string--).md 'StrixMusic.Sdk.Interfaces.ITrack.ChangeGenresAsync(System.Collections.Generic.IReadOnlyList&lt;string&gt;?)')
- [ChangeIsExplicitAsync(bool)](./StrixMusic-Sdk-Interfaces-ITrack-ChangeIsExplicitAsync(bool).md 'StrixMusic.Sdk.Interfaces.ITrack.ChangeIsExplicitAsync(bool)')
- [ChangeLanguageAsync(System.Globalization.CultureInfo)](./StrixMusic-Sdk-Interfaces-ITrack-ChangeLanguageAsync(System-Globalization-CultureInfo).md 'StrixMusic.Sdk.Interfaces.ITrack.ChangeLanguageAsync(System.Globalization.CultureInfo)')
- [ChangeLyricsAsync(StrixMusic.Sdk.Interfaces.ILyrics?)](./StrixMusic-Sdk-Interfaces-ITrack-ChangeLyricsAsync(StrixMusic-Sdk-Interfaces-ILyrics-).md 'StrixMusic.Sdk.Interfaces.ITrack.ChangeLyricsAsync(StrixMusic.Sdk.Interfaces.ILyrics?)')
- [ChangeTrackNumberAsync(int?)](./StrixMusic-Sdk-Interfaces-ITrack-ChangeTrackNumberAsync(int-).md 'StrixMusic.Sdk.Interfaces.ITrack.ChangeTrackNumberAsync(int?)')
### Events
- [AlbumChanged](./StrixMusic-Sdk-Interfaces-ITrack-AlbumChanged.md 'StrixMusic.Sdk.Interfaces.ITrack.AlbumChanged')
- [ArtistsChanged](./StrixMusic-Sdk-Interfaces-ITrack-ArtistsChanged.md 'StrixMusic.Sdk.Interfaces.ITrack.ArtistsChanged')
- [DatePublishedChanged](./StrixMusic-Sdk-Interfaces-ITrack-DatePublishedChanged.md 'StrixMusic.Sdk.Interfaces.ITrack.DatePublishedChanged')
- [GenresChanged](./StrixMusic-Sdk-Interfaces-ITrack-GenresChanged.md 'StrixMusic.Sdk.Interfaces.ITrack.GenresChanged')
- [IsExplicitChanged](./StrixMusic-Sdk-Interfaces-ITrack-IsExplicitChanged.md 'StrixMusic.Sdk.Interfaces.ITrack.IsExplicitChanged')
- [LanguageChanged](./StrixMusic-Sdk-Interfaces-ITrack-LanguageChanged.md 'StrixMusic.Sdk.Interfaces.ITrack.LanguageChanged')
- [LyricsChanged](./StrixMusic-Sdk-Interfaces-ITrack-LyricsChanged.md 'StrixMusic.Sdk.Interfaces.ITrack.LyricsChanged')
- [PlayCountChanged](./StrixMusic-Sdk-Interfaces-ITrack-PlayCountChanged.md 'StrixMusic.Sdk.Interfaces.ITrack.PlayCountChanged')
- [TrackNumberChanged](./StrixMusic-Sdk-Interfaces-ITrack-TrackNumberChanged.md 'StrixMusic.Sdk.Interfaces.ITrack.TrackNumberChanged')
