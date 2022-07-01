# Script is designed to create changelogs for either the app or the sdk
# But with any variations (alpha, release candidate)

Param (
    [Parameter(HelpMessage = "Which changelog to generate (app or sdk)", Mandatory = $true)]
    [ValidateSet('sdk', 'app')]
    [string]$target,

    [Parameter(HelpMessage = "The path where generated markdown file is placed", Mandatory = $true)]
    [string]$outputPath,

    [Parameter(HelpMessage = "The path to a toc.yml where the generated changelog should be inserted", Mandatory = $true)]
    [string]$tocYmlPath,

    [Parameter(HelpMessage = "When a tag is provided, the script will treat the current commit as if it is tagged with it")]
    [string]$forceTag = ""
)

$commitLogSuffix = ""
if ($target -eq "sdk") {
    $commitLogSuffix = ' -- ":/src/Sdk/StrixMusic.Sdk/"';
}
else {
    $commitLogSuffix = ' -- ":/src/Cores/**" -- ":/src/Platforms/**" -- ":/src/Shells/**" -- ":src/Libs/**" -- ":/Sdk/StrixMusic.Sdk.WinUI/**"'
}

Write-Output "Getting tag data"
$tagsRaw = Invoke-Expression "git tag --sort=-v:refname"
$tags = $tagsRaw -Split "`n";

if ($tags -isnot [array]) {
    $tags = @($tags);
}

$tags = $tags.Where({ $_.Contains($target) })

if ($tags -isnot [array]) {
    $tags = @($tags);
}

if ($forceTag.Length -gt 0) {
    $tags = @($forceTag) + $tags;
}

function IsTagCurrentHead ([string]$tag) {
    if ($forceTag.Length -gt 0 -and $forceTag -eq $tag) {
        return $true;
    }
    
    $tagCommitHash = Invoke-Expression "git rev-list -n 1 $tag";
    $res = (Invoke-Expression "git log $tagCommitHash...HEAD --pretty=format:'%h'")
    return $null -eq $res -or $res.length -eq 0;
}

function GetPreviousTag() {
    $hasPreviousTag = $tags.length -gt 0
    
    if (!$hasPreviousTag) {
        Write-Error "No previous tags found. At least one past tag must exist to generate a changelog against"
        exit -1;
    }

    if (IsTagCurrentHead -tag $tags[0]) {
        if ($tags.length -le 1) {
            Write-Error "Current commit is tagged, but no previous commits are tagged. At least one past tag must exist to generate a changelog against"
            exit -1;
        }
        
        return $tags[1];
    }

    return $tags[0];
}

function TransformCommitToMarkdown($commit) {
    return "``$($commit.CommitHash)``: $($commit.Message)"
}

# Find the previous tag
# If no previous tag, error and exit
$previousTag = GetPreviousTag

if (IsTagCurrentHead -tag $tags[0]) {
    Write-Output "Current commit is tagged $($tags[0])";
}
else {
    Write-Output "Current commit is not tagged. This changelog will be labeled as Unreleased.";
}

# If current latest commit is not tagged, set release label to "Unreleased"
$releaseLabel = $tags[0]
if ($releaseLabel -eq $previousTag) {
    $releaseLabel = "Unreleased";
    $releaseMessage = "These changes are not yet released and haven't been assigned a version number."
}
else {
    # Get release message if commit is tagged
    $releaseMessage = (Invoke-Expression "git tag $($tags[0]) -n 999") -Replace $tags[0], "";
}

Write-Output "Generating $target changelog as $releaseLabel for commits since tag $previousTag"

# Crawl all commits between previous tag commit and current HEAD. Merges should be squash commits.
$log = Invoke-Expression -Command "git log $($previousTag)...HEAD --pretty=format:'%ci ||| %h ||| %cn ||| %ce ||| %s'$($commitLogSuffix)"
$logItems = $log -Split "`n"

if ($logItems.length -eq 0) {
    Write-Output "No changes were found between releases."
    exit 0; 
}

$logData = @();

# Gather structured commit data
foreach ($logItem in $logItems) {
    $parts = $logItem.Split(" ||| ");

    # Ignore multiline commit messages
    if ($parts.length -le 1) {
        continue;
    }

    $date = $parts[0];
    $commitHash = $parts[1];
    $authorName = $parts[2];
    $authorEmail = $parts[3];
    $message = $parts[4].Trim();

    $matches = $null;
    if ($message -Match "\[(breaking|fix|improvement|new|refactor|cleanup)\]") {
        $category = $matches[1];
        
        $message = $message -Replace "\[$category\]", ""
    }
    else {
        $category = "Other"
    }

    $logData += [PSCustomObject]@{
        Date        = $date
        CommitHash  = $commitHash
        AuthorName  = $authorName
        AuthorEmail = $authorEmail
        Message     = $message
        Category    = $category
    }
}

