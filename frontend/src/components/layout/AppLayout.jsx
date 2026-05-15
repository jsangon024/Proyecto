import {
  CalendarDays,
  Carrot,
  ChefHat,
  ClipboardList,
  LayoutDashboard,
  LogOut,
  Settings,
  Shield,
  Soup,
  Warehouse,
} from 'lucide-react';
import { useAuth } from '../../hooks/useAuth.jsx';

const pageMeta = {
  dashboard: { label: 'Panel', icon: LayoutDashboard },
  ingredients: { label: 'Ingredientes', icon: Carrot },
  recipes: { label: 'Recetas', icon: Soup },
  inventory: { label: 'Inventario', icon: Warehouse },
  planner: { label: 'Plan mensual', icon: CalendarDays },
  settings: { label: 'Ajustes', icon: Settings },
  admin: { label: 'Admin', icon: Shield },
};

export function AppLayout({ activePage, onNavigate, pages, children }) {
  const { user, logout } = useAuth();

  return (
    <main className="app-shell">
      <aside className="sidebar">
        <div className="brand">
          <ChefHat aria-hidden="true" />
          <div>
            <strong>Meal Planner</strong>
            <span>API PostgreSQL</span>
          </div>
        </div>

        <nav aria-label="Navegacion principal">
          {Object.keys(pages).map((pageKey) => {
            const Icon = pageMeta[pageKey].icon;
            return (
              <button
                key={pageKey}
                type="button"
                className={activePage === pageKey ? 'active' : ''}
                onClick={() => onNavigate(pageKey)}
              >
                <Icon size={18} aria-hidden="true" />
                {pageMeta[pageKey].label}
              </button>
            );
          })}
        </nav>
      </aside>

      <section className="workspace">
        <header className="topbar">
          <div>
            <p>Proyecto integrado</p>
            <h1>{pageMeta[activePage]?.label ?? 'Meal Planner'}</h1>
          </div>
          <div className="user-area">
            <div className="session-pill">
              <ClipboardList size={18} aria-hidden="true" />
              {user.email} · {user.role}
            </div>
            <button type="button" className="icon-button" aria-label="Cerrar sesion" onClick={logout}>
              <LogOut size={18} />
            </button>
          </div>
        </header>
        {children}
      </section>
    </main>
  );
}
