param(
    [Parameter(Mandatory = $true)]
    [string]$PayloadRoot,

    [Parameter(Mandatory = $true)]
    [string]$RuntimePayloadRoot,

    [Parameter(Mandatory = $true)]
    [string]$OutputRoot
)

$ErrorActionPreference = 'Stop'

if (-not (Test-Path $PayloadRoot -PathType Container)) {
    throw "Subtrial A payload directory not found: $PayloadRoot"
}

if (-not (Test-Path $OutputRoot -PathType Container)) {
    throw "Gallery output directory not found: $OutputRoot"
}

$runtimeFiles = @(
    'Microsoft.Graphics.Display.dll',
    'Microsoft.InputStateManager.dll',
    'Microsoft.Internal.FrameworkUdk.dll',
    'Microsoft.UI.dll',
    'Microsoft.UI.Input.dll',
    'Microsoft.UI.pri',
    'Microsoft.UI.Windowing.Core.dll',
    'Microsoft.UI.Windowing.dll'
)

$files = @(
    'Microsoft.UI.Xaml.Controls.dll',
    'Microsoft.UI.Xaml.Controls.pri',
    'Microsoft.UI.Xaml.Controls.Tabular.dll',
    'Microsoft.UI.Xaml.Controls.Tabular.pri',
    'Microsoft.UI.Xaml.Internal.dll',
    'Microsoft.UI.Xaml.Phone.dll',
    'Microsoft.UI.Xaml.winmd',
    'Microsoft.ui.xaml.dll',
    'Microsoft.ui.xaml.resources.19h1.dll',
    'Microsoft.ui.xaml.resources.common.dll',
    'Microsoft.WinUI.dll',
    'Microsoft.Windows.ApplicationModel.Resources.dll',
    'MRM.dll',
    'WinUIEdit.dll'
)

if (-not (Test-Path $RuntimePayloadRoot -PathType Container)) {
    throw "Subtrial A runtime payload directory not found: $RuntimePayloadRoot"
}

foreach ($file in $runtimeFiles) {
    $source = Join-Path $RuntimePayloadRoot $file
    if (-not (Test-Path $source -PathType Leaf)) {
        throw "Required Subtrial A runtime file not found: $source"
    }

    $destination = Join-Path $OutputRoot $file
    if ([IO.Path]::GetFullPath($source) -ne [IO.Path]::GetFullPath($destination)) {
        Copy-Item $source $destination -Force
    }
}

foreach ($file in $files) {
    $source = Join-Path $PayloadRoot $file
    if (-not (Test-Path $source -PathType Leaf)) {
        throw "Required Subtrial A payload file not found: $source"
    }

    $destination = Join-Path $OutputRoot $file
    if ([IO.Path]::GetFullPath($source) -ne [IO.Path]::GetFullPath($destination)) {
        Copy-Item $source $destination -Force
    }
}

$forbiddenFiles = @(
    'Microsoft.UI.Composition.dll',
    'Microsoft.UI.Dispatching.dll',
    'Microsoft.UI.Content.dll',
    'CoreMessagingXP.dll',
    'dcompi.dll',
    'Microsoft.DirectManipulation.dll',
    'Microsoft.UI.Composition.OSSupport.dll',
    'DwmSceneI.dll',
    'dwmcorei.dll',
    'marshal.dll',
    'wuceffectsi.dll'
)

foreach ($file in $forbiddenFiles) {
    $path = Join-Path $OutputRoot $file
    if (Test-Path $path -PathType Leaf) {
        Remove-Item $path -Force
    }
}

$runtimeFiles | ForEach-Object {
    $source = Get-FileHash (Join-Path $RuntimePayloadRoot $_)
    $destination = Get-FileHash (Join-Path $OutputRoot $_)
    if ($source.Hash -ne $destination.Hash) {
        throw "Runtime payload hash mismatch: $_"
    }
}

$files | ForEach-Object {
    $source = Get-FileHash (Join-Path $PayloadRoot $_)
    $destination = Get-FileHash (Join-Path $OutputRoot $_)
    if ($source.Hash -ne $destination.Hash) {
        throw "Payload hash mismatch: $_"
    }
}
