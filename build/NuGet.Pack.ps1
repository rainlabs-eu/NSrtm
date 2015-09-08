
. .\Common.ps1

if(Test-Path $TargetNuGet) {
	Remove-Item $TargetNuGet -Force -Recurse
    md $TargetNuGet
}


nuget pack $SlnRoot/NSrtm.Core/NSrtm.Core.csproj -OutputDirectory $TargetNuGet -Prop Configuration=Release -Build -IncludeReferencedProjects