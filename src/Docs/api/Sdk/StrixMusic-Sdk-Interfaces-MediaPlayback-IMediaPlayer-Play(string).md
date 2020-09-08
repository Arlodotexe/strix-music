#### [StrixMusic.Sdk](./index.md 'index')
### [StrixMusic.Sdk.Interfaces.MediaPlayback](./StrixMusic-Sdk-Interfaces-MediaPlayback.md 'StrixMusic.Sdk.Interfaces.MediaPlayback').[IMediaPlayer](./StrixMusic-Sdk-Interfaces-MediaPlayback-IMediaPlayer.md 'StrixMusic.Sdk.Interfaces.MediaPlayback.IMediaPlayer')
## IMediaPlayer.Play(string) Method
Plays a preloaded track.  
```csharp
System.Threading.Tasks.Task Play(string id);
```
#### Parameters
<a name='StrixMusic-Sdk-Interfaces-MediaPlayback-IMediaPlayer-Play(string)-id'></a>
`id` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')  
The ID that was registered during preload.  
  
#### Returns
[System.Threading.Tasks.Task](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.Task 'System.Threading.Tasks.Task')  
A [System.Threading.Tasks.Task](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.Task 'System.Threading.Tasks.Task') representing the asyncronous operation.  
