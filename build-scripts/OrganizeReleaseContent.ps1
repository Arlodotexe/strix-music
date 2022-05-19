Param (
    [Parameter(HelpMessage = "The path to the directory containing the compiled webassembly app", Mandatory = $true)]
    [string]$wasmAppPath,

    [Parameter(HelpMessage = "The path to a directory containing ready-to-deploy files for the website root", Mandatory = $true)]
    [string]$websitePath,

    [Parameter(HelpMessage = "The path to a directory containing the compiled documentation website.", Mandatory = $true)]
    [string]$docsPath,

    [Parameter(HelpMessage = "The path to a directory containing the git repo, without any artifacts.", Mandatory = $true)]
    [string]$cleanRepoPath,

    [Parameter(HelpMessage = "The path to a directory containing all snapshotted build dependencies", Mandatory = $true)]
    [string]$buildDependenciesPath,

    [Parameter(HelpMessage = "The path to the directory where the release content is placed.", Mandatory = $true)]
    [string]$outputPath
)

$docsdest = "$outputPath/docs"
$wasmdest = "$outputPath/app"
$gitdest = "$outputPath/git"
$builddepdest = "$outputPath/dependencies"
$websitedest = $outputPath

Write-Host "Creating folder $outputPath"
mkdir $outputPath

Write-Host "Creating folder $docsdest"
mkdir $docsdest

Write-Host "Creating folder $wasmdest"
mkdir $wasmdest

Write-Host "Creating folder $gitdest"
mkdir $gitdest

Write-Host "Creating folder $builddepdest"
mkdir $builddepdest

Write-Host "Copying contents from $websitePath to $websitedest"
Copy-Item -PassThru -Recurse -Path $websitePath -Destination $websitedest

Write-Host "Copying contents from $docsPath to $docsdest"
Copy-Item -PassThru -Recurse -Path $wasmAppPath -Destination $docsdest

Write-Host "Copying contents from $wasmsrc to $wasmdest"
Copy-Item -PassThru -Recurse -Path $wasmAppPath -Destination $wasmdest

Write-Host "Copying contents from $cleanRepoPath/* to $gitdest"
Copy-Item -PassThru -Recurse -Path $cleanRepoPath/* -Destination $gitdest

Write-Host "Copying contents from $buildDependenciesPath to $builddepdest"
Copy-Item -PassThru -Recurse -Path $buildDependenciesPath -Destination $builddepdest