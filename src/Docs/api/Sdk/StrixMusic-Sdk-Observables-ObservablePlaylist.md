#### [StrixMusic.Sdk](./index.md 'index')
### [StrixMusic.Sdk.Observables](./StrixMusic-Sdk-Observables.md 'StrixMusic.Sdk.Observables')
## ObservablePlaylist Class
A bindable wrapper for [IPlaylist](./StrixMusic-Sdk-Interfaces-IPlaylist.md 'StrixMusic.Sdk.Interfaces.IPlaylist').  
```csharp
public class ObservablePlaylist : ObservableMergeableObject<IPlaylist>
```
Inheritance [Microsoft.Toolkit.Mvvm.ComponentModel.ObservableObject](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.Toolkit.Mvvm.ComponentModel.ObservableObject 'Microsoft.Toolkit.Mvvm.ComponentModel.ObservableObject') &#129106; [StrixMusic.Sdk.Observables.ObservableMergeableObject&lt;](./StrixMusic-Sdk-Observables-ObservableMergeableObject-T-.md 'StrixMusic.Sdk.Observables.ObservableMergeableObject&lt;T&gt;')[IPlaylist](./StrixMusic-Sdk-Interfaces-IPlaylist.md 'StrixMusic.Sdk.Interfaces.IPlaylist')[&gt;](./StrixMusic-Sdk-Observables-ObservableMergeableObject-T-.md 'StrixMusic.Sdk.Observables.ObservableMergeableObject&lt;T&gt;') &#129106; ObservablePlaylist  
### Constructors
- [ObservablePlaylist(StrixMusic.Sdk.Interfaces.IPlaylist)](./StrixMusic-Sdk-Observables-ObservablePlaylist-ObservablePlaylist(StrixMusic-Sdk-Interfaces-IPlaylist).md 'StrixMusic.Sdk.Observables.ObservablePlaylist.ObservablePlaylist(StrixMusic.Sdk.Interfaces.IPlaylist)')
### Properties
- [ChangeDescriptionAsyncCommand](./StrixMusic-Sdk-Observables-ObservablePlaylist-ChangeDescriptionAsyncCommand.md 'StrixMusic.Sdk.Observables.ObservablePlaylist.ChangeDescriptionAsyncCommand')
- [ChangeDurationAsyncCommand](./StrixMusic-Sdk-Observables-ObservablePlaylist-ChangeDurationAsyncCommand.md 'StrixMusic.Sdk.Observables.ObservablePlaylist.ChangeDurationAsyncCommand')
- [ChangeImagesAsyncCommand](./StrixMusic-Sdk-Observables-ObservablePlaylist-ChangeImagesAsyncCommand.md 'StrixMusic.Sdk.Observables.ObservablePlaylist.ChangeImagesAsyncCommand')
- [ChangeNameAsyncCommand](./StrixMusic-Sdk-Observables-ObservablePlaylist-ChangeNameAsyncCommand.md 'StrixMusic.Sdk.Observables.ObservablePlaylist.ChangeNameAsyncCommand')
- [Description](./StrixMusic-Sdk-Observables-ObservablePlaylist-Description.md 'StrixMusic.Sdk.Observables.ObservablePlaylist.Description')
- [Duration](./StrixMusic-Sdk-Observables-ObservablePlaylist-Duration.md 'StrixMusic.Sdk.Observables.ObservablePlaylist.Duration')
- [Id](./StrixMusic-Sdk-Observables-ObservablePlaylist-Id.md 'StrixMusic.Sdk.Observables.ObservablePlaylist.Id')
- [Images](./StrixMusic-Sdk-Observables-ObservablePlaylist-Images.md 'StrixMusic.Sdk.Observables.ObservablePlaylist.Images')
- [Name](./StrixMusic-Sdk-Observables-ObservablePlaylist-Name.md 'StrixMusic.Sdk.Observables.ObservablePlaylist.Name')
- [Owner](./StrixMusic-Sdk-Observables-ObservablePlaylist-Owner.md 'StrixMusic.Sdk.Observables.ObservablePlaylist.Owner')
- [PauseAsyncCommand](./StrixMusic-Sdk-Observables-ObservablePlaylist-PauseAsyncCommand.md 'StrixMusic.Sdk.Observables.ObservablePlaylist.PauseAsyncCommand')
- [PlayAsyncCommand](./StrixMusic-Sdk-Observables-ObservablePlaylist-PlayAsyncCommand.md 'StrixMusic.Sdk.Observables.ObservablePlaylist.PlayAsyncCommand')
- [PlaybackState](./StrixMusic-Sdk-Observables-ObservablePlaylist-PlaybackState.md 'StrixMusic.Sdk.Observables.ObservablePlaylist.PlaybackState')
- [RelatedItems](./StrixMusic-Sdk-Observables-ObservablePlaylist-RelatedItems.md 'StrixMusic.Sdk.Observables.ObservablePlaylist.RelatedItems')
- [SourceCore](./StrixMusic-Sdk-Observables-ObservablePlaylist-SourceCore.md 'StrixMusic.Sdk.Observables.ObservablePlaylist.SourceCore')
- [TotalTracksCount](./StrixMusic-Sdk-Observables-ObservablePlaylist-TotalTracksCount.md 'StrixMusic.Sdk.Observables.ObservablePlaylist.TotalTracksCount')
- [Tracks](./StrixMusic-Sdk-Observables-ObservablePlaylist-Tracks.md 'StrixMusic.Sdk.Observables.ObservablePlaylist.Tracks')
- [Url](./StrixMusic-Sdk-Observables-ObservablePlaylist-Url.md 'StrixMusic.Sdk.Observables.ObservablePlaylist.Url')
### Methods
- [ChangeDescriptionAsync(string?)](./StrixMusic-Sdk-Observables-ObservablePlaylist-ChangeDescriptionAsync(string-).md 'StrixMusic.Sdk.Observables.ObservablePlaylist.ChangeDescriptionAsync(string?)')
- [ChangeDurationAsync(System.TimeSpan)](./StrixMusic-Sdk-Observables-ObservablePlaylist-ChangeDurationAsync(System-TimeSpan).md 'StrixMusic.Sdk.Observables.ObservablePlaylist.ChangeDurationAsync(System.TimeSpan)')
- [ChangeImagesAsync(System.Collections.Generic.IReadOnlyList&lt;StrixMusic.Sdk.Interfaces.IImage&gt;)](./StrixMusic-Sdk-Observables-ObservablePlaylist-ChangeImagesAsync(System-Collections-Generic-IReadOnlyList-StrixMusic-Sdk-Interfaces-IImage-).md 'StrixMusic.Sdk.Observables.ObservablePlaylist.ChangeImagesAsync(System.Collections.Generic.IReadOnlyList&lt;StrixMusic.Sdk.Interfaces.IImage&gt;)')
- [ChangeNameAsync(string)](./StrixMusic-Sdk-Observables-ObservablePlaylist-ChangeNameAsync(string).md 'StrixMusic.Sdk.Observables.ObservablePlaylist.ChangeNameAsync(string)')
- [PauseAsync()](./StrixMusic-Sdk-Observables-ObservablePlaylist-PauseAsync().md 'StrixMusic.Sdk.Observables.ObservablePlaylist.PauseAsync()')
- [PlayAsync()](./StrixMusic-Sdk-Observables-ObservablePlaylist-PlayAsync().md 'StrixMusic.Sdk.Observables.ObservablePlaylist.PlayAsync()')
- [PopulateTracksAsync(int, int)](./StrixMusic-Sdk-Observables-ObservablePlaylist-PopulateTracksAsync(int_int).md 'StrixMusic.Sdk.Observables.ObservablePlaylist.PopulateTracksAsync(int, int)')
### Events
- [DescriptionChanged](./StrixMusic-Sdk-Observables-ObservablePlaylist-DescriptionChanged.md 'StrixMusic.Sdk.Observables.ObservablePlaylist.DescriptionChanged')
- [ImagesChanged](./StrixMusic-Sdk-Observables-ObservablePlaylist-ImagesChanged.md 'StrixMusic.Sdk.Observables.ObservablePlaylist.ImagesChanged')
- [NameChanged](./StrixMusic-Sdk-Observables-ObservablePlaylist-NameChanged.md 'StrixMusic.Sdk.Observables.ObservablePlaylist.NameChanged')
- [PlaybackStateChanged](./StrixMusic-Sdk-Observables-ObservablePlaylist-PlaybackStateChanged.md 'StrixMusic.Sdk.Observables.ObservablePlaylist.PlaybackStateChanged')
- [TracksChanged](./StrixMusic-Sdk-Observables-ObservablePlaylist-TracksChanged.md 'StrixMusic.Sdk.Observables.ObservablePlaylist.TracksChanged')
- [UrlChanged](./StrixMusic-Sdk-Observables-ObservablePlaylist-UrlChanged.md 'StrixMusic.Sdk.Observables.ObservablePlaylist.UrlChanged')
