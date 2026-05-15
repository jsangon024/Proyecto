import { useState } from 'react';
import { ChefHat, LogIn, UserPlus } from 'lucide-react';
import { useAuth } from '../hooks/useAuth.jsx';
import { useAsyncAction } from '../hooks/useAsyncAction.js';
import { Field } from '../components/ui/Field.jsx';

export function LoginPage() {
  const { login, register } = useAuth();
  const { loading, error, run } = useAsyncAction();
  const [mode, setMode] = useState('login');
  const [email, setEmail] = useState('admin@example.com');
  const [password, setPassword] = useState('Admin123!');
  const [confirmPassword, setConfirmPassword] = useState('Admin123!');

  async function submit(event) {
    event.preventDefault();
    await run(() => (mode === 'login' ? login(email, password) : register(email, password, confirmPassword)));
  }

  return (
    <main className="login-screen">
      <section className="login-panel">
        <div className="brand login-brand">
          <ChefHat aria-hidden="true" />
          <div>
            <strong>Meal Planner</strong>
            <span>Gestion nutricional</span>
          </div>
        </div>

        <div className="segmented">
          <button type="button" className={mode === 'login' ? 'active' : ''} onClick={() => setMode('login')}>
            Login
          </button>
          <button type="button" className={mode === 'register' ? 'active' : ''} onClick={() => setMode('register')}>
            Registro
          </button>
        </div>

        <form className="stack" onSubmit={submit}>
          <Field label="Email">
            <input value={email} onChange={(event) => setEmail(event.target.value)} />
          </Field>
          <Field label="Password">
            <input type="password" value={password} onChange={(event) => setPassword(event.target.value)} />
          </Field>
          {mode === 'register' && (
            <Field label="Confirmar password">
              <input type="password" value={confirmPassword} onChange={(event) => setConfirmPassword(event.target.value)} />
            </Field>
          )}
          {error && <div className="notice">{error}</div>}
          <button className="primary" disabled={loading}>
            {mode === 'login' ? <LogIn size={18} /> : <UserPlus size={18} />}
            {mode === 'login' ? 'Entrar' : 'Crear cuenta'}
          </button>
        </form>
      </section>
    </main>
  );
}
