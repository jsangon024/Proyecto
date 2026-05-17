import { apiRequest, authHeaders } from './httpClient.js';

export const plansApi = {
  list: (token) =>
    apiRequest('/api/plans', {
      headers: authHeaders(token),
    }),
  get: (token, id) =>
    apiRequest(`/api/plans/${id}`, {
      headers: authHeaders(token),
    }),
  generate: (token, year, month) =>
    apiRequest('/api/plans/generate', {
      method: 'POST',
      headers: authHeaders(token),
      body: JSON.stringify({ year, month }),
    }),
  latest: (token) =>
    apiRequest('/api/plans/latest', {
      headers: authHeaders(token),
    }),
  remove: (token, id) =>
    apiRequest(`/api/plans/${id}`, {
      method: 'DELETE',
      headers: authHeaders(token),
    }),
  completeDay: (token, planId, date) =>
    apiRequest(`/api/plans/${planId}/days/${date}/complete`, {
      method: 'POST',
      headers: authHeaders(token),
    }),
  shoppingList: (token, planIds, selectedPlanId, dates = [], onlyMissing = true) =>
    apiRequest('/api/shopping-list/from-plan', {
      method: 'POST',
      headers: authHeaders(token),
      body: JSON.stringify({ planId: selectedPlanId, planIds, dates, onlyMissing }),
    }),
};
