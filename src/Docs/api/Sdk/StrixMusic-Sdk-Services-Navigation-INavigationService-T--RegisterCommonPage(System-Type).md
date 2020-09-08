#### [StrixMusic.Sdk](./index.md 'index')
### [StrixMusic.Sdk.Services.Navigation](./StrixMusic-Sdk-Services-Navigation.md 'StrixMusic.Sdk.Services.Navigation').[INavigationService&lt;T&gt;](./StrixMusic-Sdk-Services-Navigation-INavigationService-T-.md 'StrixMusic.Sdk.Services.Navigation.INavigationService&lt;T&gt;')
## INavigationService&lt;T&gt;.RegisterCommonPage(System.Type) Method
Registers a page to have its state cached.  
```csharp
void RegisterCommonPage(System.Type type);
```
#### Parameters
<a name='StrixMusic-Sdk-Services-Navigation-INavigationService-T--RegisterCommonPage(System-Type)-type'></a>
`type` [System.Type](https://docs.microsoft.com/en-us/dotnet/api/System.Type 'System.Type')  
The [System.Type](https://docs.microsoft.com/en-us/dotnet/api/System.Type 'System.Type') of the page cached.  
  
### Remarks
[type](#StrixMusic-Sdk-Services-Navigation-INavigationService-T--RegisterCommonPage(System-Type)-type 'StrixMusic.Sdk.Services.Navigation.INavigationService&lt;T&gt;.RegisterCommonPage(System.Type).type') must inherit [T](./StrixMusic-Sdk-Services-Navigation-INavigationService-T-.md#StrixMusic-Sdk-Services-Navigation-INavigationService-T--T 'StrixMusic.Sdk.Services.Navigation.INavigationService&lt;T&gt;.T').  
            A registered page cannot contain arguments.  
