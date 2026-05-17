# Meal Planner Backend

API REST en .NET 10 para la gestion de usuarios, ingredientes, recetas, inventario, planes mensuales y listas de compra.

## Responsabilidades

- Autenticacion, registro con verificacion por correo y recuperacion de password.
- Perfil de usuario y objetivos nutricionales.
- Administracion de usuarios.
- Catalogo de ingredientes globales.
- Recetas globales de administrador y recetas propias de usuario.
- Pasos e ingredientes de recetas.
- Inventario personal.
- Generacion y consulta de planes mensuales.
- Lista de compra a partir de todos los planes guardados.

## Capas

```text
src/MealPlanner.Api/
  Data/Entities/       Entidades EF Core
  Dtos/                Contratos de entrada/salida
  Endpoints/           Rutas HTTP agrupadas por modulo
  Middleware/          Token Bearer y cabeceras de seguridad
  Models/              Modelos auxiliares
  Services/            Logica de negocio
  Program.cs           Configuracion de DI, CORS, middleware y endpoints
```

## Ejecutar

```powershell
dotnet run --project .\backend\src\MealPlanner.Api\MealPlanner.Api.csproj
```

Por defecto usa la cadena `MealPlannerDb` de `appsettings.json`.

## Seguridad

- Passwords con PBKDF2 y sal aleatoria.
- Tokens Bearer opacos con expiracion.
- Endpoints privados protegidos por middleware.
- Operaciones de admin protegidas por rol.
- CORS limitado por `ALLOWED_ORIGINS`.
- Cabeceras HTTP basicas de seguridad.
- Verificacion de email antes del primer login.
- Tokens de verificacion y recuperacion guardados hasheados en base de datos.

## Correo

La API usa estas variables opcionales:

- `FRONTEND_BASE_URL`: URL del frontend para construir enlaces. Por defecto `http://localhost:5173`.
- `SMTP_HOST`, `SMTP_PORT`, `SMTP_USERNAME`, `SMTP_PASSWORD`, `SMTP_FROM`: configuracion SMTP.
- `SMTP_TIMEOUT_SECONDS`: tiempo maximo de espera de SMTP. Por defecto 15 segundos.
- `RESEND_API_KEY`, `EMAIL_FROM`: envio alternativo por API HTTPS de Resend. Si existe `RESEND_API_KEY`, se usa Resend antes que SMTP.

Si `SMTP_HOST` no esta configurado, la API no envia correo real y escribe el enlace en la consola. Esto permite probar la verificacion y la recuperacion en local sin contratar SMTP.

El envio SMTP se realiza con MailKit. En Docker local puede activarse `SMTP_ALLOW_INVALID_CERTIFICATES=true` si la red o el entorno interceptan certificados TLS. Esta opcion no debe usarse en produccion.

En despliegues cloud se usa Resend mediante `RESEND_API_KEY`. SMTP queda como alternativa local.

## Endpoints principales

- Auth: `POST /api/auth/register`, `POST /api/auth/login`, `POST /api/auth/verify-email`, `POST /api/auth/forgot-password`, `POST /api/auth/reset-password`
- Usuario: `GET /api/me`, `PUT /api/me/goals`, `PUT /api/me/password`, `DELETE /api/me`
- Admin: `GET /api/admin/users`, `PUT /api/admin/users/{id}/role`, `PUT /api/admin/users/{id}/password`, `DELETE /api/admin/users/{id}`
- Ingredientes: CRUD en `/api/ingredients`
- Recetas: CRUD en `/api/recipes`, guardado en `/api/recipes/{id}/save`
- Inventario: CRUD en `/api/inventory`, compra en `/api/inventory/purchase-list`
- Planes: `/api/plans`, `/api/plans/generate`, `/api/plans/{id}/days/{date}/complete`
- Lista de compra: `POST /api/shopping-list/from-plan`

## Notas

Los tokens se guardan en memoria. Si se reinicia la API, los usuarios deben iniciar sesion de nuevo.
