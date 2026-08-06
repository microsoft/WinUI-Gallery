[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$Package,

    [Parameter(Mandatory = $true)]
    [string]$Entry,

    [Parameter(Mandatory = $true)]
    [string]$Out
)

$ErrorActionPreference = 'Stop'

if (-not (Test-Path -LiteralPath $Package)) {
    throw "App package does not exist: $Package"
}

Add-Type -AssemblyName System.IO.Compression.FileSystem

$archive = [System.IO.Compression.ZipFile]::OpenRead($Package)
try {
    $packageEntry = $archive.GetEntry($Entry)
    if ($null -eq $packageEntry) {
        throw "App package '$Package' does not contain '$Entry'."
    }

    $outDirectory = Split-Path -Parent $Out
    if (-not (Test-Path -LiteralPath $outDirectory)) {
        New-Item -ItemType Directory -Path $outDirectory -Force | Out-Null
    }

    $temporaryOut = "$Out.$PID.tmp"
    $source = $packageEntry.Open()
    try {
        $destination = [System.IO.File]::Create($temporaryOut)
        try {
            $source.CopyTo($destination)
        }
        finally {
            $destination.Dispose()
        }
    }
    finally {
        $source.Dispose()
    }

    Move-Item -LiteralPath $temporaryOut -Destination $Out -Force
    [System.IO.File]::SetLastWriteTimeUtc($Out, $packageEntry.LastWriteTime.UtcDateTime)
}
finally {
    $archive.Dispose()
    if ($null -ne $temporaryOut -and (Test-Path -LiteralPath $temporaryOut)) {
        Remove-Item -LiteralPath $temporaryOut -Force
    }
}
