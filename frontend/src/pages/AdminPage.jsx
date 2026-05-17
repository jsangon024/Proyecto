import { Check, KeyRound, ShieldCheck, Trash2, X } from 'lucide-react';
import { useMemo, useState } from 'react';
import { adminApi } from '../api/adminApi.js';
import { Panel } from '../components/ui/Panel.jsx';
import { SearchBox } from '../components/ui/SearchBox.jsx';
import { useAuth } from '../hooks/useAuth.jsx';
import { useResource } from '../hooks/useResource.js';
import { useToast } from '../hooks/useToast.jsx';
import { confirmDelete } from '../utils/confirmDelete.js';
import { filterByText } from '../utils/search.js';

export function AdminPage() {
  const { token, user } = useAuth();
  const { showToast } = useToast();
  const users = useResource(() => adminApi.users(token), [token]);
  const [userSearch, setUserSearch] = useState('');
  const [passwordEditor, setPasswordEditor] = useState({ userId: '', newPassword: '', confirmPassword: '' });
  const [passwordError, setPasswordError] = useState('');
  const filteredUsers = useMemo(
    () => filterByText(users.data, userSearch, (targetUser) => `${targetUser.email} ${targetUser.role}`),
    [users.data, userSearch],
  );

  async function changeRole(targetUser, role) {
    await adminApi.updateRole(token, targetUser.id, role);
    showToast('Rol actualizado');
    users.reload();
  }

  async function deleteUser(targetUser) {
    if (!confirmDelete(`la cuenta de "${targetUser.email}"`)) {
      return;
    }

    await adminApi.removeUser(token, targetUser.id);
    showToast('Usuario eliminado');
    users.reload();
  }

  function startPasswordEdit(targetUser) {
    setPasswordError('');
    setPasswordEditor({ userId: targetUser.id, newPassword: '', confirmPassword: '' });
  }

  function cancelPasswordEdit() {
    setPasswordError('');
    setPasswordEditor({ userId: '', newPassword: '', confirmPassword: '' });
  }

  async function savePassword(targetUser) {
    setPasswordError('');
    try {
      await adminApi.changePassword(token, targetUser.id, passwordEditor.newPassword, passwordEditor.confirmPassword);
      showToast('Password actualizada');
      cancelPasswordEdit();
    } catch (caught) {
      setPasswordError(caught.message);
    }
  }

  return (
    <Panel title="Administracion de usuarios" icon={<ShieldCheck size={18} />} className="wide">
      {users.error && <div className="notice">{users.error}</div>}
      {passwordError && <div className="notice">{passwordError}</div>}
      <SearchBox value={userSearch} onChange={setUserSearch} placeholder="Buscar por email o rol..." />
      <div className="data-list">
        {filteredUsers.map((targetUser) => (
          <div className="data-row" key={targetUser.id}>
            <div>
              <strong>{targetUser.email}</strong>
              <span>{targetUser.id}</span>
            </div>
            <span>{targetUser.role}</span>
            {passwordEditor.userId === targetUser.id ? (
              <div className="password-editor">
                <input
                  type="password"
                  placeholder="Nueva password"
                  value={passwordEditor.newPassword}
                  onChange={(event) => setPasswordEditor({ ...passwordEditor, newPassword: event.target.value })}
                />
                <input
                  type="password"
                  placeholder="Confirmar"
                  value={passwordEditor.confirmPassword}
                  onChange={(event) => setPasswordEditor({ ...passwordEditor, confirmPassword: event.target.value })}
                />
                <button type="button" className="confirm-button" onClick={() => savePassword(targetUser)}>
                  <Check size={16} />
                </button>
                <button type="button" className="cancel-button" onClick={cancelPasswordEdit}>
                  <X size={16} />
                </button>
              </div>
            ) : (
              <button
                type="button"
                className="icon-button"
                disabled={targetUser.role === 'admin'}
                aria-label="Cambiar password"
                onClick={() => startPasswordEdit(targetUser)}
              >
                <KeyRound size={16} />
              </button>
            )}
            <div className="row-actions">
              <button type="button" disabled={targetUser.role === 'admin'} onClick={() => changeRole(targetUser, 'admin')}>
                Hacer admin
              </button>
              <button
                type="button"
                disabled={targetUser.role === 'user' || targetUser.id === user.id}
                onClick={() => changeRole(targetUser, 'user')}
              >
                Hacer user
              </button>
              <button type="button" className="danger-button compact" disabled={targetUser.id === user.id} onClick={() => deleteUser(targetUser)}>
                <Trash2 size={16} />
                Eliminar
              </button>
            </div>
          </div>
        ))}
      </div>
    </Panel>
  );
}
