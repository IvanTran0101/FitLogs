$currentFolder = $PSScriptRoot

$certsFolder = Join-Path $currentFolder "certs"

If(!(Test-Path -Path $certsFolder))
{
    New-Item -ItemType Directory -Force -Path $certsFolder
    if(!(Test-Path -Path (Join-Path $certsFolder "localhost.pfx") -PathType Leaf)){
        Set-Location $certsFolder
        dotnet dev-certs https -v -ep localhost.pfx -p d096eb9c-687a-4f32-ba00-23607ce89868 -t        
    }
}

Set-Location $currentFolder
docker-compose up -d
exit $LASTEXITCODE