# Resultados de pruebas

Fecha de ultima actualizacion: 2026-05-17.

## Resumen

| Tipo | Total | Correctas | Fallidas | Pendientes |
| --- | ---: | ---: | ---: | ---: |
| Tecnicas automaticas | 3 | 3 | 0 | 0 |
| Funcionales manuales | 36 | 36 | 0 | 0 |
| Backup/restauracion | 2 | 2 | 0 | 0 |

## Entorno de ejecucion

- Entorno: local.
- Frontend: React/Vite en navegador.
- Backend: API .NET.
- Base de datos: PostgreSQL local.
- Correo: SMTP configurado con Gmail y MailKit.
- Ejecutor de pruebas funcionales: usuario del proyecto durante el desarrollo.

## Pruebas tecnicas ejecutadas

| ID | Prueba | Comando | Resultado obtenido | Estado |
| --- | --- | --- | --- | --- |
| PT-001 | Compilar backend | `dotnet build .\MealPlanner.slnx` | Compilacion correcta, 0 errores | Correcta |
| PT-002 | Compilar frontend | `npm run build` | Build correcto con Vite, bundle generado | Correcta |
| PT-041 | Validar scripts PowerShell | Parser de PowerShell sobre scripts de backup/restauracion | Scripts parseados correctamente | Correcta |

## Pruebas funcionales ejecutadas manualmente

Estas pruebas se han ejecutado manualmente durante el desarrollo en entorno local. Los fallos detectados durante la conversacion fueron corregidos y se volvio a comprobar el flujo afectado.

| ID | Caso | Resultado esperado | Resultado obtenido | Estado |
| --- | --- | --- | --- | --- |
| PT-003 | Health check | Devuelve `status: ok` | API responde correctamente | Correcta |
| PT-004 | Login admin correcto | Acceso al panel | Login correcto y acceso al panel | Correcta |
| PT-005 | Login incorrecto | Mensaje de credenciales incorrectas | Mensaje claro sin mostrar solo `401` | Correcta |
| PT-006 | Registro usuario | Aviso de verificar correo | Usuario creado y aviso mostrado | Correcta |
| PT-007 | Activacion email | Cuenta activada | Correo recibido y cuenta activada desde enlace | Correcta |
| PT-008 | Login antes de activar | Bloqueo por email no verificado | Login bloqueado hasta verificar email | Correcta |
| PT-009 | Recuperar password | Correo de recuperacion recibido | Correo recibido correctamente | Correcta |
| PT-010 | Reset password | Login con nueva password | Password cambiada y login operativo | Correcta |
| PT-011 | Listar usuarios | Lista usuarios | Panel admin muestra usuarios | Correcta |
| PT-012 | Hacer admin | Rol actualizado | Usuario actualizado a admin | Correcta |
| PT-013 | Cambiar password usuario normal | Password actualizada | Admin cambia password de usuario normal | Correcta |
| PT-014 | Eliminar usuario | Usuario eliminado | Usuario eliminado tras confirmacion | Correcta |
| PT-015 | Buscar ingrediente | Filtra resultados | Busqueda filtra el catalogo | Correcta |
| PT-016 | Crear ingrediente | Ingrediente creado | Ingrediente creado y visible | Correcta |
| PT-017 | Editar ingrediente | Cambios guardados | Ingrediente actualizado | Correcta |
| PT-018 | Eliminar ingrediente | Eliminado o error controlado | Eliminacion confirmada o bloqueo controlado si esta en uso | Correcta |
| PT-019 | Ver catalogo recetas | Muestra recetas visibles | Catalogo muestra recetas globales y propias | Correcta |
| PT-020 | Crear receta multiple | Receta creada | Receta creada con varios ingredientes y pasos | Correcta |
| PT-021 | Evitar ingrediente repetido | Error de validacion | Se evita repetir ingrediente en receta | Correcta |
| PT-022 | Detalle receta | Layout correcto | Detalle muestra ingredientes y pasos sin romper formato | Correcta |
| PT-023 | Editar receta propia | Cambios guardados | Receta propia actualizada | Correcta |
| PT-024 | Eliminar receta propia | Receta eliminada | Receta eliminada tras confirmacion | Correcta |
| PT-025 | Permisos receta ajena | Acceso denegado | Usuario sin permisos no puede editar/eliminar receta ajena | Correcta |
| PT-026 | Ver inventario | Lista sin IDs internos | Inventario visible sin mostrar tokens/IDs internos | Correcta |
| PT-027 | Editar cantidad inventario | Cantidad actualizada | Cantidad editable con aceptar | Correcta |
| PT-028 | Cancelar edicion inventario | Valor anterior conservado | Cancelar conserva el valor previo | Correcta |
| PT-029 | Generar plan mensual | Calendario guardado | Plan mensual generado y guardado | Correcta |
| PT-030 | Ver plan guardado | Plan visible tras recargar | Plan recuperable al volver a entrar | Correcta |
| PT-031 | Eliminar plan | Plan eliminado | Plan eliminado tras confirmacion | Correcta |
| PT-032 | Modal lista compra | Lista visible | Lista de compra visible en modal | Correcta |
| PT-033 | Filtrar por semana | Lista recalculada | Lista cambia segun semanas seleccionadas | Correcta |
| PT-034 | Dia completado no suma | Compra recalculada | Ingredientes de dias completados no suman en compra | Correcta |
| PT-035 | Pasar compra a inventario | Inventario actualizado | Compra incorporada al inventario | Correcta |
| PT-036 | Completar dia | Dia verde e inventario descontado | Dia completado y cantidades descontadas | Correcta |
| PT-037 | Cambiar password propia | Login con nueva password | Password propia actualizada desde ajustes | Correcta |
| PT-038 | Eliminar cuenta propia | Cuenta eliminada | Cuenta eliminada tras introducir password | Correcta |

## Pruebas de backup y restauracion

| ID | Caso | Comando base | Resultado esperado | Resultado obtenido | Estado |
| --- | --- | --- | --- | --- | --- |
| PT-039 | Crear backup | `.\tools\database\backup-database.ps1 -Password admin` | Archivo `.backup` generado | Backup creado en `documentation/database/backups/meal_planner_db-20260517_160709.backup` | Correcta |
| PT-040 | Restaurar backup | `.\tools\database\restore-database.ps1 -BackupPath .\documentation\database\backups\meal_planner_db-20260517_160709.backup -Database meal_planner_restore_test -Password admin` | BBDD restaurada | Restauracion correcta en `meal_planner_restore_test`; conteos verificados: 3 usuarios, 55 ingredientes, 107 recetas, 428 pasos, 3 planes | Correcta |

## Observaciones

- Las pruebas automaticas validan que el codigo compila y que el frontend genera build de produccion.
- Las pruebas funcionales se han ejecutado manualmente en local durante el desarrollo.
- El envio de correos de activacion y recuperacion funciona correctamente con SMTP configurado.
- La prueba de backup se completo correctamente tras localizar `pg_dump` en la instalacion local de PostgreSQL.
- La prueba de restauracion se ejecuto correctamente sobre la base de prueba `meal_planner_restore_test`, sin afectar a la base principal `meal_planner_db`.
