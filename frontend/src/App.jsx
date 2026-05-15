import { useMemo, useState } from 'react';
import { AdminPage } from './pages/AdminPage.jsx';
import { DashboardPage } from './pages/DashboardPage.jsx';
import { IngredientsPage } from './pages/IngredientsPage.jsx';
import { InventoryPage } from './pages/InventoryPage.jsx';
import { LoginPage } from './pages/LoginPage.jsx';
import { PlannerPage } from './pages/PlannerPage.jsx';
import { RecipesPage } from './pages/RecipesPage.jsx';
import { SettingsPage } from './pages/SettingsPage.jsx';
import { AppLayout } from './components/layout/AppLayout.jsx';
import { AuthProvider, useAuth } from './hooks/useAuth.jsx';
import { ToastProvider } from './hooks/useToast.jsx';

const pages = {
  dashboard: DashboardPage,
  ingredients: IngredientsPage,
  recipes: RecipesPage,
  inventory: InventoryPage,
  planner: PlannerPage,
  settings: SettingsPage,
  admin: AdminPage,
};

export function App() {
  return (
    <ToastProvider>
      <AuthProvider>
        <AppShell />
      </AuthProvider>
    </ToastProvider>
  );
}

function AppShell() {
  const { user, initializing } = useAuth();
  const [activePage, setActivePage] = useState('dashboard');

  const availablePages = useMemo(() => {
    return user?.role === 'admin' ? pages : Object.fromEntries(Object.entries(pages).filter(([key]) => key !== 'admin'));
  }, [user]);

  if (initializing) {
    return (
      <main className="login-screen">
        <section className="login-panel">
          <strong>Validando sesion...</strong>
        </section>
      </main>
    );
  }

  if (!user) {
    return <LoginPage />;
  }

  const Page = availablePages[activePage] ?? DashboardPage;

  return (
    <AppLayout activePage={activePage} onNavigate={setActivePage} pages={availablePages}>
      <Page onNavigate={setActivePage} />
    </AppLayout>
  );
}
