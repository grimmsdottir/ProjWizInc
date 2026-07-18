$o="grimmsdottir"; $r="ProjWizInc"; $h=@{"User-Agent"="PS"}
$b=(Invoke-RestMethod "https://api.github.com/repos/$o/$r" -H $h).default_branch
$sha=(Invoke-RestMethod "https://api.github.com/repos/$o/$r/branches/$b" -H $h).commit.commit.tree.sha
$tree=(Invoke-RestMethod "https://api.github.com/repos/$o/$r/git/trees/$sha?recursive=1" -H $h).tree

$exts=@(".cs",".csproj",".sln",".json",".md",".txt")
$urls=[System.Collections.Generic.List[string]]::new()
$w=[System.IO.StreamWriter]::new("z_solution_generated.txt")

foreach($i in $tree){
  if($i.type -eq "blob"){
    $ext=[System.IO.Path]::GetExtension($i.path).ToLower()
    if($exts -contains $ext){
      $url="https://raw.githubusercontent.com/$o/$r/refs/heads/$b/$([Uri]::EscapeUriString($i.path))"
      $urls.Add($url)
      $w.WriteLine("="*80); $w.W