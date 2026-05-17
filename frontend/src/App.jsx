import { useEffect, useMemo, useState } from 'react';
import { AdminPage } from './pages/AdminPage.jsx';
import { DashboardPage } from './pages/DashboardPage.jsx';
import { IngredientsPage } from './pages/IngredientsPage.jsx';
import { InventoryPage } from './pages/InventoryPage.jsx';
import { LoginPage } from './pages/LoginPage.jsx';
import { PlannerPage } from './pages/PlannerPage.jsx';
import { RecipeDetailPage } from './pages/RecipeDetailPage.jsx';
import { RecipesPage } from './pages/RecipesPage.jsx';
import { ResetPasswordPage } from './pages/ResetPasswordPage.jsx';
import { SettingsPage } from './pages/SettingsPage.jsx';
import { VerifyEmailPage } from './pages/VerifyEmailPage.jsx';
import { AppLayout } from './components/layout/AppLayout.jsx';
import { AuthProvider, useAuth } from './hooks/useAuth.jsx';
import { ToastProvider } from './hooks/useToast.jsx';

const pages = {
  dashboard: DashboardPage,
  ingredients: IngredientsPage,
  recipes: RecipesPage,
  recipeDetail: RecipeDetailPage,
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
  const [route, setRoute] = useState(() => parseHashRoute());

  const availablePages = useMemo(() => {
    return user?.role === 'admin' ? pages : Object.fromEntries(Object.entries(pages).filter(([key]) => key !== 'admin'));
  }, [user]);

  useEffect(() => {
    function syncRoute() {
      setRoute(parseHashRoute());
    }

    window.addEventListener('hashchange', syncRoute);
    return () => window.removeEventListener('hashchange', syncRoute);
  }, []);

  useEffect(() => {
    if (!initializing && user && !window.location.hash) {
      navigateToHash('dashboard');
    }
  }, [initializing, user]);

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
    if (route.page === 'verifyEmail') {
      return <VerifyEmailPage token={route.token} />;
    }

    if (route.page === 'resetPassword') {
      return <ResetPasswordPage token={route.token} />;
    }

    return <LoginPage />;
  }

  const activePage = availablePages[route.page] ? route.page : 'dashboard';
  const Page = availablePages[activePage] ?? DashboardPage;
  function navigate(nextPage) {
    if (typeof nextPage === 'object') {
      navigateToHash(nextPage);
      return;
    }

    navigateToHash(nextPage);
  }

  return (
    <AppLayout activePage={activePage} onNavigate={navigate} pages={availablePages}>
      <Page onNavigate={navigate} recipeId={route.recipeId} />
    </AppLayout>
  );
}

function parseHashRoute() {
  const rawHash = window.location.hash.replace(/^#\/?/, '');
  const [page = 'dashboard', id] = rawHash.split('/');

  if (page === 'recipes' && id) {
    return { page: 'recipeDetail', recipeId: id };
  }

  if (page === 'verify-email') {
    return { page: 'verifyEmail', token: id ?? '', recipeId: null };
  }

  if (page === 'reset-password') {
    return { page: 'resetPassword', token: id ?? '', recipeId: null };
  }

  return { page: page || 'dashboard', recipeId: null, token: null };
}

function navigateToHash(nextPage) {
  const nextHash = typeof nextPage === 'object'
    ? routeToHash(nextPage)
    : `#/${nextPage}`;

  if (window.location.hash === nextHash) {
    window.dispatchEvent(new HashChangeEvent('hashchange'));
    return;
  }

  window.location.hash = nextHash;
}

function routeToHash(nextPage) {
  if (nextPage.page === 'recipeDetail') {
    return `#/recipes/${nextPage.recipeId}`;
  }

  return `#/${nextPage.page}`;
}
