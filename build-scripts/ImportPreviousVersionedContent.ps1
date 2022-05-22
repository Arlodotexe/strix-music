Param (
    [Parameter(HelpMessage = "The path to an existing, organized release directory", Mandatory = $true)]
    [string]$outputPath,

    [Parameter(HelpMessage = "A cid pointing to an existing release.")]
    [string]$cid,

    [Parameter(HelpMessage = 'A web url to an existing release. Must be resolvable with "ipfs name resolve example.com"')]
    [string]$url
)

$noCid = $null -eq $cid -or $cid.Length -eq 0;
$noUrl = $null -eq $url -or $url.Length -eq 0;

if ($noCid -and $noUrl) {
    Write-Error "No cid or url was supplied. Supply one with -cid thecid or -url example.com"
} 

if (!$noCid -and !$noUrl) {
    Write-Error "Both cid or url was supplied Supply only one with -cid thecid or -url example.com"
}

$ipfsCid = "";

if (!$noCid) {
    $ipfsCid = "/ipfs/$cid";
}

if (!$noUrl) {
    $ipfsCid = ipfs name resolve $url
}

if ($ipfsCid.length -eq 0) {
    Write-Error "Something went wrong. No CID was constructed."
    exit -1;
}
   
if ($ipfsCid.ToLower().Contains("error")) {
    exit -1;
}

$versionsJsonPath = "$outputPath/versions.json";
Write-Output "Importing content"

# Strix Music SDK Nuget packages
ipfs get --output $outputPath/sdk/nupkg/ $ipfsCid/sdk/nupkg/

# List of previous versions
ipfs get --output $outputPath/versions.json $ipfsCid/versions.json

Write-Output "Updating versions.json";
if (Test-Path $versionsJsonPath) {
    $versions = Get-Content -Path $versionsJsonPath -Force | ConvertFrom-Json;
} else {
    New-Item -ItemType File -Path $versionsJsonPath -ErrorAction Stop | Out-Null;
    $versions = ("[]" | ConvertFrom-Json);
}

if ($versions -isnot [array]) {
    $versions = @($versions);
}

if (($versions | Where-Object {$_.cid -eq $ipfsCid})) {
    Write-Error "Unable to continue. Version already published."
    exit -1;
}

$versions += [PSCustomObject]@{
    cid = $ipfsCid
    unixTimestamp = ((([System.DateTime]::Now) - ([System.DateTime]::UnixEpoch)).TotalSeconds)
}

Set-Content -Path $versionsJsonPath -Value ($versions | ConvertTo-Json);
Write-Output "Archived CID of previous version under $versionsJsonPath"