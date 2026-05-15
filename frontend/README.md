# Meal Planner Frontend

Aplicacion React separada del backend .NET. Esta organizada por capas:

```text
src/
  api/
  components/
  hooks/
  pages/
  App.jsx
  main.jsx
  styles.css
```

## Paginas

- Login y registro.
- Panel principal.
- Catalogo de ingredientes.
- Catalogo de recetas y recetas guardadas.
- Inventario personal.
- Generacion de plan mensual y lista de compra.
- Administracion de usuarios, solo visible para rol `admin`.

## Configuracion

Crear `.env` a partir de `.env.example` si el backend no esta en `http://localhost:5088`.

```text
VITE_API_BASE_URL=http://localhost:8080
```

Usa `http://localhost:8080` si levantas la API con Docker Compose.
Usa `http://localhost:5088` si levantas la API con `dotnet run` o IntelliJ.

## Ejecutar en local

```powershell
cd frontend
npm install
npm run dev
```

Abrir:

```text
http://localhost:5173
```

## Backend necesario

Antes de usar el frontend, la API debe estar arrancada:

```powershell
dotnet run --project backend/src/MealPlanner.Api/MealPlanner.Api.csproj
```
