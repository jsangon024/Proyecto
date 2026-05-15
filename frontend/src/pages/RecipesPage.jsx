import { Plus } from 'lucide-react';
import { useMemo, useState } from 'react';
import { ingredientsApi } from '../api/ingredientsApi.js';
import { recipesApi } from '../api/recipesApi.js';
import { Field } from '../components/ui/Field.jsx';
import { Panel } from '../components/ui/Panel.jsx';
import { RecipeCard } from '../components/recipes/RecipeCard.jsx';
import { useAuth } from '../hooks/useAuth.jsx';
import { useResource } from '../hooks/useResource.js';
import { useToast } from '../hooks/useToast.jsx';

const emptyRecipe = {
  name: '',
  mealType: 'lunch',
  description: '',
  servings: 1,
  tags: '',
  ingredientId: '',
  quantity: 100,
  unit: 'g',
};

export function RecipesPage() {
  const { token, user } = useAuth();
  const { showToast } = useToast();
  const recipes = useResource(() => recipesApi.list(), []);
  const saved = useResource(() => recipesApi.saved(token), [token]);
  const ingredients = useResource(() => ingredientsApi.list(), []);
  const [form, setForm] = useState(emptyRecipe);

  const savedIds = useMemo(() => new Set(saved.data.map((recipe) => recipe.id)), [saved.data]);
  const isAdmin = user.role === 'admin';

  async function toggleSaved(recipe) {
    if (savedIds.has(recipe.id)) {
      await recipesApi.unsave(token, recipe.id);
      showToast('Receta quitada de guardadas');
    } else {
      await recipesApi.save(token, recipe.id);
      showToast('Receta guardada');
    }
    saved.reload();
  }

  async function createRecipe(event) {
    event.preventDefault();
    await recipesApi.create(token, {
      name: form.name,
      mealType: form.mealType,
      description: form.description,
      servings: Number(form.servings),
      tags: form.tags.split(',').map((tag) => tag.trim()).filter(Boolean),
      ingredients: [
        {
          ingredientId: form.ingredientId,
          quantity: Number(form.quantity),
          unit: form.unit,
        },
      ],
    });
    setForm(emptyRecipe);
    showToast('Receta creada');
    recipes.reload();
  }

  return (
    <section className="content-grid">
      {isAdmin && (
        <Panel title="Nueva receta" icon={<Plus size={18} />}>
          <form className="stack" onSubmit={createRecipe}>
            <Field label="Nombre">
              <input value={form.name} onChange={(event) => setForm({ ...form, name: event.target.value })} />
            </Field>
            <Field label="Tipo">
              <select value={form.mealType} onChange={(event) => setForm({ ...form, mealType: event.target.value })}>
                <option value="breakfast">Desayuno</option>
                <option value="lunch">Comida</option>
                <option value="dinner">Cena</option>
                <option value="snack">Snack</option>
              </select>
            </Field>
            <Field label="Descripcion">
              <textarea value={form.description} onChange={(event) => setForm({ ...form, description: event.target.value })} />
            </Field>
            <Field label="Etiquetas">
              <input value={form.tags} onChange={(event) => setForm({ ...form, tags: event.target.value })} placeholder="vegetarian, gluten_free" />
            </Field>
            <div className="form-grid">
              <Field label="Raciones">
                <input type="number" min="1" value={form.servings} onChange={(event) => setForm({ ...form, servings: event.target.value })} />
              </Field>
              <Field label="Ingrediente">
                <select value={form.ingredientId} onChange={(event) => setForm({ ...form, ingredientId: event.target.value })}>
                  <option value="">Seleccionar</option>
                  {ingredients.data.map((ingredient) => (
                    <option key={ingredient.id} value={ingredient.id}>{ingredient.name}</option>
                  ))}
                </select>
              </Field>
              <Field label="Cantidad">
                <input type="number" value={form.quantity} onChange={(event) => setForm({ ...form, quantity: event.target.value })} />
              </Field>
              <Field label="Unidad">
                <input value={form.unit} onChange={(event) => setForm({ ...form, unit: event.target.value })} />
              </Field>
            </div>
            <button className="primary">Crear receta</button>
          </form>
        </Panel>
      )}

      <Panel title="Catalogo de recetas" className={isAdmin ? '' : 'wide'}>
        {recipes.error && <div className="notice">{recipes.error}</div>}
        <div className="card-list">
          {recipes.data.map((recipe) => (
            <RecipeCard key={recipe.id} recipe={recipe} saved={savedIds.has(recipe.id)} onToggleSaved={toggleSaved} />
          ))}
        </div>
      </Panel>
    </section>
  );
}
