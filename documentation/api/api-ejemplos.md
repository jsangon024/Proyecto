# Ejemplos de API

Los ejemplos usan la API en Docker: `http://localhost:8080`.

## Health

```powershell
curl http://localhost:8080/api/health
```

## Login

```powershell
curl -Method POST http://localhost:8080/api/auth/login `
  -ContentType "application/json" `
  -Body '{"email":"admin@example.com","password":"Admin123!"}'
```

Guarda `accessToken`.

## Registro

```powershell
curl -Method POST http://localhost:8080/api/auth/register `
  -ContentType "application/json" `
  -Body '{"email":"usuario@example.com","password":"Usuario123!","confirmPassword":"Usuario123!"}'
```

Si no tienes SMTP configurado, copia de la consola de la API el enlace `#/verify-email/...` y abrelo en el navegador.

Si tienes SMTP configurado, el enlace llega al correo del usuario. El registro no devuelve token de sesion hasta que la cuenta queda verificada.

## Verificar email

```powershell
curl -Method POST http://localhost:8080/api/auth/verify-email `
  -ContentType "application/json" `
  -Body '{"token":"TOKEN_DEL_CORREO"}'
```

## Recuperar password

```powershell
curl -Method POST http://localhost:8080/api/auth/forgot-password `
  -ContentType "application/json" `
  -Body '{"email":"usuario@example.com"}'
```

La respuesta es generica para evitar revelar si el correo existe o no.

## Cambiar password con token

```powershell
curl -Method POST http://localhost:8080/api/auth/reset-password `
  -ContentType "application/json" `
  -Body '{"token":"TOKEN_DEL_CORREO","newPassword":"Nueva123!","confirmPassword":"Nueva123!"}'
```

## Recetas visibles

```powershell
curl http://localhost:8080/api/recipes `
  -Headers @{ Authorization = "Bearer TU_TOKEN" }
```

## Crear receta

```powershell
curl -Method POST http://localhost:8080/api/recipes `
  -Headers @{ Authorization = "Bearer TU_TOKEN" } `
  -ContentType "application/json" `
  -Body '{
    "name":"Receta demo",
    "mealType":"lunch",
    "description":"Receta creada desde API",
    "servings":1,
    "tags":["demo"],
    "ingredients":[
      {"ingredientId":"ID_INGREDIENTE","quantity":120,"unit":"g"}
    ],
    "steps":[
      {"description":"Preparar ingredientes"},
      {"description":"Cocinar y servir"}
    ]
  }'
```

Si el token es de admin, la receta sera global. Si es de usuario normal, sera propia.

## Editar receta

```powershell
curl -Method PUT http://localhost:8080/api/recipes/ID_RECETA `
  -Headers @{ Authorization = "Bearer TU_TOKEN" } `
  -ContentType "application/json" `
  -Body '{
    "name":"Receta demo actualizada",
    "mealType":"dinner",
    "description":"Texto actualizado",
    "servings":1,
    "tags":["demo","updated"],
    "ingredients":[
      {"ingredientId":"ID_INGREDIENTE","quantity":150,"unit":"g"}
    ],
    "steps":[
      {"description":"Paso actualizado 1"},
      {"description":"Paso actualizado 2"}
    ]
  }'
```

## Guardar receta

```powershell
curl -Method POST http://localhost:8080/api/recipes/ID_RECETA/save `
  -Headers @{ Authorization = "Bearer TU_TOKEN" }
```

## Admin: cambiar rol

```powershell
curl -Method PUT http://localhost:8080/api/admin/users/ID_USUARIO/role `
  -Headers @{ Authorization = "Bearer TOKEN_ADMIN" } `
  -ContentType "application/json" `
  -Body '{"role":"admin"}'
```

## Admin: cambiar password de usuario normal

```powershell
curl -Method PUT http://localhost:8080/api/admin/users/ID_USUARIO/password `
  -Headers @{ Authorization = "Bearer TOKEN_ADMIN" } `
  -ContentType "application/json" `
  -Body '{"newPassword":"Nueva123!","confirmPassword":"Nueva123!"}'
```

No se permite cambiar la password de otro administrador.

## Cambiar password propia

```powershell
curl -Method PUT http://localhost:8080/api/me/password `
  -Headers @{ Authorization = "Bearer TU_TOKEN" } `
  -ContentType "application/json" `
  -Body '{"currentPassword":"Admin123!","newPassword":"Nueva123!","confirmPassword":"Nueva123!"}'
```

## Eliminar cuenta propia

```powershell
curl -Method DELETE http://localhost:8080/api/me `
  -Headers @{ Authorization = "Bearer TU_TOKEN" } `
  -ContentType "application/json" `
  -Body '{"password":"TU_PASSWORD"}'
```

## Generar plan mensual

```powershell
curl -Method POST http://localhost:8080/api/plans/generate `
  -Headers @{ Authorization = "Bearer TU_TOKEN" } `
  -ContentType "application/json" `
  -Body '{"year":2026,"month":5}'
```

## Lista de compra

```powershell
curl -Method POST http://localhost:8080/api/shopping-list/from-plan `
  -Headers @{ Authorization = "Bearer TU_TOKEN" } `
  -ContentType "application/json" `
  -Body '{"planId":"ID_PLAN","planIds":["ID_PLAN"],"dates":[],"onlyMissing":true}'
```

## Pasar compra a inventario

```powershell
curl -Method POST http://localhost:8080/api/inventory/purchase-list `
  -Headers @{ Authorization = "Bearer TU_TOKEN" } `
  -ContentType "application/json" `
  -Body '{"items":[{"ingredientId":"ID_INGREDIENTE","quantity":500,"unit":"g"}]}'
```

## Completar dia

```powershell
curl -Method POST http://localhost:8080/api/plans/ID_PLAN/days/2026-05-16/complete `
  -Headers @{ Authorization = "Bearer TU_TOKEN" }
```
