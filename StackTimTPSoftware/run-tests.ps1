# params
$test = ".\StacktimApi.Tests\StacktimApi.Tests.csproj"
$out  = ".\TestResults"
$rep  = ".\CoverageReport"

# build
Write-Host "== build =="
dotnet build

# tests + couverture
Write-Host "== tests + couverture =="
dotnet test $test --collect:"XPlat Code Coverage" --results-directory $out -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=cobertura
if ($LASTEXITCODE -ne 0) { 
    Write-Host "tests KO"
    exit 1 
}

# fichier de couverture
$cov = Get-ChildItem -Recurse -Filter "coverage.cobertura.xml" $out | Select-Object -First 1
if (-not $cov) { 
    Write-Host "pas de coverage.cobertura.xml"
    exit 1 
}

# reset dossier rapport
if (Test-Path $rep) { Remove-Item $rep -Recurse -Force }

# rapport (controllers only)
Write-Host "== rapport html (Controllers only) =="
dotnet tool run reportgenerator `
    -reports:"$($cov.FullName)" `
    -targetdir:"$rep" `
    -reporttypes:"Html;TextSummary" `
    -assemblyfilters:"+StackTimAPI;-StacktimApi.Tests*" `
    -classfilters:"+StackTimAPI.Controllers.*"

# résumé
if (Test-Path "$rep\Summary.txt") {
    Write-Host "`n===== Résumé couverture (Controllers) ====="
    Get-Content "$rep\Summary.txt"
    Write-Host "===========================================`n"
}

# ouvre le rapport
if (Test-Path "$rep\index.html") {
    Start-Process "$rep\index.html"
} else {
    Write-Host "rapport pas trouvé, voir $rep"
}
