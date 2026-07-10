@echo off
setlocal enabledelayedexpansion

:: Define output file name
set "OUTPUT_FILE=z_solution.txt"

:: Clear or create the output file
type nul > "%OUTPUT_FILE%"

echo Scanning and merging C# files...
echo ===================================

:: Loop through all .cs files recursively
for /r %%F in (*.cs) do (
    set "FILE_PATH=%%F"
    
    :: Skip files located inside "obj" or "bin" directories
    echo !FILE_PATH! | findstr /i "\\obj\\ \\bin\\" >nul
    if errorlevel 1 (
        echo Adding: %%~nxF
        
        :: Write a visual header divider to the text file
        echo ============================================================================== >> "%OUTPUT_FILE%"
        echo FILE: %%F >> "%OUTPUT_FILE%"
        echo ============================================================================== >> "%OUTPUT_FILE%"
        echo. >> "%OUTPUT_FILE%"
        
        :: Append the actual code content
        type "%%F" >> "%OUTPUT_FILE%"
        
        :: Append extra spacing at the end of each file
        echo. >> "%OUTPUT_FILE%"
        echo. >> "%OUTPUT_FILE%"
    )
)

echo ===================================
echo Done! All C# code merged into: %OUTPUT_FILE%
pause
