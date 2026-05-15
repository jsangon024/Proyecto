# Modelo de datos preliminar

La API actual usa datos en memoria para acelerar la fase central. El modelo relacional previsto incluye:

- `Users`: usuarios, email, password hash, rol.
- `NutritionGoals`: objetivos diarios y restricciones por usuario.
- `Recipes`: recetas base.
- `RecipeIngredients`: ingredientes de cada receta.
- `InventoryItems`: inventario por usuario.
- `MealPlans`: planes mensuales generados.
- `MealPlanDays`: dias del calendario.
- `PlannedMeals`: comidas asignadas a cada dia.

## Relaciones principales

- Un usuario tiene un conjunto de objetivos nutricionales.
- Un usuario tiene muchos ingredientes de inventario.
- Un usuario tiene muchos planes mensuales.
- Una receta tiene muchos ingredientes.
- Un plan mensual tiene muchos dias.
- Un dia tiene varias comidas planificadas.
