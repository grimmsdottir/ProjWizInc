# --- Configuration ---
$OutputFile = "z_SolutionChanges.txt"
$MarkerFile = ".last_run_marker"
$ExcludeFolders = @("bin", "obj", ".vs", "Properties")

# Get the last run time (if marker doesn't exist, go back 10 years)
if (Test-Path $MarkerFile) {
    $LastRun = Get-Item $MarkerFile | Select-Object -ExpandProperty LastWriteTime
} else {
    $LastRun = (Get-Date).AddYears(-10)
}

$CurrentRunTime = Get-Date
$FileCount = 0

# Clear the output file
"" > $OutputFile

# Find all .cs files recursively
$Files = Get-ChildItem -Recurse -Filter "*.cs" | Where-Object {
    $path = $_.FullName
    $modified = $_.LastWriteTime
    
    # Check if modified since last run AND not in an excluded folder
    $isModified = $modified -gt $LastRun
    $isNotExcluded = $true
    foreach ($folder in $ExcludeFolders) {
        if ($path -like "*\$folder\*") { $isNotExcluded = $false; break }
    }
    
    $isModified -and $isNotExcluded
}

foreach ($file in $Files) {
    $relativeName = $file.FullName.Replace($(Get-Location).Path, "")
    
    Add-Content $OutputFile "`n=============================================================================="
    Add-Content $OutputFile "FILE: $relativeName"
    Add-Content $OutputFile "==============================================================================`n"
    Add-Content $OutputFile (Get-Content $file.FullName -Raw)
    
    Write-Host "Added: $relativeName" -ForegroundColor Cyan
    $FileCount++
}

# Update the marker file timestamp
New-Item -ItemType File -Path $MarkerFile -Force | Out-Null

if ($FileCount -eq 0) {
    Write-Host "No files have been modified since the last run." -ForegroundColor Yellow
} else {
    Write-Host "`nDone! Added $FileCount files to $OutputFile" -ForegroundColor Green
}