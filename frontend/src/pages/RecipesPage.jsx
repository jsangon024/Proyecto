import { BookOpen, Edit3, Plus, Trash2 } from 'lucide-react';
import { useMemo, useState } from 'react';
import { ingredientsApi } from '../api/ingredientsApi.js';
import { recipesApi } from '../api/recipesApi.js';
import { RecipeCard } from '../components/recipes/RecipeCard.jsx';
import { Field } from '../components/ui/Field.jsx';
import { Panel } from '../components/ui/Panel.jsx';
import { SearchBox } from '../components/ui/SearchBox.jsx';
import { SearchableSelect } from '../components/ui/SearchableSelect.jsx';
import { useAuth } from '../hooks/useAuth.jsx';
import { useResource } from '../hooks/useResource.js';
import { useToast } from '../hooks/useToast.jsx';
import { confirmDelete } from '../utils/confirmDelete.js';
import { filterByText } from '../utils/search.js';

const emptyRecipe = {
  name: '',
  mealType: 'lunch',
  description: '',
  servings: 1,
  tags: '',
  ingredients: [{ ingredientId: '', quantity: 100, unit: 'g' }],
  steps: [{ description: '' }],
};

const mealTypeOptions = [
  { value: 'breakfast', label: 'Desayuno' },
  { value: 'lunch', label: 'Comida' },
  { value: 'dinner', label: 'Cena' },
  { value: 'snack', label: 'Snack' },
];

