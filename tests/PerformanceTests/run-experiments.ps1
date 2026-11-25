# Runs all experiments one after another.
# Runs that already exist in results/experiments.csv are skipped,
# so the script can be stopped and started again at any time.
# Start the script from the PerformanceTests folder, with all seven services already running.
$csv = "results/experiments.csv"

function Run($exp, $variant, $rate, $rep) {
    $key = "$exp,$variant,$rate,$rep,"
    if ((Test-Path $csv) -and (Select-String -Path $csv -Pattern ("^" + [regex]::Escape($key)) -Quiet)) { return }
    Write-Host ">>> $exp $variant $rate rep=$rep"
    dotnet run -c Release -- $exp $variant $rate $rep
}

foreach ($rep in 1..3) {
    foreach ($rate in 0, 0.2, 0.4, 0.6, 0.8) {
        foreach ($v in "V0","V2","R3","V3","V1") { Run "E1" $v $rate $rep }
    }
    foreach ($v in "R1","R2","R5") { Run "E2" $v 0.4 $rep }
    foreach ($v in "V0","V3","V1") { Run "E3" $v 1 $rep }
}