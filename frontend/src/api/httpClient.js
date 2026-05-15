import { env, requireEnv } from '../config/env.js';

const API_BASE_URL = requireEnv(env.apiBaseUrl, 'VITE_API_BASE_URL');

export async function apiRequest(path, options = {}) {
  const headers = {
    'Content-Type': 'application/json',
    ...options.headers,
  };

  const response = await fetch(`${API_BASE_URL}${path}`, {
    ...options,
    headers,
  });

  if (!response.ok) {
    const error = await response.json().catch(() => ({ message: `HTTP ${response.status}` }));
    throw new Error(error.message ?? `HTTP ${response.status}`);
  }

  if (response.status === 204) {
    return null;
  }

  return response.json();
}

export function authHeaders(token) {
  return token ? { Authorization: `Bearer ${token}` } : {};
}
