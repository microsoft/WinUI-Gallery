<#
.SYNOPSIS
    Collects the pull requests and contributors for a WinUI Gallery release blog post.

.DESCRIPTION
    Lists the merged pull requests between the previous release tag and a target ref,
    groups them by area, and separates community contributors from the core team.
    Requires git and an authenticated GitHub CLI (gh).

.EXAMPLE
    .\Get-ReleaseData.ps1
    .\Get-ReleaseData.ps1 -FromTag v2.9.3 -ToRef origin/main -OutFile release-data.md
#>
[CmdletBinding()]
param(
    [string]$FromTag,
    [string]$ToRef = 'origin/main',
    [string]$Repo = 'microsoft/WinUI-Gallery',
    [string[]]$CoreTeam = @('niels9001', 'marcelwgn', 'Copilot', 'app/copilot-swe-agent', 'dependabot', 'app/dependabot'),
    [string]$OutFile
)

$ErrorActionPreference = 'Stop'

git fetch origin --tags --quiet

if (-not $FromTag) {
    $FromTag = gh release view --repo $Repo --json tagName --jq '.tagName'
}

$numbers = git log "$FromTag..$ToRef" --pretty=format:'%s' |
    ForEach-Object { if ($_ -match '\(#(\d+)\)\s*$') { [int]$Matches[1] } } |
    Sort-Object -Unique

$prs = foreach ($number in $numbers) {
    $pr = gh pr view $number --repo $Repo --json number,title,author,url | ConvertFrom-Json
    $title = $pr.title
    $area = switch -Regex ($title) {
        '(?i)accessib|a11y|narrator|screen reader|contrast|axe|keyboard|focus|accessible' { 'Accessibility'; break }
        '(?i)pipeline|build|\bci\b|wapproj|bump|update .*(sdk|package)|nuget|version|publish' { 'Infrastructure'; break }
        '(?i)\bfix|crash|bug|correct|issue' { 'Fixes'; break }
        '(?i)\badd|new|sample|showcase|support|implement|enhance|improve|inking' { 'Features and samples'; break }
        default { 'Other' }
    }
    [pscustomobject]@{
        Number    = $pr.number
        Title     = $title
        Author    = $pr.author.login
        Url       = $pr.url
        Area      = $area
        Community = $CoreTeam -notcontains $pr.author.login
    }
}

$sb = [System.Text.StringBuilder]::new()
[void]$sb.AppendLine("# Release data: $FromTag..$ToRef")
[void]$sb.AppendLine()
[void]$sb.AppendLine("Pull requests: $(@($prs).Count)")
[void]$sb.AppendLine("Full changelog: https://github.com/$Repo/compare/$FromTag...<new-tag>")
[void]$sb.AppendLine()

foreach ($group in ($prs | Group-Object Area | Sort-Object Name)) {
    [void]$sb.AppendLine("## $($group.Name)")
    [void]$sb.AppendLine()
    foreach ($pr in ($group.Group | Sort-Object Number)) {
        [void]$sb.AppendLine("- $($pr.Title) ([#$($pr.Number)]($($pr.Url))) by @$($pr.Author)")
    }
    [void]$sb.AppendLine()
}

[void]$sb.AppendLine('## Contributors outside the core team')
[void]$sb.AppendLine()
[void]$sb.AppendLine('Review this list: Microsoft employees on other teams are thanked differently than community members.')
[void]$sb.AppendLine()
foreach ($author in ($prs | Where-Object Community | Group-Object Author | Sort-Object Count -Descending)) {
    $links = ($author.Group | Sort-Object Number | ForEach-Object { "#$($_.Number)" }) -join ', '
    [void]$sb.AppendLine("- [@$($author.Name)](https://github.com/$($author.Name)): $links")
}

$report = $sb.ToString()
if ($OutFile) {
    Set-Content -Path $OutFile -Value $report -Encoding utf8
    Write-Host "Wrote $OutFile"
}
else {
    $report
}
