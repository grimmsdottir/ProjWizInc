# ProjWizInc/z_cs_to_txt.ps1

$outputFile = "z_solution.txt"
$tokenFile = "github_token.txt"
$gistIdFile = "gist_id.txt"
$excludeFolders = @("obj", "bin", ".vs", ".git")

# 1. Verify the security token exists
if (-not (Test-Path -Path $tokenFile)) {
    Write-Error "Security Error: '$tokenFile' not found. Please create it and paste your GitHub PAT inside."
    exit
}

$githubToken = (Get-Content -Path $tokenFile -Raw).Trim()

# 2. MERGE FILES
New-Item -ItemType File -Path $outputFile -Force | Out-Null

$csFiles = Get-ChildItem -Recurse -Filter "*.cs"

foreach ($file in $csFiles) {
    $path = $file.FullName
    $shouldExclude = $false
    
    foreach ($folder in $excludeFolders) {
        if ($path -like "*\$folder\*") { 
            $shouldExclude = $true
            break 
        }
    }
    
    if (-not $shouldExclude) {
        Write-Host "Adding: $($file.Name)"
        $header = @"
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