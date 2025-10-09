<#
upload_release.ps1

Helper script to create a GitHub release and upload the pre-built ZIP artifacts.
Requires the GitHub CLI (`gh`) to be installed and authenticated.
Usage:
  .\upload_release.ps1 -Tag v1.1 -Title "v1.1" -NotesFile .\RELEASE_NOTES.md
#>

param(
    [Parameter(Mandatory=$true)][string]$Tag,
    [Parameter(Mandatory=$true)][string]$Title,
    [Parameter(Mandatory=$true)][string]$NotesFile
)

$publishDir = Join-Path -Path $PSScriptRoot -ChildPath "FileConverter\publish"
$artifacts = @(
    Join-Path $publishDir 'FileConverter-win-x86-selfcontained.zip',
    Join-Path $publishDir 'FileConverter-win-x64-selfcontained.zip',
    Join-Path $publishDir 'FileConverter-linux-x64-selfcontained.zip'
)

if (-not (Get-Command gh -ErrorAction SilentlyContinue)) {
    Write-Error "GitHub CLI 'gh' is not installed or not in PATH. Install it from https://cli.github.com/"
    exit 1
}

$notes = Get-Content -Raw -Path $NotesFile

# Create release
gh release create $Tag --title $Title --notes "$notes"

# Upload artifacts
foreach ($a in $artifacts) {
    if (Test-Path $a) {
        gh release upload $Tag $a
    } else {
        Write-Warning "Artifact not found: $a"
    }
}
