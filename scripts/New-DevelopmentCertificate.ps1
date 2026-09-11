#requires -Version 7.3
[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$certificateDirectory = Join-Path $root '.certificates'
$certificatePath = Join-Path $certificateDirectory 'WinUIGallery_Development.pfx'
New-Item -ItemType Directory -Path $certificateDirectory -Force | Out-Null

& winapp cert generate --manifest (Join-Path $root 'WinUIGallery\Debug.appxmanifest') `
    --output $certificatePath --password '' --valid-days 365 --export-cer --if-exists Skip --json
if ($LASTEXITCODE -ne 0) {
    throw "WinAppCLI could not generate the development certificate (exit $LASTEXITCODE)."
}
