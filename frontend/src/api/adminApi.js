import { apiRequest, authHeaders } from './httpClient.js';

export const adminApi = {
  users: (token) =>
    apiRequest('/api/admin/users', {
      headers: authHeaders(token),
    }),
  updateRole: (token, userId, role) =>
    apiRequest(`/api/admin/users/${userId}/role`, {
      method: 'PUT',
      headers: authHeaders(token),
      body: JSON.stringify({ role }),
    }),
  removeUser: (token, userId) =>
    apiRequest(`/api/admin/users/${userId}`, {
      method: 'DELETE',
      headers: authHeaders(token),
    }),
};
