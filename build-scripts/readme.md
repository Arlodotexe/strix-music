# Strix Music
## Build scripts

Strix Music is part of the permanant web. We believe in preserving our software, making sure it functions the same now as it would decades from now.

Our build process reflects that. Cross platform PowerShell scripts are used to automate all the things we would have done by hand, and [IPFS](https://ipfs.io/) is used to store and distribute as much of our dependency chain as possible.

IPFS is self-described as:
> A peer-to-peer hypermedia protocol
> designed to preserve and grow humanity's knowledge
> by making the web upgradeable, resilient, and more open.

## Making a release?
Before getting started, if you plan on using these scripts to publish your own release, you'll need to take responsibility for hosting your snapshotted dependencies on your own IPFS node (or use a third party like Pinata) to make sure your developers can access them.

## Getting started
When developers download the dependencies to their IPFS node, they temporarily host for the data to other nodes who want it to access it. The more devs holding onto the data, the faster and more available the dependencies will become.

Finally, if you have a machine where you used this recently (even once), you can set up a brand new machine on your local network to do it again, without internet access, and even if the original links all 404.

Not even an apocalypse could ruin our hard work. 

## Snapshot scripts
These are scripts which download as much of our dependency chain as possible, uploads it to IPFS, captures the CID, and records it in a `dependency.json` file for others to find later.

- **SnapshotDotnetSdk.ps1**
  - Uses the official dotnet installer scripts to download the dotnet runtime for a select combination of OSs and architectures.
  - The version of dotnet that is downloaded is defined by the project in [global.json](../global.json).
  - Downloaded dependencies ARE imported to IPFS and recorded in [dependencies.json](dependencies.json).

- **SnapshotNugetPackages.ps1**
  - Uses [dotnet.ps1](dotnet.ps1) to automatically download the needed .NET SDK for the running OS and Architecture, and uses it to restore dependencies for the given project.
  - For the best experience, point `-outputPath` to `$PSScriptRoot/../src/.nuget/`. This folder is registered as nuget package source for the entire repo, and subsequent restores will reuse the data.
  - Downloaded dependencies ARE imported to IPFS and recorded in [dependencies.json](dependencies.json).
  - Use the `-latestOnly` switch to only download from the original source
  - Use the `-fallbackOnly` switch to only download from IPFS. This is the faster option if you've already done it once.
  - Use the `-skipDownload` switch to avoid downloading anything at all. Uses existing data on disk, if present.
  - Use the `-skipExtract` switch to avoid extracting anything at all. Uses existing data on disk, if present.

- **SnapshotGoIpfsBinaries.ps1**
  - Scrapes the go-ipfs release page, finds the latest version, and downloads the archived binaries to the given location.
  - Downloaded dependencies are NOT imported to IPFS and recorded in [dependencies.json](dependencies.json).

## Restoring dependencies
Download a dependency from a known source, and use ipfs as a fallback if the original sources fail.

- **GatherDependencies.ps1**
  - Handles downloading dependencies from known sources.
  - Most other scripts use this automatically, and only when needed.
  - Running this can take some time if downloading all dependencies
  - Use the `-latestOnly` switch to only download from the original source
  - Use the `-fallbackOnly` switch to only download from IPFS. This is the faster option if you've already done it once.
  - Provide a `-dependencyName name` argument to download a specific dependency from [dependencies.json](dependencies.json)
  - Provide a `-dependencySourcesPath ./some/path/somefile.json` argument to specify the dependency json to pull links and CIDs from.

## Generating 
These are scripts which build, tag, and generate things.

  - **CreateSdkRelease.ps1**
    - Creates git tags and bumps versions for a new release.
    - If the tag version and the sdk version match, the build number on both are bumped, and a new tag is created.
    - If the tag version is greater than the sdk version, the csproj is updated with the existing tag's version.
    - Designed to be used by CI to automatically bump the build number when a major or minor bump wasn't specified via tag.

  - **CreateAppRelease.ps1**
    - Creates git tags and bumps versions for a new release.
    - If the tag version and the app version match, the build number on both are bumped, and a new tag is created.
    - If the tag version is greater than the app version, the app manifest is updated with the existing tag's version.
    - Designed to be used by CI to automatically bump the build number when a major or minor bump wasn't specified via tag.

  - **GenerateChangelogs.ps1**
    - Can be called with either `-variant sdk` or `-variant app`.
      - `app` will generate changelogs from most non-sdk folders that affect the end user to generate changelogs (shells, cores, platforms, etc)
      - `sdk` will only use the sdk folder to generate a list of changes
    - Finds all the changes since the last tag and uses the commit data to generate markdown
    - Squashing all your PRs is recommended, since all commits between the tags will show, including merge commits.
    - Automatically group commits tagged with `[breaking]`, `[fix]`, `[improvement]`, `[new]`, `[refactor]`, `[cleanup]` into different sections. If the commit has no tag in the commit message, it defaults to the "Other" category.

  - **GenerateDocs.ps1**
    - Automatically gathers the docfx binaries needed and generates documentation using the `./docs/docfx.json` file.
    - Has limited support on linux. Make sure you install `mono-devel` from mono directly (not your vendor).

## Organizing

- **OrganizeReleaseContent.ps1**
  - When given a path to the compiled documentation, the compiled WASM app, and the raw website, it organizes the content into the default folder structure for a release.

## Publishing

- **PublishToIpfs.ps1**
  - When given a path to ready-to-publish release content, this script imports the files into ipfs, grabs the CID, and publishes it to IPNS under the provided `-ipnsKey MyKey`