@echo off
:: Get the directory of the current batch file and append the script name
set "ScriptPath=C:\Users\grimm\source\repos\ProjWizInc\z_mapper.ps1"

:: Check if the PowerShell script exists before attempting to run it
if not exist "%ScriptPath%" (
    echo [ERROR] Could not find 'z_mapper.ps1' in this directory.
    echo Expected location: "%ScriptPath%"
    echo.
    pause
    exit /b 1
)

echo Starting C# Map Generator...
echo.

:: Launch PowerShell with execution policy bypass to ensure the script runs
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%ScriptPath%"

echo.
echo Process finished.
pause