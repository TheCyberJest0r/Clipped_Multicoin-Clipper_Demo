@echo off
setlocal

:: Find latest csc.exe (handles multiple .NET versions automatically)
for /f "delims=" %%i in ('dir /b /s /od "%WINDIR%\Microsoft.NET\Framework\*csc.exe" ^| findstr /e "csc.exe"') do set "CSC=%%i"

if "%CSC%"=="" (
    echo Error: csc.exe not found. Install .NET Framework/SDK.
    pause
    exit /b 1
)

echo Using CSC: %CSC%

%CSC% /nologo /target:exe /out:clipped.exe Program.cs

if %errorlevel%==0 (
    echo Compiled successfully! Run clipped.exe
    :: Uncomment next line to auto-run
    :: clipped.exe
) else (
    echo Compilation failed.
)

pause