# Generate markdown for changes, grouped by category
Write-Output "Generating markdown"
$changelogMarkdownLines = @();

$groupedCommitData = $logData | Group-Object -Property Category;

function GetGroupByName([string] $groupName) {
    return $groupedCommitData | Where-Object { $_.Name -eq $groupName } | Select-Object -ExpandProperty Group | Where-Object { $_.Message.length -gt 0 };
}

if ((GetGroupByName "breaking").length -gt 0) {
    $changelogMarkdownLines += "`n## Breaking changes"
    foreach ($commit in GetGroupByName "breaking") {
        $changelogMarkdownLines += TransformCommitToMarkdown $commit;
    }
}

if ((GetGroupByName "fix").length -gt 0) {
    $changelogMarkdownLines += "`n## Bug fixes"
    foreach ($commit in GetGroupByName "fix") {
        $changelogMarkdownLines += TransformCommitToMarkdown $commit;
    }
}

if ((GetGroupByName "improvement").length -gt 0) {
    $changelogMarkdownLines += "`n## Improvements"
    foreach ($commit in GetGroupByName "improvement") {
        $changelogMarkdownLines += TransformCommitToMarkdown $commit
    }
}

if ((GetGroupByName "new").length -gt 0) {
    $changelogMarkdownLines += "`n## New"
    foreach ($commit in GetGroupByName "new") {
        $changelogMarkdownLines += TransformCommitToMarkdown $commit
    }
}

if ((GetGroupByName "refactor").length -gt 0 -or (GetGroupByName "cleanup").length -gt 0) {
    $changelogMarkdownLines += "`n## Maintainence"
    foreach ($commit in GetGroupByName "refactor") {
        $changelogMarkdownLines += TransformCommitToMarkdown $commit
    }

    foreach ($commit in GetGroupByName "cleanup") {
        $changelogMarkdownLines += TransformCommitToMarkdown $commit
    }
}

if ((GetGroupByName "Other").length -gt 0) {
    $changelogMarkdownLines += "`n## Other"
    foreach ($commit in GetGroupByName "Other") {
        $changelogMarkdownLines += TransformCommitToMarkdown $commit
    }
}

$releaseLabelMdHeader = $releaseLabel;
if (IsTagCurrentHead -tag $tags[0]) {
    $releaseLabelMdHeader = "Release $releaseLabel";
}

$changelogMarkdownHeader = "# $releaseLabelMdHeader
$releaseMessage

Generated on $(Get-Date -AsUTC) UTC";

# Not all lines are bullet points, so the lines with empty bullet points get removed manually
$markdownBody = $changelogMarkdownLines -Join "`n - "

$markdownBody = ($markdownBody -Split "`n" | Where-Object {$_.Trim() -ne "-"}) -Join "`n"

$changelog = "$changelogMarkdownHeader`n$markdownBody";
Write-Output "Markdown created"

Write-Output "Saving to $outputPath/$releaseLabel.md"
Set-Content -Path "$outputPath/$releaseLabel.md" -Value $changelog -ErrorAction Stop

Write-Output "Loading toc.yml"
$tocYml = Get-Content -Path $tocYmlPath -Raw -ErrorAction Stop;

if ($null -eq $tocYml) {
    $tocYml = "";
}

Write-Output "Creating toc entry"

$relativeMdPath = [System.IO.Path]::GetRelativePath((Split-Path $tocYmlPath -Parent), "$outputPath")

$lines = [System.Collections.ArrayList]$tocYml.Split("`n");

for ($i = 0; $i -lt $lines.Count; $i++) {
    if ($lines[$i].Contains("- name: $($releaseLabel)")) {
        Write-Output "Existing entry found, removing."
        # Find the line with the existing name
        # Remove that line + 2 lines down (href and homepage)

        $lines.RemoveAt($i);
        $lines.RemoveAt($i);
        $lines.RemoveAt($i);
        $tocYml = $lines -Join "`n";
        break;
    }
}
 
$newTocEntry = "
- name: $($releaseLabel)
  href: $relativeMdPath/$releaseLabel.md
  homepage: $relativeMdPath/$releaseLabel.md"
    
$tocYml = $newTocEntry + $tocYml;


Write-Output "Saving to $tocYmlPath"
Set-Content -Path $tocYmlPath -Value $tocYml.TrimEnd() -ErrorAction Stop;

Write-Output "Done";
