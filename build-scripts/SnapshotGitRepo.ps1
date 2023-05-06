Param (
    [Parameter(HelpMessage = "The repository to 'git clone' for this snapshot.", Mandatory = $true)]
    [string]$repository,

    [Parameter(HelpMessage = "The path where the source directory should be copied to.", Mandatory = $true)]
    [string]$outputPath,

    [Parameter(HelpMessage = "The git remote to use for cloning a fresh copy for release.")]
    [string]$gitRemote
)

Remove-Item -Recurse -Path $outputPath -Force -ErrorAction SilentlyContinue
New-Item -ItemType Directory -Path $outputPath -Force | Out-Null

if ($null -eq $gitRemote -or $gitRemote.Length -eq 0) {
    $gitRemote = "origin";
}

# From https://docs.ipfs.tech/how-to/host-git-repo/
# Snapshot https://ipfs.io/ipfs/Qmdr2NB8BRtNFrB3gsD3NhWPeY2ydx7eGQwKteaTbDaZvz/how-to/host-git-repo/
git clone $repository -l $outputPath --origin $gitRemote --mirror

# Set working directory to cloned folder for git operations
$originalPath = Get-Location;
Set-Location  $outputPath;

Write-Output "Updating git server info";
git update-server-info;

<# TODO: This is disabled because PowerShell cannot properly pass .pack file contents to the git unpack-objects command.
# Unpack all of git's objects

Write-Output "Moving git packs";
Move-Item -Path "objects/pack/*.pack" -Destination $outputPath

Write-Output "Unpacking git objects";
foreach ($item in Get-Item "$outputPath/*.pack") {
    Write-Output "Verifying pack $item";
    git verify-pack -v $item;

    Write-Output "Unpacking $item";
    Get-Content -AsByteStream $item | git unpack-objects -n -r --strict
}

Write-Output "Cleaning up git packs";
Remove-Item -Force -Path "*.pack"
Remove-Item -Path "objects/pack/*" -Recurse -Force
Write-Output "Unpacked and cleaned git objects"; #>

# Restore original working directory
Set-Location $originalPath;
