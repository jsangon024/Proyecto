# Copias de seguridad y restauracion

## Objetivo

Garantizar que la base de datos PostgreSQL del proyecto puede copiarse y restaurarse ante perdida de datos, cambio de equipo o despliegue.

## Herramientas usadas

- `pg_dump`: genera copia de seguridad.
- `pg_restore`: restaura copias en formato custom.
- `psql`: recrea la base de datos si se usa restauracion completa.

Estas herramientas vienen con PostgreSQL. Si PowerShell no las reconoce, anade al `PATH` la carpeta `bin` de PostgreSQL.

Ejemplo habitual en Windows:

```text
C:\Program Files\PostgreSQL\16\bin
```

## Script de backup

Archivo:

```text
tools/database/backup-database.ps1
```

Uso basico desde la raiz del proyecto:

```powershell
.\tools\database\backup-database.ps1
```

Uso indicando credenciales:

```powershell
.\tools\database\backup-database.ps1 `
  -DbHost localhost `
  -DbPort 5432 `
  -Database meal_planner_db `
  -Username postgres `
  -Password TU_PASSWORD
```

El backup se crea por defecto en:

```text
documentation/database/backups/
```

Los archivos `.backup` estan ignorados por Git para no subir datos reales.

## Script de restauracion

Archivo:

```text
tools/database/restore-database.ps1
```

Restaurar sobre la base existente:

```powershell
.\tools\database\restore-database.ps1 `
  -BackupPath .\documentation\database\backups\meal_planner_db-YYYYMMDD_HHMMSS.backup `
  -Password TU_PASSWORD
```

Restaurar recreando la base completa:

```powershell
.\tools\database\restore-database.ps1 `
  -BackupPath .\documentation\database\backups\meal_planner_db-YYYYMMDD_HHMMSS.backup `
  -Password TU_PASSWORD `
  -RecreateDatabase
```

La opcion `-RecreateDatabase` elimina y vuelve a crear `meal_planner_db`. Usarla solo cuando se quiera restaurar una copia completa.

## Procedimiento recomendado

1. Detener temporalmente la API si se va a restaurar.
2. Ejecutar backup antes de cambios importantes.
3. Guardar el backup fuera del proyecto si contiene datos reales.
4. Restaurar en una base de pruebas antes de restaurar en la base principal.
5. Comprobar login, recetas, inventario y planes tras restaurar.

## Prueba de funcionamiento

Caso minimo:

1. Crear un backup.
2. Crear una receta o modificar un dato.
3. Restaurar el backup.
4. Comprobar que la receta o cambio posterior ya no aparece.
5. Comprobar que el usuario admin puede iniciar sesion.

Resultado esperado:

- El archivo `.backup` se genera sin errores.
- La restauracion finaliza sin errores.
- La aplicacion vuelve al estado existente en el momento del backup.
