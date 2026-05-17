# Modelo de datos

## Tablas principales

- `users`: usuarios, password hash, rol, objetivos nutricionales, alimentos prohibidos y tokens temporales de verificacion/recuperacion.
- `ingredients`: alimentos globales con informacion nutricional.
- `recipes`: recetas globales o propias segun `owner_id`.
- `recipe_ingredients`: ingredientes de cada receta.
- `recipe_steps`: pasos ordenados de cada receta.
- `user_saved_recipes`: recetas guardadas por usuario.
- `inventory_items`: inventario personal.
- `meal_plans`: planes mensuales guardados por usuario.
- `meal_plan_days`: dias del calendario mensual.
- `planned_meals`: recetas asignadas a desayuno, comida o cena.

## Recetas globales y propias

`recipes.owner_id` define visibilidad:

- `00000000-0000-0000-0000-000000000001`: receta global de administrador.
- `users.id`: receta propia de un usuario.

Cada usuario ve recetas globales y recetas propias.

## Scripts

Script principal:

```text
documentation/database/sql/database-setup.sql
```

Seed grande:

```text
documentation/database/sql/seed-100-recipes.sql
```

El seed grande inserta o actualiza 100 recetas globales con ingredientes y pasos.

## Seguridad de cuenta

La tabla `users` incluye campos para activacion y recuperacion:

- `daily_calories`: objetivo diario de calorias.
- `minimum_protein_grams`: objetivo minimo diario de proteina.
- `excluded_ingredients`: identificadores de ingredientes que el usuario no puede o no quiere consumir.
- `is_email_verified`: indica si el usuario puede iniciar sesion.
- `email_verification_token_hash`: hash del token de activacion.
- `email_verification_token_expires_at`: caducidad del token de activacion.
- `password_reset_token_hash`: hash del token de recuperacion.
- `password_reset_token_expires_at`: caducidad del token de recuperacion.

Los tokens no se almacenan en claro. La API genera el token, guarda solo su hash y envia el enlace al correo del usuario.
