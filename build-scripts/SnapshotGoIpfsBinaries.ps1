Param (
    [Parameter(HelpMessage = "The binary variants to download.")]
    [ValidateSet('all', 'win-x64', "win-x86", 'osx-x64', 'linux-x64', 'linux-arm', 'linux-arm64')]
    [string[]]$variants = @("all"),
    
    [Parameter(HelpMessage = "The path where binaries are downloaded to.", Mandatory = $true)]
    [string]$outputPath
)

New-Item -ItemType Directory -Path $outputPath -Force -ErrorAction Stop | Out-Null;

if ($variants.Contains("all")) {
    $variants = @('win-x64', "win-x86", 'osx-x64', 'linux-x64', 'linux-arm', 'linux-arm64')
}

# Get the latest stable release of go-ipfs
$versions = Invoke-WebRequest -Uri https://dist.ipfs.io/go-ipfs/versions;

$stableVersions = $versions -Replace "v", "" -Split "`n" | Where-Object { !$_.Contains("rc") };

$highestMajorVersion = 0
$highestMinorVersion = 0
$highestBuildVersion = 0

foreach ($version in $stableVersions) {
    $parts = $version.Split(".");
    
    $major = $parts[0];
    $minor = $parts[1];
    $build = $parts[2];

    if ($major -ge $highestMajorVersion -or ($minor -ge $highestMinorVersion -and $major -ne $highestMajorVersion) -or ($build -ge $highestBuildVersion -and $minor -ne $highestMinorVersion -and $major -ne $highestMajorVersion)) {
        $highestMajorVersion = $major;
        $highestMinorVersion = $minor;
        $highestBuildVersion = $build;
    }
}

$highestVersion = "v$highestMajorVersion.$highestMinorVersion.$highestBuildVersion";

Write-Output "Selected latest stable go-ipfs $highestVersion"

$distData = Invoke-WebRequest -Uri https://dist.ipfs.io/go-ipfs/$highestVersion/dist.json | ConvertFrom-Json -ErrorAction Stop;

foreach ($variant in $variants) {
    $parts = $variant.Split("-");
    $os = $parts[0];
    $arch = $parts[1];

    if ($os -eq "win") {
        $os = "windows";
    }

    if ($os -eq "osx") {
        $os = "darwin";
    }

    if ($arch -eq "x86") {
        $arch = "386";
    }

    if ($arch -eq "x64") {
        $arch = "amd64";
    }

    $archivePathRelative = $distData.platforms.$os.archs.$arch.link;
    $url = "https://dist.ipfs.io/go-ipfs/$highestVersion$archivePathRelative";
    
    Write-Output "Downloading $url to $outputPath$archivePathRelative"
    Invoke-WebRequest -Uri $url -OutFile "$outputPath$archivePathRelative"
}
