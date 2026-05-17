import { useState } from 'react';
import { ChefHat, KeyRound, LogIn, UserPlus } from 'lucide-react';
import { useAuth } from '../hooks/useAuth.jsx';
import { useAsyncAction } from '../hooks/useAsyncAction.js';
import { Field } from '../components/ui/Field.jsx';
import { authApi } from '../api/authApi.js';

export function LoginPage() {
  const { login, register } = useAuth();
  const { loading, error, run } = useAsyncAction();
  const [mode, setMode] = useState('login');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [message, setMessage] = useState('');

  async function submit(event) {
    event.preventDefault();
    setMessage('');
    try {
      const response = await run(() => {
        if (mode === 'login') {
          return login(email, password);
        }

        if (mode === 'forgot') {
          return authApi.forgotPassword(email);
        }

        return register(email, password, confirmPassword);
      });

      if (response?.message) {
        setMessage(response.message);
        if (mode === 'register') {
          setMode('login');
          setPassword('');
          setConfirmPassword('');
        }
      }
    } catch {}
  }

  function changeMode(nextMode) {
    setMode(nextMode);
    setMessage('');
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
          <button type="button" className={mode === 'login' ? 'active' : ''} onClick={() => changeMode('login')}>
            Login
          </button>
          <button type="button" className={mode === 'register' ? 'active' : ''} onClick={() => changeMode('register')}>
            Registro
          </button>
        </div>

        <form className="stack" onSubmit={submit}>
          <Field label="Email">
            <input
              type="email"
              autoComplete="email"
              value={email}
              onChange={(event) => setEmail(event.target.value)}
            />
          </Field>
          {mode !== 'forgot' && (
            <Field label="Password">
              <input
                type="password"
                autoComplete={mode === 'login' ? 'current-password' : 'new-password'}
                value={password}
                onChange={(event) => setPassword(event.target.value)}
              />
            </Field>
          )}
          {mode === 'register' && (
            <Field label="Confirmar password">
              <input
                type="password"
                autoComplete="new-password"
                value={confirmPassword}
                onChange={(event) => setConfirmPassword(event.target.value)}
              />
            </Field>
          )}
          {error && <div className="notice">{error}</div>}
          {message && <div className="notice success-notice">{message}</div>}
          <button className="primary" disabled={loading}>
            {mode === 'login' && <LogIn size={18} />}
            {mode === 'register' && <UserPlus size={18} />}
            {mode === 'forgot' && <KeyRound size={18} />}
            {mode === 'login' && 'Entrar'}
            {mode === 'register' && 'Crear cuenta'}
            {mode === 'forgot' && 'Enviar enlace'}
          </button>
          {mode === 'login' && (
            <button type="button" className="text-button" onClick={() => changeMode('forgot')}>
              ¿Has olvidado tu contraseña?
            </button>
          )}
          {mode === 'forgot' && (
            <button type="button" className="text-button" onClick={() => changeMode('login')}>
              Volver al login
            </button>
          )}
        </form>
      </section>
    </main>
  );
}
