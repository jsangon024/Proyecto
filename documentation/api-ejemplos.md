# Ejemplos de uso de la API

## Salud del servicio

```powershell
curl http://localhost:8080/api/health
```

## Login como administrador

```powershell
curl -Method POST http://localhost:8080/api/auth/login `
  -ContentType "application/json" `
  -Body '{"email":"admin@example.com","password":"Admin123!"}'
```

Guarda el valor `accessToken` de la respuesta.

## Actualizar objetivos nutricionales

```powershell
curl -Method PUT http://localhost:8080/api/me/goals `
  -Headers @{ Authorization = "Bearer TU_TOKEN" } `
  -ContentType "application/json" `
  -Body '{"dailyCalories":2200,"minimumProteinGrams":120,"excludedIngredients":["salmon"]}'
```

## Consultar recetas

```powershell
curl http://localhost:8080/api/recipes
```

## Consultar ingredientes

```powershell
curl http://localhost:8080/api/ingredients
```

## Convertir un usuario en administrador

```powershell
curl -Method PUT http://localhost:8080/api/admin/users/ID_DEL_USUARIO/role `
  -Headers @{ Authorization = "Bearer TOKEN_DE_ADMIN" } `
  -ContentType "application/json" `
  -Body '{"role":"admin"}'
```

## Volver a dejar un usuario como usuario normal

```powershell
curl -Method PUT http://localhost:8080/api/admin/users/ID_DEL_USUARIO/role `
  -Headers @{ Authorization = "Bearer TOKEN_DE_ADMIN" } `
  -ContentType "application/json" `
  -Body '{"role":"user"}'
```

## Eliminar un usuario desde administrador

```powershell
curl -Method DELETE http://localhost:8080/api/admin/users/ID_DEL_USUARIO `
  -Headers @{ Authorization = "Bearer TOKEN_DE_ADMIN" }
```

## Cambiar objetivos personales

```powershell
curl -Method PUT http://localhost:8080/api/me/goals `
  -Headers @{ Authorization = "Bearer TU_TOKEN" } `
  -ContentType "application/json" `
  -Body '{"dailyCalories":2300,"minimumProteinGrams":130,"excludedIngredients":[]}'
```

## Cambiar password

```powershell
curl -Method PUT http://localhost:8080/api/me/password `
  -Headers @{ Authorization = "Bearer TU_TOKEN" } `
  -ContentType "application/json" `
  -Body '{"currentPassword":"Admin123!","newPassword":"Nueva123!","confirmPassword":"Nueva123!"}'
```

## Eliminar mi cuenta

```powershell
curl -Method DELETE http://localhost:8080/api/me `
  -Headers @{ Authorization = "Bearer TU_TOKEN" }
```

## Anadir inventario

Primero consulta `GET /api/ingredients` y usa el `id` del ingrediente.

```powershell
curl -Method POST http://localhost:8080/api/inventory `
  -Headers @{ Authorization = "Bearer TU_TOKEN" } `
  -ContentType "application/json" `
  -Body '{"ingredientId":"ID_DEL_INGREDIENTE","quantity":500,"unit":"g"}'
```

## Generar plan mensual

```powershell
curl -Method POST http://localhost:8080/api/plans/generate `
  -Headers @{ Authorization = "Bearer TU_TOKEN" } `
  -ContentType "application/json" `
  -Body '{"year":2026,"month":5}'
```

## Ver planes guardados del usuario

```powershell
curl http://localhost:8080/api/plans `
  -Headers @{ Authorization = "Bearer TU_TOKEN" }
```

## Ver ultimo plan guardado

```powershell
curl http://localhost:8080/api/plans/latest `
  -Headers @{ Authorization = "Bearer TU_TOKEN" }
```

## Generar lista de compra

```powershell
curl -Method POST http://localhost:8080/api/shopping-list/from-plan `
  -Headers @{ Authorization = "Bearer TU_TOKEN" } `
  -ContentType "application/json" `
  -Body '{"planId":"ID_DEL_PLAN"}'
```
