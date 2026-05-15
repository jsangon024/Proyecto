import { createContext, useContext, useMemo, useState } from 'react';
import { X } from 'lucide-react';

const ToastContext = createContext(null);

export function ToastProvider({ children }) {
  const [message, setMessage] = useState('');

  const value = useMemo(
    () => ({
      showToast: setMessage,
      clearToast: () => setMessage(''),
    }),
    [],
  );

  return (
    <ToastContext.Provider value={value}>
      {children}
      {message && (
        <div className="toast" role="status">
          <span>{message}</span>
          <button type="button" aria-label="Cerrar aviso" onClick={() => setMessage('')}>
            <X size={16} />
          </button>
        </div>
      )}
    </ToastContext.Provider>
  );
}

export function useToast() {
  const context = useContext(ToastContext);
  if (!context) {
    throw new Error('useToast must be used inside ToastProvider');
  }

  return context;
}
