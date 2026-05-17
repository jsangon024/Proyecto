import { KeyRound, Trash2, UserCog } from 'lucide-react';
import { useState } from 'react';
import { userApi } from '../api/userApi.js';
import { Field } from '../components/ui/Field.jsx';
import { Panel } from '../components/ui/Panel.jsx';
import { useAuth } from '../hooks/useAuth.jsx';
import { useAsyncAction } from '../hooks/useAsyncAction.js';
import { useToast } from '../hooks/useToast.jsx';
import { confirmDelete } from '../utils/confirmDelete.js';

export function SettingsPage() {
  const { token, user, logout } = useAuth();
  const { showToast } = useToast();
  const passwordAction = useAsyncAction();
  const deleteAction = useAsyncAction();
  const [passwordForm, setPasswordForm] = useState({
    currentPassword: '',
    newPassword: '',
    confirmPassword: '',
  });
  const [deletePassword, setDeletePassword] = useState('');

  async function changePassword(event) {
    event.preventDefault();
    await passwordAction.run(() => userApi.changePassword(token, passwordForm));
    setPasswordForm({ currentPassword: '', newPassword: '', confirmPassword: '' });
    showToast('Password actualizada');
  }

  async function deleteAccount() {
    if (!confirmDelete('tu cuenta y todos tus datos asociados')) {
      return;
    }

    await deleteAction.run(() => userApi.deleteAccount(token, deletePassword));
    logout();
  }

  return (
    <section className="content-grid">
      <Panel title="Datos de cuenta" icon={<UserCog size={18} />}>
        <div className="status-list">
          <span>Email: {user.email}</span>
          <span>Rol: {user.role}</span>
          <span>Calorias objetivo: {user.goals.dailyCalories}</span>
          <span>Proteina objetivo: {user.goals.minimumProteinGrams} g</span>
        </div>
      </Panel>

      <Panel title="Cambiar password" icon={<KeyRound size={18} />}>
        <form className="stack" onSubmit={changePassword}>
          <Field label="Password actual">
            <input
              type="password"
              value={passwordForm.currentPassword}
              onChange={(event) => setPasswordForm({ ...passwordForm, currentPassword: event.target.value })}
            />
          </Field>
          <Field label="Nueva password">
            <input
              type="password"
              value={passwordForm.newPassword}
              onChange={(event) => setPasswordForm({ ...passwordForm, newPassword: event.target.value })}
            />
          </Field>
          <Field label="Confirmar nueva password">
            <input
              type="password"
              value={passwordForm.confirmPassword}
              onChange={(event) => setPasswordForm({ ...passwordForm, confirmPassword: event.target.value })}
            />
          </Field>
          {passwordAction.error && <div className="notice">{passwordAction.error}</div>}
          <button className="primary" disabled={passwordAction.loading}>Actualizar password</button>
        </form>
      </Panel>

      <Panel title="Eliminar cuenta" icon={<Trash2 size={18} />} className="wide">
        <p className="muted">La eliminacion de cuenta borra tu usuario y los datos asociados por las relaciones de la base de datos.</p>
        <Field label="Password actual">
          <input
            type="password"
            value={deletePassword}
            onChange={(event) => setDeletePassword(event.target.value)}
          />
        </Field>
        {deleteAction.error && <div className="notice">{deleteAction.error}</div>}
        <button type="button" className="danger-button" disabled={deleteAction.loading} onClick={deleteAccount}>
          Eliminar mi cuenta
        </button>
      </Panel>
    </section>
  );
}
