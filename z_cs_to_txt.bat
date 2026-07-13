@echo off
:: Change directory to where the script is located
cd /d "%~dp0"

echo Launching PowerShell merge script...
echo ===================================

:: -ExecutionPolicy Bypass: Allows the script to run without security prompts
:: -File: Specifies the path to your PowerShell script
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "z_cs_to_txt.ps1"

echo ===================================
echo Process Complete.
pause
