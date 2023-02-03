Param (
    [Parameter(HelpMessage = "The path to the project to scan for nuget packages.", Mandatory = $true)]
    [string]$projectPath = "$PSScriptRoot/../src/Platforms/StrixMusic.Wasm/",

    [Parameter(HelpMessage = "The path where nupkg files are saved.", Mandatory = $true)]
    [string]$outputPath = "$PSScriptRoot/build/dependencies/nuget",

    [Parameter(HelpMessage = "The path where nupkg files are saved.")]
    [string]$workingDirectory = "$PSScriptRoot/build",
    
    [Parameter(HelpMessage = "Only download the fallback dependencies.")] 
    [switch]$fallbackOnly = $false,

    [Parameter(HelpMessage = "The path to a dependencies.json file")]
    [string]$dependencySourcesPath = "$PSScriptRoot/dependencies.json",
    
    [Parameter(HelpMessage = "Skip download of dotnet archive")] 
    [switch]$skipDownload = $false,
    
    [Parameter(HelpMessage = "Skip extraction of dotnet archive")] 
    [switch]$skipExtract = $false
)

New-Item -ItemType Directory $outputPath -Force | Out-Null;

$dependencyJsonPath = Resolve-Path -Relative -Path $dependencySourcesPath -ErrorAction Stop;
$dependencies = Get-Content -Path $dependencyJsonPath | ConvertFrom-Json -ErrorAction Stop;

New-Item -ItemType Directory "$workingDirectory/restore" -Force | Out-Null;

Invoke-Expression './dotnet.ps1 -workingDirectory $workingDirectory -dependencySourcesPath $dependencySourcesPath -command "restore $projectPath --configfile ..\nuget.config --force --packages $workingDirectory/restore/" -skipDownload:$skipDownload -skipExtract:$skipExtract -fallbackOnly:$fallbackOnly';
 
foreach ($nupkg in Get-ChildItem -Recurse -Path "$workingDirectory/restore/**/*.nupkg") {
    $fileName = $nupkg | Select-Object -ExpandProperty Name;
    Write-Output "Copying $fileName";
    Copy-Item -Path $nupkg -Destination "$outputPath/$fileName"
}

Remove-Item -Recurse -Force "$workingDirectory/restore/" -ErrorAction Stop;

try {
    ipfs version
}
catch {
    Write-Error "ipfs could not be executed. Unable to import nuget packages. Make sure go-ipfs is installed, and the path to the binary is present in your PATH.";
    exit -1;
}

$result = ipfs add $outputPath --recursive --progress --pin --wrap-with-directory --fscache

Write-Output ""
Write-Output "Getting new CID from output"

$lines = $result.Split([Environment]::NewLine);
$rootdirline = $lines[$lines.Length - 1]
$match = select-string "added ([a-zA-Z0-9]+)" -inputobject $rootdirline
$cid = $match.matches.groups[1].value;

Write-Output "Imported to IPFS as $cid";

foreach ($dependency in $dependencies) {
    if ($dependency.Name -eq "nupkg") {
        $dependency.cid = $cid;
    }
}

Set-Content -Path $dependencyJsonPath -Value (ConvertTo-Json $dependencies) -ErrorAction Stop;
