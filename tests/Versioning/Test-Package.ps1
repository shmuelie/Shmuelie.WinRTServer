#Requires -Version 7.0
[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string] $PackagePath,

    [Parameter(Mandatory)]
    [string] $ExpectedVersion
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$package = Get-Item -LiteralPath $PackagePath
$identityVersion = $ExpectedVersion.Split('+')[0]
$assemblyVersion = ($identityVersion.Split('-')[0]) + '.0'
if ($package.Name -cne "Shmuelie.WinRTServer.$identityVersion.nupkg") {
    throw "Unexpected package filename '$($package.Name)' for version '$ExpectedVersion'."
}

$archive = [System.IO.Compression.ZipFile]::OpenRead($package.FullName)
$extractionPath = Join-Path ([System.IO.Path]::GetTempPath()) ([guid]::NewGuid().ToString())
try {
    $nuspecs = @($archive.Entries | Where-Object FullName -Like '*.nuspec')
    if ($nuspecs.Count -ne 1) {
        throw 'Expected exactly one package manifest.'
    }

    $reader = [System.IO.StreamReader]::new($nuspecs[0].Open())
    try {
        [xml] $manifest = $reader.ReadToEnd()
    }
    finally {
        $reader.Dispose()
    }

    if ($manifest.package.metadata.id -cne 'Shmuelie.WinRTServer' -or
        $manifest.package.metadata.version -cne $ExpectedVersion) {
        throw "Unexpected package identity: $($manifest.package.metadata.id) $($manifest.package.metadata.version). Expected Shmuelie.WinRTServer $ExpectedVersion."
    }

    $assemblies = @($archive.Entries | Where-Object FullName -Match '^lib/[^/]+/Shmuelie\.WinRTServer\.dll$')
    if ($assemblies.Count -eq 0) {
        throw 'The package contains no library assemblies.'
    }

    New-Item -ItemType Directory -Path $extractionPath | Out-Null
    foreach ($assembly in $assemblies) {
        $destination = Join-Path $extractionPath ([guid]::NewGuid().ToString() + '.dll')
        [System.IO.Compression.ZipFileExtensions]::ExtractToFile($assembly, $destination)
        $actualAssemblyVersion = [System.Reflection.AssemblyName]::GetAssemblyName($destination).Version.ToString()
        $fileInfo = [System.Diagnostics.FileVersionInfo]::GetVersionInfo($destination)
        if ($actualAssemblyVersion -cne $assemblyVersion -or $fileInfo.FileVersion -cne $assemblyVersion) {
            throw "Assembly/file version mismatch in '$($assembly.FullName)': expected $assemblyVersion, found $actualAssemblyVersion / $($fileInfo.FileVersion)."
        }

        $provenanceSeparator = if ($ExpectedVersion.Contains('+')) { '.' } else { '+' }
        if ($fileInfo.ProductVersion -cne $ExpectedVersion -and
            !$fileInfo.ProductVersion.StartsWith("$ExpectedVersion$provenanceSeparator", [StringComparison]::Ordinal)) {
            throw "Informational version '$($fileInfo.ProductVersion)' does not preserve '$ExpectedVersion'."
        }
    }
}
finally {
    $archive.Dispose()
    if (Test-Path -LiteralPath $extractionPath) {
        Remove-Item -LiteralPath $extractionPath -Recurse -Force
    }
}

Write-Information "Verified $($package.Name): package version $ExpectedVersion; assembly version $assemblyVersion." -InformationAction Continue
