# save as merge_files.ps1
$outputFile = "z_solution.txt"
$excludeFolders = @("obj", "bin", ".vs", ".git")

# Verify the security token exists before starting
if (-not (Test-Path $tokenFile)) {
    Write-Error "Security Error: '$tokenFile' not found. Please create it and paste your GitHub PAT inside."
    exit
}

[string]$githubToken = (Get-Content -Path $tokenFile -Raw).Trim()

# 1. MERGE FILES (Same logic as before, using traditional commands)
New-Item -ItemType File -Path $outputFile -Force | Out-Null

[System.IO.FileInfo[]]$csFiles = Get-ChildItem -Recurse -Filter "*.cs"

foreach ($file in $csFiles) {
    [string]$path = $file.FullName
    [bool]$shouldExclude = $false
    
    foreach ($folder in $excludeFolders) {
        if ($path -like "*\$folder\*") { 
            $shouldExclude = $true
            break 
        }
    }
    
    if (-not $shouldExclude) {
        Write-Host "Adding: $($file.Name)"
        [string]$header = @"
==============================================================================
FILE: $($file.FullName)
==============================================================================
"@
        $header | Out-File -FilePath $outputFile -Append -Encoding utf8
        Get-Content -Path $file.FullName | Out-File -FilePath $outputFile -Append -Encoding utf8
        "`n`n" | Out-File -FilePath $outputFile -Append -Encoding utf8
    }
}

Write-Host "Done! Saved locally to $outputFile"

# 2. GIST API INTEGRATION (The "Active" Uploader)
[string]$solutionContent = Get-Content -Path $outputFile -Raw
[string]$currentDateTime = Get-Date -Format "yyyy-MM-dd HH:mm"

# Setup headers for the GitHub REST API
[System.Collections.Generic.Dictionary[string, string]]$headers = New-Object 'System.Collections.Generic.Dictionary[string, string]'
$headers.Add("Authorization", "Bearer $githubToken")
$headers.Add("Accept", "application/vnd.github+json")
$headers.Add("X-GitHub-Api-Version", "2022-11-28")

# Build the payload object
[hashtable]$payload = @{
    description = "Auto-merge update: $currentDateTime"
    public = $true # Set to $false if you want a secret Gist
    files = @{
        "z_solution.txt" = @{
            content = $solutionContent
        }
    }
}

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
}