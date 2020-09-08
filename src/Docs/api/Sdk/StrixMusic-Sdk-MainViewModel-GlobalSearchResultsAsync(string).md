#### [StrixMusic.Sdk](./index.md 'index')
### [StrixMusic.Sdk](./StrixMusic-Sdk.md 'StrixMusic.Sdk').[MainViewModel](./StrixMusic-Sdk-MainViewModel.md 'StrixMusic.Sdk.MainViewModel')
## MainViewModel.GlobalSearchResultsAsync(string) Method
Performs a search on all loaded cores, and loads it into [SearchResults](https://docs.microsoft.com/en-us/dotnet/api/SearchResults 'SearchResults').  
```csharp
public System.Threading.Tasks.Task<StrixMusic.Sdk.Interfaces.ISearchResults> GlobalSearchResultsAsync(string query);
```
#### Parameters
<a name='StrixMusic-Sdk-MainViewModel-GlobalSearchResultsAsync(string)-query'></a>
`query` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')  
The query to search for.  
  
#### Returns
[System.Threading.Tasks.Task&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.Task-1 'System.Threading.Tasks.Task`1')[ISearchResults](./StrixMusic-Sdk-Interfaces-ISearchResults.md 'StrixMusic.Sdk.Interfaces.ISearchResults')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.Task-1 'System.Threading.Tasks.Task`1')  
The merged search results.  
