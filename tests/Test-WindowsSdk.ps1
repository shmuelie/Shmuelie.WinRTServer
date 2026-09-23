#Requires -Version 7.0
[CmdletBinding()]
param(
    [string] $WindowsSdkRoot = (Get-ItemPropertyValue 'HKLM:\SOFTWARE\Microsoft\Windows Kits\Installed Roots' -Name KitsRoot10 -ErrorAction Stop)
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

[xml] $props = Get-Content (Join-Path $PSScriptRoot 'Directory.Build.props') -Raw
$version = $props.Project.PropertyGroup.SampleWindowsSdkVersion
if ($version -notmatch '^\d+\.\d+\.\d+\.\d+$') {
    throw 'SampleWindowsSdkVersion in tests\Directory.Build.props must be a concrete Windows SDK version.'
}

$requiredPaths = @(
    "Platforms\UAP\$version\Platform.xml",
    "UnionMetadata\$version\Windows.winmd",
    "Include\$version\um\Windows.h",
    "Include\$version\winrt\Windows.Foundation.h",
    "Lib\$version\um\x64\kernel32.lib",
    "Lib\$version\ucrt\x64\ucrt.lib"
)
$requiredPaths += 'midl', 'midlrt', 'rc', 'makepri', 'makeappx' | ForEach-Object { "bin\$version\x64\$_.exe" }
$missing = @($requiredPaths | Where-Object { !(Test-Path -LiteralPath (Join-Path $WindowsSdkRoot $_) -PathType Leaf) })
if ($missing.Count) {
    throw "Windows SDK $version is incomplete or missing under '$WindowsSdkRoot'. Install it with the Visual Studio Installer (C++ tools and UWP development support). Missing files:`n$($missing -join "`n")"
}

Write-Information "Windows SDK $version prerequisites are available under '$WindowsSdkRoot'." -InformationAction Continue
