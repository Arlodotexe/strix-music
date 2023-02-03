Param (
  [Parameter(HelpMessage = "The path to a folder where release content will be placed.", Mandatory = $true)]
  [string]$outputPath,

  [Parameter(HelpMessage = "The git remote to use for snapshotting the repo, pushing tags, version updates, snapshotted dependency data, and generated changelogs.")]
  [string]$gitRemote,

  [Parameter(HelpMessage = "If true, no content will be pushed or published.")]
  [switch]$noPublish = $false,
  
  [Parameter(HelpMessage = "The name of the IPNS key to publish this release under", Mandatory = $true)]
  [string]$ipnsPublishKey,
  
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
$sdkTag = &".\CreateSdkRelease.ps1" -variant alpha -dryRun | Select-Object -Last 1
$sdkChangelogLastOutput = &".\GenerateChangelogs.ps1" -target sdk -forceTag $sdkTag | Select-Object -Last 1
$emptySdkChangelog = $sdkChangelogLastOutput.ToLower().Contains("no changes");

if (!$emptySdkChangelog) {
  # Excluding -dryRun allows creation of tags and writing to disk.
  &".\CreateSdkRelease.ps1" -variant alpha
}

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
.\SnapshotDotnetSdk.ps1
.\SnapshotNugetPackages.ps1 -fallbackOnly
.\RestoreDependencies.ps1 -fallbackOnly

#################
# Commit changes
#################
# This should be done after versions are bumped, changelogs are generated and dependencies.json is updated.
$commitMessages = "";
if (!$emptyAppChangelog) { $commitMessages += " -m `"Release $appTag`""; }
if (!$emptySdkChangelog) { $commitMessages += " -m `"Release $sdkTag`""; }

if (($noPublish -eq $false) -and ($null -ne $gitOrigin) -and (!$emptyAppChangelog -or !$emptySdkChangelog) -and $commitMessages.length -gt 0) {
  # Stage files and create a new commit.
  git add .  -- "$PSScriptRoot/../"
  Invoke-Expression "git commit $commitMessages"

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
  Write-Output "Pushing changes and tags"
  git push $gitRemote
  git push -f $gitRemote --tags
}

#################
# Snapshot git repo
#################
# This should be done after everything is committed.
.\SnapshotGitRepo.ps1 -outputPath build/source

#################
# Compile
#################
# Build documentation website
.\GenerateDocs.ps1

# Build WebAssembly
.\dotnet.ps1 -Command 'build ../src/Platforms/StrixMusic.Wasm/StrixMusic.Wasm.csproj /r /p:Configuration="Release"'

# Build UWP (Requires Windows with correct tooling installed)
msbuild ../src/Platforms/StrixMusic.UWP/StrixMusic.UWP.csproj /r /m /p:AppxBundlePlatforms="x86|x64|ARM" /p:Configuration="Release" /p:AppxBundle=Always /p:UapAppxPackageBuildMode=StoreUpload

# Create SDK nuget package
# Include -skipExtract -skipDownload if you've already successfully run dotnet.ps1 at least once to avoid unecessary operations
.\dotnet.ps1 -Command 'build "../src/Sdk/StrixMusic.Sdk/StrixMusic.Sdk.csproj" -c Release' -skipExtract -skipDownload
.\dotnet.ps1 -Command 'pack "../src/Sdk/StrixMusic.Sdk/StrixMusic.Sdk.csproj" -c Release --output build/sdk/$sdkTag' -skipExtract -skipDownload

#################
# Organize
#################
# The resulting folder can be uploaded anywhere (not just ipfs)
.\OrganizeReleaseContent.ps1 -wasmAppPath "$(Get-Location)/../src/Platforms/StrixMusic.Wasm/bin/Any CPU/Release/net7.0/dist/*" -uwpSideloadBuildPath "$(Get-Location)/../src/Platforms/StrixMusic.UWP/AppPackages/*" -websitePath ../www/* -docsPath ../docs/wwwroot/* -sdkNupkgFolder build/sdk/$sdkTag -cleanRepoPath build/source -buildDependenciesPath build/dependencies/* -outputPath $outputPath

if ($pastReleaseCid.length -gt 0 -or $pastReleaseIpns.length -gt 0) {
  # Grab previous versioned content such as nuget packages and app installers (requires ipfs)
  .\ImportPreviousVersionedContent.ps1 -url $pastReleaseIpns -cid $pastReleaseCid -outputPath $outputPath
}
else {
  Write-Warning "No past release was specified, so no past release CIDs will be added to versions.json. Specify -pastReleaseIpns <addr> or -pastReleaseCid <cid> to import past published releases ";
}

#################
# Publish!
#################
if ($noPublish -eq $false) {
  .\PublishToIpfs.ps1 $outputPath -ipnsKey $ipnsPublishKey
}