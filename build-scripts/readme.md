Build scripts
---

In our [Pledges](../README.md#permanent), we promised:

> The entire project (docs, website, build process, dependencies, SDK, app, etc) are perpetually preserved in every released binary and on our website, all hosted on [IPFS](https://ipfs.io/). If anyone has these things, you'll be able to access it over IPFS.

To make this happen, we've made our CI process available to run locally as a set of PowerShell scripts.

---

### `GatherDependencies.ps1`
Downloads the latest version of any or all dependencies in our build process, or a fallback from IPFS with the last published version if the servers are down.

Syntax:
```
GatherDependencies.ps1 [-outputPath] <string> [[-dependencySourcesPath] <string>] [[-dependencyName] <string>] [-latestOnly] [-fallbackOnly] [<CommonParameters>]
```

<!-- 
Generated via command:

(get-command .\GatherDependencies.ps1).ParameterSets | select -ExpandProperty parameters | ft Name, ParameterType, HelpMessage, IsMandatory
 -->

Parameters
```
Name                  ParameterType                                 HelpMessage                                            IsMandatory
----                  -------------                                 -----------                                            -----------
outputPath            System.String                                 The path where dependencies are downloaded to on disk.        True
dependencySourcesPath System.String                                 The path to a dependencies.json file                         False
dependencyName        System.String                                 The name of a specific dependency to download.               False
latestOnly            System.Management.Automation.SwitchParameter  Don't use any fallbacks if a download fails.                 False
fallbackOnly          System.Management.Automation.SwitchParameter  Only download the fallback dependencies.                     False
```

---
### `GenerateDocs.ps1`
Sets up the DocFX binaries to build the project and generate the code documentation website.

This is supported on both Windows and Linux, but Linux requires you to install `mono-devel` (direct from mono, not your OS vendor). We are not yet able to supply a snapshot backup of this dependency.

Syntax:
```
GenerateDocs.ps1 [-outputPath] <string> [-docfxArchivePath] <string> [-autoInstallMissing] [<CommonParameters>]
```

<!-- 
Generated via command:

(get-command .\GenerateDocs.ps1).ParameterSets | select -ExpandProperty parameters | ft Name, ParameterType, HelpMessage, IsMandatory
 -->

Parameters
```
Name                ParameterType                                 HelpMessage                                                                               IsMandatory
----                -------------                                 -----------                                                                               -----------
outputPath          System.String                                 The path where dependencies are unzipped on disk.                                                True
docfxArchivePath    System.String                                 The path to a zip file containing a docfx release                                                True
autoInstallMissing  System.Management.Automation.SwitchParameter  If true, the script will auto-install missing dependencies.                                     False
```