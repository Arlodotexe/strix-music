#### [StrixMusic.Sdk](./index.md 'index')
### [StrixMusic.Sdk.Interfaces](./StrixMusic-Sdk-Interfaces.md 'StrixMusic.Sdk.Interfaces').[ICore](./StrixMusic-Sdk-Interfaces-ICore.md 'StrixMusic.Sdk.Interfaces.ICore')
## ICore.GetSearchResultsAsync(string) Method
Gets search results for a given query.  
```csharp
System.Threading.Tasks.Task<StrixMusic.Sdk.Interfaces.ISearchResults> GetSearchResultsAsync(string query);
```
#### Parameters
<a name='StrixMusic-Sdk-Interfaces-ICore-GetSearchResultsAsync(string)-query'></a>
`query` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')  
The search query.  
  
#### Returns
[System.Threading.Tasks.Task&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.Task-1 'System.Threading.Tasks.Task`1')[ISearchResults](./StrixMusic-Sdk-Interfaces-ISearchResults.md 'StrixMusic.Sdk.Interfaces.ISearchResults')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.Task-1 'System.Threading.Tasks.Task`1')  
A task representing the async operation. Returns [ISearchResults](./StrixMusic-Sdk-Interfaces-ISearchResults.md 'StrixMusic.Sdk.Interfaces.ISearchResults') containing multiple.  
