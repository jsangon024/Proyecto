# Guia de despliegue

Esta guia describe un despliegue con Docker en un servidor o VPS. Es el camino recomendado para entregar el proyecto porque mantiene separados frontend, backend y configuracion.

## 1. Requisitos del servidor

- Docker instalado.
- Docker Compose instalado.
- PostgreSQL accesible desde el backend.
- Puerto publico para frontend.
- Puerto publico o interno para API.
- Cuenta SMTP para correos de activacion y recuperacion.

## 2. Componentes a desplegar

```text
frontend  React + Vite compilado y servido con Nginx
backend   API .NET 10
database  PostgreSQL
smtp      Gmail, Brevo, Mailgun, SendGrid u otro proveedor SMTP
```

En local se ha usado PostgreSQL instalado fuera de Docker. En servidor puedes mantener esa opcion o usar PostgreSQL gestionado por el proveedor.

## 3. Variables de entorno

No escribas secretos directamente en el codigo. Usa variables del servidor, `.env` local ignorado por Git o el panel de secretos del proveedor.

Variables del backend:

```text
ASPNETCORE_ENVIRONMENT=Production
ALLOWED_ORIGINS=https://TU_FRONTEND
FRONTEND_BASE_URL=https://TU_FRONTEND
ConnectionStrings__MealPlannerDb=Host=TU_HOST;Port=5432;Database=meal_planner_db;Username=TU_USUARIO;Password=TU_PASSWORD
SMTP_HOST=smtp.gmail.com
SMTP_PORT=587
SMTP_USERNAME=meal.planner.project.no.reply@gmail.com
SMTP_PASSWORD=PASSWORD_DE_APLICACION
SMTP_FROM=meal.planner.project.no.reply@gmail.com
```

Variable del frontend en build:

```text
VITE_API_BASE_URL=https://TU_API
```

No uses `SMTP_ALLOW_INVALID_CERTIFICATES=true` en produccion. Esa variable solo sirve para desarrollo local si Docker no confia en la cadena de certificados de la red.

## 4. Preparar base de datos

1. Crea la base `meal_planner_db`.
2. Ejecuta el script principal:

```text
documentation/database/sql/database-setup.sql
```

3. Para pruebas completas, ejecuta:

```text
documentation/database/sql/seed-100-recipes.sql
```

4. Cambia el password del administrador inicial tras el primer acceso:

```text
admin@example.com
Admin123!
```

## 5. Despliegue con Docker Compose

Desde la raiz del proyecto:

```powershell
cd docker
docker compose build --no-cache
docker compose up -d
```

Comprobar servicios:

```powershell
docker compose ps
docker compose logs backend --tail=120
docker compose logs frontend --tail=80
```

Comprobar API:

```text
https://TU_API/api/health
```

Respuesta esperada:

```json
{
  "status": "ok",
  "service": "meal-planner-api"
}
```

## 6. Adaptar `docker-compose.yml` para servidor

En local se usa:

```text
FRONTEND_BASE_URL=http://localhost:5173
ALLOWED_ORIGINS=http://localhost:5173,http://localhost:3000
VITE_API_BASE_URL=http://localhost:8080
```

En servidor debe quedar equivalente a:

```text
FRONTEND_BASE_URL=https://meal-planner.tudominio.com
ALLOWED_ORIGINS=https://meal-planner.tudominio.com
VITE_API_BASE_URL=https://api.meal-planner.tudominio.com
```

Si frontend y API comparten dominio mediante proxy inverso, puedes exponer la API bajo `/api` y ajustar `VITE_API_BASE_URL` al mismo dominio.

## 7. Proxy inverso y HTTPS

En produccion se recomienda poner Nginx, Caddy o Traefik delante de los contenedores.

Objetivos:

- Servir frontend por HTTPS.
- Exponer API por HTTPS.
- Redirigir HTTP a HTTPS.
- Mantener `ALLOWED_ORIGINS` limitado al dominio real.

Ejemplo de dominios:

```text
https://meal-planner.tudominio.com      frontend
https://api.meal-planner.tudominio.com  backend
```

## 8. Flujo de validacion tras desplegar

1. Abrir frontend.
2. Comprobar `/api/health`.
3. Registrar un usuario nuevo.
4. Confirmar que llega el correo de activacion.
5. Activar cuenta desde el enlace.
6. Iniciar sesion.
7. Probar recuperacion de password.
8. Entrar como admin.
9. Crear ingrediente.
10. Crear receta con varios ingredientes y pasos.
11. Generar plan mensual.
12. Abrir lista de compra.
13. Pasar compra a inventario.
14. Completar un dia y comprobar descuento de inventario.

## 9. Mantenimiento

Ver logs:

```powershell
docker compose logs backend --tail=200
docker compose logs frontend --tail=100
```

Reiniciar:

```powershell
docker compose restart
```

Actualizar imagenes tras cambios:

```powershell
docker compose down
docker compose build --no-cache
docker compose up -d
```

## 10. Seguridad antes de entregar

- No subir passwords reales a GitHub.
- Cambiar `Admin123!` tras primer acceso.
- Usar `ASPNETCORE_ENVIRONMENT=Production`.
- Quitar `SMTP_ALLOW_INVALID_CERTIFICATES`.
- Revisar `ALLOWED_ORIGINS`.
- Usar HTTPS.
- Usar una BBDD con password fuerte.
- Guardar backups de PostgreSQL.

## 11. Problemas frecuentes

### No llegan correos

Revisa:

- `SMTP_HOST`
- `SMTP_PORT`
- `SMTP_USERNAME`
- `SMTP_PASSWORD`
- `SMTP_FROM`
- password de aplicacion de Gmail
- logs del backend

Comando:

```powershell
docker compose logs backend --tail=120
```

### Error de CORS

El dominio real del frontend debe estar en:

```text
ALLOWED_ORIGINS
```

### El frontend llama a localhost

El frontend se compila con `VITE_API_BASE_URL`. Si se construyo con `localhost`, hay que recompilar la imagen con la URL real de la API.

### La API no conecta a PostgreSQL

Comprueba:

- host
- puerto
- usuario
- password
- firewall
- que la base `meal_planner_db` existe
- que los scripts SQL se han ejecutado
