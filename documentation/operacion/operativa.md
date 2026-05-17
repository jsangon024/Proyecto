# Documentacion operativa

## Objetivo

Dejar el proyecto ejecutable como sistema funcional: API .NET, frontend React, PostgreSQL, seguridad basica, datos de prueba y documentacion de mantenimiento.

## Servicios

- PostgreSQL: base `meal_planner_db`.
- Backend: API en .NET 10.
- Frontend: React con Vite.
- Docker Compose: backend y frontend contenedorizados.
- SMTP: envio de correos de activacion y recuperacion.

## Arranque recomendado

1. Arrancar PostgreSQL.
2. Crear o actualizar la base de datos.
3. Levantar API.
4. Levantar frontend.

## Base de datos

Script principal:

```text
documentation/database/sql/database-setup.sql
```

Seed de pruebas reales:

```text
documentation/database/sql/seed-100-recipes.sql
```

Orden:

```text
1. database-setup.sql
2. seed-100-recipes.sql
```

## Copias de seguridad

Documentacion:

```text
documentation/database/backups.md
```

Crear backup:

```powershell
.\tools\database\backup-database.ps1 -Password TU_PASSWORD
```

Restaurar backup:

```powershell
.\tools\database\restore-database.ps1 -BackupPath .\documentation\database\backups\ARCHIVO.backup -Password TU_PASSWORD
```

Restaurar recreando la base:

```powershell
.\tools\database\restore-database.ps1 -BackupPath .\documentation\database\backups\ARCHIVO.backup -Password TU_PASSWORD -RecreateDatabase
```

## Docker

```powershell
cd docker
docker compose up --build
```

URLs:

- Frontend: `http://localhost:5173`
- API: `http://localhost:8080`
- Health: `http://localhost:8080/api/health`

Si cambia la password local de PostgreSQL, actualiza `ConnectionStrings__MealPlannerDb` en `docker/docker-compose.yml`.

Variables importantes del backend:

```text
ALLOWED_ORIGINS=http://localhost:5173,http://localhost:3000
FRONTEND_BASE_URL=http://localhost:5173
SMTP_HOST=smtp.gmail.com
SMTP_PORT=587
SMTP_USERNAME=correo_remitente@gmail.com
SMTP_PASSWORD=password_de_aplicacion
SMTP_FROM=correo_remitente@gmail.com
SMTP_ALLOW_INVALID_CERTIFICATES=true
SMTP_TIMEOUT_SECONDS=15
ConnectionStrings__MealPlannerDb=Host=host.docker.internal;Port=5432;Database=meal_planner_db;Username=postgres;Password=TU_PASSWORD
```

`SMTP_ALLOW_INVALID_CERTIFICATES=true` solo debe usarse en desarrollo local si Docker o la red bloquean la cadena de certificados. En despliegue real debe quitarse o dejarse en `false`.

## Local sin Docker

Backend:

```powershell
dotnet run --project .\backend\src\MealPlanner.Api\MealPlanner.Api.csproj
```

Frontend:

```powershell
cd frontend
npm install
npm run dev
```

En `frontend/.env`:

```text
VITE_API_BASE_URL=http://localhost:5088
```

## Usuario inicial

```text
admin@example.com
Admin123!
```

Debe cambiarse si el proyecto se utiliza fuera de desarrollo.

## Seguridad aplicada

- Hash PBKDF2 para passwords.
- Tokens Bearer opacos.
- Autorizacion por rol `admin`.
- CORS configurado por entorno.
- Cabeceras HTTP de seguridad.
- Confirmacion en operaciones de borrado.
- Eliminacion de cuenta propia con password obligatoria.
- Activacion de cuenta por email antes del login.
- Recuperacion de password por email.
- Tokens de email guardados como hash.

## Limitaciones conocidas

- Los tokens viven en memoria y se pierden al reiniciar la API.
- No hay refresh tokens.
- No hay proyecto de tests automatizados.
- La planificacion usa una heuristica simple de calorias, tipo de comida y restricciones.
- En desarrollo puede usarse `SMTP_ALLOW_INVALID_CERTIFICATES=true`; no debe usarse en produccion.

## Verificaciones recomendadas

```powershell
dotnet build .\MealPlanner.slnx
cd frontend
npm run build
```

Plan formal de pruebas:

```text
documentation/pruebas/plan-pruebas.md
documentation/pruebas/resultados-pruebas.md
```

Pruebas funcionales manuales:

- Login admin.
- Crear usuario.
- Verificar usuario desde enlace de email.
- Recuperar password desde enlace de email.
- Crear receta propia.
- Editar receta propia.
- Generar plan mensual.
- Pasar lista de compra al inventario.
- Completar dia del calendario.
- Cambiar password desde ajustes.
- Cambiar password de usuario normal desde admin.
