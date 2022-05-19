Param (
    [Parameter(HelpMessage = "The path to the directory containing the compiled webassembly app", Mandatory = $true)]
    [string]$wasmAppPath,

    [Parameter(HelpMessage = "The path to a directory containing ready-to-deploy files for the website root", Mandatory = $true)]
    [string]$websitePath,

    [Parameter(HelpMessage = "The path to a directory containing the compiled documentation website.", Mandatory = $true)]
    [string]$docsPath,

    [Parameter(HelpMessage = "The path to the directory where the release content is placed.", Mandatory = $true)]
    [string]$outputPath
)

$docsdest = "$outputPath/docs"
$wasmdest = "$outputPath/app"
$websitedest = $outputPath

Write-Host "Creating folder $outputPath"
mkdir $outputPath

Write-Host "Creating folder $docsdest"
mkdir $docsdest

Write-Host "Creating folder $wasmdest"
mkdir $wasmdest

Write-Host "Copying contents from $websitePath to $websitedest"
Copy-Item -PassThru -Recurse -Path $websitePath -Destination $websitedest

Write-Host "Copying contents from $docsPath to $docsdest"
Copy-Item -PassThru -Recurse -Path $wasmAppPath -Destination $docsdest

Write-Host "Copying contents from $wasmsrc to $wasmdest"
Copy-Item -PassThru -Recurse -Path $wasmAppPath -Destination $wasmdest