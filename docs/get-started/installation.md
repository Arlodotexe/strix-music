> [!NOTE] 
> The Strix Music SDK will work anywhere .NET Standard 2.0 is supported.

# Installation methods

Assuming you have your nuget sources configured, use any of these installation methods.

### Visual Studio
1. Open the Nuget Package Manager for the project you want to install to.
2. Search for "Strix" or "StrixMusic".
3. Select the `StrixMusic.Sdk` package.
4. Install the latest version.

### dotnet CLI
1. Open a command line in the directory for the project you want to install the SDK to.
2. Run `dotnet add package StrixMusic.Sdk`.
3. Restore nuget packages if needed.

> [!TIP] 
> If you don't have the dotnet cli or have the wrong version, you can download it automatically and execute any command using `.\dotnet.ps1` in the `build-scripts` directory. This command will download and extract from official sources, or download archived binaries from ipfs as a fallback if you don't have an internet connection.


### PowerShell

1. Open PowerShell in the directory for the project you want to install the SDK to.
2. In the project directory, run `Install-Package StrixMusic.Sdk`.
3. Restore nuget packages if needed.

### Manually
For advanced scenarios, or when all else fails.

1. Open the `.csproj` for the project you want to install the SDK to.
2. Add the line `<PackageReference Include="StrixMusic.Sdk" Version="0.0.0" />`, replacing `0.0.0` with your desired version.
3. Restore your nuget packages.

# Installation sources

### Direct from NuGet
This option is recommend when available. The latest versions are automatically published to [nuget.org](https://www.nuget.org/packages/StrixMusic.Sdk/).

If you have NuGet configured as a package source, you can use any of the above installation methods with no additional work.

If you lose access to NuGet for any reason, use one of the below.

### Manual via .nupkg
1. Directly download and use the [.nupkg](../../sdk/nupkg) file.
2. Add the file to your global package folder
  - Windows: `%userprofile%\.nuget\packages`
  - Mac/Linux: `~/.nuget/packages`

### When isolated from the internet
If you find yourself isolated from the rest of the internet, there's still an option to set up the SDK in a new project.

To do this, you must meet these requirements:

- You've set up a local IPFS node on your current machine.
- You've cached our [`/dependencies/.nuget`](../../dependencies/.nuget) folder and [`.nupkg`](../../sdk/nupkg) file on another machine, either by pinning to your local node, importing the website, or by having done this once already.
- Your current machine can access the other machine. (local network, limited WAN, etc)

If you meet the above requirements, simply:
1. Download the SDK as usual by following the "Manual via .nupkg" instructions above.
2. To get all dependencies, from the `build-script` folder, run `.\GatherDependencies.ps1 -dependencyName nupkg -outputPath /temp/download/path/`.
3. Copy the downloaded `.nupkg` files into your global package folder.

