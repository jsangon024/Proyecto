import { useEffect, useState } from 'react';
import { CheckCircle2, ChefHat } from 'lucide-react';
import { authApi } from '../api/authApi.js';

export function VerifyEmailPage({ token }) {
  const [state, setState] = useState({ loading: true, message: '', error: '' });

  useEffect(() => {
    let cancelled = false;

    async function verify() {
      try {
        const response = await authApi.verifyEmail(token);
        if (!cancelled) {
          setState({ loading: false, message: response.message, error: '' });
        }
      } catch (caught) {
        if (!cancelled) {
          setState({ loading: false, message: '', error: caught.message });
        }
      }
    }

    verify();

    return () => {
      cancelled = true;
    };
  }, [token]);

  return (
    <main className="login-screen">
      <section className="login-panel stack">
        <div className="brand login-brand">
          <ChefHat aria-hidden="true" />
          <div>
            <strong>Meal Planner</strong>
            <span>Activacion de cuenta</span>
          </div>
        </div>
        {state.loading && <strong>Activando cuenta...</strong>}
        {state.error && <div className="notice">{state.error}</div>}
        {state.message && (
          <>
            <div className="notice success-notice">
              <CheckCircle2 size={18} />
              {state.message}
            </div>
            <button type="button" className="primary" onClick={() => { window.location.hash = '#/login'; }}>
              Ir al login
            </button>
          </>
        )}
      </section>
    </main>
  );
}
