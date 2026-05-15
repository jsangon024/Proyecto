export const env = {
  apiBaseUrl: import.meta.env.VITE_API_BASE_URL,
};

export function requireEnv(value, name) {
  if (!value) {
    throw new Error(`Missing environment variable: ${name}`);
  }

  return value;
}
