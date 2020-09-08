#### [StrixMusic.Sdk](./index.md 'index')
### [StrixMusic.Sdk.Interfaces](./StrixMusic-Sdk-Interfaces.md 'StrixMusic.Sdk.Interfaces').[ITrack](./StrixMusic-Sdk-Interfaces-ITrack.md 'StrixMusic.Sdk.Interfaces.ITrack')
## ITrack.Language Property
The language for this track.  
```csharp
System.Globalization.CultureInfo? Language { get; }
```
#### Property Value
[System.Globalization.CultureInfo](https://docs.microsoft.com/en-us/dotnet/api/System.Globalization.CultureInfo 'System.Globalization.CultureInfo')  
### Remarks
If track has no spoken words (instrumental), value is [System.Globalization.CultureInfo.InvariantCulture](https://docs.microsoft.com/en-us/dotnet/api/System.Globalization.CultureInfo.InvariantCulture 'System.Globalization.CultureInfo.InvariantCulture'). If unknown, value is https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null.  
