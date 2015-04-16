Properties {
  $build_dir = Split-Path $psake.build_script_file

  $root_dir = Normalize-Path "$build_dir\.."
  $src_dir =  "$root_dir\src"

  $sln_path = "$src_dir\Serilog.JsonFormatters.sln"
  $nuspec = (Get-Childitem -Path "$src_dir" -Filter *.nuspec -Recurse | Select-Object -First 1)
  $configuration = "Release"

  $packages_dir = "$src_dir\packages"
  $release_dir = "$root_dir\_release"
}


Task default -Depends Nuget

Task Nuget -Depends Compile, CreateNugetPackage

Task Compile -Depends Clean {
  Exec { msbuild $sln_path /p:Configuration=$configuration /v:quiet }
}


Task CreateNugetPackage -Depends PrepareRelease {
  $nuget=$global:build_defaults["nuget_exe"]
  $nuget_proj_path = (Get-Childitem -Path $nuspec.DirectoryName -Filter *.csproj | Select-Object -First 1).FullName

  Exec { &$nuget pack $nuget_proj_path -Prop Configuration=$configuration -Symbols -OutputDirectory $release_dir}
  Write-Info "Created nuget packages in $release_dir"
}

Task Clean {
    if(Test-Path $release_dir) {
        Remove-Item $release_dir -Recurse -Force
    }
}

Task PrepareRelease {
    if(!(Test-Path $release_dir)) {
        New-Item $release_dir -ItemType Directory | Out-Null
    }
}






FormatTaskName {
   param($taskName)
   $s="$taskName "
   write-host ($s + ("-"* (70-$s.Length))) -foregroundcolor Cyan
}


Task ? -Description "Helper to display task info" {
  Write-Documentation
}


Task ShowMsBuildVersion -Description "Displays the version of msbuild" {
  msbuild /version
}

function Write-Info([string]$Message, $ForegroundColor="Magenta") {
   Write-Host $message  -ForegroundColor $ForegroundColor
}

function Normalize-Path([string]$Path){
  [System.IO.Path]::GetFullPath($Path)
}
