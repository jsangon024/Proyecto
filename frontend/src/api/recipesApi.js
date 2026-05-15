import { apiRequest, authHeaders } from './httpClient.js';

export const recipesApi = {
  list: () => apiRequest('/api/recipes'),
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
};
