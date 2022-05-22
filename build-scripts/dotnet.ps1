# Downloads the required .NET SDK, extracts it, and runs the provided commands.

Param (
    [Parameter(HelpMessage = "The path to a dependencies.json file")]
    [string]$dependencySourcesPath = "$PSScriptRoot/dependencies.json",
    
    [Parameter(HelpMessage = "The path where binaries are downloaded to, extracted and run")]
    [string]$workingDirectory = "$PSScriptRoot/build",
    
    [Parameter(HelpMessage = "The comand to run, excluding the `"dotnet`"", Mandatory = $true)]
    [string]$command,
    
    [Parameter(HelpMessage = "Only download the fallback dependencies.")] 
    [switch]$fallbackOnly = $false,
    
    [Parameter(HelpMessage = "Skip download of dotnet archive")] 
    [switch]$skipDownload = $false,
    
    [Parameter(HelpMessage = "Skip extraction of dotnet archive")] 
    [switch]$skipExtract = $false
)

function Get-Machine-Architecture {
    if ($IsLinux -or $IsMacOS) {
        $arch = uname -m;

        return $arch;
    }

    # On PS x86, PROCESSOR_ARCHITECTURE reports x86 even on x64 systems.
    # To get the correct architecture, we need to use PROCESSOR_ARCHITEW6432.
    # PS x64 doesn't define this, so we fall back to PROCESSOR_ARCHITECTURE.
    # Possible values: amd64, x64, x86, arm64, arm
    if ( $null -ne $ENV:PROCESSOR_ARCHITEW6432 ) {
        return $ENV:PROCESSOR_ARCHITEW6432
    }

    return $ENV:PROCESSOR_ARCHITECTURE
}

# Linux and MacOS are only capable of running PowerShell Core 6+, which have automatic variables.
$IsLin = $IsLinux;
$IsMac = $IsMacOS;
$IsWin = $PSVersionTable.Platform -eq "Win32NT" -or $PSVersionTable.PSEdition -eq "Desktop";

$runtime = "";
if ($IsWin) { $runtime += "win"; }
if ($IsLin) { $runtime += "linux"; }
if ($IsMac) { $runtime += "osx"; }

$arch = (Get-Machine-Architecture).ToLower() -Replace "x86_64", "x64" -Replace "amd64", "x64" -Replace "armv[3-7]h?l?", "arm" -Replace "armv8", "arm64" -Replace "aarch64", "arm64"

$runtime += "-$arch";

$dependencies = Get-Content -Path $dependencySourcesPath | ConvertFrom-Json -ErrorAction Stop;

if ($skipDownload -eq $false) {
    Invoke-Expression -Command './GatherDependencies.ps1 -dependencyName "dotnet-sdk-$runtime" -outputPath $workingDirectory -dependencySourcesPath $dependencySourcesPath -fallbackOnly:$fallbackOnly -ErrorAction Stop'
}

$dependency = $dependencies | Where-Object { $_.name -eq "dotnet-sdk-$runtime" };
$extractPath = "$workingDirectory/dotnet/$($dependency.name)";

New-Item -ItemType Directory -Force $extractPath -ErrorAction Stop | Out-Null

if ($skipExtract -eq $false) {
    Write-Output "Extracting archive"

    if (($dependency.outputPath).Contains('.tar.gz')) {
        tar xzvf $workingDirectory/$($dependency.outputPath) -C $extractPath
    }
    elseif (($dependency.outputPath).Contains('.zip')) {
        Expand-Archive -Path "$workingDirectory/$($dependency.outputPath)" -DestinationPath $extractPath -Force -ErrorAction Stop
    }
}

Invoke-Expression -Command "$extractPath/dotnet $command" 