Param (
    [Parameter(HelpMessage = "The path where dependencies are downloaded to on disk.", Mandatory = $true)]
    [string]$outputPath,

    [Parameter(HelpMessage = "The path to a dependencies.json file")]
    [string]$dependencySourcesPath = "./dependencies.json",
    
    [Parameter(HelpMessage = "The name of a specific dependency to download.")]
    [string]$dependencyName = "all",
    
    [Parameter(HelpMessage = "Don't use any fallbacks if a download fails.")] 
    [switch]$latestOnly = $false,
    
    [Parameter(HelpMessage = "Only download the fallback dependencies.")] 
    [switch]$fallbackOnly = $false
)

$ipfsVersion = "";

Write-Verbose "Setting up output folder"
New-Item -ItemType Directory -Force -Path $outputPath | out-null;
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

    $cid = ($dependency.ipfsFallback) -Replace "ipfs://", "";

    ipfs get $cid -o "$outputFolder/$($dependency.outputFileName)";
}

foreach ($dependency in $dependencies) {
    if (!($dependency.name -eq $dependencyName) -and !($dependencyName -eq "all")) {
        continue;
    }

    Write-Output "Downloading latest $($dependency.name) release"
    
    if ($fallbackOnly) {
        DownloadFromIpfs
        continue;
    }

    try {
        Invoke-WebRequest -Uri $dependency.latestRelease -OutFile "$outputFolder/$($dependency.outputFileName)";
    }
    catch {
        if ($latestOnly) {
            Write-Error "Failed to download $($dependency.latestRelease)";
            exit -1;
        }

        Write-Output "Failed to download from GitHub, using fallback from IPFS"
        DownloadFromIpfs
    }    
}
