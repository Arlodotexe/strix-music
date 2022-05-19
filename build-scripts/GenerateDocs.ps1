Param (
    [Parameter(HelpMessage = "The path where dependencies are unzipped on disk.")]
    [string]$workingDirectory = "$PSScriptRoot/build",

    [Parameter(HelpMessage = "The path to a dependencies.json file")]
    [string]$dependencySourcesPath = "$PSScriptRoot/dependencies.json",
    
    [Parameter(HelpMessage = "Skip download of docfx archive")] 
    [switch]$skipDownload = $false,
    
    [Parameter(HelpMessage = "Skip extraction of docfx archive")] 
    [switch]$skipExtract = $false,
    
    [Parameter(HelpMessage = "Only download the fallback dependencies.")] 
    [switch]$fallbackOnly = $false
)

New-Item -ItemType Directory -Force -Path $workingDirectory | out-null;

$dependencies = Get-Content -Path $dependencySourcesPath | ConvertFrom-Json -ErrorAction Stop;
$dependency = $dependencies | Where-Object { $_.name -eq "docfx" };

if ($skipDownload -eq $false) {
    ./GatherDependencies.ps1 -outputPath $workingDirectory -dependencyName docfx -fallbackOnly:$fallbackOnly
}

$archivePath = Resolve-Path -Relative -Path "$workingDirectory/$($dependency.outputPath)" -ErrorAction Stop;

if ($skipExtract -eq $false) {
    Expand-Archive -Path $archivePath -DestinationPath "$workingDirectory/docfx" -Force
}

 if ($PSVersionTable.Platform -eq "Unix") {
    Write-Output "Linux detected"
    Write-Output "Checking if mono-devel is installed";

    $monoDevelRes = Invoke-Expression -Command ("dpkg -l | grep mono-devel");
    if ($monoDevelRes -eq "") {
        Write-Error "mono-devel is not installed. Please install it directly from mono (not your vendor) and try again.";
        exit -1;
    }

    Write-Output "Building docs"

    try {
        mono $workingDirectory/docfx/docfx.exe metadata ../docs/docfx.json
        mono $workingDirectory/docfx/docfx.exe metadata ../docs/docfx.json
        
        ./../docs/build-scripts/unflatten-namespaces.ps1 ../docs/reference/api/toc.yml
        mono $workingDirectory/docfx/docfx.exe build ../docs/docfx.json
    }
    catch {
        Write-Warning "Errors detected with build. Ensure you've installed mono-devel from mono directly, not from your vendor. This dependency does not yet have a fallback source on IPFS."
    }
}

if ($PSVersionTable.Platform -eq "Win32NT" -or $PSVersionTable.PSEdition -eq "Desktop") {
    Write-Output "Windows detected"
    Write-Output "Building docs"

    Invoke-Expression "$workingDirectory/docfx/docfx.exe metadata ../docs/docfx.json"
    Invoke-Expression "$workingDirectory/docfx/docfx.exe metadata ../docs/docfx.json"
    
    ./../docs/build-scripts/unflatten-namespaces.ps1 ../docs/reference/api/toc.yml
    Invoke-Expression "$workingDirectory/docfx/docfx.exe build ../docs/docfx.json"
}


Write-Output "Done"