import { apiRequest } from './httpClient.js';

export const authApi = {
  login: (email, password) =>
    apiRequest('/api/auth/login', {
      method: 'POST',
      body: JSON.stringify({ email, password }),
    }).catch((error) => {
      if (error.message === 'HTTP 401') {
        throw new Error('Email o password incorrectos.');
      }
      throw error;
    }),
  register: (email, password, confirmPassword) =>
    apiRequest('/api/auth/register', {
      method: 'POST',
      body: JSON.stringify({ email, password, confirmPassword }),
    }),
  verifyEmail: (token) =>
    apiRequest('/api/auth/verify-email', {
      method: 'POST',
      body: JSON.stringify({ token }),
    }),
  forgotPassword: (email) =>
    apiRequest('/api/auth/forgot-password', {
      method: 'POST',
      body: JSON.stringify({ email }),
    }),
  resetPassword: (token, newPassword, confirmPassword) =>
    apiRequest('/api/auth/reset-password', {
      method: 'POST',
      body: JSON.stringify({ token, newPassword, confirmPassword }),
    }),
  me: (token) =>
    apiRequest('/api/me', {
      headers: { Authorization: `Bearer ${token}` },
    }),
};
