# Documentacion operativa inicial

## Objetivo de la fase

Convertir la idea inicial en un sistema funcional basico: API operativa, servicios principales, seguridad minima y preparacion para despliegue mediante Docker.

## Servicios implementados

- Autenticacion: registro, login y token Bearer.
- Usuarios: consulta de perfil y actualizacion de objetivos nutricionales.
- Recetas: consulta publica y alta restringida a administrador.
- Inventario: alta, consulta y eliminacion de ingredientes disponibles en casa.
- Planificacion: generacion de calendario mensual de desayuno, comida y cena.
- Lista de compra: calculo de ingredientes faltantes a partir de un plan.
- Frontend React independiente para consumir la API.
- Backend organizado por capas internas: endpoints, modelos, DTOs, servicios, middleware y datos.
- Persistencia en PostgreSQL mediante Entity Framework Core y Npgsql.

## Seguridad aplicada

- Las passwords se guardan con hash PBKDF2 y sal aleatoria.
- Los endpoints privados requieren token Bearer.
- Los endpoints de administracion validan rol `admin`.
- CORS queda limitado a origenes configurados en `ALLOWED_ORIGINS`.
- Cabeceras basicas de endurecimiento HTTP: `X-Content-Type-Options`, `X-Frame-Options`, `Referrer-Policy` y `Permissions-Policy`.
- Credenciales de demostracion documentadas como no validas para produccion.

## Limitaciones actuales

- Tokens en memoria: se invalidan al reiniciar el servicio.
- El motor de planificacion usa una heuristica simple por calorias y restricciones.
- No hay pruebas automatizadas hasta instalar SDK y crear proyecto de tests.

## Operacion local

1. Instalar .NET SDK 10.0 o superior.
2. Crear la base `meal_planner_db` en PostgreSQL con el script SQL del proyecto.
3. Revisar la cadena de conexion en `backend/src/MealPlanner.Api/appsettings.json`.
4. Ejecutar el backend:

```powershell
cd backend/src/MealPlanner.Api
dotnet run
```

5. Comprobar salud:

```powershell
curl http://localhost:5088/api/health
```

4. Ejecutar el frontend:

```powershell
cd frontend
npm install
npm run dev
```

## Operacion con Docker

```powershell
cd docker
docker compose up --build
```

La API quedara disponible en:

```text
http://localhost:8080/api/health
```

El frontend quedara disponible en:

```text
http://localhost:5173
```

## Proximos hitos

- Sustituir almacenamiento en memoria por Entity Framework Core.
- Crear migraciones y base de datos relacional.
- Anadir proyecto de pruebas del motor de planificacion.
- Crear frontend y conectar login, calendario, inventario y lista de compra.
- Separar configuracion de desarrollo y produccion.
