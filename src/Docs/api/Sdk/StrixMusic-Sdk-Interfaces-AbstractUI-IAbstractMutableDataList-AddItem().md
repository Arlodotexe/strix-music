#### [StrixMusic.Sdk](./index.md 'index')
### [StrixMusic.Sdk.Interfaces.AbstractUI](./StrixMusic-Sdk-Interfaces-AbstractUI.md 'StrixMusic.Sdk.Interfaces.AbstractUI').[IAbstractMutableDataList](./StrixMusic-Sdk-Interfaces-AbstractUI-IAbstractMutableDataList.md 'StrixMusic.Sdk.Interfaces.AbstractUI.IAbstractMutableDataList')
## IAbstractMutableDataList.AddItem() Method
Called when the user wants to add a new item in the list. Behavior is defined by the core.  
```csharp
System.Threading.Tasks.Task<StrixMusic.Sdk.Interfaces.AbstractUI.IAbstractUIMetadata> AddItem();
```
#### Returns
[System.Threading.Tasks.Task&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.Task-1 'System.Threading.Tasks.Task`1')[IAbstractUIMetadata](./StrixMusic-Sdk-Interfaces-AbstractUI-IAbstractUIMetadata.md 'StrixMusic.Sdk.Interfaces.AbstractUI.IAbstractUIMetadata')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.Task-1 'System.Threading.Tasks.Task`1')  
A [System.Threading.Tasks.Task](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.Task 'System.Threading.Tasks.Task') representing the asynchronous operation. Value is the added item.  
