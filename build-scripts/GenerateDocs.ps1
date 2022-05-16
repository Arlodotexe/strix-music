Param (
    [Parameter(HelpMessage = "The path where dependencies are unzipped on disk.", Mandatory = $true)]
    [string]$outputPath,

    [Parameter(HelpMessage = "The path to a zip file containing a docfx release", Mandatory = $true)]
    [string]$docfxArchivePath,
    
    [Parameter(HelpMessage = "If true, the script will auto-install missing dependencies.")] 
    [switch]$autoInstallMissing = $false
)

$extractPath = Resolve-Path -Relative -Path $outputPath -ErrorAction Stop;
$archivePath = Resolve-Path -Relative -Path $docfxArchivePath -ErrorAction Stop;

if ($autoInstallMissing) {
    ./GatherDependencies.ps1 -outputPath ./build -dependencyName docfx
}

Expand-Archive -Path $archivePath -DestinationPath "$extractPath/docfx" -Force

if ($PSVersionTable.Platform -eq "Unix") {
    Write-Output "Linux detected"
    Write-Output "Checking if mono-devel is installed";

    $monoDevelRes = Invoke-Expression -Command ("dpkg -l | grep mono-devel");
    if ($monoDevelRes -eq "") {
        Write-Error "mono-devel is not installed. Please install it and try again.";
        exit -1;
    }

    Write-Output "Building docs"

    try {
        mono build/docfx/docfx.exe metadata ../docs/docfx.json
        mono build/docfx/docfx.exe metadata ../docs/docfx.json
        
        ./../docs/build-scripts/unflatten-namespaces.ps1 ../docs/reference/api/toc.yml
        mono build/docfx/docfx.exe build ../docs/docfx.json
    }
    catch {
        Write-Warning "Errors detected with build. Ensure you've installed mono-devel from mono directly, not from your vendor. This dependency does not yet have a fallback source on IPFS."
    }
}

if ($PSVersionTable.Platform -eq "Win32NT" -or $PSVersionTable.PSEdition -eq "Desktop") {
    Write-Output "Windows detected"
    Write-Output "Building docs"

    build/docfx/docfx.exe metadata ../docs/docfx.json
    build/docfx/docfx.exe metadata ../docs/docfx.json
    
    ./../docs/build-scripts/unflatten-namespaces.ps1 ../docs/reference/api/toc.yml
    build/docfx/docfx.exe build ../docs/docfx.json
}

Remove-Item -Recurse $extractPath;

Write-Output "Done"