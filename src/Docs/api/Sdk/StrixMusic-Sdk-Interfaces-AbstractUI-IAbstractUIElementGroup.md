#### [StrixMusic.Sdk](./index.md 'index')
### [StrixMusic.Sdk.Interfaces.AbstractUI](./StrixMusic-Sdk-Interfaces-AbstractUI.md 'StrixMusic.Sdk.Interfaces.AbstractUI')
## IAbstractUIElementGroup Interface
Presents a group of abstracted UI elements to the user.  
```csharp
public interface IAbstractUIElementGroup :
IAbstractUIElement,
IAbstractUIBase
```
Implements [IAbstractUIElement](./StrixMusic-Sdk-Interfaces-AbstractUI-IAbstractUIElement.md 'StrixMusic.Sdk.Interfaces.AbstractUI.IAbstractUIElement'), [IAbstractUIBase](./StrixMusic-Sdk-Interfaces-AbstractUI-IAbstractUIBase.md 'StrixMusic.Sdk.Interfaces.AbstractUI.IAbstractUIBase')  
### Remarks
Recommended to create a new [IAbstractUIElementGroup](./StrixMusic-Sdk-Interfaces-AbstractUI-IAbstractUIElementGroup.md 'StrixMusic.Sdk.Interfaces.AbstractUI.IAbstractUIElementGroup') inside of [Items](./StrixMusic-Sdk-Interfaces-AbstractUI-IAbstractUIElementGroup-Items.md 'StrixMusic.Sdk.Interfaces.AbstractUI.IAbstractUIElementGroup.Items') for each section (Settings, About, etc).  
You can then create [IAbstractUIElementGroup](./StrixMusic-Sdk-Interfaces-AbstractUI-IAbstractUIElementGroup.md 'StrixMusic.Sdk.Interfaces.AbstractUI.IAbstractUIElementGroup')s inside of each of these to group your settings, "About" data, etc.  
### Properties
- [Items](./StrixMusic-Sdk-Interfaces-AbstractUI-IAbstractUIElementGroup-Items.md 'StrixMusic.Sdk.Interfaces.AbstractUI.IAbstractUIElementGroup.Items')
- [PrefferedOrientation](./StrixMusic-Sdk-Interfaces-AbstractUI-IAbstractUIElementGroup-PrefferedOrientation.md 'StrixMusic.Sdk.Interfaces.AbstractUI.IAbstractUIElementGroup.PrefferedOrientation')
