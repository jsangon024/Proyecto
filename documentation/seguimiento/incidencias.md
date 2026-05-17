# Bitacora de incidencias

## Objetivo

Registrar las incidencias, errores, bloqueos y decisiones correctivas aparecidas durante el desarrollo del proyecto Meal Planner.

## Resumen

| Estado | Total |
| --- | ---: |
| Resueltas | 42 |
| Pendientes | 0 |

## Incidencias registradas

| ID | Area | Incidencia | Causa detectada | Solucion aplicada | Estado |
| --- | --- | --- | --- | --- | --- |
| INC-001 | Entorno | El proyecto necesitaba .NET pero el entorno inicial no tenia SDK completo | Solo estaba disponible runtime o faltaba SDK adecuado | Se instalo .NET SDK 10 y se adapto el proyecto a `net10.0` | Resuelta |
| INC-002 | Arquitectura | Frontend y backend estaban inicialmente demasiado acoplados | El proyecto necesitaba separacion clara para entrega | Se separo en `backend/` con .NET y `frontend/` con React/Vite | Resuelta |
| INC-003 | Frontend | Error `ERR_UNKNOWN_FILE_EXTENSION .jsx` al ejecutar `App.jsx` con Node | Se intento ejecutar un componente React directamente con `node` | Se aclaro que React debe ejecutarse con Vite mediante `npm run dev` | Resuelta |
| INC-004 | Ejecucion local | Dudas sobre donde estaban levantados los servidores | No habia una guia clara de ejecucion local/Docker | Se documento ejecucion local y Docker, con puertos `5173`, `5088` y `8080` | Resuelta |
| INC-005 | Navegador/API | Error al cargar `http://localhost:5088/api/health` desde `chrome-error://chromewebdata` | Se estaba probando desde una pagina de error del navegador, no desde la app/API correctamente | Se explico comprobar la API directamente y revisar que el backend estuviera levantado | Resuelta |
| INC-006 | Backend | Toda la logica estaba concentrada en `Program.cs` | Prototipo inicial sin separacion por capas | Se refactorizo a entidades, DTOs, servicios, endpoints, middleware y `DbContext` | Resuelta |
| INC-007 | Base de datos | Necesidad de adaptar ingredientes y recetas al modelo real | El modelo inicial dependia demasiado del usuario o de recetas concretas | Se definieron ingredientes globales, recetas globales/propias y recetas guardadas por usuario | Resuelta |
| INC-008 | PostgreSQL | Error `relation "recipes" does not exist` | La consulta se ejecuto antes de crear tablas o sobre una BBDD incorrecta | Se genero y documento `database-setup.sql` y la guia de integracion PostgreSQL | Resuelta |
| INC-009 | Docker backend | Error `NETSDK1064 Microsoft.EntityFrameworkCore.Analyzers not found` durante `docker compose up --build` | El Dockerfile publicaba con restauracion incompleta/cacheada tras copiar proyecto | Se ajusto el flujo de build/publish del Dockerfile y se verifico compilacion | Resuelta |
| INC-010 | Frontend/API | Duda sobre si el frontend estaba conectado a la API | Faltaba centralizar `VITE_API_BASE_URL` | Se creo/configuro `.env` y cliente HTTP usando `VITE_API_BASE_URL` | Resuelta |
| INC-011 | Permisos | Panel admin e ingredientes devolvian `401` | Token/sesion/rol no se estaban manejando correctamente en flujos protegidos | Se reviso autenticacion Bearer y uso de token en clientes API | Resuelta |
| INC-012 | Planificador | Plan generado se perdia al volver a entrar | El plan se generaba pero no quedaba recuperable para el usuario | Se persistieron planes mensuales y se cargo el ultimo plan guardado al entrar | Resuelta |
| INC-013 | Planificador | Planes guardados devolvian `401` | Endpoint protegido sin token o cliente sin cabecera Authorization | Se corrigio el cliente de planes para enviar token correctamente | Resuelta |
| INC-014 | Planificador | Faltaba eliminar planes mensuales | No existia accion de borrado desde UI | Se anadio boton de eliminar plan con confirmacion | Resuelta |
| INC-015 | Lista compra | Tras pasar compra a inventario ya no se generaba lista | El calculo estaba considerando inventario y podia dejar la lista vacia de forma no deseada | Se ajusto el calculo para sumar necesidades de planes guardados y respetar dias completados | Resuelta |
| INC-016 | Lista compra | La lista debia depender del mes seleccionado | La lista aparecia desalineada respecto al plan activo | Se limito la lista al plan/mes seleccionado en la pantalla de planes | Resuelta |
| INC-017 | UI calendario | Colores de dias pasados/actuales no encajaban con el criterio final | El criterio cambio: verde solo si el dia esta completado | Se ajusto el calendario para pintar verde solo dias completados y boton `Terminar dia` | Resuelta |
| INC-018 | Inventario | Se mostraba token o ID en ingredientes disponibles | Datos internos visibles en interfaz | Se elimino la visualizacion de IDs internos | Resuelta |
| INC-019 | Inventario | Cantidades no editables manualmente | Faltaba flujo de edicion en UI | Se anadio edicion con icono, aceptar y cancelar | Resuelta |
| INC-020 | Inventario/Plan | Faltaba completar dia y descontar inventario | No existia accion diaria ligada a consumo real | Se implemento `Terminar dia`, descuento de ingredientes y marcado visual | Resuelta |
| INC-021 | Login | Password incorrecta mostraba solo `401` | Error HTTP sin mensaje de negocio en UI | Se transformo en mensaje claro: email o password incorrectos | Resuelta |
| INC-022 | UI listas | Faltaban barras de busqueda en catalogos/listas/desplegables | Usabilidad baja con muchos datos | Se anadieron `SearchBox` y `SearchableSelect` reutilizables | Resuelta |
| INC-023 | UI recetas/ingredientes | Nombre y tipo aparecian demasiado pegados | Espaciado visual insuficiente | Se ajustaron estilos de catalogos e ingredientes | Resuelta |
| INC-024 | Recetas | Las recetas no tenian pasos variables | Modelo insuficiente para receta real | Se creo `recipe_steps` y pantalla de detalle con ingredientes y pasos | Resuelta |
| INC-025 | Recetas | Cualquier usuario debia crear recetas propias | Solo administracion estaba preparada para gestion de recetas | Se anadio `owner_id`, recetas globales y propias, permisos de propietario/admin | Resuelta |
| INC-026 | Recetas | Pasos largos rompian el layout lateralmente | Texto sin wrapping suficiente en detalle | Se ajusto CSS para que los pasos ocupen varias lineas sin salirse de pantalla | Resuelta |
| INC-027 | Navegacion | La URL no cambiaba al navegar y al recargar volvia al panel | Navegacion por estado interno sin ruta persistente | Se implemento navegacion por hash `#/dashboard`, `#/recipes/{id}`, etc. | Resuelta |
| INC-028 | API/Frontend | Login devolvia `failed to fetch` | Problema de conexion frontend/API o backend no levantado/configurado | Se reviso CORS, URL de API y configuracion de entorno | Resuelta |
| INC-029 | Seguridad correo | Registro devolvia `HTTP 500` y dejaba usuario creado sin email | Usuario se guardaba antes de completar envio SMTP | Se hizo rollback/eliminacion si falla envio y reenvio para cuentas pendientes | Resuelta |
| INC-030 | SMTP Docker | Gmail no enviaba por error TLS `UntrustedRoot` | El contenedor no confiaba en la cadena de certificados de la red/local | Se instalaron certificados en Docker y se migro el envio SMTP a MailKit con opcion local `SMTP_ALLOW_INVALID_CERTIFICATES` | Resuelta |
| INC-031 | Seguridad secretos | Password SMTP real estaba en `docker-compose.yml` | Configuracion sensible escrita en archivo versionable | Se cambio a variables de entorno y se creo `docker/.env.example` | Resuelta |
| INC-032 | Seeds SQL | Error de llave duplicada en `recipe_ingredients` | Seed ejecutado sobre datos ya existentes o duplicados en combinacion receta/ingrediente/unidad | Se ajusto/documento carga idempotente y uso ordenado de scripts | Resuelta |
| INC-033 | Pruebas | Faltaba plan formal de pruebas y resultados | Solo habia validaciones sueltas durante desarrollo | Se crearon `plan-pruebas.md` y `resultados-pruebas.md` | Resuelta |
| INC-034 | Backup | `pg_dump` no se encontraba aunque PostgreSQL estaba instalado | La ruta de PostgreSQL no estaba disponible en el PATH de la sesion | Se mejoraron scripts para buscar automaticamente en `C:\Program Files\PostgreSQL\*\bin` | Resuelta |
| INC-035 | Restauracion | Faltaba comprobar restauracion sin tocar la BBDD principal | Restaurar sobre principal podia sobrescribir datos actuales | Se restauro backup en `meal_planner_restore_test` y se verificaron conteos | Resuelta |
| INC-036 | Herramientas | `git` no estaba disponible en PATH | Git no accesible desde la sesion | Se evito depender de `git diff` para la entrega y se verifico con builds/documentos | Resuelta |
| INC-037 | Build frontend | `npm run build` podia fallar con `spawn EPERM` en sandbox | Windows/sandbox bloqueaba proceso de esbuild | Se ejecuto build con permisos adecuados y se verifico Vite | Resuelta |
| INC-038 | Neon | Restaurar backup local en Neon mostraba errores de propietario `role "postgres" does not exist` | El backup local contenia propietarios del rol `postgres`, que no existe en Neon | Se anadio `--no-owner` al script de restauracion y se repitio la carga correctamente | Resuelta |
| INC-039 | Railway | La API necesitaba adaptarse al puerto dinamico de Railway | Railway publica servicios esperando que escuchen en `0.0.0.0:$PORT` | Se modifico el Dockerfile para usar `${PORT:-8080}` y se anadio `railway.toml` con Dockerfile path y healthcheck | Resuelta |
| INC-040 | Vercel | El frontend vive dentro de la carpeta `frontend` y Vercel necesita saber como construirlo | El repositorio no tiene la app Vite en la raiz | Se anadio `vercel.json` con `rootDirectory`, build command y output directory, y se documento `VITE_API_BASE_URL` | Resuelta |
| INC-041 | Railway/SMTP | Registro en Railway tardaba 120 segundos y terminaba en `400` | El envio SMTP esperaba el timeout completo al conectar con Gmail desde Railway | Se anadio `SMTP_TIMEOUT_SECONDS`, timeout corto por defecto y soporte de puerto 465 con SSL directo | Resuelta |
| INC-042 | Railway/Email | Gmail SMTP no era fiable desde Railway | Plataforma cloud con problemas o bloqueo de puertos SMTP salientes | Se anadio soporte Resend por API HTTPS mediante `RESEND_API_KEY` | Resuelta |

