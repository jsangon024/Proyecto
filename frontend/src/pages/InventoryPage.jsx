import { PackagePlus, Trash2 } from 'lucide-react';
import { useState } from 'react';
import { ingredientsApi } from '../api/ingredientsApi.js';
import { inventoryApi } from '../api/inventoryApi.js';
import { Field } from '../components/ui/Field.jsx';
import { Panel } from '../components/ui/Panel.jsx';
import { useAuth } from '../hooks/useAuth.jsx';
import { useResource } from '../hooks/useResource.js';
import { useToast } from '../hooks/useToast.jsx';

export function InventoryPage() {
  const { token } = useAuth();
  const { showToast } = useToast();
  const ingredients = useResource(() => ingredientsApi.list(), []);
  const inventory = useResource(() => inventoryApi.list(token), [token]);
  const [form, setForm] = useState({ ingredientId: '', quantity: 100, unit: 'g' });

  async function addItem(event) {
    event.preventDefault();
    await inventoryApi.create(token, {
      ingredientId: form.ingredientId,
      quantity: Number(form.quantity),
      unit: form.unit,
    });
    showToast('Inventario actualizado');
    inventory.reload();
  }

  async function removeItem(id) {
    await inventoryApi.remove(token, id);
    showToast('Ingrediente eliminado del inventario');
    inventory.reload();
  }

  return (
    <section className="content-grid">
      <Panel title="Actualizar inventario" icon={<PackagePlus size={18} />}>
        <form className="stack" onSubmit={addItem}>
          <Field label="Ingrediente">
            <select value={form.ingredientId} onChange={(event) => setForm({ ...form, ingredientId: event.target.value })}>
              <option value="">Seleccionar</option>
              {ingredients.data.map((ingredient) => (
                <option key={ingredient.id} value={ingredient.id}>{ingredient.name}</option>
              ))}
            </select>
          </Field>
          <div className="form-grid">
            <Field label="Cantidad">
              <input type="number" value={form.quantity} onChange={(event) => setForm({ ...form, quantity: event.target.value })} />
            </Field>
            <Field label="Unidad">
              <input value={form.unit} onChange={(event) => setForm({ ...form, unit: event.target.value })} />
            </Field>
          </div>
          <button className="primary">Guardar</button>
        </form>
      </Panel>

      <Panel title="Ingredientes disponibles">
        {inventory.error && <div className="notice">{inventory.error}</div>}
        <div className="data-list">
          {inventory.data.map((item) => (
            <div className="data-row" key={item.id}>
              <div>
                <strong>{item.ingredientName}</strong>
                <span>{item.ingredientId}</span>
              </div>
              <span>{item.quantity} {item.unit}</span>
              <button type="button" className="icon-button danger" onClick={() => removeItem(item.id)}>
                <Trash2 size={16} />
              </button>
            </div>
          ))}
        </div>
      </Panel>
    </section>
  );
}
