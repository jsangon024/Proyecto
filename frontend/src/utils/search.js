export function filterByText(items, query, selector) {
  const normalizedQuery = query.trim().toLowerCase();
  if (!normalizedQuery) {
    return items;
  }

  return items.filter((item) => selector(item).toLowerCase().includes(normalizedQuery));
}
