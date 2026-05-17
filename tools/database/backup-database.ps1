param(
    [string]$DbHost = "localhost",
    [int]$DbPort = 5432,
    [string]$Database = "meal_planner_db",
    [string]$Username = "postgres",
    [string]$Password = "admin",
    [string]$OutputDirectory = ".\documentation\database\backups"
)

$ErrorActionPreference = "Stop"

$pgDump = Get-Command pg_dump -ErrorAction SilentlyContinue
if (-not $pgDump) {
    $pgDump = Get-ChildItem -Path "C:\Program Files\PostgreSQL" -Recurse -Filter pg_dump.exe -ErrorAction SilentlyContinue |
        Where-Object { $_.FullName -like "*\bin\pg_dump.exe" } |
        Sort-Object FullName -Descending |
        Select-Object -First 1
}

if (-not $pgDump) {
    throw "No se encontro pg_dump. Anade la carpeta bin de PostgreSQL al PATH."
}

$pgDumpPath = if ($pgDump.Source) { $pgDump.Source } else { $pgDump.FullName }

if (-not (Test-Path $OutputDirectory)) {
    New-Item -ItemType Directory -Path $OutputDirectory | Out-Null
}

$timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
$backupPath = Join-Path $OutputDirectory "$Database-$timestamp.backup"

$env:PGPASSWORD = $Password

& $pgDumpPath `
    --host $DbHost `
    --port $DbPort `
    --username $Username `
    --format custom `
    --blobs `
    --verbose `
    --file $backupPath `
    $Database

Remove-Item Env:\PGPASSWORD -ErrorAction SilentlyContinue

Write-Host "Backup creado en: $backupPath"
