# Meal Planner Project

Aplicacion web para generar planes mensuales de comidas personalizados segun objetivos nutricionales, restricciones alimentarias e inventario domestico.

## Estructura

```text
meal-planner-project/
  backend/
    src/
      MealPlanner.Api/
  frontend/
    src/
  database/
    schema/
  docker/
  documentation/
```

El backend y el frontend son aplicaciones separadas:

- `backend/`: API REST en .NET 10.
- `frontend/`: aplicacion React con Vite.

## Entrega actual

Esta fase cubre:

- Estructura principal del proyecto.
- Backend .NET con API REST inicial.
- Frontend React separado del backend.
- Servicios operativos de autenticacion, recetas, inventario, planificacion y lista de compra.
- Medidas de seguridad basicas.
- Documentacion operativa y bitacora.

Documentos utiles:

- `documentation/operativa.md`
- `documentation/api-ejemplos.md`
- `documentation/bitacora.md`

## Requisito para ejecutar

Instalar .NET SDK 10.0 o superior para el backend y Node.js 22 o superior para el frontend.

Backend:

```powershell
cd backend/src/MealPlanner.Api
dotnet run
```

Frontend:

```powershell
cd frontend
npm install
npm run dev
```
