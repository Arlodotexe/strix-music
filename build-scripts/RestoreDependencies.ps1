# Downloads dependencies from the provided json file. Uses IPFS as a fallback if the links are dead.

Param (
    [Parameter(HelpMessage = "The path where dependencies are downloaded to on disk.")]
    [string]$outputPath = "$PSScriptRoot/build/",

    [Parameter(HelpMessage = "The path to a dependencies.json file")]
    [string]$dependencySourcesPath = "$PSScriptRoot/dependencies.json",
    
    [Parameter(HelpMessage = "The name of a specific dependency to download.")]
    [string]$dependencyName = "all",
    
    [Parameter(HelpMessage = "Don't use any fallbacks if a download fails.")] 
    [switch]$latestOnly = $false,
    
    [Parameter(HelpMessage = "Only download the fallback dependencies.")] 
    [switch]$fallbackOnly = $false
)

$ipfsVersion = "";

Write-Verbose "Setting up output folder"
New-Item -ItemType Directory -Force -Path $outputPath -ErrorAction Stop | out-null;
$outputFolder = Resolve-Path -Relative -Path $outputPath -ErrorAction Stop;

$dependencyJsonPath = Resolve-Path -Relative -Path $dependencySourcesPath -ErrorAction Stop;
$dependencies = Get-Content -Path $dependencyJsonPath | ConvertFrom-Json -ErrorAction Stop;

$dependencyNameExists = $false;

foreach ($dependency in $dependencies) {
    if ($dependency.name -eq $dependencyName -or $dependencyName -eq "all") {
        $dependencyNameExists = $true;
        break;
    }
}

if (!$dependencyNameExists) {
    Write-Error "Dependency name ""$dependencyName"" not found"; 
}

function DownloadFromIpfs() {
    if ($ipfsVersion -eq "") {   
        try {
            $ipfsVersion = ipfs version
        }
        catch {
            Write-Error "ipfs could not be executed. Make sure go-ipfs is installed, and the path to the binary is present in your PATH.";
            exit -1;
        }
    }

    $path = "$outputFolder/$($dependency.outputPath)"

    ipfs get $dependency.cid -o $path
    
    if ($dependency.name -eq "nupkg") {
        foreach ($nupkg in Get-ChildItem -Recurse -Path "$path/*.nupkg") {
            $fileName = $nupkg | Select-Object -ExpandProperty Name;
            $dest = "$PSScriptRoot/../src/.nuget/$fileName";
            Write-Output "Copying $fileName to $dest";
            Copy-Item -Path $nupkg -Destination $dest;
        }
    }
}

foreach ($dependency in $dependencies) {
    if ($dependency.name -ne $dependencyName -and $dependencyName -ne "all") {
        continue;
    }

    if (Test-Path "$outputFolder/$($dependency.outputPath)") {
        continue;
    }
    else {
        New-Item -ItemType File -Force -Path "$outputFolder/$($dependency.outputPath)" | Out-Null;
    }
    
    if ($fallbackOnly -or $null -eq ($dependency.originalUrl)) {
        DownloadFromIpfs
        continue;
    }
    
    Write-Output "Downloading latest $($dependency.name)"

    try {
        Invoke-WebRequest -Uri ($dependency.originalUrl) -OutFile "$outputFolder/$($dependency.outputPath)";
    }
    catch {
        if ($latestOnly) {
            Write-Error "Failed to download $($dependency.originalUrl)";
            exit -1;
        }

        Write-Output "Failed to download $($dependency.originalUrl), using fallback from IPFS"
        DownloadFromIpfs
    }    
}
