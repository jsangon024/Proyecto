import { CalendarDays, Carrot, Soup, Warehouse } from 'lucide-react';
import { ingredientsApi } from '../api/ingredientsApi.js';
import { inventoryApi } from '../api/inventoryApi.js';
import { recipesApi } from '../api/recipesApi.js';
import { useAuth } from '../hooks/useAuth.jsx';
import { useResource } from '../hooks/useResource.js';
import { Panel } from '../components/ui/Panel.jsx';

export function DashboardPage({ onNavigate }) {
  const { token } = useAuth();
  const ingredients = useResource(() => ingredientsApi.list(), []);
  const recipes = useResource(() => recipesApi.list(), []);
  const inventory = useResource(() => inventoryApi.list(token), [token]);

  return (
    <section className="dashboard-grid">
      <MetricCard icon={<Carrot />} label="Ingredientes" value={ingredients.data.length} onClick={() => onNavigate('ingredients')} />
      <MetricCard icon={<Soup />} label="Recetas" value={recipes.data.length} onClick={() => onNavigate('recipes')} />
      <MetricCard icon={<Warehouse />} label="Inventario" value={inventory.data.length} onClick={() => onNavigate('inventory')} />
      <MetricCard icon={<CalendarDays />} label="Planificador" value="Mes" onClick={() => onNavigate('planner')} />

      <Panel title="Estado del sistema" className="wide">
        <div className="status-list">
          <span>PostgreSQL como fuente de datos</span>
          <span>Ingredientes y recetas globales</span>
          <span>Recetas guardadas por usuario</span>
          <span>Roles de usuario y administrador</span>
        </div>
      </Panel>
    </section>
  );
}

function MetricCard({ icon, label, value, onClick }) {
  return (
    <button type="button" className="metric-card" onClick={onClick}>
      {icon}
      <span>{label}</span>
      <strong>{value}</strong>
    </button>
  );
}
