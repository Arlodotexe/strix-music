#### [StrixMusic.Sdk](./index.md 'index')
### [StrixMusic.Sdk.Services.Navigation](./StrixMusic-Sdk-Services-Navigation.md 'StrixMusic.Sdk.Services.Navigation').[INavigationService&lt;T&gt;](./StrixMusic-Sdk-Services-Navigation-INavigationService-T-.md 'StrixMusic.Sdk.Services.Navigation.INavigationService&lt;T&gt;')
## INavigationService&lt;T&gt;.NavigateTo(System.Type, bool, object?) Method
Raises the [NavigationRequested](./StrixMusic-Sdk-Services-Navigation-INavigationService-T--NavigationRequested.md 'StrixMusic.Sdk.Services.Navigation.INavigationService&lt;T&gt;.NavigationRequested') event based on the arguments  
```csharp
void NavigateTo(System.Type type, bool overlay=false, object? args=null);
```
#### Parameters
<a name='StrixMusic-Sdk-Services-Navigation-INavigationService-T--NavigateTo(System-Type_bool_object-)-type'></a>
`type` [System.Type](https://docs.microsoft.com/en-us/dotnet/api/System.Type 'System.Type')  
The type of the page to navigate to.  
  
<a name='StrixMusic-Sdk-Services-Navigation-INavigationService-T--NavigateTo(System-Type_bool_object-)-overlay'></a>
`overlay` [System.Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean 'System.Boolean')  
Whether or not the page is an overlay.  
  
<a name='StrixMusic-Sdk-Services-Navigation-INavigationService-T--NavigateTo(System-Type_bool_object-)-args'></a>
`args` [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object')  
The arguments for creating the page object.  
  
