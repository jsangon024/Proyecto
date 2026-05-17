import { useMemo, useState } from 'react';
import { Plus, Trash2 } from 'lucide-react';
import { ingredientsApi } from '../api/ingredientsApi.js';
import { Field } from '../components/ui/Field.jsx';
import { Panel } from '../components/ui/Panel.jsx';
import { SearchBox } from '../components/ui/SearchBox.jsx';
import { useAuth } from '../hooks/useAuth.jsx';
import { useResource } from '../hooks/useResource.js';
import { useToast } from '../hooks/useToast.jsx';
import { confirmDelete } from '../utils/confirmDelete.js';
import { filterByText } from '../utils/search.js';

const emptyIngredient = {
  name: '',
  category: 'verdura',
  caloriesPer100G: 0,
  proteinPer100G: 0,
  carbsPer100G: 0,
  sugarsPer100G: 0,
  fatPer100G: 0,
  fiberPer100G: 0,
  isGlutenFree: true,
  isDiabeticFriendly: true,
  glycemicIndex: 0,
};

export function IngredientsPage() {
  const { token, user } = useAuth();
  const { showToast } = useToast();
  const { data, loading, error, reload } = useResource(() => ingredientsApi.list(), []);
  const [form, setForm] = useState(emptyIngredient);
  const [ingredientSearch, setIngredientSearch] = useState('');
  const isAdmin = user.role === 'admin';
  const filteredIngredients = useMemo(
    () => filterByText(data, ingredientSearch, (ingredient) => `${ingredient.name} ${ingredient.category} ${ingredient.isGlutenFree ? 'sin gluten' : 'gluten'} ${ingredient.isDiabeticFriendly ? 'diabetico' : 'control'}`),
    [data, ingredientSearch],
  );

  async function createIngredient(event) {
    event.preventDefault();
    await ingredientsApi.create(token, normalizeIngredient(form));
    setForm(emptyIngredient);
    showToast('Ingrediente creado');
    reload();
  }

  async function deleteIngredient(ingredient) {
    if (!confirmDelete(`el ingrediente "${ingredient.name}"`)) {
      return;
    }

    await ingredientsApi.remove(token, ingredient.id);
    showToast('Ingrediente eliminado');
    reload();
  }

  return (
    <section className="content-grid">
      {isAdmin && (
        <Panel title="Nuevo ingrediente" icon={<Plus size={18} />}>
          <form className="stack" onSubmit={createIngredient}>
            <Field label="Nombre">
              <input value={form.name} onChange={(event) => setForm({ ...form, name: event.target.value })} />
            </Field>
            <Field label="Categoria">
              <input value={form.category} onChange={(event) => setForm({ ...form, category: event.target.value })} />
            </Field>
            <div className="form-grid">
              <NumberField label="Kcal/100g" value={form.caloriesPer100G} onChange={(value) => setForm({ ...form, caloriesPer100G: value })} />
              <NumberField label="Proteina/100g" value={form.proteinPer100G} onChange={(value) => setForm({ ...form, proteinPer100G: value })} />
              <NumberField label="Carbs/100g" value={form.carbsPer100G} onChange={(value) => setForm({ ...form, carbsPer100G: value })} />
              <NumberField label="Azucares/100g" value={form.sugarsPer100G} onChange={(value) => setForm({ ...form, sugarsPer100G: value })} />
              <NumberField label="Grasa/100g" value={form.fatPer100G} onChange={(value) => setForm({ ...form, fatPer100G: value })} />
              <NumberField label="Fibra/100g" value={form.fiberPer100G} onChange={(value) => setForm({ ...form, fiberPer100G: value })} />
            </div>
            <div className="checkbox-row">
              <label><input type="checkbox" checked={form.isGlutenFree} onChange={(event) => setForm({ ...form, isGlutenFree: event.target.checked })} /> Celiacos</label>
              <label><input type="checkbox" checked={form.isDiabeticFriendly} onChange={(event) => setForm({ ...form, isDiabeticFriendly: event.target.checked })} /> Diabeticos</label>
            </div>
            <NumberField label="Indice glucemico" value={form.glycemicIndex} onChange={(value) => setForm({ ...form, glycemicIndex: value })} />
            <button className="primary">Crear ingrediente</button>
          </form>
        </Panel>
      )}

      <Panel title="Catalogo de ingredientes" className={isAdmin ? '' : 'wide'}>
        {loading && <div className="empty-state">Cargando...</div>}
        {error && <div className="notice">{error}</div>}
        <SearchBox value={ingredientSearch} onChange={setIngredientSearch} placeholder="Buscar por nombre, categoria o restriccion..." />
        <div className="data-list">
          {filteredIngredients.map((ingredient) => (
            <div className="data-row" key={ingredient.id}>
              <div className="item-title">
                <strong>{ingredient.name}</strong>
                <span>{ingredient.category}</span>
              </div>
              <span>{ingredient.caloriesPer100G} kcal</span>
              <span>{ingredient.proteinPer100G} g prot.</span>
              <span>{ingredient.isGlutenFree ? 'Sin gluten' : 'Gluten'}</span>
              <span>{ingredient.isDiabeticFriendly ? 'Diabetico ok' : 'Control'}</span>
              {isAdmin && (
                <button type="button" className="icon-button danger" onClick={() => deleteIngredient(ingredient)}>
                  <Trash2 size={16} />
                </button>
              )}
            </div>
          ))}
        </div>
      </Panel>
    </section>
  );
}

function NumberField({ label, value, onChange }) {
  return (
    <Field label={label}>
      <input type="number" step="0.01" value={value} onChange={(event) => onChange(Number(event.target.value))} />
    </Field>
  );
}

function normalizeIngredient(ingredient) {
  return {
    ...ingredient,
    glycemicIndex: ingredient.glycemicIndex === '' ? null : Number(ingredient.glycemicIndex),
  };
}
