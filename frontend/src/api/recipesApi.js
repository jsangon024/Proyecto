import { apiRequest, authHeaders } from './httpClient.js';

export const recipesApi = {
  list: (token) =>
    apiRequest('/api/recipes', {
      headers: authHeaders(token),
    }),
  get: (token, id) =>
    apiRequest(`/api/recipes/${id}`, {
      headers: authHeaders(token),
    }),
  saved: (token) =>
    apiRequest('/api/recipes/saved', {
      headers: authHeaders(token),
    }),
  save: (token, id) =>
    apiRequest(`/api/recipes/${id}/save`, {
      method: 'POST',
      headers: authHeaders(token),
    }),
  unsave: (token, id) =>
    apiRequest(`/api/recipes/${id}/save`, {
      method: 'DELETE',
      headers: authHeaders(token),
    }),
  create: (token, recipe) =>
    apiRequest('/api/recipes', {
      method: 'POST',
      headers: authHeaders(token),
      body: JSON.stringify(recipe),
    }),
  update: (token, id, recipe) =>
    apiRequest(`/api/recipes/${id}`, {
      method: 'PUT',
      headers: authHeaders(token),
      body: JSON.stringify(recipe),
    }),
  remove: (token, id) =>
    apiRequest(`/api/recipes/${id}`, {
      method: 'DELETE',
      headers: authHeaders(token),
    }),
};
