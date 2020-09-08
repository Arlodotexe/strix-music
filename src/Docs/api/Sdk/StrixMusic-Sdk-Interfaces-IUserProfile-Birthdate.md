#### [StrixMusic.Sdk](./index.md 'index')
### [StrixMusic.Sdk.Interfaces](./StrixMusic-Sdk-Interfaces.md 'StrixMusic.Sdk.Interfaces').[IUserProfile](./StrixMusic-Sdk-Interfaces-IUserProfile.md 'StrixMusic.Sdk.Interfaces.IUserProfile')
## IUserProfile.Birthdate Property
The [System.DateTime](https://docs.microsoft.com/en-us/dotnet/api/System.DateTime 'System.DateTime') the user was born.  
```csharp
System.DateTime? Birthdate { get; }
```
#### Property Value
[System.Nullable&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.Nullable-1 'System.Nullable`1')[System.DateTime](https://docs.microsoft.com/en-us/dotnet/api/System.DateTime 'System.DateTime')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Nullable-1 'System.Nullable`1')  
### Remarks
If missing data, replace the day, month and/or year with part of 1/1/1970.  
