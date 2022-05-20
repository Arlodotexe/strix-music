Param (
    [Parameter(HelpMessage = "The binary variants to download.")]
    [ValidateSet('all', 'win-x64', "win-x86", "win-arm", "win-arm64", 'osx-x64', 'linux-x64', 'linux-arm', 'linux-arm64')]
    [string[]]$variants = @("all"),
    
    [Parameter(HelpMessage = "The path where binaries are downloaded to.", Mandatory = $true)]
    [string]$outputPath,
    
    [Parameter(HelpMessage = "The path to a dependencies.json file")]
    [string]$dependencySourcesPath = "$PSScriptRoot/dependencies.json"
)

function Get-UrlStatusCode([string] $Url) {
    try {
        (Invoke-WebRequest -Uri $Url -UseBasicParsing -DisableKeepAlive -Method head).StatusCode
    }
    catch [Net.WebException] {
        [int]$_.Exception.Response.StatusCode
    }
}

function GetUrlFromDryRun([string] $result) {
    $match = select-string "primary: (.+?) " -inputobject $result

    foreach ($captureGroup in $match.matches.groups) {
        $url = $captureGroup.value -Replace "primary: ", "";
        
        Write-Verbose "Testing url at $url";
        if (Get-UrlStatusCode $url -eq 200) {
            Write-Verbose "Success"
            return $url.Trim();
        }
        else {
            Write-Verbose "Failed, trying next uri"
        }
    }

    Write-Error "No valid URLs were found."
    exit -1;
}

function GetDownloadUrl([string] $osTarget, [string] $arch) {
    if ($osTarget -eq "linux" -or $osTarget -eq "osx") {
        $globalJson = Get-Content -Path ../global.json | ConvertFrom-Json -ErrorAction Stop;

        $result = bash dotnet-install.sh -Version $($globalJson.sdk.version) -Architecture $arch --os $osTarget -DryRun 6>&1
        
        return GetUrlFromDryRun $result;
    }
    
    
    if ($osTarget -eq "win") {
        $result = ./dotnet-install.ps1 -JSonFile ../global.json -Architecture $arch -DryRun 6>&1
        
        return GetUrlFromDryRun $result;
    }
}

if ($variants.Contains("all")) {
    $variants = @('win-x64', "win-x86", "win-arm", "win-arm64", 'osx-x64', 'linux-x64', 'linux-arm', 'linux-arm64');
}

New-Item -ItemType Directory -Path $outputPath -Force | Out-Null

foreach ($variant in $variants) {
    $parts = $variant.Split("-");

    $os = $parts[0];
    $arch = $parts[1];
    
    $url = GetDownloadUrl $os $arch

    $fileName = ([uri]$url).Segments[-1]

    Write-Output "Downloading $url";
    New-Item -ItemType File -Path "$outputPath/$fileName" -Force | Out-Null
    Invoke-WebRequest -Uri $url -Outfile "$outputPath/$fileName";

    try {
        ipfs version
    }
    catch {
        Write-Error "ipfs could not be executed. Unable to import nuget packages. Make sure go-ipfs is installed, and the path to the binary is present in your PATH.";
        exit -1;
    }
    
    $result = ipfs add "$outputPath/$fileName" --recursive --progress --pin --fscache
    
    Write-Output "Getting new CID from output"
    
    $lines = $result.Split([Environment]::NewLine);
    $rootdirline = $lines[$lines.Length - 1]
    $match = select-string "added ([a-zA-Z0-9]+)" -inputobject $rootdirline
    $cid = $match.matches.groups[1].value;
    
    ipfs pin add $cid;
    
    Write-Output "Imported to IPFS as $cid";
    Write-Output ""
    
    $dependencyJsonPath = Resolve-Path -Relative -Path $dependencySourcesPath -ErrorAction Stop;
    $dependencies = Get-Content -Path $dependencyJsonPath | ConvertFrom-Json -ErrorAction Stop;
    
    $found = $false;

    foreach ($dependency in $dependencies) {
        if ($dependency.name -eq "dotnet-sdk-$variant") {
            $dependency.cid = $cid;
            $found = $true;
        }
    }

    if (!$found) {
        $dependencies += [PSCustomObject]@{
            name = "dotnet-sdk-$variant"
            originalUrl = $url
            cid = $cid
            outputPath = "dotnet/$fileName"
        }
    }
    
    Set-Content -Path $dependencyJsonPath -Value (ConvertTo-Json $dependencies) -ErrorAction Stop;
}
