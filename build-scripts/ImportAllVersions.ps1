
Param (
    [Parameter(HelpMessage = "An ipfs path to a version.json file. Should contain an array of objects with a 'cid' property. Each cid will be imported.", Mandatory = $true)]
    [string]$ipfsPath
)

ipfs cat $ipfsPath | ConvertFrom-Json | ForEach-Object -Parallel {
    Write-Output "Preloading $($_.cid)"
    ipfs refs -r $_.cid;

    Write-Output "Pinning $($_.cid)"
    ipfs pin add $_.cid
}

Write-Host "All content pinned to local node"