Param (
  [Parameter(HelpMessage = "The path to a folder where release content will be placed.", Mandatory = $true)]
  [string]$outputPath,

  [Parameter(HelpMessage = "The git remote to use for snapshotting the repo, pushing tags, version updates, snapshotted dependency data, and generated changelogs.")]
  [string]$gitRemote,

  [Parameter(HelpMessage = "If true, no content will be pushed or published.")]
  [switch]$noPublish = $false,
  
  [Parameter(HelpMessage = "The name of the IPNS key to publish this release under.")]
  [string]$ipnsPublishKey = "",
  
  [Parameter(HelpMessage = "A cid pointing to an existing release.")]
  [string]$pastReleaseCid = "",

  [Parameter(HelpMessage = 'A web url to an existing release. Must be resolvable with "ipfs name resolve example.com"')]
  [string]$pastReleaseIpns = ""
)

# The following combines all the above commands in the correct order, with the correct parameters, to create a release.
# You can use the script in full, or pick it apart and use them in a CI agent.

#  NOTICE: This script will
# - Use your working tree to make and commit changes (version bumps, changelogs, tags, etc)
# - Automatically push generated changes.
# - Need to be run from the build-scripts directory
# - Require an installation of IPFS for most of it.

#################
# Version bumps
#################
Write-Output "Bumping version and generating changelog for Strix Music SDK"
$sdkTag = &".\CreateSdkRelease.ps1" -variant alpha -dryRun | Select-Object -Last 1
$sdkChangelogLastOutput = &".\GenerateChangelogs.ps1" -target sdk -forceTag $sdkTag | Select-Object -Last 1
$emptySdkChangelog = $sdkChangelogLastOutput.ToLower().Contains("no changes");

if (!$emptySdkChangelog) {
  # Excluding -dryRun allows creation of tags and writing to disk.
  &".\CreateSdkRelease.ps1" -variant alpha
}


Write-Output "Bumping version and generating changelog for Strix Music App"
$appTag = &".\CreateAppRelease.ps1" -variant alpha -dryRun | Select-Object -Last 1
$appChangelogLastOutput = &".\GenerateChangelogs.ps1" -target app -forceTag $appTag | Select-Object -Last 1
$emptyAppChangelog = $appChangelogLastOutput.ToLower().Contains("no changes");

if (!$emptyAppChangelog) {
  # Excluding -dryRun allows creation of tags and writing to disk.
  &".\CreateAppRelease.ps1" -variant alpha
}

#################
# Snapshot dependencies
#################
# Download build dependencies, upload to IPFS, and update the CIDs and URLs in dependencies.json.
Write-Output "Snapshotting dotnet binaries"
.\SnapshotDotnetSdk.ps1

Write-Output "Snapshotting nuget packages needed to build WebAssembly"
.\SnapshotNugetPackages.ps1 -fallbackOnly

Write-Output "Creating snapshot of docfx binaries"
.\RestoreDependencies.ps1 -fallbackOnly -dependencyName "docfx"

