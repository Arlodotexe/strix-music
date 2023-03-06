Param (
    [Parameter(HelpMessage = "The path to the directory where the release content is placed.", Mandatory = $true)]
    [string]$releaseContentPath,

    [Parameter(HelpMessage = "The path to the directory where the release content is placed.", Mandatory = $true)]
    [string]$ipnsKey = "StrixMusicWebsite"
)

##########
# Cleanup data
##########
Write-Host "Cleaning up previous folder"
Try { ipfs files rm -r /strixmusicapp/ } Catch { }

Write-Host "Recreating folder"
ipfs files mkdir /strixmusicapp/

##########
# Import content
##########
Write-Host "Adding content in $releaseContentPath to IPFS"
$result = Invoke-Expression "ipfs add -H --fscache -r $releaseContentPath";

Write-Host "Getting CID of added content"
$lines = $result.Split([Environment]::NewLine);
$rootdirline = $lines[$lines.Length - 1]
$match = select-string "added ([a-zA-Z0-9]+)" -inputobject $rootdirline
$cid = $match.matches.groups[1].value;
Write-Host "CID is $cid"

##########
# Publish IPNS
##########
Write-Host "Publishing IPNS key $ipnsKey as /ipfs/$cid"
Invoke-Expression "ipfs name publish /ipfs/$cid --key=$ipnsKey"

Write-Host "Pinning $cid to local node"
ipfs pin add $cid

Write-Host "Published to IPFS"