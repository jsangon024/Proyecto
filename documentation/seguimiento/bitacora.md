# Bitacora de proyecto

Registro detallado de incidencias:

```text
documentation/seguimiento/incidencias.md
```

## 2026-05-05

### Decisiones tecnicas

- Se define el backend en .NET 10 para ajustarlo al SDK instalado y a una base actual de ASP.NET Core.
- Se crea una API REST minimalista para reducir complejidad inicial.
- Se usa almacenamiento en memoria en esta fase para demostrar flujos funcionales sin depender todavia de la base de datos.
- Se incorpora autenticacion propia con token Bearer opaco y hash PBKDF2 para cubrir medidas basicas de seguridad.
- Se prepara Docker para documentar el camino de despliegue.
- Se separa el frontend del backend y se crea una aplicacion React con Vite.

### Problemas encontrados

- Inicialmente el entorno no tenia .NET SDK instalado, solo runtime. Despues se instalo .NET SDK 10.
- El comando `git` no esta disponible en el PATH.
- Docker no esta disponible en el PATH del entorno actual.

### Soluciones aplicadas

- Se crean los archivos del proyecto manualmente.
- Se verifica la compilacion tras instalar .NET SDK 10.
- Se deja preparado `docker-compose.yml`, aunque el arranque de contenedores queda pendiente de un entorno con Docker.
- Se instala el frontend React y se verifica el build de produccion con Vite.
- Se refactoriza la API para sacar la logica de `Program.cs` y separar modelos, DTOs, servicios, endpoints, middleware y datos.
- Se amplia el CRUD de recetas e inventario con consulta por id, actualizacion y borrado.
- Se adapta la API al modelo PostgreSQL: ingredientes globales, recetas globales, recetas guardadas por usuario e inventario enlazado a ingredientes.
- Se sustituye el almacenamiento en memoria por Entity Framework Core con Npgsql.
- Se anade administracion de usuarios: solo un administrador puede cambiar el rol de otro usuario a `admin` o `user`.
- Se reconstruye el frontend React con separacion por paginas, componentes, hooks y cliente API.
- El frontend se adapta a PostgreSQL: ingredientes globales, recetas guardadas, inventario por `ingredientId` y administracion de usuarios.
- Se anaden permisos para que administradores eliminen cuentas de usuarios.
- Se anaden ajustes de usuario: objetivos nutricionales, cambio de password y eliminacion de cuenta propia.
- El registro incorpora confirmacion de password.
- Se anade recuperacion de planes guardados: el usuario puede volver a ver su calendario al entrar en la aplicacion.
- El frontend del planificador carga automaticamente el ultimo plan y permite seleccionar planes anteriores.
- Se anade filtrado semanal de lista de compra desde el calendario mensual.
- Se anade accion para pasar la lista de compra seleccionada al inventario del usuario.
- Se anade edicion manual de cantidades en inventario sin mostrar IDs internos.
- Se anade marcado de dia completado: descuenta ingredientes del inventario y pinta el dia en verde.
- Se anaden recetas con pasos multiples e ingredientes multiples.
- Se anade visibilidad de recetas globales y recetas propias por `owner_id`.
- Se permite crear recetas a cualquier usuario.
- Se permite editar/eliminar recetas a administradores y propietarios.
- Se anade navegacion por hash para conservar pagina al recargar.
- Se centraliza confirmacion de borrados.
- Se exige password para eliminar cuenta propia.
- Se permite a administradores cambiar password de usuarios normales.
- Se crea `documentation/database/sql/seed-100-recipes.sql` con 100 recetas globales, ingredientes y pasos para pruebas reales.
- Se actualiza documentacion operativa, API, arquitectura, frontend, backend y modelo de datos.
- Se anade verificacion de cuenta por email y recuperacion de password por enlace.
- Se sustituye el envio SMTP por MailKit para controlar mejor TLS en Docker local.
- Se documentan variables SMTP, activacion de usuarios, recuperacion de password y despliegue con Docker.
- Se elimina la password SMTP del `docker-compose.yml` y se crea `docker/.env.example`.
- Se crea plan formal de pruebas y hoja de resultados.
- Se crean scripts PowerShell para backup y restauracion de PostgreSQL.
- Se actualiza la hoja de resultados para reflejar las pruebas funcionales manuales ejecutadas en local.
- Se confirma que el envio de correos de activacion y recuperacion funciona correctamente con SMTP configurado.

### Verificaciones

- `dotnet build .\backend\src\MealPlanner.Api\MealPlanner.Api.csproj`: correcto, 0 errores y 0 advertencias.
- `npm install` en `frontend/`: correcto, 0 vulnerabilidades.
- `npm run build` en `frontend/`: correcto, salida generada en `frontend/dist`.
- Backend arrancado en `http://localhost:5088` y comprobado con `/api/health`.
- Frontend React arrancado en `http://localhost:5173`.
- `dotnet build .\MealPlanner.slnx` tras el refactor de API: correcto, 0 errores y 0 advertencias.
- Prueba temporal en `http://localhost:5090`: health, login, recetas y generacion de plan correctos.
- `dotnet build .\MealPlanner.slnx` tras integrar PostgreSQL: correcto, 0 errores y 0 advertencias.
- `dotnet build .\MealPlanner.slnx` tras anadir gestion de administradores: correcto, 0 errores y 0 advertencias.
- La instalacion npm del frontend queda pendiente de verificar porque el entorno actual mantiene npm en modo offline o bloquea el acceso al registro.
- `dotnet build .\MealPlanner.slnx` y `npm run build` tras cambios funcionales de usuarios/planificador: correctos.
- `dotnet build .\MealPlanner.slnx` y `npm run build` tras persistencia visible de planes: correctos.
- `dotnet build .\MealPlanner.slnx` y `npm run build` tras filtro semanal y compra a inventario: correctos.
- `dotnet build .\MealPlanner.slnx` y `npm run build` tras edicion de inventario y dias completados: correctos.
- `dotnet build .\MealPlanner.slnx` y `npm run build` tras administracion de passwords y documentacion: correctos.
- `dotnet build .\MealPlanner.slnx` tras integracion de MailKit: correcto, 0 errores y 0 advertencias.
- Pruebas funcionales manuales principales: registro, verificacion email, login, recuperacion password, admin, ingredientes, recetas, inventario, plan mensual, lista de compra, completar dia y ajustes de usuario: correctas en entorno local.
- Backup PostgreSQL: correcto, archivo generado en `documentation/database/backups/meal_planner_db-20260517_160709.backup`.
- Restauracion PostgreSQL: correcta sobre la base de prueba `meal_planner_restore_test`, con datos verificados por conteo.
- Restauracion en Neon: correcta, con datos verificados por conteo en la base gestionada.

### Lecciones aprendidas

- Conviene validar herramientas base antes de planificar tareas de compilacion, pruebas y control de versiones.
