import { apiRequest, authHeaders } from './httpClient.js';

export const inventoryApi = {
  list: (token) =>
    apiRequest('/api/inventory', {
      headers: authHeaders(token),
    }),
  create: (token, item) =>
    apiRequest('/api/inventory', {
      method: 'POST',
      headers: authHeaders(token),
      body: JSON.stringify(item),
    }),
  purchaseList: (token, items) =>
    apiRequest('/api/inventory/purchase-list', {
      method: 'POST',
      headers: authHeaders(token),
      body: JSON.stringify({ items }),
    }),
  update: (token, id, item) =>
    apiRequest(`/api/inventory/${id}`, {
      method: 'PUT',
      headers: authHeaders(token),
      body: JSON.stringify(item),
    }),
  remove: (token, id) =>
    apiRequest(`/api/inventory/${id}`, {
      method: 'DELETE',
      headers: authHeaders(token),
    }),
};
