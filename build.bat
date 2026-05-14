@echo off
REM Build script for Explorer Tab Utility
REM Uses Visual Studio MSBuild as COM references aren't supported by dotnet build

set PROJECT_PATH=ExplorerTabUtility\ExplorerTabUtility.csproj
set CONFIGURATION=Release

REM Find Visual Studio 2022 MSBuild
set MSBUILD=
for /f "delims=" %%i in ('where /r "C:\Program Files\Microsoft Visual Studio\2022" MSBuild.exe 2^>nul') do (
    set "MSBUILD=%%i"
    goto :found
)

:found
if not defined MSBUILD (
    echo Error: Visual Studio 2022 MSBuild not found.
    echo Please install Visual Studio 2022.
    exit /b 1
)

echo Using MSBuild: %MSBUILD%

"%MSBUILD%" %PROJECT_PATH% /p:Configuration=%CONFIGURATION% /v:minimal

if errorlevel 1 (
    echo Build failed with exit code %errorlevel%
    exit /b %errorlevel%
)

echo Build completed successfully!