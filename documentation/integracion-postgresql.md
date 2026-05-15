# Guia de integracion de PostgreSQL

Esta guia explica como conectar la API .NET con PostgreSQL y dejar funcionando login, registro, recetas, ingredientes, inventario y planificador.

## 1. Requisitos

- PostgreSQL instalado.
- pgAdmin instalado.
- .NET SDK 10 instalado.
- El proyecto descargado en local.

## 2. Crear la base de datos

Abre pgAdmin y conectate al servidor PostgreSQL.

Primero abre una query conectada a la base `postgres` y ejecuta:

```sql
DROP DATABASE IF EXISTS meal_planner_db;
CREATE DATABASE meal_planner_db;
```

Despues cambia la conexion de pgAdmin a:

```text
meal_planner_db
```

## 3. Crear tablas y datos iniciales

Abre este archivo:

```text
backend/src/MealPlanner.Api/database-setup.txt
```

Copia todo el bloque que empieza en:

```sql
CREATE EXTENSION IF NOT EXISTS pgcrypto;
```

y pegalo en una query conectada a `meal_planner_db`.

Ejecutalo completo.

Ese script crea:

- `users`
- `ingredients`
- `recipes`
- `recipe_ingredients`
- `user_saved_recipes`
- `inventory_items`
- `meal_plans`
- `meal_plan_days`
- `planned_meals`

Tambien inserta:

- usuario administrador inicial
- ingredientes base
- recetas base

Usuario administrador inicial:

```text
admin@example.com
Admin123!
```

## 4. Configurar la cadena de conexion

Abre:

```text
backend/src/MealPlanner.Api/appsettings.json
```

Debe tener una cadena similar a esta:

```json
{
  "ConnectionStrings": {
    "MealPlannerDb": "Host=localhost;Port=5432;Database=meal_planner_db;Username=postgres;Password=postgres"
  },
  "AllowedHosts": "*"
}
```

Cambia `Password=postgres` por tu password real de PostgreSQL.

Ejemplo:

```json
"MealPlannerDb: Host=localhost;Port=5432;Database=meal_planner_db;Username=postgres;Password=TU_PASSWORD"
```

## 5. Ejecutar la API

Desde la raiz del proyecto:

```powershell
cd C:\Users\Usuario\Documents\Codex\2026-05-05\files-mentioned-by-the-user-arranque\meal-planner-project
dotnet run --project .\backend\src\MealPlanner.Api\MealPlanner.Api.csproj
```

La API deberia arrancar en:

```text
http://localhost:5088
```

Comprueba que responde:

```text
http://localhost:5088/api/health
```

Debe devolver:

```json
{
  "status": "ok",
  "service": "meal-planner-api"
}
```

## 6. Probar conexion con la BBDD

Prueba este endpoint en navegador:

```text
http://localhost:5088/api/ingredients
```

Si la BBDD esta bien conectada, deberias ver un listado JSON de ingredientes.

Si falla, revisa:

- PostgreSQL esta arrancado.
- La base `meal_planner_db` existe.
- Las tablas estan creadas.
- La password de `appsettings.json` es correcta.
- El puerto de PostgreSQL es `5432`.

## 7. Ejecutar el frontend

En otra terminal:

```powershell
cd C:\Users\Usuario\Documents\Codex\2026-05-05\files-mentioned-by-the-user-arranque\meal-planner-project\frontend
npm install
npm run dev
```

Abre:

```text
http://localhost:5173
```

Si quieres abrirlo automaticamente:

```powershell
npm run open
```

## 8. Orden correcto de arranque

El orden recomendado es:

1. PostgreSQL.
2. API .NET.
3. Frontend React.

Si PostgreSQL no esta funcionando, la API puede arrancar, pero fallaran login, registro, ingredientes, recetas e inventario.

Si la API no esta funcionando, el frontend abre, pero no podra hacer login ni consultar datos.

## 9. Prueba de login

En el frontend usa:

```text
admin@example.com
Admin123!
```

Si entra correctamente, significa que:

- PostgreSQL esta operativo.
- La API esta conectada a la BBDD.
- El frontend esta conectado a la API.

## 10. Problemas frecuentes

### Error de password

Revisa `appsettings.json`:

```json
"Password=TU_PASSWORD"
```

### Base de datos no encontrada

Comprueba en pgAdmin que existe:

```text
meal_planner_db
```

### Tabla no encontrada

Vuelve a ejecutar el contenido de:

```text
backend/src/MealPlanner.Api/database-setup.txt
```

conectado a `meal_planner_db`.

### El frontend no conecta

Revisa:

```text
frontend/.env.example
```

La API debe apuntar a:

```text
VITE_API_BASE_URL=http://localhost:5088
```

Si creas un archivo `.env`, debe tener ese mismo valor.
