#### [StrixMusic.Sdk](./index.md 'index')
### [StrixMusic.Sdk.Services.Navigation](./StrixMusic-Sdk-Services-Navigation.md 'StrixMusic.Sdk.Services.Navigation').[INavigationService&lt;T&gt;](./StrixMusic-Sdk-Services-Navigation-INavigationService-T-.md 'StrixMusic.Sdk.Services.Navigation.INavigationService&lt;T&gt;')
## INavigationService&lt;T&gt;.NavigateTo(T, bool) Method
Raises the [NavigationRequested](./StrixMusic-Sdk-Services-Navigation-INavigationService-T--NavigationRequested.md 'StrixMusic.Sdk.Services.Navigation.INavigationService&lt;T&gt;.NavigationRequested') event based on the arguments  
```csharp
void NavigateTo(T type, bool overlay=false);
```
#### Parameters
<a name='StrixMusic-Sdk-Services-Navigation-INavigationService-T--NavigateTo(T_bool)-type'></a>
`type` [T](./StrixMusic-Sdk-Services-Navigation-INavigationService-T-.md#StrixMusic-Sdk-Services-Navigation-INavigationService-T--T 'StrixMusic.Sdk.Services.Navigation.INavigationService&lt;T&gt;.T')  
The page object to navigate to.  
  
<a name='StrixMusic-Sdk-Services-Navigation-INavigationService-T--NavigateTo(T_bool)-overlay'></a>
`overlay` [System.Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean 'System.Boolean')  
Whether or not the page is an overlay.  
  
