# Arquitectura de la API

La API esta organizada por capas para mantener `Program.cs` como archivo de configuracion y evitar concentrar logica de negocio en endpoints.

## Capas

- `Program.cs`: DI, CORS, middlewares, seguridad y mapeo de endpoints.
- `Data/Entities`: entidades persistentes.
- `Data/MealPlannerDbContext.cs`: mapeo EF Core a PostgreSQL.
- `Dtos`: contratos HTTP.
- `Endpoints`: rutas agrupadas por recurso.
- `Services`: reglas de negocio.
- `EmailService`: envio SMTP de activacion y recuperacion.
- `Middleware`: autenticacion Bearer y cabeceras de seguridad.
- `Models`: modelos auxiliares no persistentes.

## Reglas de dominio destacadas

- Solo `admin` puede gestionar usuarios e ingredientes.
- Un admin puede hacer admin a otro usuario.
- Un admin puede cambiar password de usuarios normales, pero no de administradores.
- Cada usuario puede cambiar su password y eliminar su cuenta introduciendo password actual.
- Las cuentas nuevas deben verificar su email antes de iniciar sesion.
- La recuperacion de password se hace con token temporal enviado por correo.
- Los tokens de verificacion y recuperacion no se guardan en claro, solo como hash.
- Las recetas pueden ser globales o propias.
- Una receta global usa `owner_id = 00000000-0000-0000-0000-000000000001`.
- Una receta propia usa `owner_id = users.id`.
- Administradores y propietarios pueden editar o eliminar recetas.
- No se elimina una receta usada en planes mensuales.
- Al completar un dia se descuenta inventario y el dia queda completado.

## Endpoints

### Auth

- `POST /api/auth/register`
- `POST /api/auth/login`
- `POST /api/auth/verify-email`
- `POST /api/auth/forgot-password`
- `POST /api/auth/reset-password`

### Usuario

- `GET /api/me`
- `PUT /api/me/goals`
- `PUT /api/me/password`
- `DELETE /api/me`

### Admin

- `GET /api/admin/users`
- `GET /api/admin/users/{id}`
- `PUT /api/admin/users/{id}/role`
- `PUT /api/admin/users/{id}/password`
- `DELETE /api/admin/users/{id}`

### Ingredientes

- `GET /api/ingredients`
- `GET /api/ingredients/{id}`
- `POST /api/ingredients`
- `PUT /api/ingredients/{id}`
- `DELETE /api/ingredients/{id}`

### Recetas

- `GET /api/recipes`
- `GET /api/recipes/{id}`
- `POST /api/recipes`
- `PUT /api/recipes/{id}`
- `DELETE /api/recipes/{id}`
- `GET /api/recipes/saved`
- `POST /api/recipes/{id}/save`
- `DELETE /api/recipes/{id}/save`

### Inventario

- `GET /api/inventory`
- `GET /api/inventory/{id}`
- `POST /api/inventory`
- `PUT /api/inventory/{id}`
- `DELETE /api/inventory/{id}`
- `POST /api/inventory/purchase-list`

### Planes

- `GET /api/plans`
- `GET /api/plans/latest`
- `GET /api/plans/{id}`
- `POST /api/plans/generate`
- `DELETE /api/plans/{id}`
- `POST /api/plans/{planId}/days/{date}/complete`

### Lista de compra

- `POST /api/shopping-list/from-plan`

La lista de compra puede calcularse para uno o varios planes, y permite filtrar fechas del plan seleccionado.
