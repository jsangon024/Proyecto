import { apiRequest, authHeaders } from './httpClient.js';

export const userApi = {
  updateGoals: (token, goals) =>
    apiRequest('/api/me/goals', {
      method: 'PUT',
      headers: authHeaders(token),
      body: JSON.stringify(goals),
    }),
  changePassword: (token, payload) =>
    apiRequest('/api/me/password', {
      method: 'PUT',
      headers: authHeaders(token),
      body: JSON.stringify(payload),
    }),
  deleteAccount: (token, password) =>
    apiRequest('/api/me', {
      method: 'DELETE',
      headers: authHeaders(token),
      body: JSON.stringify({ password }),
    }),
};
