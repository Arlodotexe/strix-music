Param (
    [Parameter(HelpMessage = "The path to the directory containing the compiled webassembly app", Mandatory = $true)]
    [string]$wasmAppPath,

    [Parameter(HelpMessage = "The path to the directory containing the compiled UWP app", Mandatory = $true)]
    [string]$uwpSideloadBuildPath,

    [Parameter(HelpMessage = "The path to a directory containing ready-to-deploy files for the website root", Mandatory = $true)]
    [string]$websitePath,

    [Parameter(HelpMessage = "The path to a directory containing the compiled documentation website.", Mandatory = $true)]
    [string]$docsPath,

    [Parameter(HelpMessage = "The path to a directory containing the output from dotnet pack (nuget packages / symbols).", Mandatory = $true)]
    [string]$sdkNupkgFolder,

    [Parameter(HelpMessage = "The path to a directory containing the git repo, without any artifacts.", Mandatory = $true)]
    [string]$cleanRepoPath,

    [Parameter(HelpMessage = "The path to a directory containing all snapshotted build dependencies", Mandatory = $true)]
    [string]$buildDependenciesPath,

    [Parameter(HelpMessage = "The path to the directory where the release content is placed.", Mandatory = $true)]
    [string]$outputPath
)

$docsdest = "$outputPath/docs"
$wasmdest = "$outputPath/app/web"
$uwpdest = "$outputPath/app/windows"
$sdkdest = "$outputPath/sdk/nupkg"
$gitdest = "$outputPath/source"
$builddepdest = "$outputPath/dependencies"
$websitedest = $outputPath

mkdir -Force $outputPath | Out-Null
mkdir -Force $docsdest | Out-Null
mkdir -Force $wasmdest | Out-Null
mkdir -Force $uwpdest | Out-Null
mkdir -Force $sdkdest | Out-Null
mkdir -Force $gitdest | Out-Null
mkdir -Force $builddepdest | Out-Null

Write-Host "Copying contents from $websitePath to $websitedest"
Copy-Item -Force -PassThru -Recurse -Path $websitePath -Destination $websitedest -ErrorAction Stop | Out-Null

Write-Host "Copying contents from $docsPath to $docsdest"
Copy-Item -Force -PassThru -Recurse -Path $docsPath -Destination $docsdest -ErrorAction Stop | Out-Null

Write-Host "Copying contents from $uwpSideloadBuildPath to $uwpdest"
Copy-Item -Force -PassThru -Recurse -Path $uwpSideloadBuildPath -Destination $uwpdest -ErrorAction Stop | Out-Null

Write-Host "Copying contents from $wasmAppPath to $wasmdest"
Copy-Item -Force -PassThru -Recurse -Path $wasmAppPath -Destination $wasmdest -ErrorAction Stop | Out-Null

Write-Host "Copying contents from $sdkNupkgFolder to $sdkdest"
Copy-Item -Force -PassThru -Recurse -Path $sdkNupkgFolder -Destination $sdkdest -ErrorAction Stop | Out-Null

Write-Host "Copying contents from $cleanRepoPath/* to $gitdest"
Copy-Item -Force -PassThru -Recurse -Path $cleanRepoPath/* -Destination $gitdest -ErrorAction Stop | Out-Null

Write-Host "Copying contents from $buildDependenciesPath to $builddepdest"
Copy-Item -Force -PassThru -Recurse -Path $buildDependenciesPath -Destination $builddepdest -ErrorAction Stop | Out-Null

Write-Host "Done organizing release content into $outputPath"