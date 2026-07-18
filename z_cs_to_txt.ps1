$baseFileName = "z_solution"
$excludeFolders = @("obj", "bin", ".vs", ".git")
$maxCharCount = 3000 # Set your arbitrary character threshold here

# Remove any existing output files from previous runs to prevent mixing old and new files
Remove-Item "${baseFileName}_*.txt" -ErrorAction SilentlyContinue

$csFiles = Get-ChildItem -Recurse -Filter "*.cs"

$fileIndex = 1
$currentFileCharCount = 0
$currentOutputFile = "${baseFileName}_${fileIndex}.txt"

# Initialize the first output file
New-Item -ItemType File -Path $currentOutputFile -Force | Out-Null

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
        
        # Read the file content as a single string to accurately count characters
        $content = Get-Content -Path $file.FullName -Raw
        
        # Construct the entry block
        $header = @"
==============================================================================
FILE: $($file.FullName)
==============================================================================
"@
        $entryText = $header + "`r`n" + $content + "`r`n`r`n"
        $entryLength = $entryText.Length

        # If adding this entry exceeds the limit, switch to a new output file.
        # We ensure $currentFileCharCount > 0 so that if a single C# file itself 
        # is larger than $maxCharCount, it is still written rather than skipped.
        if ($currentFileCharCount -gt 0 -and ($currentFileCharCount + $entryLength -gt $maxCharCount)) {
            $fileIndex++
            $currentOutputFile = "${baseFileName}_${fileIndex}.txt"
            $currentFileCharCount = 0
            New-Item -ItemType File -Path $currentOutputFile -Force | Out-Null
            Write-Host "Threshold reached. Rolling over to: $currentOutputFile"
        }

        # Append the combined entry to the current output file
        $entryText | Out-File -FilePath $currentOutputFile -NoNewline -Append -Encoding utf8
        $currentFileCharCount += $entryLength
    }
}

Write-Host "Done! Saved files locally matching: ${baseFileName}_*.txt"