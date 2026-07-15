# Build script for Explorer Tab Utility
# Uses Visual Studio MSBuild as COM references aren't supported by dotnet build

$ErrorActionPreference = "Stop"

$ProjectPath = "ExplorerTabUtility\ExplorerTabUtility.csproj"
$Configuration = "Release"

# Find VS installation using vswhere
$VSInstallPath = & "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" -latest -property installationPath

$MSBuild = $null
if ($VSInstallPath) {
    $MSBuildPath = Join-Path $VSInstallPath "MSBuild\Current\Bin\MSBuild.exe"
    if (Test-Path $MSBuildPath) {
        $MSBuild = $MSBuildPath
    }
}

if (-not $MSBuild) {
    Write-Error "Visual Studio 2022 MSBuild not found. Please install Visual Studio 2022."
    exit 1
}

Write-Host "Using MSBuild: $MSBuild" -ForegroundColor Cyan

# Set SDK path for MSBuild to resolve Microsoft.NET.Sdk
$env:MSBuildSDKsPath = "C:\Program Files\dotnet\sdk\10.0.302\Sdks"

# Build
& $MSBuild $ProjectPath /t:Restore /p:Configuration=$Configuration /v:minimal
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

& $MSBuild $ProjectPath /t:Build /p:Configuration=$Configuration /v:minimal

if ($LASTEXITCODE -ne 0) {
    Write-Error "Build failed with exit code $LASTEXITCODE"
    exit $LASTEXITCODE
}

Write-Host "Build completed successfully!" -ForegroundColor Green