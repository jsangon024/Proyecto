import { apiRequest } from './httpClient.js';

export const authApi = {
  login: (email, password) =>
    apiRequest('/api/auth/login', {
      method: 'POST',
      body: JSON.stringify({ email, password }),
    }),
  register: (email, password, confirmPassword) =>
    apiRequest('/api/auth/register', {
      method: 'POST',
      body: JSON.stringify({ email, password, confirmPassword }),
    }),
  me: (token) =>
    apiRequest('/api/me', {
      headers: { Authorization: `Bearer ${token}` },
    }),
};
