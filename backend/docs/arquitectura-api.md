# Arquitectura de la API

El backend esta organizado por responsabilidades para evitar concentrar toda la logica en `Program.cs`.

## Estructura principal

```text
MealPlanner.Api/
  Program.cs
  Data/
  Dtos/
  Endpoints/
  Middleware/
  Models/
  Services/
```

## Responsabilidades

- `Program.cs`: configuracion de servicios, CORS, middleware, seed inicial y mapeo de endpoints.
- `Models/`: modelos auxiliares de dominio no persistentes.
- `Data/Entities/`: entidades persistentes mapeadas a PostgreSQL.
- `Dtos/`: contratos de entrada y salida de la API.
- `Data/`: `DbContext` de Entity Framework Core y mapeo de tablas.
- `Services/`: logica de negocio, validaciones, seguridad, planificacion y lista de compra.
- `Endpoints/`: definicion de rutas HTTP agrupadas por modulo funcional.
- `Middleware/`: autenticacion por token Bearer y cabeceras de seguridad.

## CRUD implementado

### Administracion de usuarios

- `GET /api/admin/users` requiere rol `admin`
- `GET /api/admin/users/{id}` requiere rol `admin`
- `PUT /api/admin/users/{id}/role` requiere rol `admin`
- `DELETE /api/admin/users/{id}` requiere rol `admin`

El cuerpo para cambiar rol es:

```json
{
  "role": "admin"
}
```

Roles permitidos:

- `admin`
- `user`

### Ingredientes

- `GET /api/ingredients`
- `GET /api/ingredients/{id}`
- `POST /api/ingredients` requiere rol `admin`
- `PUT /api/ingredients/{id}` requiere rol `admin`
- `DELETE /api/ingredients/{id}` requiere rol `admin`

### Ajustes de usuario

- `PUT /api/me/goals` requiere token
- `PUT /api/me/password` requiere token
- `DELETE /api/me` requiere token

### Planes mensuales

- `GET /api/plans` requiere token
- `GET /api/plans/latest` requiere token
- `GET /api/plans/{id}` requiere token
- `POST /api/plans/generate` requiere token

### Recetas

- `GET /api/recipes`
- `GET /api/recipes/{id}`
- `POST /api/recipes` requiere rol `admin`
- `PUT /api/recipes/{id}` requiere rol `admin`
- `DELETE /api/recipes/{id}` requiere rol `admin`
- `GET /api/recipes/saved` requiere token
- `POST /api/recipes/{id}/save` requiere token
- `DELETE /api/recipes/{id}/save` requiere token

### Inventario

- `GET /api/inventory` requiere token
- `GET /api/inventory/{id}` requiere token
- `POST /api/inventory` requiere token
- `PUT /api/inventory/{id}` requiere token
- `DELETE /api/inventory/{id}` requiere token

## Estado de persistencia

La persistencia usa PostgreSQL mediante Entity Framework Core y Npgsql.

La cadena de conexion esta en `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "MealPlannerDb": "Host=localhost;Port=5432;Database=meal_planner_db;Username=postgres;Password=postgres"
  }
}
```

Si tu password de PostgreSQL es distinta, cambia el valor antes de ejecutar la API.
