# ==========================================
# Configuration
# ==========================================
$solutionFolder = "C:\Users\grimm\source\repos\ProjWizInc\Engine"
$outputFile = "C:\Users\grimm\source\repos\ProjWizInc\z_full_Map.txt"

# Convert to absolute path to prevent Split-Path errors on relative paths
$outputFile = [System.IO.Path]::GetFullPath($outputFile)

# Set a safe byte limit (28 KB) to guarantee files remain strictly under 30 KB
$maxBytes = 28 * 1024 

# ==========================================
# C# Lexical Minifier Compilation
# ==========================================
# Generate a unique class name to bypass PowerShell's Add-Type caching limit
$randomId = Get-Random -Minimum 100000 -Maximum 999999
$className = "CSharpFileCompressor_$randomId"

$source = @'
using System;
using System.Text;
using System.Collections.Generic;

public class CSharpFileCompressor
{
    public static string Minify(string code)
    {
        StringBuilder sb = new StringBuilder();
        int i = 0;
        int len = code.Length;
        char lastWritten = '\0';

        while (i < len)
        {
            char c = code[i];

            // 1. Handle String Literals (Verbatim, Interpolated, Standard)
            if (c == '"')
            {
                bool isVerbatim = false;
                if (i > 0 && code[i - 1] == '@') isVerbatim = true;
                if (i > 1 && code[i - 1] == '$' && code[i - 2] == '@') isVerbatim = true;
                if (i > 1 && code[i - 1] == '@' && code[i - 2] == '$') isVerbatim = true;

                sb.Append(c);
                lastWritten = c;
                i++;
                while (i < len)
                {
                    char sc = code[i];
                    if (sc == '"')
                    {
                        if (isVerbatim && i + 1 < len && code[i + 1] == '"')
                        {
                            sb.Append("\"\"");
                            i += 2;
                            continue;
                        }
                        sb.Append(sc);
                        lastWritten = sc;
                        i++;
                        break;
                    }
                    else if (sc == '\\' && !isVerbatim)
                    {
                        if (i + 1 < len)
                        {
                            sb.Append(sc);
                            sb.Append(code[i + 1]);
                            i += 2;
                            continue;
                        }
                    }
                    sb.Append(sc);
                    lastWritten = sc;
                    i++;
                }
                continue;
            }

            // 2. Handle Character Literals
            if (c == '\'')
            {
                sb.Append(c);
                lastWritten = c;
                i++;
                while (i < len)
                {
                    char cc = code[i];
                    if (cc == '\'')
                    {
                        sb.Append(cc);
                        lastWritten = cc;
                        i++;
                        break;
                    }
                    else if (cc == '\\')
                    {
                        if (i + 1 < len)
                        {
                            sb.Append(cc);
                            sb.Append(code[i + 1]);
                            i += 2;
                            continue;
                        }
                    }
                    sb.Append(cc);
                    lastWritten = cc;
                    i++;
                }
                continue;
            }

            // 3. Handle Comments (discard them entirely)
            if (c == '/' && i + 1 < len && code[i + 1] == '/')
            {
                i += 2;
                while (i < len && code[i] != '\n' && code[i] != '\r')
                {
                    i++;
                }
                continue;
            }
            if (c == '/' && i + 1 < len && code[i + 1] == '*')
            {
                i += 2;
                while (i < len)
                {
                    if (code[i] == '*' && i + 1 < len && code[i + 1] == '/')
                    {
                        i += 2;
                        break;
                    }
                    i++;
                }
                continue;
            }

            // 4. Handle Whitespace in Raw Code
            if (char.IsWhiteSpace(c))
            {
                while (i < len && char.IsWhiteSpace(code[i]))
                {
                    i++;
                }
                // Only write a space separator if both adjacent tokens are word characters
                if (i < len)
                {
                    char next = code[i];
                    if (IsWordChar(lastWritten) && IsWordChar(next))
                    {
                        sb.Append(' ');
                        lastWritten = ' ';
                    }
                }
                continue;
            }

            // 5. Normal raw code character
            sb.Append(c);
            lastWritten = c;
            i++;
        }

        return sb.ToString().Trim();
    }

    private static bool IsWordChar(char c)
    {
        return char.IsLetterOrDigit(c) || c == '_';
    }
}
'@

# Replace the static class name with our uniquely generated runtime class name
$sourceWithUniqueName = $source -replace "CSharpFileCompressor", $className
Add-Type -TypeDefinition $sourceWithUniqueName

# Resolve the type object safely for older PowerShell versions using the [type] accelerator
$generatorType = [type]$className
$minifyMethod = $generatorType.GetMethod("Minify")

# ==========================================
# Partition Helper Functions
# ==========================================
$utf8NoBom = New-Object System.Text.UTF8Encoding($false)

