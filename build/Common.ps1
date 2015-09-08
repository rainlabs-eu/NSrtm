#Requires -Version 3.0

. .\Common.BeforeBuild.ps1

$TargetNuGet = Join-Path $RepoRoot "release"

if($ShouldPrint) {
    Write-Host "Script Root:`n$PSScriptRoot`n"
    Write-Host "Repository Root:`n$RepoRoot`n"
    Write-Host "Sln Root:`n$SlnRoot`n"
}