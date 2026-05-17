-- ============================================================
-- Datos de prueba para recipe_steps
-- Ejecutar conectado a meal_planner_db
-- ============================================================

DELETE FROM recipe_steps
WHERE recipe_id IN (
    SELECT id
    FROM recipes
    WHERE name IN (
        'Avena con yogur',
        'Tortilla de espinacas',
        'Pollo con arroz',
        'Lentejas estofadas',
        'Salmon con patata',
        'Ensalada de garbanzos'
    )
);

WITH recipe AS (
    SELECT id FROM recipes WHERE name = 'Avena con yogur'
)
INSERT INTO recipe_steps (recipe_id, step_number, description)
SELECT id, 1, 'Pesar la avena y colocarla en un bol.' FROM recipe
UNION ALL
SELECT id, 2, 'Anadir el yogur y mezclar hasta que la avena quede bien integrada.' FROM recipe
UNION ALL
SELECT id, 3, 'Cortar el platano en rodajas y colocarlo por encima.' FROM recipe
UNION ALL
SELECT id, 4, 'Dejar reposar unos minutos antes de servir.' FROM recipe;

WITH recipe AS (
    SELECT id FROM recipes WHERE name = 'Tortilla de espinacas'
)
INSERT INTO recipe_steps (recipe_id, step_number, description)
SELECT id, 1, 'Lavar y escurrir bien las espinacas.' FROM recipe
UNION ALL
SELECT id, 2, 'Batir los huevos en un bol hasta que queden homogeneos.' FROM recipe
UNION ALL
SELECT id, 3, 'Saltear las espinacas en una sarten hasta que reduzcan su volumen.' FROM recipe
UNION ALL
SELECT id, 4, 'Anadir los huevos batidos y cocinar la tortilla por ambos lados.' FROM recipe;

WITH recipe AS (
    SELECT id FROM recipes WHERE name = 'Pollo con arroz'
)
INSERT INTO recipe_steps (recipe_id, step_number, description)
SELECT id, 1, 'Cocer el arroz en agua hasta que quede tierno.' FROM recipe
UNION ALL
SELECT id, 2, 'Cortar el pollo en trozos y cocinarlo en una sarten hasta que este dorado.' FROM recipe
UNION ALL
SELECT id, 3, 'Cocer o saltear el brocoli hasta que quede al dente.' FROM recipe
UNION ALL
SELECT id, 4, 'Servir el pollo junto al arroz y el brocoli.' FROM recipe;

WITH recipe AS (
    SELECT id FROM recipes WHERE name = 'Lentejas estofadas'
)
INSERT INTO recipe_steps (recipe_id, step_number, description)
SELECT id, 1, 'Lavar las lentejas y preparar la zanahoria y la patata en trozos.' FROM recipe
UNION ALL
SELECT id, 2, 'Colocar las lentejas, la zanahoria y la patata en una olla con agua.' FROM recipe
UNION ALL
SELECT id, 3, 'Cocinar a fuego medio hasta que las lentejas esten tiernas.' FROM recipe
UNION ALL
SELECT id, 4, 'Ajustar textura y servir caliente.' FROM recipe;

WITH recipe AS (
    SELECT id FROM recipes WHERE name = 'Salmon con patata'
)
INSERT INTO recipe_steps (recipe_id, step_number, description)
SELECT id, 1, 'Cocer la patata hasta que este tierna.' FROM recipe
UNION ALL
SELECT id, 2, 'Cocinar el salmon a la plancha hasta que quede dorado por fuera.' FROM recipe
UNION ALL
SELECT id, 3, 'Preparar la ensalada como acompanamiento.' FROM recipe
UNION ALL
SELECT id, 4, 'Servir el salmon con la patata y la ensalada.' FROM recipe;

WITH recipe AS (
    SELECT id FROM recipes WHERE name = 'Ensalada de garbanzos'
)
INSERT INTO recipe_steps (recipe_id, step_number, description)
SELECT id, 1, 'Escurrir los garbanzos y colocarlos en un bol amplio.' FROM recipe
UNION ALL
SELECT id, 2, 'Lavar y cortar el tomate y el pepino en dados.' FROM recipe
UNION ALL
SELECT id, 3, 'Mezclar todos los ingredientes hasta repartirlos de forma uniforme.' FROM recipe
UNION ALL
SELECT id, 4, 'Servir fria o conservar en la nevera hasta el momento de consumir.' FROM recipe;
