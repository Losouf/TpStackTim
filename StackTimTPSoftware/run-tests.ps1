$testProj = ".\StacktimApi.Tests\StacktimApi.Tests.csproj"
$results  = ".\TestResults"
$report   = ".\CoverageReport"

Write-Host "== build =="
dotnet build

Write-Host "== tests + couverture =="
dotnet test $testProj --collect:"XPlat Code Coverage" --results-directory $results -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=cobertura

# on chope le premier fichier de couverture trouvé
$cov = Get-ChildItem -Recurse -Filter "coverage.cobertura.xml" $results | Select-Object -First 1
if (-not $cov) {
  Write-Host "pas trouvé de couverture :(" 
  exit 1
}

Write-Host "== rapport html =="
reportgenerator -reports:$($cov.FullName) -targetdir:$report -reporttypes:Html;TextSummary

# petit résumé si dispo
if (Test-Path "$report\Summary.txt") {
  Get-Content "$report\Summary.txt"
}

# ouvrir le rapport si possible
if (Test-Path "$report\index.html") {
  Start-Process "$report\index.html"
} else {
  Write-Host "rapport pas trouvé, voir dossier $report"
}
