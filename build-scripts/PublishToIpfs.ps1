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
Write-Host "Adding content to IPFS"
$result = iex "ipfs add -r $releaseContentPath";

Write-Host "Getting new CID from output"
$lines = $result.Split([Environment]::NewLine);
$rootdirline = $lines[$lines.Length - 1]
$match = select-string "added ([a-zA-Z0-9]+)" -inputobject $rootdirline
$cid = $match.matches.groups[1].value;

##########
# Publish IPNS
##########
Write-Output "Publishing IPNS to /ipfs/$cid"
ipfs name publish /ipfs/$cid -key=$ipnsKey
ipfs pin add $cid