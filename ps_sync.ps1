# --- CONFIGURATION ---
$OWNER = "grimmsdottir"
$REPO = "HelpMeLLMSempai"
$FILE_PATH = "combined_solution.txt"
$TOKEN = "YOUR_TOKEN_HERE" 
# ---------------------

Write-Host "1. Merging C# files locally..." -ForegroundColor Cyan
$LocalContent = Get-ChildItem -Recurse -Filter *.cs | Where-Object { $_.FullName -notmatch '\\(obj|bin|Packages)' } | ForEach-Object { 
    "=== FILE: $($_.Name) ==="
    Get-Content $_.FullName
    "`n" 
} | Out-String

# Convert file string into raw bytes for GitHub's upload architecture
$Bytes = [System.Text.Encoding]::UTF8.GetBytes($LocalContent)
$Base64Content = [Convert]::ToBase64String($Bytes)

Write-Host "2. Checking for existing file on GitHub to prevent conflicts..." -ForegroundColor Cyan
$Url = "https://github.com"
$AuthHeader = @{ Authorization = "Bearer $TOKEN"; "Accept" = "application/vnd.github+json" }

$Sha = $null
try {
    $ExistingFile = Invoke-RestMethod -Uri $Url -Method Get -Headers $AuthHeader -ErrorAction Stop
    $Sha = $ExistingFile.sha
    Write-Host "Found existing file version online." -ForegroundColor Yellow
} catch {
    Write-Host "No existing file found online. Creating a brand new file entry." -ForegroundColor Yellow
}

Write-Host "3. Uploading code directly to GitHub via API..." -ForegroundColor Cyan
$Body = @{
    message = "Auto-update combined_solution.txt via API"
    content = $Base64Content
    branch  = "main"
}
if ($Sha) { $Body.sha = $Sha } # Include SHA hash if overwriting an existing file
$JsonBody = $Body | ConvertTo-Json

try {
    $Response = Invoke-RestMethod -Uri $Url -Method Put -Headers $AuthHeader -Body $JsonBody -ContentType "application/json"
    Write-Host "`nSUCCESS! Your code has been forced online directly to your repository." -ForegroundColor Green
} catch {
    Write-Host "`nAPI Upload Failed: $_" -ForegroundColor Red
}

Read-Host "`nPress Enter to exit..."
