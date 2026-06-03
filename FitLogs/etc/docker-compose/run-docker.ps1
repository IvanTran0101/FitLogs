$currentFolder = $PSScriptRoot

$certsFolder = Join-Path $currentFolder "certs"

If(!(Test-Path -Path $certsFolder))
{
    New-Item -ItemType Directory -Force -Path $certsFolder
    if(!(Test-Path -Path (Join-Path $certsFolder "localhost.pfx") -PathType Leaf)){
        Set-Location $certsFolder
        dotnet dev-certs https -v -ep localhost.pfx -p f25d87e2-b70d-458f-b4d9-16edc40d1751 -t        
    }
}

Set-Location $currentFolder
docker-compose up -d
exit $LASTEXITCODE