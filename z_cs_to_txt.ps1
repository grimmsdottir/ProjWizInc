# save as merge_files.ps1
$outputFile = "z_solution.txt"
$excludeFolders = @("obj", "bin", ".vs", ".git")

# Clear the output file
New-Item -ItemType File -Path $outputFile -Force | Out-Null

Get-ChildItem -Recurse -Filter "*.cs" | Where-Object { 
    $path = $_.FullName
    $shouldExclude = $false
    foreach ($folder in $excludeFolders) {
        if ($path -like "*\$folder\*") { $shouldExclude = $true; break }
    }
    -not $shouldExclude
} | ForEach-Object {
    Write-Host "Adding: $($_.Name)"
    $header = @"
==============================================================================
FILE: $($_.FullName)
==============================================================================
"@
    $header | Out-File -FilePath $outputFile -Append -Encoding utf8
    Get-Content $_.FullName | Out-File -FilePath $outputFile -Append -Encoding utf8
    "`n`n" | Out-File -FilePath $outputFile -Append -Encoding utf8
}

Write-Host "Done! Saved to $outputFile"

if (Test-Path ".git") {
    git add $outputFile
    $status = git status --porcelain $outputFile
    if ($status) {
        $dt = Get-Date -Format "yyyy-MM-dd HH:mm"
        git commit -m "Auto-merge update: $dt"
        git push
        Write-Host "Changes pushed to GitHub successfully." -ForegroundColor Green
    } else {
        Write-Host "No changes to merge file; skipping push." -ForegroundColor Yellow
    }
} else {
    Write-Error "Not a git repository."
}