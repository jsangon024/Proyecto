param(
    [Parameter(Mandatory = $true)]
    [string]$BackupPath,
    [string]$DbHost = "localhost",
    [int]$DbPort = 5432,
    [string]$Database = "meal_planner_db",
    [string]$Username = "postgres",
    [string]$Password = "admin",
    [switch]$RecreateDatabase
)

$ErrorActionPreference = "Stop"

if (-not (Test-Path $BackupPath)) {
    throw "No existe el archivo de backup: $BackupPath"
}

$pgRestore = Get-Command pg_restore -ErrorAction SilentlyContinue
if (-not $pgRestore) {
    $pgRestore = Get-ChildItem -Path "C:\Program Files\PostgreSQL" -Recurse -Filter pg_restore.exe -ErrorAction SilentlyContinue |
        Where-Object { $_.FullName -like "*\bin\pg_restore.exe" } |
        Sort-Object FullName -Descending |
        Select-Object -First 1
}

if (-not $pgRestore) {
    throw "No se encontro pg_restore. Anade la carpeta bin de PostgreSQL al PATH."
}

$pgRestorePath = if ($pgRestore.Source) { $pgRestore.Source } else { $pgRestore.FullName }

$psql = Get-Command psql -ErrorAction SilentlyContinue
if (-not $psql) {
    $psql = Get-ChildItem -Path "C:\Program Files\PostgreSQL" -Recurse -Filter psql.exe -ErrorAction SilentlyContinue |
        Where-Object { $_.FullName -like "*\bin\psql.exe" } |
        Sort-Object FullName -Descending |
        Select-Object -First 1
}

if ($RecreateDatabase -and -not $psql) {
    throw "No se encontro psql. Es necesario para recrear la base de datos."
}

$psqlPath = if ($psql) {
    if ($psql.Source) { $psql.Source } else { $psql.FullName }
} else {
    $null
}

$env:PGPASSWORD = $Password

if ($RecreateDatabase) {
    & $psqlPath `
        --host $DbHost `
        --port $DbPort `
        --username $Username `
        --dbname postgres `
        --command "SELECT pg_terminate_backend(pid) FROM pg_stat_activity WHERE datname = '$Database';"

    & $psqlPath `
        --host $DbHost `
        --port $DbPort `
        --username $Username `
        --dbname postgres `
        --command "DROP DATABASE IF EXISTS $Database;"

    & $psqlPath `
        --host $DbHost `
        --port $DbPort `
        --username $Username `
        --dbname postgres `
        --command "CREATE DATABASE $Database;"
}

& $pgRestorePath `
    --host $DbHost `
    --port $DbPort `
    --username $Username `
    --dbname $Database `
    --verbose `
    --clean `
    --if-exists `
    $BackupPath

Remove-Item Env:\PGPASSWORD -ErrorAction SilentlyContinue

Write-Host "Restauracion completada desde: $BackupPath"
