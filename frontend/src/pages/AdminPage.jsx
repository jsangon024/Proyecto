import { ShieldCheck, Trash2 } from 'lucide-react';
import { adminApi } from '../api/adminApi.js';
import { Panel } from '../components/ui/Panel.jsx';
import { useAuth } from '../hooks/useAuth.jsx';
import { useResource } from '../hooks/useResource.js';
import { useToast } from '../hooks/useToast.jsx';

export function AdminPage() {
  const { token, user } = useAuth();
  const { showToast } = useToast();
  const users = useResource(() => adminApi.users(token), [token]);

  async function changeRole(targetUser, role) {
    await adminApi.updateRole(token, targetUser.id, role);
    showToast('Rol actualizado');
    users.reload();
  }

  async function deleteUser(targetUser) {
    const confirmed = window.confirm(`Eliminar la cuenta de ${targetUser.email}?`);
    if (!confirmed) {
      return;
    }

    await adminApi.removeUser(token, targetUser.id);
    showToast('Usuario eliminado');
    users.reload();
  }

  return (
    <Panel title="Administracion de usuarios" icon={<ShieldCheck size={18} />} className="wide">
      {users.error && <div className="notice">{users.error}</div>}
      <div className="data-list">
        {users.data.map((targetUser) => (
          <div className="data-row" key={targetUser.id}>
            <div>
              <strong>{targetUser.email}</strong>
              <span>{targetUser.id}</span>
            </div>
            <span>{targetUser.role}</span>
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
