# Build script for Explorer Tab Utility
# Uses Visual Studio MSBuild as COM references aren't supported by dotnet build

$ErrorActionPreference = "Stop"

$ProjectPath = "ExplorerTabUtility\ExplorerTabUtility.csproj"
$Configuration = "Release"

# Find Visual Studio 2022 MSBuild
$VS2022Paths = @(
    "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe",
    "C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe",
    "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe"
)

$MSBuild = $null
foreach ($path in $VS2022Paths) {
    if (Test-Path $path) {
        $MSBuild = $path
        break
    }
}

if (-not $MSBuild) {
    Write-Error "Visual Studio 2022 MSBuild not found. Please install Visual Studio 2022."
    exit 1
}

Write-Host "Using MSBuild: $MSBuild" -ForegroundColor Cyan

# Build
& $MSBuild $ProjectPath /p:Configuration=$Configuration /v:minimal

if ($LASTEXITCODE -ne 0) {
    Write-Error "Build failed with exit code $LASTEXITCODE"
    exit $LASTEXITCODE
}

Write-Host "Build completed successfully!" -ForegroundColor Green