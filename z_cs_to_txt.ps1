$baseFileName = "z_solution"
$mapFileName = "z_map.txt"
$excludeFolders = @("obj", "bin", ".vs", ".git")

# Target limit of 29,000 bytes to leave a safe margin below 30 KB (30,720 bytes)
$maxByteCount = 29000 

# Clean up previous output files and the map from earlier runs
Remove-Item "${baseFileName}_*.txt" -ErrorAction SilentlyContinue
Remove-Item $mapFileName -ErrorAction SilentlyContinue

$csFiles = Get-ChildItem -Recurse -Filter "*.cs"

$fileIndex = 1
$currentFileByteCount = 0
$currentOutputFile = "${baseFileName}_${fileIndex}.txt"
$currentChunkFiles = @()
$mapEntries = @()

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
        # Get a clean relative path for the map and headers
        $relativePath = (Resolve-Path -Path $file.FullName -Relative) -replace "^\.\\", ""
        Write-Host "Processing: $relativePath"
        
        $content = Get-Content -Path $file.FullName -Raw
        
        # Use the relative path in the header to hide absolute local drive paths
        $header = @"
==============================================================================
FILE: $relativePath
==============================================================================
"@
        $entryText = $header + "`r`n" + $content + "`r`n`r`n"
        
        # Calculate precise byte count based on UTF-8 encoding
        $entryBytes = [System.Text.Encoding]::UTF8.GetByteCount($entryText)

        # If a single file exceeds the entire limit on its own
        if ($entryBytes -gt $maxByteCount) {
            Write-Warning "File '$relativePath' is $($entryBytes) bytes and exceeds the limit on its own."
        }

        # Rollover check
        if ($currentFileByteCount -gt 0 -and ($currentFileByteCount + $entryBytes -gt $maxByteCount)) {
            # Record map data for the completed chunk
            $mapEntries += [PSCustomObject]@{
                ChunkName = $currentOutputFile
                Files     = $currentChunkFiles
                SizeKb    = [Math]::Round($currentFileByteCount / 1024, 2)
            }
            
            # Reset chunk tracking and increment file index
            $fileIndex++
            $currentOutputFile = "${baseFileName}_${fileIndex}.txt"
            $currentFileByteCount = 0
            $currentChunkFiles = @()
            
            New-Item -ItemType File -Path $currentOutputFile -Force | Out-Null
            Write-Host "Size limit approached. Switched to: $currentOutputFile"
        }

        # Write to the current active file
        $entryText | Out-File -FilePath $currentOutputFile -NoNewline -Append -Encoding utf8
        $currentFileByteCount += $entryBytes
        $currentChunkFiles += $relativePath
    }
}

# Record the final chunk mapping if there are files in it
if ($currentChunkFiles.Count -gt 0) {
    $mapEntries += [PSCustomObject]@{
        ChunkName = $currentOutputFile
        Files     = $currentChunkFiles
        SizeKb    = [Math]::Round($currentFileByteCount / 1024, 2)
    }
}

# Generate and write the z_map.txt file
if ($mapEntries.Count -gt 0) {
    $mapContent = @()
    $mapContent += "=============================================================================="
    $mapContent += "REPOSITORY MAP & INDEX"
    $mapContent += "=============================================================================="
    $mapContent += ""
    
    foreach ($entry in $mapEntries) {
        $mapContent += "[$($entry.ChunkName)] (Size: $($entry.SizeKb) KB)"
        foreach ($f in $entry.Files) {
            $mapContent += "  - $f"
        }
        $mapContent += ""
    }
    
    $mapContent | Out-File -FilePath $mapFileName -Encoding utf8
    Write-Host "Index map generated successfully: $mapFileName"
}

Write-Host "Done! All files processed."