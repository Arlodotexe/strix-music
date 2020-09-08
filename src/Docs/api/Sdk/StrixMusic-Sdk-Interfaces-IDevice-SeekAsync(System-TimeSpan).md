#### [StrixMusic.Sdk](./index.md 'index')
### [StrixMusic.Sdk.Interfaces](./StrixMusic-Sdk-Interfaces.md 'StrixMusic.Sdk.Interfaces').[IDevice](./StrixMusic-Sdk-Interfaces-IDevice.md 'StrixMusic.Sdk.Interfaces.IDevice')
## IDevice.SeekAsync(System.TimeSpan) Method
Seeks the track to a given timestamp.  
```csharp
System.Threading.Tasks.Task SeekAsync(System.TimeSpan position);
```
#### Parameters
<a name='StrixMusic-Sdk-Interfaces-IDevice-SeekAsync(System-TimeSpan)-position'></a>
`position` [System.TimeSpan](https://docs.microsoft.com/en-us/dotnet/api/System.TimeSpan 'System.TimeSpan')  
Time to seek the song to.  
  
#### Returns
[System.Threading.Tasks.Task](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.Task 'System.Threading.Tasks.Task')  
https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/true if the [ITrack](./StrixMusic-Sdk-Interfaces-ITrack.md 'StrixMusic.Sdk.Interfaces.ITrack') was seeked to successfully, https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/false otherwise.  
