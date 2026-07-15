@echo off
REM Build script for ExplorerTabUtility
REM Usage: .\build.cmd [Debug|Release]

setlocal

set "CONFIG=%~1"
if "%CONFIG%"=="" set "CONFIG=Debug"

REM Find VS 2022 using vswhere
for /f "tokens=*" %%i in ('"%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" -latest -property installationPath') do set VS_PATH=%%i

if not defined VS_PATH (
    echo Error: Visual Studio 2022 not found. Please install Visual Studio 2022.
    exit /b 1
)

REM Set up SDK path for MSBuild
set "MSBuildSDKsPath=C:\Program Files\dotnet\sdk\10.0.302\Sdks"

REM Build
echo Building ExplorerTabUtility (%CONFIG%)...
"%VS_PATH%\MSBuild\Current\Bin\MSBuild.exe" ExplorerTabUtility.sln /t:Rebuild /p:Configuration=%CONFIG% /v:m

if %ERRORLEVEL% EQU 0 (
    echo.
    echo Build succeeded!
    echo Output: ExplorerTabUtility\bin\%CONFIG%\net481\ExplorerTabUtility.exe
) else (
    echo.
    echo Build failed!
)

endlocal