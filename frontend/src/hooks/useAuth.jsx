import { createContext, useContext, useEffect, useMemo, useState } from 'react';
import { authApi } from '../api/authApi.js';

const AuthContext = createContext(null);
const tokenKey = 'mealPlannerToken';
const userKey = 'mealPlannerUser';

export function AuthProvider({ children }) {
  const [token, setToken] = useState(() => localStorage.getItem(tokenKey) ?? '');
  const [user, setUser] = useState(() => {
    const stored = localStorage.getItem(userKey);
    return stored ? JSON.parse(stored) : null;
  });
  const [initializing, setInitializing] = useState(Boolean(token));

  useEffect(() => {
    if (!token) {
      setInitializing(false);
      return;
    }

    let cancelled = false;

    async function validateSession() {
      try {
        const currentUser = await authApi.me(token);
        if (!cancelled) {
          persistSession(token, currentUser);
        }
      } catch {
        if (!cancelled) {
          clearSession();
        }
      } finally {
        if (!cancelled) {
          setInitializing(false);
        }
      }
    }

    validateSession();

    return () => {
      cancelled = true;
    };
  }, []);

  async function login(email, password) {
    const response = await authApi.login(email, password);
    persistSession(response.accessToken, response.user);
    return response.user;
  }

  async function register(email, password, confirmPassword) {
    const response = await authApi.register(email, password, confirmPassword);
    return response;
  }

  function persistSession(nextToken, nextUser) {
    setToken(nextToken);
    setUser(nextUser);
    localStorage.setItem(tokenKey, nextToken);
    localStorage.setItem(userKey, JSON.stringify(nextUser));
  }

  function updateUser(nextUser) {
    setUser(nextUser);
    localStorage.setItem(userKey, JSON.stringify(nextUser));
  }

  function logout() {
    clearSession();
  }

  function clearSession() {
    setToken('');
    setUser(null);
    localStorage.removeItem(tokenKey);
    localStorage.removeItem(userKey);
  }

  const value = useMemo(
    () => ({ token, user, initializing, login, register, logout, updateUser }),
    [token, user, initializing],
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used inside AuthProvider');
  }

  return context;
}
