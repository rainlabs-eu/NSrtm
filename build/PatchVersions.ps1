#Requires -Version 3.0
param([UInt32]$BuildCounter=0)

. .\Common.BeforeBuild.ps1

$Branch = git rev-parse --abbrev-ref HEAD

$ErrorActionPreference= 'silentlycontinue' #writes error if no tag found.
$ThisCommitTag = git describe --exact-match --abbrev=0 --tags 
$ErrorActionPreference= 'stop'

Write-Host "Branch: $Branch"
Write-Host "This Commit Tag: $ThisCommitTag"

if(($ThisCommitTag -match [regex]'(\d.\d.\d)') -and $Branch.equals("master")) {
    Write-Host "`r`nRelease for master branch`r`n"

    $AssemblyVersion = $ThisCommitTag.Substring(1,5) + ".$BuildCounter"
    $AssemblyInformationalVersion = $ThisCommitTag.Substring(1)
    
    Write-Host "Tagged release ($AssemblyInformationalVersion)"
    Write-Host "##teamcity[buildNumber 'Rel-$AssemblyInformationalVersion']"
} else {
    Write-Host "`r`nRelease for other branch or not tagged master`r`n"

    $PreviousTag = git describe --abbrev=0 --match=v*.*.*
    $CurrentDescription = git describe --match=v*.*.*

    $AssemblyVersion = $PreviousTag.Substring(1,5) + ".$BuildCounter";
    $AssemblyInformationalVersion = ($CurrentDescription.Substring(1) -replace '(\d+\.\d+\.\d+)-(\d+)-', '$1-dev$2-') +"-$BuildCounter"

    Write-Host "Developer release ($AssemblyInformationalVersion)"
    Write-Host "##teamcity[buildNumber 'Dev-$AssemblyInformationalVersion']"
}

Write-Host "AssemblyVersion: $AssemblyVersion"
Write-Host "AssemblyInformationalVersion: $AssemblyInformationalVersion"

(gc $SlnRoot/GlobalAssemblyInfo.cs.in).replace('[assembly: AssemblyVersion("0.0.0.0")]', "[assembly: AssemblyVersion(""$AssemblyVersion"")]").replace('[assembly: AssemblyInformationalVersion("0.0.0.0")]', "[assembly: AssemblyInformationalVersion(""$AssemblyInformationalVersion"")]")`
| Out-File -Encoding UTF8 $SlnRoot/GlobalAssemblyInfo.cs