param ($version='latest')

$currentFolder = $PSScriptRoot
$slnFolder = Join-Path $currentFolder "../../"

Write-Host "********* BUILDING DbMigrator *********" -ForegroundColor Green
$dbMigratorFolder = Join-Path $slnFolder "src/FitLogs.DbMigrator"
Set-Location $dbMigratorFolder
dotnet publish -c Release
docker build -f Dockerfile.local -t fitlogs-db-migrator:$version .






Write-Host "********* BUILDING Blazor Web Application *********" -ForegroundColor Green
$blazorWebAppFolder = Join-Path $slnFolder "src/FitLogs.Blazor"
Set-Location $blazorWebAppFolder
dotnet publish -c Release
docker build -f Dockerfile.local -t fitlogs-blazor:$version .



### ALL COMPLETED
Write-Host "COMPLETED" -ForegroundColor Green
Set-Location $currentFolder
exit $LASTEXITCODE