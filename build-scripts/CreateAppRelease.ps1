Param (
    [Parameter(HelpMessage = "The variant for this release (alpha, stable, rc.0, rc.1). Used to create release tag.")]
    [string]$variant = "alpha"
)

# Get the identity version from the UWP appxmanifest
# The app syncs to this version for all platforms
$pathToManifest = "$PSScriptRoot/../src/Platforms/StrixMusic.UWP/Package.appxmanifest";
Write-Output "Loading $pathToManifest"
$manifestContent = Get-Content $pathToManifest -Force -Raw;

if(!($manifestContent -Match "[^A-Za-z1-9]Version=`"([0-9]+?\.[0-9]+?\.[0-9]+?)\.[0-9]+?`"")) {
    Write-Error "Couldn't extract version number from appxmanifest"   
}

$appVersion = $matches[1];

if ($appVersion -eq "") {
    Write-Error "Could not find app version";
    exit -1;
}

Write-Output "App is currently using version $appVersion"

# Get the most recent app release from git tags
Write-Output "Getting tag data"
$tagsRaw = Invoke-Expression "git tag --sort=-v:refname"
$tags = $tagsRaw -Split "`n";

if ($tags -isnot [array]) {
    $tags = @($tags);
}

$tags = $tags.Where({ $_.Contains("app") })

if ($tags -isnot [array]) {
    $tags = @($tags);
}

function SaveVersion([string]$newVersion) {
    $manifestContent = $manifestContent -Replace  "[^A-Za-z1-9]Version=`"[0-9]+?\.[0-9]+?\.[0-9]+?(\.[0-9]+?)`"", " Version=`"$newVersion.0`""

    Write-Output "Saving $pathToManifest";
    Set-Content -Path $pathToManifest -Value $manifestContent.TrimEnd();
}

# Check if the current App version matches the most recent release tag
$versionAlreadyReleased = [bool]($tags -Match $appVersion)

$parts = $appVersion.Split(".");
    
$major = $parts[0];
$minor = $parts[1];
$build = $parts[2];

# If yes, bump the build number and save to disk
if ($versionAlreadyReleased) {
    $newVersion = "$major.$minor.$([int]$build + 1)";
    Write-Output "App version $appVersion already released. Selecting $newVersion";

    SaveVersion $newVersion;

    # Then create a new tag marking the release 
    Invoke-Expression 'git tag -a $newVersion-app-$variant -m "No extended description was provided. Changes are listed below."' -ErrorAction Stop
}
else {
    Write-Output "Updating project with existing tag $($tags[0])"
    # If no, use the new version number instead of bumping automatically
    $taggedVersion = $tags[0].Split("-")[0]

    # Ensure this version is an increment over previous releases.
    # A smaller version number means someone made a mistake.
    $taggedParts = $taggedVersion.Split(".");
    
    $taggedMajor = $taggedParts[0];
    $taggedMinor = $taggedParts[1];
    $taggedBuild = $taggedParts[2];

    if ($taggedMajor -lt $major -or ($taggedMinor -lt $minor -and $major -ne $taggedMajor) -or ($taggedBuild -lt $build -and $minor -ne $taggedMinor -and $major -ne $taggedMajor)) {
        Write-Error "FATAL: The most recent tagged release is $taggedVersion, and is smaller than the current App version $appVersion. It cannot be used to create a new version. Please correct the issue and try again."
        exit -1
    }

    SaveVersion $taggedVersion
}

Write-Host "Changes complete. Please review your working tree before pushing."