export function RecipesPage({ onNavigate }) {
  const { token, user } = useAuth();
  const { showToast } = useToast();
  const recipes = useResource(() => recipesApi.list(token), [token]);
  const saved = useResource(() => recipesApi.saved(token), [token]);
  const ingredients = useResource(() => ingredientsApi.list(), []);
  const [activeTab, setActiveTab] = useState('catalog');
  const [form, setForm] = useState(emptyRecipe);
  const [editingRecipeId, setEditingRecipeId] = useState('');
  const [recipeSearch, setRecipeSearch] = useState('');

  const savedIds = useMemo(() => new Set(saved.data.map((recipe) => recipe.id)), [saved.data]);
  const ingredientOptions = useMemo(
    () => ingredients.data.map((ingredient) => ({
      value: ingredient.id,
      label: ingredient.name,
      searchText: ingredient.category,
    })),
    [ingredients.data],
  );
  const filteredRecipes = useMemo(
    () => filterByText(recipes.data, recipeSearch, (recipe) => `${recipe.name} ${recipe.mealType} ${recipe.description} ${(recipe.tags ?? []).join(' ')}`),
    [recipes.data, recipeSearch],
  );

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

  async function saveRecipe(event) {
    event.preventDefault();
    const payload = {
      name: form.name,
      mealType: form.mealType,
      description: form.description,
      servings: Number(form.servings),
      tags: form.tags.split(',').map((tag) => tag.trim()).filter(Boolean),
      ingredients: form.ingredients.map((ingredient) => ({
        ingredientId: ingredient.ingredientId,
        quantity: Number(ingredient.quantity),
        unit: ingredient.unit,
      })),
      steps: form.steps.map((step) => ({ description: step.description })),
    };

    if (editingRecipeId) {
      await recipesApi.update(token, editingRecipeId, payload);
      showToast('Receta actualizada');
    } else {
      await recipesApi.create(token, payload);
      showToast('Receta creada');
    }

    resetForm();
    recipes.reload();
    setActiveTab('catalog');
  }

  async function deleteRecipe(recipe) {
    if (!confirmDelete(`la receta "${recipe.name}"`)) {
      return;
    }

    await recipesApi.remove(token, recipe.id);
    showToast('Receta eliminada');
    recipes.reload();
    saved.reload();
  }

  function startEditing(recipe) {
    setEditingRecipeId(recipe.id);
    setForm({
      name: recipe.name,
      mealType: recipe.mealType,
      description: recipe.description ?? '',
      servings: recipe.servings,
      tags: (recipe.tags ?? []).join(', '),
      ingredients: recipe.ingredients.map((ingredient) => ({
        ingredientId: ingredient.ingredientId,
        quantity: ingredient.quantity,
        unit: ingredient.unit,
      })),
      steps: recipe.steps.map((step) => ({ description: step.description })),
    });
    setActiveTab('create');
  }

  function resetForm() {
    setEditingRecipeId('');
    setForm(emptyRecipe);
  }

  function addIngredient() {
    setForm({ ...form, ingredients: [...form.ingredients, { ingredientId: '', quantity: 100, unit: 'g' }] });
  }

  function updateIngredient(index, patch) {
    setForm({
      ...form,
      ingredients: form.ingredients.map((ingredient, currentIndex) => (
        currentIndex === index ? { ...ingredient, ...patch } : ingredient
      )),
    });
  }

  function removeIngredient(index) {
    setForm({ ...form, ingredients: form.ingredients.filter((_, currentIndex) => currentIndex !== index) });
  }

  function addStep() {
    setForm({ ...form, steps: [...form.steps, { description: '' }] });
  }

  function updateStep(index, description) {
    setForm({
      ...form,
      steps: form.steps.map((step, currentIndex) => (currentIndex === index ? { description } : step)),
    });
  }

  function removeStep(index) {
    setForm({ ...form, steps: form.steps.filter((_, currentIndex) => currentIndex !== index) });
  }

  function ingredientOptionsFor(index) {
    const selectedByOthers = new Set(form.ingredients
      .map((ingredient, currentIndex) => (currentIndex === index ? null : ingredient.ingredientId))
      .filter(Boolean));

    return ingredientOptions.filter((option) => !selectedByOthers.has(option.value));
  }

  function canManage(recipe) {
    return user.role === 'admin' || recipe.ownerId === user.id;
  }

  return (
    <section className="wide-page">
      <div className="visual-tabs">
        <button type="button" className={activeTab === 'catalog' ? 'active' : ''} onClick={() => setActiveTab('catalog')}>
          <BookOpen size={20} />
          <span>Catalogo</span>
          <small>{recipes.data.length} recetas disponibles</small>
        </button>
        <button type="button" className={activeTab === 'create' ? 'active' : ''} onClick={() => setActiveTab('create')}>
          {editingRecipeId ? <Edit3 size={20} /> : <Plus size={20} />}
          <span>{editingRecipeId ? 'Editar receta' : 'Nueva receta'}</span>
          <small>{editingRecipeId ? 'Modifica ingredientes y pasos' : 'Crea una receta propia'}</small>
        </button>
      </div>

      {activeTab === 'catalog' && (
        <Panel title="Catalogo de recetas" className="wide">
          {recipes.error && <div className="notice">{recipes.error}</div>}
          <SearchBox value={recipeSearch} onChange={setRecipeSearch} placeholder="Buscar por nombre, tipo, descripcion o etiqueta..." />
          <div className="card-list">
            {filteredRecipes.map((recipe) => (
              <RecipeCard
                key={recipe.id}
                recipe={recipe}
                saved={savedIds.has(recipe.id)}
                onToggleSaved={toggleSaved}
                onOpen={(selectedRecipe) => onNavigate({ page: 'recipeDetail', recipeId: selectedRecipe.id })}
                canManage={canManage(recipe)}
                onEdit={startEditing}
                onDelete={deleteRecipe}
              />
            ))}
          </div>
        </Panel>
      )}

      {activeTab === 'create' && (
        <Panel title={editingRecipeId ? 'Editar receta' : 'Nueva receta'} icon={editingRecipeId ? <Edit3 size={18} /> : <Plus size={18} />} className="wide">
          <form className="stack" onSubmit={saveRecipe}>
            <div className="form-grid">
              <Field label="Nombre">
                <input value={form.name} onChange={(event) => setForm({ ...form, name: event.target.value })} />
              </Field>
              <SearchableSelect
                label="Tipo"
                value={form.mealType}
                onChange={(value) => setForm({ ...form, mealType: value })}
                options={mealTypeOptions}
                placeholder="Seleccionar tipo"
                searchPlaceholder="Filtrar tipos..."
              />
            </div>
            <Field label="Descripcion">
              <textarea value={form.description} onChange={(event) => setForm({ ...form, description: event.target.value })} />
            </Field>
            <div className="form-grid">
              <Field label="Raciones">
                <input type="number" min="1" value={form.servings} onChange={(event) => setForm({ ...form, servings: event.target.value })} />
              </Field>
              <Field label="Etiquetas">
                <input value={form.tags} onChange={(event) => setForm({ ...form, tags: event.target.value })} placeholder="vegetarian, gluten_free" />
              </Field>
            </div>

            <div className="dynamic-section">
              <div className="section-heading">
                <strong>Ingredientes</strong>
                <button type="button" className="secondary-button" onClick={addIngredient}>Anadir ingrediente</button>
              </div>
              {form.ingredients.map((ingredient, index) => (
                <div className="dynamic-row" key={`ingredient-${index}`}>
                  <SearchableSelect
                    label={`Ingrediente ${index + 1}`}
                    value={ingredient.ingredientId}
                    onChange={(value) => updateIngredient(index, { ingredientId: value })}
                    options={ingredientOptionsFor(index)}
                    searchPlaceholder="Filtrar ingredientes..."
                  />
                  <Field label="Cantidad">
                    <input type="number" min="0.01" step="0.01" value={ingredient.quantity} onChange={(event) => updateIngredient(index, { quantity: event.target.value })} />
                  </Field>
                  <Field label="Unidad">
                    <input value={ingredient.unit} onChange={(event) => updateIngredient(index, { unit: event.target.value })} />
                  </Field>
                  <button type="button" className="icon-button danger" disabled={form.ingredients.length === 1} onClick={() => removeIngredient(index)}>
                    <Trash2 size={16} />
                  </button>
                </div>
              ))}
            </div>

            <div className="dynamic-section">
              <div className="section-heading">
                <strong>Pasos</strong>
                <button type="button" className="secondary-button" onClick={addStep}>Anadir paso</button>
              </div>
              {form.steps.map((step, index) => (
                <div className="dynamic-row step-row" key={`step-${index}`}>
                  <Field label={`Paso ${index + 1}`}>
                    <textarea value={step.description} onChange={(event) => updateStep(index, event.target.value)} />
                  </Field>
                  <button type="button" className="icon-button danger" disabled={form.steps.length === 1} onClick={() => removeStep(index)}>
                    <Trash2 size={16} />
                  </button>
                </div>
              ))}
            </div>

            <div className="row-actions">
              <button className="primary">{editingRecipeId ? 'Guardar cambios' : 'Crear receta'}</button>
              {editingRecipeId && (
                <button type="button" className="secondary-button" onClick={resetForm}>
                  Cancelar edicion
                </button>
              )}
            </div>
          </form>
        </Panel>
      )}
    </section>
  );
}
