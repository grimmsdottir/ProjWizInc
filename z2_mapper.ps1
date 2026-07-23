# ==========================================
# Configuration
# ==========================================
$solutionFolder = "C:\Users\grimm\source\repos\ProjWizInc\Engine"
$outputFile = "C:\Users\grimm\source\repos\ProjWizInc\z2_Output_Map.txt"

# Convert to absolute path to prevent Split-Path errors on relative paths
$outputFile = [System.IO.Path]::GetFullPath($outputFile)

# Set a safe byte limit (28 KB) to guarantee files remain strictly under 30 KB
$maxBytes = 28 * 1024 

# ==========================================
# Parser Compilation (With Unique Session Class-Naming)
# ==========================================
# Generate a unique class name to bypass PowerShell's Add-Type caching limit
$randomId = Get-Random -Minimum 100000 -Maximum 999999
$className = "CSharpMapGenerator_$randomId"

$source = @'
using System;
using System.Text;
using System.Collections.Generic;

public class CSharpMapGenerator
{
    private static readonly HashSet<string> KeepKeywords = new HashSet<string> {
        "namespace", "class", "interface", "struct", "record", "enum"
    };

    public static string GenerateMap(string code)
    {
        StringBuilder sb = new StringBuilder();
        int i = 0;
        int len = code.Length;
        StringBuilder headerBuilder = new StringBuilder();

        while (i < len)
        {
            char c = code[i];

            // 1. Handle strings and characters
            if (c == '"')
            {
                bool isVerbatim = false;
                if (i > 0 && code[i - 1] == '@') isVerbatim = true;
                if (i > 1 && code[i - 1] == '$' && code[i - 2] == '@') isVerbatim = true;
                if (i > 1 && code[i - 1] == '@' && code[i - 2] == '$') isVerbatim = true;

                sb.Append(c);
                headerBuilder.Append(c);
                i++;
                while (i < len)
                {
                    char sc = code[i];
                    if (sc == '"')
                    {
                        if (isVerbatim && i + 1 < len && code[i + 1] == '"')
                        {
                            sb.Append("\"\"");
                            headerBuilder.Append("\"\"");
                            i += 2;
                            continue;
                        }
                        sb.Append(sc);
                        headerBuilder.Append(sc);
                        i++;
                        break;
                    }
                    else if (sc == '\\' && !isVerbatim)
                    {
                        if (i + 1 < len)
                        {
                            sb.Append(sc);
                            sb.Append(code[i + 1]);
                            headerBuilder.Append(sc);
                            headerBuilder.Append(code[i + 1]);
                            i += 2;
                            continue;
                        }
                    }
                    sb.Append(sc);
                    headerBuilder.Append(sc);
                    i++;
                }
                continue;
            }
            else if (c == '\'')
            {
                sb.Append(c);
                headerBuilder.Append(c);
                i++;
                while (i < len)
                {
                    char cc = code[i];
                    if (cc == '\'')
                    {
                        sb.Append(cc);
                        headerBuilder.Append(cc);
                        i++;
                        break;
                    }
                    else if (cc == '\\')
                    {
                        if (i + 1 < len)
                        {
                            sb.Append(cc);
                            sb.Append(code[i + 1]);
                            headerBuilder.Append(cc);
                            headerBuilder.Append(code[i + 1]);
                            i += 2;
                            continue;
                        }
                    }
                    sb.Append(cc);
                    headerBuilder.Append(cc);
                    i++;
                }
                continue;
            }

            // 2. Handle comments
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

            // 3. Delimiters that reset the header
            if (c == ';' || c == '}')
            {
                sb.Append(c);
                headerBuilder.Clear();
                i++;
                continue;
            }

            // 4. Handle braces
            if (c == '{')
            {
                string header = headerBuilder.ToString();
                if (ShouldKeepBlock(header))
                {
                    sb.Append(c);
                    headerBuilder.Clear();
                    i++;
                }
                else
                {
                    sb.Append(" { }");
                    headerBuilder.Clear();
                    
                    i++;
                    int depth = 1;
                    while (i < len && depth > 0)
                    {
                        char bc = code[i];
                        
                        if (bc == '"')
                        {
                            bool isVerbatim = false;
                            if (i > 0 && code[i - 1] == '@') isVerbatim = true;
                            if (i > 1 && code[i - 1] == '$' && code[i - 2] == '@') isVerbatim = true;
                            if (i > 1 && code[i - 1] == '@' && code[i - 2] == '$') isVerbatim = true;
                            i++;
                            while (i < len)
                            {
                                if (code[i] == '"')
                                {
                                    if (isVerbatim && i + 1 < len && code[i + 1] == '"') { i += 2; continue; }
                                    i++;
                                    break;
                                }
                                else if (code[i] == '\\' && !isVerbatim && i + 1 < len) { i += 2; continue; }
                                i++;
                            }
                            continue;
                        }
                        if (bc == '\'')
                        {
                            i++;
                            while (i < len)
                            {
                                if (code[i] == '\'') { i++; break; }
                                else if (code[i] == '\\' && i + 1 < len) { i += 2; continue; }
                                i++;
                            }
                            continue;
                        }
                        if (bc == '/' && i + 1 < len && code[i + 1] == '/')
                        {
                            i += 2;
                            while (i < len && code[i] != '\n' && code[i] != '\r') i++;
                            continue;
                        }
                        if (bc == '/' && i + 1 < len && code[i + 1] == '*')
                        {
                            i += 2;
                            while (i < len)
                            {
                                if (code[i] == '*' && i + 1 < len && code[i + 1] == '/') { i += 2; break; }
                                i++;
                            }
                            continue;
                        }

                        if (bc == '{') depth++;
                        else if (bc == '}') depth--;
                        i++;
                    }
                }
                continue;
            }

            sb.Append(c);
            headerBuilder.Append(c);
            i++;
        }

        return sb.ToString();
    }

