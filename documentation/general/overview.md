# Meal Planner Project

Aplicacion web para planificar comidas mensuales, gestionar recetas, controlar inventario domestico y calcular listas de compra segun objetivos nutricionales.

## Estado actual

El proyecto se encuentra en fase de entrega final desplegada. La funcionalidad principal esta integrada, probada manualmente y desplegada en servicios cloud:

- Backend separado en .NET 10 con ASP.NET Core Minimal API.
- Frontend separado en React con Vite.
- Persistencia en PostgreSQL mediante Entity Framework Core, Npgsql y Neon.
- Autenticacion con password hasheada y token Bearer opaco.
- Registro con verificacion por correo.
- Recuperacion de password mediante enlace enviado por correo.
- Roles `admin` y `user`.
- Gestion de usuarios desde administracion.
- Ingredientes globales con informacion nutricional.
- Recetas globales y recetas propias por usuario.
- Recetas con ingredientes multiples y pasos multiples.
- Inventario personal editable.
- Plan mensual guardado por usuario.
- Lista de compra por planes guardados y filtro semanal.
- Marcado de dias completados con descuento de inventario.
- Docker Compose para ejecucion local.
- API desplegada en Railway.
- Frontend desplegado en Vercel.
- Base de datos desplegada en Neon.
- Correos transaccionales mediante Resend en produccion.
- Plan de pruebas documentado.
- Scripts de backup y restauracion de PostgreSQL.

## Estado de validacion final

- Integracion y organizacion: completa, con backend, frontend, Docker local, Vercel, Railway, Neon y documentacion estructurada.
- Plan de pruebas: disenado y ejecutado manualmente en local y en despliegue para los flujos principales.
- Backup/restauracion: backup implementado y restauracion probada correctamente sobre una base de prueba.

## Estructura

```text
meal-planner-project/
  backend/                 API .NET
  frontend/                Aplicacion React
  docker/                  Dockerfiles y docker-compose
  documentation/           Documentacion, guias y scripts SQL
```

## URLs de despliegue

```text
Frontend Vercel: https://mealplanner-six-xi.vercel.app
API Railway:     https://proyecto-production-c7a3.up.railway.app
Health API:      https://proyecto-production-c7a3.up.railway.app/api/health
Base de datos:   Neon PostgreSQL
Email:           Resend
```

## Arranque rapido con Docker

Requisito: PostgreSQL local con la base `meal_planner_db` creada.

```powershell
cd docker
docker compose up --build
```

Servicios:

- Frontend: `http://localhost:5173`
- API: `http://localhost:8080`
- Health check: `http://localhost:8080/api/health`

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

## Base de datos

Script principal:

```text
documentation/database/sql/database-setup.sql
```

Seed grande para pruebas reales:

```text
documentation/database/sql/seed-100-recipes.sql
```

Primero ejecuta el script principal y despues el seed de 100 recetas.

Usuario administrador inicial:

```text
admin@example.com
Admin123!
```

## Documentacion

- `documentation/operacion/operativa.md`: guia de ejecucion, despliegue local y mantenimiento.
- `documentation/operacion/despliegue.md`: guia de despliegue con Docker en servidor.
- `documentation/operacion/railway-api.md`: despliegue de API en Railway.
- `documentation/operacion/vercel-frontend.md`: despliegue de frontend en Vercel.
- `documentation/database/integracion-postgresql.md`: configuracion de PostgreSQL.
- `documentation/database/neon.md`: integracion con Neon.
- `documentation/api/api-ejemplos.md`: ejemplos de llamadas a la API.
- `documentation/estructura-proyecto.md`: organizacion del codigo.
- `documentation/seguimiento/bitacora.md`: decisiones, cambios y verificaciones.
- `documentation/backend/arquitectura-api.md`: arquitectura interna del backend.
- `documentation/pruebas/plan-pruebas.md`: casos de prueba.
- `documentation/database/backups.md`: copia y restauracion de BBDD.
