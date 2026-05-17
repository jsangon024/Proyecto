import { useState } from 'react';
import { ChefHat, KeyRound } from 'lucide-react';
import { authApi } from '../api/authApi.js';
import { Field } from '../components/ui/Field.jsx';
import { useAsyncAction } from '../hooks/useAsyncAction.js';

export function ResetPasswordPage({ token }) {
  const { loading, error, run } = useAsyncAction();
  const [newPassword, setNewPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [message, setMessage] = useState('');

  async function submit(event) {
    event.preventDefault();
    setMessage('');
    try {
      const response = await run(() => authApi.resetPassword(token, newPassword, confirmPassword));
      setMessage(response.message);
      setNewPassword('');
      setConfirmPassword('');
    } catch {}
  }

  return (
    <main className="login-screen">
      <section className="login-panel">
        <div className="brand login-brand">
          <ChefHat aria-hidden="true" />
          <div>
            <strong>Meal Planner</strong>
            <span>Recuperacion de password</span>
          </div>
        </div>
        <form className="stack" onSubmit={submit}>
          <Field label="Nueva password">
            <input type="password" value={newPassword} onChange={(event) => setNewPassword(event.target.value)} />
          </Field>
          <Field label="Confirmar password">
            <input type="password" value={confirmPassword} onChange={(event) => setConfirmPassword(event.target.value)} />
          </Field>
          {error && <div className="notice">{error}</div>}
          {message && <div className="notice success-notice">{message}</div>}
          <button className="primary" disabled={loading}>
            <KeyRound size={18} />
            Cambiar password
          </button>
          <button type="button" className="text-button" onClick={() => { window.location.hash = '#/login'; }}>
            Volver al login
          </button>
        </form>
      </section>
    </main>
  );
}
