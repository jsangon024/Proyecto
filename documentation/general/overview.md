# Meal Planner Project

Aplicacion web para planificar comidas mensuales, gestionar recetas, controlar inventario domestico y calcular listas de compra segun objetivos nutricionales.

## Estado actual

El proyecto se encuentra en fase de entrega final local. La funcionalidad principal esta integrada y probada manualmente en entorno local:

- Backend separado en .NET 10 con ASP.NET Core Minimal API.
- Frontend separado en React con Vite.
- Persistencia en PostgreSQL mediante Entity Framework Core y Npgsql.
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
- Docker Compose para levantar backend y frontend.
- Configuracion SMTP para correos de activacion y recuperacion.
- Plan de pruebas documentado.
- Scripts de backup y restauracion de PostgreSQL.

## Estado de validacion final

- Integracion y organizacion: avanzada, con backend, frontend, Docker, PostgreSQL y documentacion estructurada.
- Plan de pruebas: disenado y ejecutado manualmente en local para los flujos funcionales principales.
- Backup/restauracion: backup implementado y restauracion probada correctamente sobre una base de prueba.

## Estructura

```text
meal-planner-project/
  backend/                 API .NET
  frontend/                Aplicacion React
  docker/                  Dockerfiles y docker-compose
  documentation/           Documentacion, guias y scripts SQL
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
- `documentation/database/integracion-postgresql.md`: configuracion de PostgreSQL.
- `documentation/api/api-ejemplos.md`: ejemplos de llamadas a la API.
- `documentation/estructura-proyecto.md`: organizacion del codigo.
- `documentation/seguimiento/bitacora.md`: decisiones, cambios y verificaciones.
- `documentation/backend/arquitectura-api.md`: arquitectura interna del backend.
- `documentation/pruebas/plan-pruebas.md`: casos de prueba.
- `documentation/database/backups.md`: copia y restauracion de BBDD.
