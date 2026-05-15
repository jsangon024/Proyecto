import { useState } from 'react';

export function useAsyncAction() {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  async function run(action) {
    setLoading(true);
    setError('');
    try {
      return await action();
    } catch (caught) {
      setError(caught.message);
      throw caught;
    } finally {
      setLoading(false);
    }
  }

  return { loading, error, setError, run };
}