#################
# Commit changes
#################
# This should be done after versions are bumped, changelogs are generated and dependencies.json is updated.
$commitMessages = "";
if (!$emptyAppChangelog) { $commitMessages += " -m `"Release $appTag`""; }
if (!$emptySdkChangelog) { $commitMessages += " -m `"Release $sdkTag`""; }

if (($noPublish -eq $false) -and ($null -ne $gitOrigin) -and (!$emptyAppChangelog -or !$emptySdkChangelog) -and $commitMessages.length -gt 0) {
  Write-Output "Staging and committing files"
  # Stage files and create a new commit.
  git add .  -- "$PSScriptRoot/../"
  Invoke-Expression "git commit $commitMessages"

  Write-Output "Changes committed"
  if (!$emptyAppChangelog) {
    # Move tags to new release commit
    Write-Output "$gitRemote - Deleting App tag $appTag from remote repo $gitRemote";
    git push $gitRemote :refs/tags/$appTag

    Write-Output "Tagging current commit as tag $sdkTag"
    git tag -fa $appTag -m "Release $appTag";
  }

  if (!$emptySdkChangelog) {
    # Move tags to new release commit
    Write-Output "Deleting sdk tag $sdkTag from remote repo $gitRemote";
    git push $gitRemote :refs/tags/$sdkTag

    Write-Output "Tagging current commit as tag $sdkTag"
    git tag -fa $sdkTag -m "Release $sdkTag";
  }

  # Push the changes
  Write-Output "Pushing changes"
  git push $gitRemote
  
  Write-Output "Pushing tags"
  git push -f $gitRemote --tags
}

#################
# Snapshot git repo
#################
# This should be done after everything is committed.
Write-Output "Create snapshot of git repo"
.\SnapshotGitRepo.ps1 -outputPath build/source

#################
# Compile
#################
# Build documentation website
Write-Output "Generating documentation"
.\GenerateDocs.ps1 -fallbackOnly

# Build WebAssembly
Write-Output "Building WebAssembly app in Release mode"
.\dotnet.ps1 -Command 'build ../src/Platforms/StrixMusic.Wasm/StrixMusic.Wasm.csproj /r /p:Configuration="Release"' -skipDownload -skipExtract

# Build UWP (Requires Windows with correct tooling installed)
Write-Output "Building UWP app in Release mode"
msbuild ../src/Platforms/StrixMusic.UWP/StrixMusic.UWP.csproj /r /m /p:AppxBundlePlatforms="x86|x64|ARM" /p:Configuration="Release" /p:AppxBundle=Always /p:UapAppxPackageBuildMode=StoreUpload

# Create SDK nuget package
Write-Output "Building the Strix Music SDK in Release mode"
.\dotnet.ps1 -Command 'build "../src/Sdk/StrixMusic.Sdk/StrixMusic.Sdk.csproj" -c Release' -skipExtract -skipDownload

Write-Output "Packing the Strix Music SDK in Release mode"
.\dotnet.ps1 -Command 'pack "../src/Sdk/StrixMusic.Sdk/StrixMusic.Sdk.csproj" -c Release --output build/sdk/$sdkTag' -skipExtract -skipDownload

#################
# Organize
#################
# The resulting folder can be uploaded anywhere (not just ipfs)
Write-Output "Organizing generated release content"
.\OrganizeReleaseContent.ps1 -wasmAppPath "$(Get-Location)/../src/Platforms/StrixMusic.Wasm/bin/Any CPU/Release/net7.0/dist/*" -uwpSideloadBuildPath "$(Get-Location)/../src/Platforms/StrixMusic.UWP/AppPackages/*" -websitePath ../www/* -docsPath ../docs/wwwroot/* -sdkNupkgFolder build/sdk/$sdkTag -cleanRepoPath build/source -buildDependenciesPath build/dependencies/* -outputPath $outputPath

if ($pastReleaseCid.Length -gt 0 -or $pastReleaseIpns.Length -gt 0) {
  # Grab previous versioned content such as nuget packages and app installers (requires ipfs)
  Write-Output "Importing CIDs of previous releases from $pastReleaseIpns/versions.json"
  .\ImportPreviousVersionedContent.ps1 -url $pastReleaseIpns -cid $pastReleaseCid -outputPath $outputPath
}
else {
  Write-Warning "No past release was specified, so no past release CIDs will be added to versions.json. Specify -pastReleaseIpns <addr> or -pastReleaseCid <cid> to import past published releases.";
}

#################
# Publish!
#################
if ($ipnsPublishKey.Length -le 0) {
  Write-Warning "No ipns publish key provided, content will not be published. Specify the name of an IPNS key with -ipnsPublishKey KeyName to add generated content to IPFS and publish to IPNS."
}
elseif ($noPublish -eq $false) {
  Write-Output "Publishing to IPFS"
  .\PublishToIpfs.ps1 $outputPath -ipnsKey $ipnsPublishKey
}

Write-Output "Release created in $outputPath"