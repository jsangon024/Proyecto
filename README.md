# Meal Planner Project

Aplicacion web para planificar comidas mensuales, gestionar recetas, controlar inventario domestico y generar listas de compra segun objetivos nutricionales.

## Despliegue final

```text
Frontend: https://mealplanner-six-xi.vercel.app
API:      https://proyecto-production-c7a3.up.railway.app
Health:   https://proyecto-production-c7a3.up.railway.app/api/health
BBDD:     Neon PostgreSQL
Email:    Resend
```

Arquitectura:

```text
React/Vercel -> API .NET/Railway -> Neon PostgreSQL
                              -> Resend
```

## Funcionalidades

- Registro con verificacion por correo.
- Login con token Bearer.
- Recuperacion de password por email.
- Roles `admin` y `user`.
- Administracion de usuarios.
- Ingredientes globales con informacion nutricional.
- Recetas globales y recetas propias.
- Recetas con multiples ingredientes y pasos.
- Inventario personal editable.
- Plan mensual guardado por usuario, con objetivos nutricionales y alimentos prohibidos.
- Lista de compra por plan y filtro semanal.
- Marcado de dias completados con descuento de inventario.
- Backup y restauracion de PostgreSQL.

## Tecnologias

- Backend: .NET 10, ASP.NET Core Minimal API, Entity Framework Core, Npgsql.
- Frontend: React, Vite.
- Base de datos: PostgreSQL en Neon.
- Email: Resend en despliegue cloud, SMTP/MailKit como alternativa local.
- Despliegue: Railway para API, Vercel para frontend.
- Local: Docker Compose.

## Estructura

```text
backend/        API .NET
frontend/       Aplicacion React
docker/         Dockerfiles, compose y .env.example
documentation/  Documentacion tecnica, operativa y pruebas
tools/          Scripts de backup/restauracion
```

## Arranque local con Docker

1. Crear `docker/.env` a partir de `docker/.env.example`.
2. Ajustar variables de BBDD y correo.
3. Ejecutar:

```powershell
cd docker
docker compose up --build
```

URLs locales:

```text
Frontend: http://localhost:5173
API:      http://localhost:8080
Health:   http://localhost:8080/api/health
```

## Arranque local sin Docker

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

## Variables principales

Backend:

```text
ASPNETCORE_ENVIRONMENT=Production
ALLOWED_ORIGINS=https://mealplanner-six-xi.vercel.app
FRONTEND_BASE_URL=https://mealplanner-six-xi.vercel.app
TOKEN_SIGNING_KEY=clave_larga_aleatoria_y_estable
ConnectionStrings__MealPlannerDb=Host=...;Database=neondb;Username=...;Password=...;SSL Mode=Require
RESEND_API_KEY=...
EMAIL_FROM=Meal Planner <onboarding@resend.dev>
```

Frontend:

```text
VITE_API_BASE_URL=https://proyecto-production-c7a3.up.railway.app
```

## Base de datos

Scripts principales:

```text
documentation/database/sql/database-setup.sql
documentation/database/sql/seed-100-recipes.sql
```

Backup:

```powershell
.\tools\database\backup-database.ps1 -Password TU_PASSWORD
```

Restauracion:

```powershell
.\tools\database\restore-database.ps1 -BackupPath .\documentation\database\backups\ARCHIVO.backup -Password TU_PASSWORD
```

## Pruebas

El plan y resultados estan documentados en:

```text
documentation/pruebas/plan-pruebas.md
documentation/pruebas/resultados-pruebas.md
```

Estado final:

```text
Pruebas tecnicas:             correctas
Pruebas funcionales manuales: correctas
Backup/restauracion:          correctos
Despliegue cloud:             correcto
```

## Documentacion

Indice principal:

[documentation/README.md](documentation/README.md)

Guias destacadas:

- [Despliegue general](documentation/operacion/despliegue.md)
- [API en Railway](documentation/operacion/railway-api.md)
- [Frontend en Vercel](documentation/operacion/vercel-frontend.md)
- [Integracion Neon](documentation/database/neon.md)
- [Backup y restauracion](documentation/database/backups.md)
- [Plan de pruebas](documentation/pruebas/plan-pruebas.md)
- [Resultados de pruebas](documentation/pruebas/resultados-pruebas.md)
- [Bitacora](documentation/seguimiento/bitacora.md)
- [Incidencias](documentation/seguimiento/incidencias.md)

## Seguridad

- No subir `.env` ni credenciales reales.
- Usar variables de entorno en Railway y Vercel.
- Mantener `TOKEN_SIGNING_KEY` estable para conservar sesiones entre despliegues.
- Mantener `SMTP_ALLOW_INVALID_CERTIFICATES=false` en produccion.
- Rotar claves compartidas durante pruebas.
- Cambiar el password del administrador inicial fuera de entornos de prueba.
- No precargar credenciales administrativas en el frontend.
