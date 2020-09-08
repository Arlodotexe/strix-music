#### [StrixMusic.Sdk](./index.md 'index')
### [StrixMusic.Sdk.Interfaces](./StrixMusic-Sdk-Interfaces.md 'StrixMusic.Sdk.Interfaces').[IDevice](./StrixMusic-Sdk-Interfaces-IDevice.md 'StrixMusic.Sdk.Interfaces.IDevice')
## IDevice.PlaybackQueue Property
The current playback queue.  
```csharp
System.Collections.Generic.IReadOnlyList<StrixMusic.Sdk.Interfaces.ITrack> PlaybackQueue { get; }
```
#### Property Value
[System.Collections.Generic.IReadOnlyList&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IReadOnlyList-1 'System.Collections.Generic.IReadOnlyList`1')[ITrack](./StrixMusic-Sdk-Interfaces-ITrack.md 'StrixMusic.Sdk.Interfaces.ITrack')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IReadOnlyList-1 'System.Collections.Generic.IReadOnlyList`1')  
### Remarks
When [DeviceType](./StrixMusic-Sdk-Enums-DeviceType.md 'StrixMusic.Sdk.Enums.DeviceType') is [StrixMusic.Sdk.Enums.DeviceType.Remote](https://docs.microsoft.com/en-us/dotnet/api/StrixMusic.Sdk.Enums.DeviceType.Remote 'StrixMusic.Sdk.Enums.DeviceType.Remote'), this will override the internal playback queue.  
