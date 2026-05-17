# Estructura del proyecto

## Raiz

```text
meal-planner-project/
  backend/
  frontend/
  docker/
  documentation/
  tools/
```

## Backend

```text
backend/src/MealPlanner.Api/
  Data/
    Entities/
    MealPlannerDbContext.cs
  Dtos/
  Endpoints/
  Middleware/
  Models/
  Services/
  Program.cs
```

Criterio de organizacion:

- Endpoints solo reciben peticiones, validan usuario actual y devuelven respuestas HTTP.
- Services contienen reglas de negocio.
- Dtos definen contratos publicos.
- Entities representan tablas.
- Program.cs queda como configuracion.

## Frontend

```text
frontend/src/
  api/
  components/
  config/
  hooks/
  pages/
  utils/
```

Criterio de organizacion:

- `api`: llamadas HTTP por recurso.
- `components`: UI reutilizable.
- `hooks`: estado compartido y logica asincrona.
- `pages`: pantallas completas.
- `utils`: funciones puras o utilidades compartidas.

## Documentation

```text
documentation/
  README.md
  general/
  backend/
  frontend/
  database/
    sql/
  operacion/
  api/
  pruebas/
  seguimiento/
```

Toda la documentacion del proyecto queda centralizada en esta carpeta. La raiz conserva un `README.md` minimo solo como acceso rapido al indice.

Los documentos de modelo y scripts SQL viven en `documentation/database/`.

## Docker

```text
docker/
  docker-compose.yml
  Dockerfile.backend
  Dockerfile.frontend
```

El backend se expone en `8080` y el frontend en `5173`.

## Herramientas

```text
tools/database/
  backup-database.ps1
  restore-database.ps1
```

Los scripts permiten crear y restaurar copias de PostgreSQL mediante `pg_dump` y `pg_restore`.
