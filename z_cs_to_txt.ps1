# save as z_cs_to_gist.ps1
[string]$outputFile = "z_solution.txt"
[string]$tokenFile = "github_token.txt"
[string]$gistIdFile = "gist_id.txt"
[array]$excludeFolders = @("obj", "bin", ".vs", ".git")

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

[string]$jsonPayload = ConvertTo-Json -InputObject $payload -Depth 4

# Check if we already have an existing Gist to update
if (Test-Path $gistIdFile) {
    [string]$gistId = (Get-Content -Path $gistIdFile -Raw).Trim()
    Write-Host "Updating existing Gist (ID: $gistId)..." -ForegroundColor Yellow

    try {
        [string]$url = "https://api.github.com/gists/$gistId"
        
        # FIX: Changed [hashtable] to [PSCustomObject]
        [PSCustomObject]$response = Invoke-RestMethod -Uri $url -Method Patch -Headers $headers -Body $jsonPayload -ContentType "application/json"
        
        Write-Host "Gist updated successfully!" -ForegroundColor Green
        Write-Host "Raw URL: $($response.files.'z_solution.txt'.raw_url)" -ForegroundColor Cyan
    }
    catch {
        Write-Error "Failed to update Gist: $_"
    }
} else {
    Write-Host "Creating a NEW Gist..." -ForegroundColor Yellow

    try {
        [string]$url = "https://api.github.com/gists"
        
        # FIX: Changed [hashtable] to [PSCustomObject]
        [PSCustomObject]$response = Invoke-RestMethod -Uri $url -Method Post -Headers $headers -Body $jsonPayload -ContentType "application/json"
        
        # Save the new Gist ID so we can update it next time instead of creating duplicates
        [string]$newGistId = $response.id
        $newGistId | Out-File -FilePath $gistIdFile -Force
        
        Write-Host "New Gist created successfully! ID saved to $gistIdFile." -ForegroundColor Green
        Write-Host "Raw URL: $($response.files.'z_solution.txt'.raw_url)" -ForegroundColor Cyan
    }
    catch {
        Write-Error "Failed to create Gist: $_"
    }
}