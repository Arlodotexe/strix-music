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

Write-Host "Creating folder $outputPath"
mkdir $outputPath

Write-Host "Creating folder $docsdest"
mkdir $docsdest

Write-Host "Creating folder $wasmdest"
mkdir $wasmdest

Write-Host "Creating folder $uwpdest"
mkdir $uwpdest

Write-Host "Creating folder $sdkdest"
mkdir $sdkdest

Write-Host "Creating folder $gitdest"
mkdir $gitdest

Write-Host "Creating folder $builddepdest"
mkdir $builddepdest

Write-Host "Copying contents from $websitePath to $websitedest"
Copy-Item -PassThru -Recurse -Path $websitePath -Destination $websitedest -ErrorAction Stop

Write-Host "Copying contents from $docsPath to $docsdest"
Copy-Item -PassThru -Recurse -Path $docsPath -Destination $docsdest -ErrorAction Stop

Write-Host "Copying contents from $uwpSideloadBuildPath to $uwpdest"
Copy-Item -PassThru -Recurse -Path $uwpSideloadBuildPath -Destination $uwpdest -ErrorAction Stop

Write-Host "Copying contents from $wasmAppPath to $wasmdest"
Copy-Item -PassThru -Recurse -Path $wasmAppPath -Destination $wasmdest -ErrorAction Stop

Write-Host "Copying contents from $sdkNupkgFolder to $sdkdest"
Copy-Item -PassThru -Recurse -Path $sdkNupkgFolder -Destination $sdkdest -ErrorAction Stop

Write-Host "Copying contents from $cleanRepoPath/* to $gitdest"
Copy-Item -PassThru -Recurse -Path $cleanRepoPath/* -Destination $gitdest -ErrorAction Stop

Write-Host "Copying contents from $buildDependenciesPath to $builddepdest"
Copy-Item -PassThru -Recurse -Path $buildDependenciesPath -Destination $builddepdest -ErrorAction Stop