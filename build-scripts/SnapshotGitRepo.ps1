Param (
    [Parameter(HelpMessage = "The path where the clean repo should be stored (contents will be placed in this folder directly).", Mandatory = $true)]
    [string]$outputPath,

    [Parameter(HelpMessage = "Include this flag only if you know what you're doing. Will wipe all uncommitted changes.")] 
    [switch]$force = $false
)

if (!$force) {
    Write-Warning "Using this command will wipe all uncommitted changes before snapshotting the repo. Use -force if you're sure"
    exit 0;
}

Invoke-Expression "git clean -xdf -- `"$PSScriptRoot/../`"";

Copy-Item -PassThru -Recurse -Path "$PSScriptRoot/.." -Destination $outputPath

Compress-Archive -Path "$PSScriptRoot/.." -DestinationPath $outputPath -CompressionLevel Optimal