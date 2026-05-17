# Plan de pruebas

## Objetivo

Comprobar que Meal Planner funciona como sistema integrado: frontend, backend, PostgreSQL, autenticacion, roles, recetas, inventario, planes, lista de compra, correo y copias de seguridad.

## Entorno de pruebas

- Backend: .NET 10.
- Frontend: React + Vite.
- BBDD: PostgreSQL `meal_planner_db`.
- Ejecucion local: Docker Compose.
- Frontend: `http://localhost:5173`.
- API: `http://localhost:8080`.
- Usuario admin: `admin@example.com`.

## Datos base

Antes de probar:

1. Ejecutar `documentation/database/sql/database-setup.sql`.
2. Ejecutar `documentation/database/sql/seed-100-recipes.sql`.
3. Levantar Docker:

```powershell
cd docker
docker compose up --build
```

## Casos de prueba

| ID | Area | Caso | Pasos | Resultado esperado |
| --- | --- | --- | --- | --- |
| PT-001 | Build | Compilar backend | Ejecutar `dotnet build .\MealPlanner.slnx` | Compilacion correcta, 0 errores |
| PT-002 | Build | Compilar frontend | Ejecutar `npm run build` en `frontend` | Build correcto, carpeta `dist` generada |
| PT-003 | API | Health check | Abrir `/api/health` | Devuelve `status: ok` |
| PT-004 | Auth | Login admin correcto | Entrar con `admin@example.com` y `Admin123!` | Acceso al panel |
| PT-005 | Auth | Login incorrecto | Usar password incorrecta | Mensaje claro de credenciales incorrectas |
| PT-006 | Auth | Registro usuario | Crear usuario nuevo | Muestra aviso de verificar correo |
| PT-007 | Auth | Activacion email | Abrir enlace recibido por correo | Cuenta activada |
| PT-008 | Auth | Login antes de activar | Intentar login sin activar | No permite entrar y muestra mensaje de verificacion |
| PT-009 | Auth | Recuperar password | Usar `Has olvidado tu password?` | Llega correo de recuperacion |
| PT-010 | Auth | Reset password | Abrir enlace y cambiar password | Permite iniciar sesion con nueva password |
| PT-011 | Admin | Listar usuarios | Entrar como admin y abrir administracion | Lista usuarios |
| PT-012 | Admin | Hacer admin | Cambiar rol de usuario normal a admin | El usuario queda con rol admin |
| PT-013 | Admin | Cambiar password usuario normal | Usar accion admin de cambio password | Password actualizada |
| PT-014 | Admin | Eliminar usuario | Eliminar usuario normal y confirmar | Usuario eliminado |
| PT-015 | Ingredientes | Buscar ingrediente | Usar barra de busqueda | Filtra resultados |
| PT-016 | Ingredientes | Crear ingrediente | Crear alimento con nutricion | Aparece en catalogo |
| PT-017 | Ingredientes | Editar ingrediente | Modificar valores nutricionales | Cambios guardados |
| PT-018 | Ingredientes | Eliminar ingrediente | Eliminar con confirmacion | Ingrediente eliminado o error controlado si esta en uso |
| PT-019 | Recetas | Ver catalogo | Abrir recetas | Muestra recetas globales y propias |
| PT-020 | Recetas | Crear receta multiple | Crear receta con varios ingredientes y pasos | Receta creada correctamente |
| PT-021 | Recetas | Evitar ingrediente repetido | Intentar repetir ingrediente en receta | Muestra error de validacion |
| PT-022 | Recetas | Detalle receta | Abrir una receta | Muestra ingredientes y pasos sin romper layout |
| PT-023 | Recetas | Editar receta propia | Modificar receta propia | Cambios guardados |
| PT-024 | Recetas | Eliminar receta propia | Eliminar con confirmacion | Receta eliminada |
| PT-025 | Recetas | Permisos receta ajena | Usuario intenta editar receta ajena | Acceso denegado |
| PT-026 | Inventario | Ver inventario | Abrir inventario | Lista ingredientes disponibles sin IDs internos |
| PT-027 | Inventario | Editar cantidad | Usar boton editar, aceptar | Cantidad actualizada |
| PT-028 | Inventario | Cancelar edicion | Editar y cancelar | Se conserva valor anterior |
| PT-029 | Planes | Generar plan mensual | Crear plan del mes | Calendario guardado |
| PT-030 | Planes | Ver plan guardado | Recargar pagina y abrir plan | El plan sigue disponible |
| PT-031 | Planes | Eliminar plan | Usar boton eliminar plan | Plan eliminado tras confirmar |
| PT-032 | Lista compra | Abrir modal lista compra | Pulsar boton de lista | Modal muestra ingredientes |
| PT-033 | Lista compra | Filtrar por semana | Marcar/desmarcar semanas | Lista cambia segun semanas |
| PT-034 | Lista compra | Dia completado no suma | Completar dia | Ingredientes de ese dia dejan de sumarse |
| PT-035 | Inventario | Pasar compra a inventario | Pulsar accion de pasar compra | Ingredientes se suman al inventario |
| PT-036 | Planes | Completar dia | Pulsar `Terminar dia` | Dia queda verde e inventario descuenta ingredientes |
| PT-037 | Ajustes | Cambiar password propia | Cambiar desde ajustes | Login funciona con nueva password |
| PT-038 | Ajustes | Eliminar cuenta propia | Introducir password y confirmar | Cuenta eliminada |
| PT-039 | Backup | Crear backup | Ejecutar `backup-database.ps1` | Archivo `.backup` generado |
| PT-040 | Restore | Restaurar backup | Ejecutar `restore-database.ps1` | BBDD vuelve al estado del backup |
| PT-047 | Planes | Alimentos prohibidos | Marcar un ingrediente prohibido y generar plan | No se usan recetas que contengan ese ingrediente |

## Criterio de aceptacion

El proyecto se considera valido si:

- Compila backend y frontend sin errores.
- El usuario puede registrarse, activar cuenta, iniciar sesion y recuperar password.
- El admin puede gestionar usuarios.
- Ingredientes, recetas, inventario y planes funcionan de extremo a extremo.
- La lista de compra refleja planes guardados, semanas seleccionadas y dias completados.
- El generador respeta los alimentos prohibidos configurados por cada usuario.
- El sistema de backup/restauracion genera y restaura una copia funcional.
- No hay secretos obligatorios escritos en codigo fuente.
