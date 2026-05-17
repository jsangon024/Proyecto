# Despliegue de la API en Railway

## Objetivo

Desplegar solo la API .NET de Meal Planner en Railway, usando Neon como base de datos PostgreSQL gestionada.

## Configuracion incluida en el repositorio

Archivo:

```text
railway.toml
```

Contenido funcional:

- Usa Dockerfile.
- Dockerfile del backend: `docker/Dockerfile.backend`.
- Healthcheck: `/api/health`.
- Reinicio en caso de fallo.

El Dockerfile del backend escucha el puerto dinamico de Railway:

```text
${PORT:-8080}
```

Railway exige que la aplicacion escuche en `0.0.0.0:$PORT`.

## Variables necesarias en Railway

En Railway, abre el servicio de la API y entra en `Variables`. Anade:

```text
ASPNETCORE_ENVIRONMENT=Production
ALLOWED_ORIGINS=http://localhost:5173
FRONTEND_BASE_URL=http://localhost:5173
ConnectionStrings__MealPlannerDb=Host=HOST_DE_NEON;Port=5432;Database=neondb;Username=USUARIO_NEON;Password=PASSWORD_NEON;SSL Mode=Require
SMTP_HOST=smtp.gmail.com
SMTP_PORT=587
SMTP_USERNAME=meal.planner.project.no.reply@gmail.com
SMTP_PASSWORD=PASSWORD_DE_APLICACION
SMTP_FROM=meal.planner.project.no.reply@gmail.com
SMTP_ALLOW_INVALID_CERTIFICATES=false
SMTP_TIMEOUT_SECONDS=15
RESEND_API_KEY=API_KEY_DE_RESEND
EMAIL_FROM=Meal Planner <onboarding@resend.dev>
```

Si `RESEND_API_KEY` esta configurada, la API usa Resend por HTTPS y no usa SMTP. Es la opcion validada en Railway.

SMTP queda como alternativa local. Con Gmail se puede probar `SMTP_PORT=465`; la API usara SSL directo para ese puerto. Con `SMTP_PORT=587` usara STARTTLS.

Cuando el frontend se despliegue en internet, actualiza:

```text
ALLOWED_ORIGINS=https://mealplanner-six-xi.vercel.app
FRONTEND_BASE_URL=https://mealplanner-six-xi.vercel.app
```

## Pasos desde Railway Dashboard

1. Entrar en Railway.
2. Crear proyecto nuevo.
3. Elegir `Deploy from GitHub repo`.
4. Seleccionar `jsangon024/Proyecto`.
5. Crear un servicio para la API.
6. Railway detectara `railway.toml` y usara `docker/Dockerfile.backend`.
7. Configurar las variables indicadas.
8. Abrir `Settings > Networking`.
9. Generar dominio publico.
10. Probar:

```text
https://URL_DE_RAILWAY/api/health
```

Respuesta esperada:

```json
{
  "status": "ok",
  "service": "meal-planner-api"
}
```

URL publica actual de Railway:

```text
https://proyecto-production-c7a3.up.railway.app
```

## Pasos con Railway CLI

Si tienes Railway CLI instalado y autenticado:

```powershell
railway login
railway link
railway up
```

Despues configura variables desde el panel o con `railway variables`.

## Comprobaciones tras desplegar

1. `/api/health` responde.
2. Login admin funciona contra Neon.
3. Registro envia correo real.
4. Recuperacion de password envia correo real.
5. Recetas, ingredientes y planes se leen desde Neon.

Estado actual: verificado correctamente con Resend.

## Problemas frecuentes

### Application failed to respond

Revisar que Railway este usando el Dockerfile actualizado. La API debe escuchar en:

```text
0.0.0.0:$PORT
```

### Error de CORS

Actualizar:

```text
ALLOWED_ORIGINS
```

con la URL real del frontend.

### Error de conexion a Neon

Revisar:

```text
ConnectionStrings__MealPlannerDb
```

Debe incluir:

```text
SSL Mode=Require
```

### No llegan correos

Revisar primero `RESEND_API_KEY` y `EMAIL_FROM`. Si no usas Resend, revisar variables SMTP y que `SMTP_ALLOW_INVALID_CERTIFICATES` sea `false` en Railway.
