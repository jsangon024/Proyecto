import { Check, Edit3, PackagePlus, Trash2, X } from 'lucide-react';
import { useMemo, useState } from 'react';
import { ingredientsApi } from '../api/ingredientsApi.js';
import { inventoryApi } from '../api/inventoryApi.js';
import { Field } from '../components/ui/Field.jsx';
import { Panel } from '../components/ui/Panel.jsx';
import { SearchBox } from '../components/ui/SearchBox.jsx';
import { SearchableSelect } from '../components/ui/SearchableSelect.jsx';
import { useAuth } from '../hooks/useAuth.jsx';
import { useResource } from '../hooks/useResource.js';
import { useToast } from '../hooks/useToast.jsx';
import { confirmDelete } from '../utils/confirmDelete.js';
import { filterByText } from '../utils/search.js';

export function InventoryPage() {
  const { token } = useAuth();
  const { showToast } = useToast();
  const ingredients = useResource(() => ingredientsApi.list(), []);
  const inventory = useResource(() => inventoryApi.list(token), [token]);
  const [form, setForm] = useState({ ingredientId: '', quantity: 100, unit: 'g' });
  const [editingItemId, setEditingItemId] = useState('');
  const [editingQuantity, setEditingQuantity] = useState(0);
  const [inventorySearch, setInventorySearch] = useState('');
  const ingredientOptions = useMemo(
    () => ingredients.data.map((ingredient) => ({
      value: ingredient.id,
      label: ingredient.name,
      searchText: ingredient.category,
    })),
    [ingredients.data],
  );
  const filteredInventory = useMemo(
    () => filterByText(inventory.data, inventorySearch, (item) => `${item.ingredientName} ${item.quantity} ${item.unit}`),
    [inventory.data, inventorySearch],
  );

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

  async function removeItem(item) {
    if (!confirmDelete(`"${item.ingredientName}" del inventario`)) {
      return;
    }

    await inventoryApi.remove(token, item.id);
    showToast('Ingrediente eliminado del inventario');
    inventory.reload();
  }

  function startEditing(item) {
    setEditingItemId(item.id);
    setEditingQuantity(item.quantity);
  }

  function cancelEditing() {
    setEditingItemId('');
    setEditingQuantity(0);
  }

  async function saveQuantity(item) {
    await inventoryApi.update(token, item.id, {
      ingredientId: item.ingredientId,
      quantity: Number(editingQuantity),
      unit: item.unit,
    });
    showToast('Cantidad actualizada');
    cancelEditing();
    inventory.reload();
  }

  return (
    <section className="content-grid">
      <Panel title="Actualizar inventario" icon={<PackagePlus size={18} />}>
        <form className="stack" onSubmit={addItem}>
          <SearchableSelect
            label="Ingrediente"
            value={form.ingredientId}
            onChange={(value) => setForm({ ...form, ingredientId: value })}
            options={ingredientOptions}
            searchPlaceholder="Filtrar ingredientes..."
          />
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
        <SearchBox value={inventorySearch} onChange={setInventorySearch} placeholder="Buscar en el inventario..." />
        <div className="data-list">
          {filteredInventory.map((item) => (
            <div className="data-row" key={item.id}>
              <div className="item-title">
                <strong>{item.ingredientName}</strong>
              </div>
              {editingItemId === item.id ? (
                <input
                  className="quantity-input"
                  type="number"
                  value={editingQuantity}
                  onChange={(event) => setEditingQuantity(event.target.value)}
                />
              ) : (
                <span>{item.quantity} {item.unit}</span>
              )}
              {editingItemId === item.id ? (
                <div className="row-actions compact-actions">
                  <button type="button" className="confirm-button" onClick={() => saveQuantity(item)}>
                    <Check size={16} />
                  </button>
                  <button type="button" className="cancel-button" onClick={cancelEditing}>
                    <X size={16} />
                  </button>
                </div>
              ) : (
                <button type="button" className="icon-button" onClick={() => startEditing(item)}>
                  <Edit3 size={16} />
                </button>
              )}
              <button type="button" className="icon-button danger" onClick={() => removeItem(item)}>
                <Trash2 size={16} />
              </button>
            </div>
          ))}
        </div>
      </Panel>
    </section>
  );
}
