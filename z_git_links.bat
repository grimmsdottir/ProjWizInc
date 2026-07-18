@echo off
echo Running automated repository downloader...
echo [Notice] z_gitlinks.ps1 is no longer needed. Running inline to prevent clipboard truncation.
echo.

powershell -NoProfile -ExecutionPolicy Bypass -Command "$o='grimmsdottir';$r='ProjWizInc';$h=@{'User-Agent'='PS'};$b=(irm ('https://api.github.com/repos/'+$o+'/'+$r) -H $h).default_branch;$sha=(irm ('https://api.github.com/repos/'+$o+'/'+$r+'/branches/'+$b) -H $h).commit.commit.tree.sha;$tree=(irm ('https://api.github.com/repos/'+$o+'/'+$r+'/git/trees/'+$sha+'?recursive=1') -H $h).tree;$urls=[System.Collections.Generic.List[string]]::new();$w=[System.IO.StreamWriter]::new('z_solution_generated.txt');foreach($i in $tree){if($i.type -eq 'blob'){$ext=[System.IO.Path]::GetExtension($i.path).ToLower();if(@('.cs','.csproj','.sln','.json','.md','.txt') -contains $ext){$url='https://raw.githubusercontent.com/'+$o+'/'+$r+'/refs/heads/'+$b+'/'+[Uri]::EscapeUriString($i.path);$urls.Add($url);$w.WriteLine('='*80);$w.WriteLine('FILE: '+$i.path);$w.WriteLine('='*80);try{$w.WriteLine((irm $url -H $h))}catch{$w.WriteLine('Error')};$w.WriteLine();$w.WriteLine()}}};$w.Close();$urls|Out-File 'raw_links.txt' -Encoding utf8;Write-Host 'Codebase aggregated successfully!'"

echo.
pause