
Param (
    [Parameter(HelpMessage = "An ipfs path to a version.json file with content to import.")]
    [string]$path = "/ipns/k51qzi5uqu5dip7dqovvkldk0lz03wjkc2cndoskxpyh742gvcd5fw4mudsorj/versions.json"
)

ipfs cat $path | ConvertFrom-Json | ForEach-Object -Parallel {
    Write-Output "Preloading $($_.cid)"
    ipfs refs -r $_.cid;

    Write-Output "Pinning $($_.cid)"
    ipfs pin add $_.cid
}