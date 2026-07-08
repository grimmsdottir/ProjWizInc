@echo off
cd /d "C:\Users\grimm\source\repos\ProjWizInc"
powershell -Command "Get-ChildItem -Recurse -Filter *.cs | Where-Object { $_.FullName -notmatch '\\(obj|bin|Packages)\\?' } | ForEach-Object { Write-Output '=== FILE: $_.Name ==='; Get-Content $_.FullName; Write-Output '`n' } > combined_solution.txt"
echo Done! Your code has been merged into combined_solution.txt
