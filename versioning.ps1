param(
    [string]$version
)

$versionJson = '{ "Version": "' + $version + '" }'
Set-Content -Path latest-version.json -Value $versionJson

[xml]$csprojContents = Get-Content -Path .\SnapIt\SnapIt.csproj;
$csprojContents.Project.PropertyGroup.Version = $version
$csprojContents.Save('.\SnapIt\SnapIt.csproj')

[xml]$manifestContents = Get-Content -Path .\SnapIt.Packaging\Package.appxmanifest
Write-Host $manifestContents.Package.Identity.Version
$manifestContents.Package.Identity.Version = $version
$manifestContents.Save('.\SnapIt.Packaging\Package.appxmanifest')