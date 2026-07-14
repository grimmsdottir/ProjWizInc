@echo off
setlocal enabledelayedexpansion

:: 1. Define your GitHub base URL (Fixed to include /blob/master/)
:: If your branch is 'main', change 'master' to 'main' below.
set "BASE_URL=https://github.com/grimmsdottir/ProjWizInc/blob/master/"

:: 2. Get the current directory
set "ROOT_DIR=%~dp0"

:: 3. Define the output file name
set "OUTPUT_FILE=z_cs_links.txt"

echo Generating filtered links for .cs files...

:: Create/Overwrite the file and start the loop
(
    for /r %%f in (*.cs) do (
        set "FULL_PATH=%%f"
        
        :: Filter: Skip files in 'obj' or 'bin' folders
        set "SKIP=0"
        echo !FULL_PATH! | findstr /i "\\obj\\" >nul && set "SKIP=1"
        echo !FULL_PATH! | findstr /i "\\bin\\" >nul && set "SKIP=1"
        
        if !SKIP! equ 0 (
            :: Remove the local root path
            set "REL_PATH=!FULL_PATH:%ROOT_DIR%=!"
            
            :: Replace backslashes with forward slashes
            set "URL_PATH=!REL_PATH:\=/!"
            
            :: Print the final direct link
            echo !BASE_URL!!URL_PATH!
        )
    )
) > "%OUTPUT_FILE%"

echo Success! Links saved to %OUTPUT_FILE% (bin/obj files excluded).
pause