    private static bool ShouldKeepBlock(string header)
    {
        string normalized = header.Replace('\r', ' ').Replace('\n', ' ').Replace('\t', ' ');
        var words = normalized.Split(new[] { ' ', ':', '(', ')', '[', ']', ',', '<', '>', '=' }, StringSplitOptions.RemoveEmptyEntries);
        
        bool foundWhere = false;
        for (int k = 0; k < words.Length; k++)
        {
            if (words[k] == "where") foundWhere = true;
            if (KeepKeywords.Contains(words[k]))
            {
                if (!foundWhere) return true;
            }
        }
        return false;
    }
}
'@

# Replace the static class name with our uniquely generated runtime class name
$sourceWithUniqueName = $source -replace "CSharpMapGenerator", $className
Add-Type -TypeDefinition $sourceWithUniqueName

# Resolve the type object safely for older PowerShell versions using the [type] accelerator
$generatorType = [type]$className
$generateMapMethod = $generatorType.GetMethod("GenerateMap")

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
# Phase 2: Process C# Source Files
# ==========================================
$csFiles = Get-ChildItem -Path $solutionFolder -Filter *.cs -Recurse | Where-Object {
    $_.FullName -notmatch '\\(bin|obj|\.vs|\.git|node_modules|Properties|Tests)\\.'
}

$totalFiles = $csFiles.Count
Write-Host "Found $totalFiles C# files to map..."

foreach ($file in $csFiles) {
    try {
        $relativePath = $file.FullName.Substring($solutionFolder.Length).TrimStart('\', '/')
        $content = [System.IO.File]::ReadAllText($file.FullName)
        
        # Invoke static method dynamically using standard .NET Reflection
        $mappedContent = $generateMapMethod.Invoke($null, @($content))
        
        # Strip using directives
        $mappedContent = [regex]::Replace($mappedContent, '(?m)^\s*(global\s+)?using\s+[^;]+;\r?\n?', [string]::Empty)
        
        if (-not [string]::IsNullOrWhiteSpace($mappedContent)) {
            
            # --- TOKEN COMPRESSION TRANSFORMS ---
            # 1. Move opening curly braces to the same line as the class/method header
            $mappedContent = [regex]::Replace($mappedContent, '(?m)\r?\n\s*\{', ' {')

            # 2. Strip all leading indentation entirely (saves thousands of characters)
            $mappedContent = [regex]::Replace($mappedContent, '(?m)^[ \t]+', [string]::Empty)

            # 3. Collapse multiple consecutive newlines down to a single newline
            $mappedContent = [regex]::Replace($mappedContent, '(\r?\n){2,}', '$1')
            
            # 4. Format lightweight file separation header (no heavy decorative lines)
            $header = "`r`n// File: $relativePath`r`n"
            
            # Combine
            $linesToWrite = ($header -split '\r?\n') + ($mappedContent -split '\r?\n')
            
            foreach ($line in $linesToWrite) {
                $line = $line -replace '\x00', ''
                $lineWithNewline = $line + "`r`n"
                $lineBytes = $utf8NoBom.GetByteCount($lineWithNewline)

                if ($currentFileBytes + $lineBytes -gt $maxBytes -and $currentFileBytes -gt 0) {
                    $partitionIndex++
                    $currentPartitionFile = Get-PartitionPath $partitionIndex
                    [System.IO.File]::WriteAllText($currentPartitionFile, [string]::Empty, $utf8NoBom)
                    $currentFileBytes = 0
                }

                [System.IO.File]::AppendAllText($currentPartitionFile, $lineWithNewline, $utf8NoBom)
                $currentFileBytes += $lineBytes
            }
        }
    } catch {
        Write-Warning "Skipped file: $($file.FullName). Details: $_"
    }
}

Write-Host "Process complete. Output split across partitions up to index: $partitionIndex"