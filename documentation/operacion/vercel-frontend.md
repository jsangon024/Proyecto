# Despliegue del frontend en Vercel

## Objetivo

Desplegar el frontend React/Vite de Meal Planner en Vercel y conectarlo con la API desplegada en Railway.

## Configuracion incluida

Archivo:

```text
vercel.json
```

Configuracion:

- Framework: Vite.
- Comando de instalacion: `cd frontend && npm install`.
- Comando de build: `cd frontend && npm run build`.
- Carpeta de salida: `frontend/dist`.

## Variable necesaria en Vercel

En el proyecto de Vercel, anade:

```text
VITE_API_BASE_URL=https://URL_DE_RAILWAY
```

Ejemplo:

```text
VITE_API_BASE_URL=https://proyecto-production-c7a3.up.railway.app
```

No debe terminar en `/api`, porque el cliente frontend ya llama a rutas como `/api/auth/login`.

## Pasos en Vercel

1. Entrar en Vercel.
2. Crear `Add New Project`.
3. Importar repositorio `jsangon024/Proyecto`.
4. Mantener la raiz del proyecto en la raiz del repositorio. `vercel.json` entra en `frontend` durante instalacion y build.
5. Anadir variable `VITE_API_BASE_URL`.
6. Ejecutar deploy.
7. Copiar la URL publica del frontend.

## Ajustar Railway tras desplegar frontend

Cuando Vercel genere una URL, vuelve a Railway y cambia:

```text
ALLOWED_ORIGINS=https://mealplanner-six-xi.vercel.app
FRONTEND_BASE_URL=https://mealplanner-six-xi.vercel.app
```

`FRONTEND_BASE_URL` es importante porque los correos de activacion y recuperacion usan esa URL.

## Comprobaciones

1. Abrir frontend en Vercel.
2. Login admin.
3. Registro de usuario.
4. Recibir correo de activacion.
5. Abrir enlace de activacion.
6. Recuperar password.
7. Abrir recetas, inventario y plan mensual.

## Problemas frecuentes

### Error `Missing environment variable: VITE_API_BASE_URL`

Falta la variable en Vercel o se creo despues del deploy. Anade la variable y redeploy.

### Error CORS

La URL de Vercel no esta en `ALLOWED_ORIGINS` de Railway.

### Correos apuntan a localhost

`FRONTEND_BASE_URL` en Railway sigue apuntando a `http://localhost:5173`. Cambialo a la URL de Vercel.

## URL publica actual

```text
https://mealplanner-six-xi.vercel.app
```

### API no responde

Comprueba:

```text
https://URL_DE_RAILWAY/api/health
```

URL actual de la API:

```text
https://proyecto-production-c7a3.up.railway.app/api/health
```
