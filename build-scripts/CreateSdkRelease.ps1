Param (
    [Parameter(HelpMessage = "The variant for this release (alpha, stable, rc.0, rc.1). Used to create release tag.")]
    [string]$variant = "alpha",
    
    [Parameter(HelpMessage = "If true, the current commit will not be tagged with the updated version and no content on disk will be updated.")] 
    [switch]$dryRun = $false
)

# Get the version from the SDK
# Version and AssemblyVersion should be in sync.
# Version uses $variant, AssemblyVersion does not.
$pathToCsProj = "$PSScriptRoot/../src/Sdk/StrixMusic.Sdk/StrixMusic.Sdk.csproj";
Write-Output "Loading $pathToCsProj"
$projectXaml = New-Object System.Xml.XmlDocument
$projectXaml.PreserveWhitespace = $true
$projectXaml.Load($pathToCsProj)

$sdkVersion = "";

foreach ($item in $projectXaml.Project) {
    foreach ($propGroup in $item.PropertyGroup) {
        if ($null -ne $propGroup.AssemblyVersion -and $propGroup.AssemblyVersion -ne "") {
            $sdkVersion = $propGroup.AssemblyVersion;
            break;
        }
    }
}

if ($sdkVersion -eq "") {
    Write-Error "Could not find assembly version";
    exit -1;
}

Write-Output "Sdk is using version $sdkVersion"

# Get the most recent SDK release from git tags
Write-Output "Getting tag data"
$tagsRaw = Invoke-Expression "git tag --sort=-v:refname"
$tags = $tagsRaw -Split "`n";

if ($tags -isnot [array]) {
    $tags = @($tags);
}

$tags = $tags.Where({ $_.Contains("sdk") })

if ($tags -isnot [array]) {
    $tags = @($tags);
}

if ($tags.Length -eq 0) {
    Write-Error "No sdk tags were found. Create an initial release and try again."
    exit -1;
}

function SaveVersion([string]$newVersion) {
    if ($dryRun) {
        return;
    }

    foreach ($item in $projectXaml.Project) {
        foreach ($propGroup in $item.PropertyGroup) {
            if ($null -ne $propGroup.AssemblyVersion -and $propGroup.AssemblyVersion -ne "") {
                Write-Output "Assigned new version $newVersion";
                $propGroup.AssemblyVersion = $newVersion
                $propGroup.Version = "$newVersion-$variant"
                break;
            }
        }
    }

    Write-Output "Saving $pathToCsProj";
    $projectXaml.Save($pathToCsProj)
}

# Check if the current SDK version matches the most recent release tag
$versionAlreadyReleased = [bool]($tags[0] -Match $sdkVersion)

$parts = $sdkVersion.Split(".");
    
$major = $parts[0];
$minor = $parts[1];
$build = $parts[2];

# If yes, bump the build number and save to disk
if ($versionAlreadyReleased) {
    $newVersion = "$major.$minor.$([int]$build + 1)";
    Write-Output "Sdk version $sdkVersion already released. Selecting $newVersion";

    SaveVersion $newVersion;

    if (!$dryRun) {
        # Then create a new tag marking the release 
        Invoke-Expression 'git tag -a $newVersion-sdk-$variant -m "No extended description was provided. Changes are listed below."' -ErrorAction Stop
    }
}
# If no, use the new version number instead of bumping automatically
else {
    Write-Output "Updating project with existing tag $($tags[0])"
    $taggedVersion = $tags[0].Split("-")[0]

    # Ensure this version is an increment over previous releases.
    # A smaller version number means someone made a mistake.
    $taggedParts = $taggedVersion.Split(".");
    
    $taggedMajor = $taggedParts[0];
    $taggedMinor = $taggedParts[1];
    $taggedBuild = $taggedParts[2];

    if ($taggedMajor -lt $major -or ($taggedMinor -lt $minor -and $major -ne $taggedMajor) -or ($taggedBuild -lt $build -and $minor -ne $taggedMinor -and $major -ne $taggedMajor)) {
        Write-Error "FATAL: The most recent tagged release is $taggedVersion, and is smaller than the current SDK version $sdkVersion. It cannot be used to create a new version. Please correct the issue and try again."
        exit -1
    }

    $newVersion = $taggedVersion;
    SaveVersion $taggedVersion
}

Write-Host "Changes complete. Please review your working tree before pushing."
return "$newVersion-sdk-$variant"
