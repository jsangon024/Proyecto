# Integracion con Neon

## Objetivo

Usar Neon como PostgreSQL gestionado para el proyecto Meal Planner, manteniendo la API .NET y el frontend React separados.

## Arquitectura

```text
Frontend React -> API .NET -> Neon PostgreSQL
                         -> SMTP Gmail
```

## Configuracion local con Docker

El archivo `docker/.env` debe contener la cadena de conexion en formato Npgsql:

```text
MEAL_PLANNER_DB=Host=HOST_DE_NEON;Port=5432;Database=neondb;Username=USUARIO_NEON;Password=PASSWORD_NEON;SSL Mode=Require
```

`docker/.env` no se sube a Git. Para referencia existe:

```text
docker/.env.example
```

## Restaurar datos en Neon

Si ya existe un backup local:

```powershell
.\tools\database\restore-database.ps1 `
  -BackupPath .\documentation\database\backups\meal_planner_db-YYYYMMDD_HHMMSS.backup `
  -DbHost HOST_DE_NEON `
  -Database neondb `
  -Username USUARIO_NEON `
  -Password PASSWORD_NEON
```

## Verificacion realizada

Se restauro el backup local en Neon y se verificaron los siguientes conteos:

```text
users        3
ingredients  55
recipes      107
recipe_steps 428
meal_plans   3
```

## Recomendaciones

- No subir la URL completa de Neon a GitHub.
- Rotar la password si se ha compartido por canales no seguros.
- Usar variables de entorno en despliegue.
- Mantener `SSL Mode=Require`.
- Ejecutar backups antes de cambios grandes de datos.
