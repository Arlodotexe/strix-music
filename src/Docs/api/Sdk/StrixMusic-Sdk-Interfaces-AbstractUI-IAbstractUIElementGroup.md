#### [StrixMusic.Sdk](./index.md 'index')
### [StrixMusic.Sdk.Interfaces.AbstractUI](./StrixMusic-Sdk-Interfaces-AbstractUI.md 'StrixMusic.Sdk.Interfaces.AbstractUI')
## IAbstractUIElementGroup Interface
Presents a group of abstracted UI elements to the user.  
```csharp
public interface IAbstractUIElementGroup :
IAbstractUIMetadata
```
Implements [IAbstractUIMetadata](./StrixMusic-Sdk-Interfaces-AbstractUI-IAbstractUIMetadata.md 'StrixMusic.Sdk.Interfaces.AbstractUI.IAbstractUIMetadata')  
### Remarks
Recommended to create a new [IAbstractUIElementGroup](./StrixMusic-Sdk-Interfaces-AbstractUI-IAbstractUIElementGroup.md 'StrixMusic.Sdk.Interfaces.AbstractUI.IAbstractUIElementGroup') inside of [Items](./StrixMusic-Sdk-Interfaces-AbstractUI-IAbstractUIElementGroup-Items.md 'StrixMusic.Sdk.Interfaces.AbstractUI.IAbstractUIElementGroup.Items') for each section (Settings, About, etc). You can then group your settings further by sorting them with  
### Properties
- [Items](./StrixMusic-Sdk-Interfaces-AbstractUI-IAbstractUIElementGroup-Items.md 'StrixMusic.Sdk.Interfaces.AbstractUI.IAbstractUIElementGroup.Items')
