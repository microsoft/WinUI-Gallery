[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$Base,

    [Parameter(Mandatory = $true)]
    [string]$Registrations,

    [Parameter(Mandatory = $true)]
    [string]$Out
)

$ErrorActionPreference = 'Stop'

foreach ($path in @($Base, $Registrations)) {
    if (-not (Test-Path -LiteralPath $path)) {
        throw "Required manifest input does not exist: $path"
    }
}

[xml]$baseDocument = [System.IO.File]::ReadAllText($Base)
[xml]$registrationDocument = [System.IO.File]::ReadAllText($Registrations)

$assembly = $baseDocument.DocumentElement
if ($null -eq $assembly -or $assembly.LocalName -ne 'assembly') {
    throw "Base manifest '$Base' does not contain an assembly root."
}

$appxNamespace = $registrationDocument.DocumentElement.NamespaceURI
$namespaceManager = [System.Xml.XmlNamespaceManager]::new($registrationDocument.NameTable)
$namespaceManager.AddNamespace('m', $appxNamespace)

$serverNodes = $registrationDocument.SelectNodes(
    '/m:Data/m:Extension/m:InProcessServer',
    $namespaceManager)

$requiredDlls = @(
    'Microsoft.UI.Xaml.dll',
    'Microsoft.UI.Xaml.Controls.dll',
    'Microsoft.UI.Xaml.Phone.dll',
    'WinUIEdit.dll'
)

$assemblyNamespace = 'urn:schemas-microsoft-com:asm.v3'
$winrtNamespace = 'urn:schemas-microsoft-com:winrt.v1'
$addedClasses = [System.Collections.Generic.HashSet[string]]::new(
    [System.StringComparer]::Ordinal)
$addedDlls = 0

foreach ($requiredDll in $requiredDlls) {
    $server = $serverNodes |
        Where-Object {
            $pathNode = $_.SelectSingleNode('./m:Path', $namespaceManager)
            $null -ne $pathNode -and
                $pathNode.InnerText.Equals(
                    $requiredDll,
                    [System.StringComparison]::OrdinalIgnoreCase)
        } |
        Select-Object -First 1

    if ($null -eq $server) {
        throw "Registration file '$Registrations' does not contain '$requiredDll'."
    }

    $fileElement = $baseDocument.CreateElement('asmv3', 'file', $assemblyNamespace)
    $fileElement.SetAttribute('name', $requiredDll)

    foreach ($classNode in $server.SelectNodes('./m:ActivatableClass', $namespaceManager)) {
        $className = $classNode.GetAttribute('ActivatableClassId')
        if ([string]::IsNullOrWhiteSpace($className)) {
            continue
        }

        $threadingModel = $classNode.GetAttribute('ThreadingModel')
        if ([string]::IsNullOrWhiteSpace($threadingModel)) {
            $threadingModel = 'both'
        }

        $classElement = $baseDocument.CreateElement(
            'winrtv1',
            'activatableClass',
            $winrtNamespace)
        $classElement.SetAttribute('name', $className)
        $classElement.SetAttribute('threadingModel', $threadingModel.ToLowerInvariant())
        [void]$fileElement.AppendChild($classElement)
        [void]$addedClasses.Add($className)
    }

    [void]$assembly.AppendChild($fileElement)
    $addedDlls++
}

foreach ($requiredClass in @(
    'Microsoft.UI.Xaml.Media.Animation.EntranceNavigationTransitionInfo',
    'Microsoft.UI.Xaml.Media.RevealBrush',
    'Microsoft.UI.Text.TextConstants',
    'Microsoft.UI.Xaml.XamlTypeInfo.XamlControlsXamlMetaDataProvider'
)) {
    if (-not $addedClasses.Contains($requiredClass)) {
        throw "Augmented manifest is missing required class '$requiredClass'."
    }
}

$outDirectory = Split-Path -Parent $Out
if (-not (Test-Path -LiteralPath $outDirectory)) {
    New-Item -ItemType Directory -Path $outDirectory -Force | Out-Null
}

$settings = [System.Xml.XmlWriterSettings]::new()
$settings.Encoding = [System.Text.UTF8Encoding]::new($false)
$settings.Indent = $true

$writer = [System.Xml.XmlWriter]::Create($Out, $settings)
try {
    $baseDocument.Save($writer)
}
finally {
    $writer.Dispose()
}

Write-Host "MergeWinUIAppManifest: added $($addedClasses.Count) classes across $addedDlls DLLs."
