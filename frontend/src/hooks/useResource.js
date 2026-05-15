import { useCallback, useEffect, useState } from 'react';

export function useResource(loader, dependencies = []) {
  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  const reload = useCallback(async () => {
    setLoading(true);
    setError('');
    try {
      setData(await loader());
    } catch (caught) {
      setError(caught.message);
    } finally {
      setLoading(false);
    }
  }, dependencies);

  useEffect(() => {
    reload();
  }, [reload]);

  return { data, setData, loading, error, reload };
}
