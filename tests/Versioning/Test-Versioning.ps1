#Requires -Version 7.0
[CmdletBinding()]
param(
    [switch] $Packages,
    [string] $MSBuildPath = 'msbuild',
    [string] $RestoreSource = 'https://api.nuget.org/v3/index.json'
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$msbuild = (Get-Command $MSBuildPath -CommandType Application -ErrorAction Stop | Select-Object -First 1).Source
$repoRoot = Split-Path (Split-Path $PSScriptRoot)
$project = Join-Path $repoRoot 'src\Shmuelie.WinRTServer\Shmuelie.WinRTServer.csproj'
$testRoot = Join-Path $repoRoot "artifacts\versioning\$([guid]::NewGuid())"
New-Item -ItemType Directory -Path $testRoot | Out-Null
[xml] $buildProps = Get-Content (Join-Path $repoRoot 'Directory.Build.props') -Raw
$baseVersion = $buildProps.SelectSingleNode('//ShmuelieWinRTServerPackageVersion').InnerText
$sha = '0123456789012345678901234567890123456789'
$ci = @{
    GITHUB_ACTIONS = 'true'
    GITHUB_EVENT_NAME = 'push'
    GITHUB_REF_NAME = 'main'
    GITHUB_REF_TYPE = 'branch'
    GITHUB_RUN_ID = '12345'
    GITHUB_SHA = $sha
}

function Invoke-Build {
    param(
        [string] $ProjectPath = $project,
        [hashtable] $Properties = @{},
        [string[]] $BuildArguments = @(),
        [string] $ExpectedError
    )

    $arguments = @($ProjectPath, '-nologo', '-verbosity:quiet', "-bl:$testRoot\$([guid]::NewGuid()).binlog")
    $environmentProperties = @{
        GITHUB_ACTIONS = ''
        GITHUB_EVENT_NAME = ''
        GITHUB_REF_NAME = ''
        GITHUB_REF_TYPE = ''
        GITHUB_RUN_ID = ''
        GITHUB_RUN_ATTEMPT = ''
        GITHUB_SHA = ''
    }
    foreach ($key in $Properties.Keys) {
        $environmentProperties[$key] = $Properties[$key]
    }
    foreach ($key in $environmentProperties.Keys) {
        $arguments += "-p:$key=$($environmentProperties[$key])"
    }
    $output = & $msbuild @arguments @BuildArguments 2>&1 | Out-String
    $exitCode = $LASTEXITCODE
    if ($ExpectedError) {
        if ($exitCode -eq 0 -or $output -notmatch [regex]::Escape($ExpectedError)) {
            throw "Expected $ExpectedError, received exit code ${exitCode}:`n$output"
        }
    }
    elseif ($exitCode -ne 0) {
        throw "MSBuild failed with exit code ${exitCode}:`n$output"
    }
    return $output
}

function Assert-Version {
    param([string] $Name, [hashtable] $Properties, [string] $ExpectedVersion)

    $output = Invoke-Build -Properties $Properties -BuildArguments @(
        '-t:ValidatePackageVersion',
        '-getProperty:Version,PackageVersion,VersionPrefix,VersionSuffix,AssemblyVersion,FileVersion,InformationalVersion'
    )
    $jsonStart = $output.IndexOf('{')
    if ($jsonStart -lt 0) {
        throw "No version properties returned for ${Name}:`n$output"
    }
    $actual = ($output.Substring($jsonStart) | ConvertFrom-Json).Properties
    $identityVersion = $ExpectedVersion.Split('+')[0]
    $prefix, $suffix = $identityVersion.Split('-', 2)
    if (!$suffix) { $suffix = '' }
    $expected = @{
        Version = $ExpectedVersion
        PackageVersion = $ExpectedVersion
        InformationalVersion = $ExpectedVersion
        VersionPrefix = $prefix
        VersionSuffix = $suffix
        AssemblyVersion = "$prefix.0"
        FileVersion = "$prefix.0"
    }
    foreach ($key in $expected.Keys) {
        if ($actual.$key -cne $expected[$key]) {
            throw "$Name - ${key}: expected '$($expected[$key])', found '$($actual.$key)'."
        }
    }
    Write-Information "PASS: $Name" -InformationAction Continue
}

$cases = @(
    @{ Name = 'local default'; Properties = @{}; Version = "$baseVersion-alpha" },
    @{ Name = 'local prefix'; Properties = @{ VersionPrefix = '2.3.0' }; Version = '2.3.0-alpha' },
    @{ Name = 'local suffix'; Properties = @{ VersionSuffix = 'beta.2' }; Version = "$baseVersion-beta.2" },
    @{ Name = 'local prefix and suffix'; Properties = @{ VersionPrefix = '2.3.0'; VersionSuffix = 'rc.1' }; Version = '2.3.0-rc.1' }
)
foreach ($version in @('2.3.0', '2.3.0-preview.1', '2.3.0-beta.2', '2.3.0-rc.1', '2.3.0-preview.foo.1',
        '2.3.0-preview-name.0', '2.3.0-0', '2.3.0-01a', '2.3.0+build.01', '2.3.0-rc.1+build.01', '65534.65534.65534')) {
    $cases += @{ Name = "local $version"; Properties = @{ Version = $version }; Version = $version }
}
$cases += @{ Name = 'nightly'; Properties = $ci.Clone(); Version = "$baseVersion-alpha.12345+$sha" }
$rerun = $ci.Clone()
$rerun.GITHUB_RUN_ATTEMPT = '2'
$cases += @{ Name = 'nightly retry retains identity'; Properties = $rerun; Version = "$baseVersion-alpha.12345+$sha" }
$nextRun = $ci.Clone()
$nextRun.GITHUB_RUN_ID = '12346'
$cases += @{ Name = 'next nightly has unique identity'; Properties = $nextRun; Version = "$baseVersion-alpha.12346+$sha" }
$pr = $ci.Clone()
$pr.GITHUB_EVENT_NAME = 'pull_request'
$pr.GITHUB_REF_NAME = 'rel/2.3.0'
$cases += @{ Name = 'PR cannot become a stable release'; Properties = $pr; Version = "$baseVersion-pr.12345+$sha" }
$tag = $ci.Clone()
$tag.GITHUB_REF_TYPE = 'tag'
$tag.GITHUB_REF_NAME = 'rel/2.3.0'
$cases += @{ Name = 'tag is not a release branch'; Properties = $tag; Version = "$baseVersion-alpha.12345+$sha" }
foreach ($version in @('2.3.0', '2.3.0-rc.1', '2.3.0-preview.foo.1', '2.3.0-preview-name.0', '2.3.0-rc.1+build.01')) {
    $release = $ci.Clone()
    $release.GITHUB_REF_NAME = "rel/$version"
    $metadataSeparator = if ($version.Contains('+')) { '.' } else { '+' }
    $cases += @{ Name = "release $version"; Properties = $release; Version = "$version$metadataSeparator$sha" }
}
$matchingCI = $ci.Clone()
$matchingCI.Version = "$baseVersion-alpha.12345+$sha"
$cases += @{ Name = 'matching CI input'; Properties = $matchingCI; Version = $matchingCI.Version }

foreach ($case in $cases) {
    Assert-Version -Name $case.Name -Properties $case.Properties -ExpectedVersion $case.Version
}

$failures = @()
foreach ($version in @('2.3', '02.3.0', '2.03.0', '2.3.00', '2.3.0-preview_1', '2.3.0-01',
        '2.3.0-preview.01', '2.3.0-preview..1', '2.3.0-', '2.3.0+', '2.3.0+build..1', '2.3.0+build_1')) {
    $failures += @{ Name = "invalid local $version"; Properties = @{ Version = $version }; Error = 'WRTVER002' }
    $release = $ci.Clone()
    $release.GITHUB_REF_NAME = "rel/$version"
    $failures += @{ Name = "invalid release $version"; Properties = $release; Error = 'WRTVER002' }
}
foreach ($version in @('65535.0.0', '0.65535.0', '0.0.65535', '999999999999999999999.0.0')) {
    $failures += @{ Name = "assembly range $version"; Properties = @{ Version = $version }; Error = 'WRTVER003' }
}
$failures += @(
    @{ Name = 'conflicting local prefix'; Properties = @{ Version = '2.3.0-rc.1'; VersionPrefix = '2.4.0' }; Error = 'WRTVER004' },
    @{ Name = 'conflicting local suffix'; Properties = @{ Version = '2.3.0'; VersionSuffix = 'rc.1' }; Error = 'WRTVER004' },
    @{ Name = 'suffix casing mismatch'; Properties = @{ Version = '2.3.0-RC.1'; VersionSuffix = 'rc.1' }; Error = 'WRTVER004' },
    @{ Name = 'package casing mismatch'; Properties = @{ Version = '2.3.0-RC.1'; PackageVersion = '2.3.0-rc.1' }; Error = 'WRTVER004' },
    @{ Name = 'independent PackageVersion'; Properties = @{ PackageVersion = '2.3.0' }; Error = 'WRTVER004' }
)
foreach ($override in @(
        @{ Version = '2.3.0' }, @{ Version = '2.3.0-rc.1' }, @{ VersionPrefix = '2.3.0' },
        @{ VersionSuffix = 'rc.1' }, @{ PackageVersion = '2.3.0' },
        @{ AssemblyVersion = '2.3.0.0' }, @{ FileVersion = '2.3.0.0' }, @{ InformationalVersion = '2.3.0' })) {
    $properties = $ci.Clone()
    foreach ($key in $override.Keys) { $properties[$key] = $override[$key] }
    $failures += @{ Name = "conflicting CI $($override.Keys -join ',')"; Properties = $properties; Error = 'WRTVER004' }
}
foreach ($invalidContext in @(
        @{ GITHUB_RUN_ID = '' }, @{ GITHUB_RUN_ID = '0' }, @{ GITHUB_RUN_ID = '01' },
        @{ GITHUB_RUN_ID = 'abc' }, @{ GITHUB_SHA = 'invalid' }, @{ GITHUB_EVENT_NAME = 'workflow_dispatch' })) {
    $properties = $ci.Clone()
    foreach ($key in $invalidContext.Keys) { $properties[$key] = $invalidContext[$key] }
    $failures += @{ Name = "invalid CI $($invalidContext.Keys -join ',')=$($invalidContext.Values -join ',')"; Properties = $properties; Error = 'WRTVER001' }
}
foreach ($case in $failures) {
    $null = Invoke-Build -Properties $case.Properties -BuildArguments '-t:ValidatePackageVersion' -ExpectedError $case.Error
    Write-Information "PASS: $($case.Name)" -InformationAction Continue
}
foreach ($target in @('Build', 'Pack', 'Restore')) {
    $properties = @{ Version = '2.3.0-preview_1' }
    if ($target -eq 'Pack') { $properties.NoBuild = 'true' }
    $null = Invoke-Build -Properties $properties -BuildArguments "-t:$target" -ExpectedError 'WRTVER002'
    Write-Information "PASS: $target rejects invalid versions" -InformationAction Continue
}

if ($Packages) {
    $feed = Join-Path $testRoot 'feed'
    New-Item -ItemType Directory -Path $feed | Out-Null
    $packageCases = @(
        @{ Name = 'stable'; Properties = @{ Version = '2.3.0' }; Version = '2.3.0' },
        @{ Name = 'preview'; Properties = @{ Version = '2.4.0-preview.1' }; Version = '2.4.0-preview.1' },
        @{ Name = 'nightly'; Properties = $ci.Clone(); Version = "$baseVersion-alpha.12345+$sha" },
        @{ Name = 'PR'; Properties = $pr.Clone(); Version = "$baseVersion-pr.12345+$sha" },
        ($cases | Where-Object Name -EQ 'release 2.3.0-rc.1'),
        ($cases | Where-Object Name -EQ 'release 2.3.0')
    )
    foreach ($case in $packageCases) {
        $outputPath = Join-Path $testRoot $case.Name.Replace(' ', '-')
        $properties = $case.Properties.Clone()
        $properties.Configuration = 'Release'
        $properties.PackageOutputPath = $outputPath
        $properties.RestoreSources = $RestoreSource
        $null = Invoke-Build -Properties $properties -BuildArguments @('-restore', '-t:Build;Pack')
        $packagesFound = @(Get-ChildItem -LiteralPath $outputPath -Filter '*.nupkg')
        if ($packagesFound.Count -ne 1) {
            throw "Expected one package for $($case.Name), found $($packagesFound.Count)."
        }
        & (Join-Path $PSScriptRoot 'Test-Package.ps1') -PackagePath $packagesFound[0].FullName -ExpectedVersion $case.Version
        if ($case.Name -in @('stable', 'preview')) {
            Copy-Item -LiteralPath $packagesFound[0].FullName -Destination $feed
        }
    }

    # Isolate consumers from this repository's build imports and the user's package sources/cache.
    $consumerRoot = Join-Path $testRoot 'consumers'
    New-Item -ItemType Directory -Path $consumerRoot | Out-Null
    Set-Content -LiteralPath (Join-Path $consumerRoot 'Directory.Build.props') -Value '<Project />'
    Set-Content -LiteralPath (Join-Path $consumerRoot 'Directory.Build.targets') -Value '<Project />'
    $escapedFeed = [System.Security.SecurityElement]::Escape($feed)
    $escapedSource = [System.Security.SecurityElement]::Escape($RestoreSource)
    Set-Content -LiteralPath (Join-Path $consumerRoot 'NuGet.Config') -Value @"
<configuration><packageSources><clear /><add key="test" value="$escapedFeed" /><add key="dependencies" value="$escapedSource" /></packageSources><packageSourceMapping><clear /><packageSource key="test"><package pattern="Shmuelie.WinRTServer" /></packageSource><packageSource key="dependencies"><package pattern="*" /></packageSource></packageSourceMapping></configuration>
"@
    foreach ($selection in @(
            @{ Name = 'stable-only'; Range = '*'; Version = '2.3.0' },
            @{ Name = 'prerelease-opt-in'; Range = '*-*'; Version = '2.4.0-preview.1' })) {
        $consumerPath = Join-Path $consumerRoot $selection.Name
        New-Item -ItemType Directory -Path $consumerPath | Out-Null
        $consumerProject = Join-Path $consumerPath 'Consumer.csproj'
        Set-Content -LiteralPath $consumerProject -Value @"
<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><TargetFramework>net8.0-windows10.0.22000.0</TargetFramework></PropertyGroup><ItemGroup><PackageReference Include="Shmuelie.WinRTServer" Version="$($selection.Range)" /></ItemGroup></Project>
"@
        $null = Invoke-Build -ProjectPath $consumerProject -Properties @{
            RestorePackagesPath = (Join-Path $consumerPath 'packages')
            RestoreConfigFile = (Join-Path $consumerRoot 'NuGet.Config')
        } -BuildArguments '-t:Restore'
        $assets = Get-Content (Join-Path $consumerPath 'obj\project.assets.json') -Raw | ConvertFrom-Json
        $resolved = @($assets.libraries.PSObject.Properties.Name | Where-Object { $_.StartsWith('Shmuelie.WinRTServer/') })
        if ($resolved.Count -ne 1 -or $resolved[0] -cne "Shmuelie.WinRTServer/$($selection.Version)") {
            throw "$($selection.Name) restored an unexpected version: $($resolved -join ', ')."
        }
        Write-Information "PASS: $($selection.Name) restored $($selection.Version)" -InformationAction Continue
    }
}

Write-Information "Versioning checks passed. Logs and artifacts: $testRoot" -InformationAction Continue
