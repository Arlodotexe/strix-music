#### [StrixMusic.Sdk](./index.md 'index')
### [StrixMusic.Sdk.Interfaces](./StrixMusic-Sdk-Interfaces.md 'StrixMusic.Sdk.Interfaces').[ICore](./StrixMusic-Sdk-Interfaces-ICore.md 'StrixMusic.Sdk.Interfaces.ICore')
## ICore.GetSearchAutoCompleteAsync(string) Method
Given a query, return suggested completed queries.  
```csharp
System.Threading.Tasks.Task<IAsyncEnumerable<string>> GetSearchAutoCompleteAsync(string query);
```
#### Parameters
<a name='StrixMusic-Sdk-Interfaces-ICore-GetSearchAutoCompleteAsync(string)-query'></a>
`query` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')  
Search query  
  
#### Returns
[System.Threading.Tasks.Task&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.Task-1 'System.Threading.Tasks.Task`1')[System.Collections.Generic.IAsyncEnumerable](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IAsyncEnumerable 'System.Collections.Generic.IAsyncEnumerable')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.Task-1 'System.Threading.Tasks.Task`1')  
Suggested completed queries.  