function Get-PartitionPath ($index) {
    $dir = Split-Path -Parent $outputFile
    $base = Split-Path -Leaf $outputFile
    $ext = [System.IO.Path]::GetExtension($base)
    $name = [System.IO.Path]::GetFileNameWithoutExtension($base)
    return Join-Path -Path $dir -ChildPath ("{0}_part{1}{2}" -f $name, $index, $ext)
}

# Clean up older partition files
$existingIndex = 1
while (Test-Path (Get-PartitionPath $existingIndex)) {
    Remove-Item (Get-PartitionPath $existingIndex) -Force
    $existingIndex++
}

# Initialize first partition
$partitionIndex = 1
$currentPartitionFile = Get-PartitionPath $partitionIndex
[System.IO.File]::WriteAllText($currentPartitionFile, [string]::Empty, $utf8NoBom)
$currentFileBytes = 0

# ==========================================
# Phase 1: Extract NuGet Packages from .csproj
# ==========================================
Write-Host "Scanning for project dependencies..."
$csprojFiles = Get-ChildItem -Path $solutionFolder -Filter *.csproj -Recurse | Where-Object {
    $_.FullName -notmatch '\\(bin|obj|\.vs|\.git|node_modules|Properties|Tests)\\.'
}

$packageSummary = ""
if ($csprojFiles.Count -gt 0) {
    $packageSummary += "// PROJECT DEPENDENCIES`r`n"
    
    foreach ($proj in $csprojFiles) {
        try {
            [xml]$xml = Get-Content $proj.FullName -ErrorAction Stop
            $pkgRefs = $xml.SelectNodes("//*[local-name()='PackageReference']")
            
            if ($pkgRefs.Count -gt 0) {
                $projRelative = $proj.FullName.Substring($solutionFolder.Length).TrimStart('\', '/')
                $packageSummary += "Project: $projRelative`r`n"
                
                foreach ($ref in $pkgRefs) {
                    $name = $ref.Include
                    $version = $ref.Version
                    
                    if (-not $version) {
                        $versionNode = $ref.SelectSingleNode("*[local-name()='Version']")
                        if ($versionNode) { $version = $versionNode.InnerText }
                    }
                    
                    if ($name) {
                        if ($version) {
                            $packageSummary += "  - $name (v$version)`r`n"
                        } else {
                            $packageSummary += "  - $name`r`n"
                        }
                    }
                }
            }
        } catch {
            # Skip malformed XML
        }
    }
    $packageSummary += "`r`n"
}

if (-not [string]::IsNullOrWhiteSpace($packageSummary)) {
    [System.IO.File]::AppendAllText($currentPartitionFile, $packageSummary, $utf8NoBom)
    $currentFileBytes = $utf8NoBom.GetByteCount($packageSummary)
}

# ==========================================
# Phase 2: Process C# Source Files (Full Implementations)
# ==========================================
$csFiles = Get-ChildItem -Path $solutionFolder -Filter *.cs -Recurse | Where-Object {
    $_.FullName -notmatch '\\(bin|obj|\.vs|\.git|node_modules|Properties|Tests)\\.'
}

$totalFiles = $csFiles.Count
Write-Host "Found $totalFiles C# files to compress..."

foreach ($file in $csFiles) {
    try {
        $relativePath = $file.FullName.Substring($solutionFolder.Length).TrimStart('\', '/')
        $content = [System.IO.File]::ReadAllText($file.FullName)
        
        # 1. Strip using directives BEFORE minification (while newlines are still present)
        $content = [regex]::Replace($content, '(?m)^\s*(global\s+)?using\s+[^;]+;\r?\n?', [string]::Empty)

        # 2. Invoke static method dynamically to minify code (including full implementations)
        $mappedContent = $minifyMethod.Invoke($null, @($content))
        
        if (-not [string]::IsNullOrWhiteSpace($mappedContent)) {
            # 3. Format lightweight file separation header (one line)
            $header = "`r`n// File: $relativePath`r`n"
            
            # Combine
            $linesToWrite = ($header -split '\r?\n') + ($mappedContent -split '\r?\n')
            
            foreach ($line in $linesToWrite) {
                # Strip out any rare null bytes
                $line = $line -replace '\x00', ''
                $lineWithNewline = $line + "`r`n"
                $lineBytes = $utf8NoBom.GetByteCount($lineWithNewline)

                # Rotate to next partition if we hit the limit
                if ($currentFileBytes + $lineBytes -gt $maxBytes -and $currentFileBytes -gt 0) {
                    $partitionIndex++
                    $currentPartitionFile = Get-PartitionPath $partitionIndex
                    [System.IO.File]::WriteAllText($currentPartitionFile, [string]::Empty, $utf8NoBom)
                    $currentFileBytes = 0
                }

                # Write line in clean, BOM-less UTF-8
                [System.IO.File]::AppendAllText($currentPartitionFile, $lineWithNewline, $utf8NoBom)
                $currentFileBytes += $lineBytes
            }
        }
    } catch {
        Write-Warning "Skipped file: $($file.FullName). Details: $_"
    }
}

Write-Host "Process complete. Output split across partitions up to index: $partitionIndex"