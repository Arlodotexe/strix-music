Param (
    [Parameter(HelpMessage = "The path where the source directory should be copied to.", Mandatory = $true)]
    [string]$outputPath
)

New-Item -ItemType Directory -Path $outputPath -Force | Out-Null
$bundlePath = "$outputPath/repo.bundle"

git bundle create $bundlePath master --branches --tags
git clone -b master $bundlePath "$outputPath/repo/"

Invoke-Expression "git bundle verify $bundlePath" -ErrorAction Stop;