## Incidencias destacadas por impacto

### Correo de activacion y recuperacion

Problemas acumulados:

- Sin SMTP configurado no llegaban correos reales.
- Con SMTP, el registro podia devolver `HTTP 500`.
- Gmail fallaba en Docker por TLS `UntrustedRoot`.
- La cuenta podia quedar creada aunque el correo no se enviara.

Solucion final:

- Servicio `EmailService` con MailKit.
- Variables SMTP en entorno.
- Enlaces de activacion y recuperacion.
- Rollback de usuario si falla el envio inicial.
- Reenvio de activacion para cuentas pendientes.
- `SMTP_ALLOW_INVALID_CERTIFICATES` solo para desarrollo local.

Estado: resuelto. El usuario confirma recepcion normal de correos.

### Persistencia de planes y lista de compra

Problemas acumulados:

- El plan mensual se perdia al volver a entrar.
- La lista de compra se quedaba vacia tras pasar compra a inventario.
- Era necesario filtrar por semanas y excluir dias completados.

Solucion final:

- Planes guardados por usuario.
- Modal de planes guardados.
- Lista de compra del plan seleccionado.
- Filtro semanal.
- Dias completados excluidos del calculo.
- Accion para pasar compra a inventario.

Estado: resuelto.

### Backup y restauracion

Problemas acumulados:

- Faltaban scripts.
- `pg_dump` no estaba en PATH.
- Restaurar sobre la base principal era arriesgado.

Solucion final:

- `backup-database.ps1`.
- `restore-database.ps1`.
- Busqueda automatica de binarios PostgreSQL.
- Backup probado.
- Restauracion probada sobre `meal_planner_restore_test`.

Estado: resuelto.

## Estado final de incidencias

A fecha 2026-05-17, todas las incidencias registradas se encuentran resueltas. El proyecto queda orientado a entrega final local, con pruebas funcionales manuales documentadas, backup/restauracion comprobados y documentacion operativa actualizada.
