import { apiRequest, authHeaders } from './httpClient.js';

export const ingredientsApi = {
  list: () => apiRequest('/api/ingredients'),
  create: (token, ingredient) =>
    apiRequest('/api/ingredients', {
      method: 'POST',
      headers: authHeaders(token),
      body: JSON.stringify(ingredient),
    }),
  update: (token, id, ingredient) =>
    apiRequest(`/api/ingredients/${id}`, {
      method: 'PUT',
      headers: authHeaders(token),
      body: JSON.stringify(ingredient),
    }),
  remove: (token, id) =>
    apiRequest(`/api/ingredients/${id}`, {
      method: 'DELETE',
      headers: authHeaders(token),
    }),
};
