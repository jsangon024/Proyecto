# Bitacora de proyecto

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

### Lecciones aprendidas

- Conviene validar herramientas base antes de planificar tareas de compilacion, pruebas y control de versiones.
