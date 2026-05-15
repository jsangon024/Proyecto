# Meal Planner Backend

API REST en .NET para una aplicacion web que genera planes mensuales de comidas, gestiona inventario domestico y calcula listas de compra.

## Estado de esta fase

- Estructura principal creada.
- API funcional conectada a PostgreSQL mediante Entity Framework Core y Npgsql.
- Logica separada en modelos, DTOs, servicios, endpoints, middleware y capa de datos temporal.
- Registro e inicio de sesion.
- Passwords almacenadas con PBKDF2.
- Tokens Bearer opacos con expiracion.
- Rol `admin` para gestion de recetas.
- Administradores con capacidad de convertir otros usuarios en administradores.
- CRUD de ingredientes globales.
- CRUD de recetas globales.
- Recetas guardadas por usuario.
- CRUD de inventario por usuario usando ingredientes globales.
- Endpoints de plan mensual y lista de compra.

## Ejecutar en local

Requisito: .NET SDK 10.0 o superior.

Requisito de base de datos: PostgreSQL con la base `meal_planner_db` creada usando el script acordado.

Configura la cadena de conexion en `src/MealPlanner.Api/appsettings.json`:

```json
"MealPlannerDb": "Host=localhost;Port=5432;Database=meal_planner_db;Username=postgres;Password=postgres"
```

```powershell
cd backend/src/MealPlanner.Api
dotnet run
```

La API expone `GET /api/health` para comprobar que el servicio esta operativo.

Usuario administrador de demo:

- Email: `admin@example.com`
- Password: `Admin123!`

Estas credenciales son solo para desarrollo y deben cambiarse antes de publicar el proyecto.

## Endpoints principales

- `POST /api/auth/register`
- `POST /api/auth/login`
- `GET /api/me`
- `PUT /api/me/goals`
- `GET /api/admin/users` requiere rol `admin`
- `GET /api/admin/users/{id}` requiere rol `admin`
- `PUT /api/admin/users/{id}/role` requiere rol `admin`
- `GET /api/ingredients`
- `GET /api/ingredients/{id}`
- `POST /api/ingredients` requiere rol `admin`
- `PUT /api/ingredients/{id}` requiere rol `admin`
- `DELETE /api/ingredients/{id}` requiere rol `admin`
- `GET /api/recipes`
- `GET /api/recipes/{id}`
- `POST /api/recipes` requiere rol `admin`
- `PUT /api/recipes/{id}` requiere rol `admin`
- `DELETE /api/recipes/{id}` requiere rol `admin`
- `GET /api/recipes/saved` requiere token
- `POST /api/recipes/{id}/save` requiere token
- `DELETE /api/recipes/{id}/save` requiere token
- `GET /api/inventory` requiere token
- `GET /api/inventory/{id}` requiere token
- `POST /api/inventory` requiere token
- `PUT /api/inventory/{id}` requiere token
- `DELETE /api/inventory/{id}` requiere token
- `POST /api/plans/generate` requiere token
- `GET /api/plans/latest` requiere token
- `POST /api/shopping-list/from-plan` requiere token

## Arquitectura

Consulta `docs/arquitectura-api.md` para ver la distribucion de responsabilidades por carpetas.

## Siguiente paso tecnico

Sustituir `AppStore` por una capa de persistencia con Entity Framework Core y una base de datos relacional, manteniendo los contratos de API ya definidos.
