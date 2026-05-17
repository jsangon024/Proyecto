import { X } from 'lucide-react';

export function Modal({ title, children, onClose }) {
  return (
    <div className="modal-backdrop" role="presentation" onClick={onClose}>
      <section className="modal-panel" role="dialog" aria-modal="true" aria-label={title} onClick={(event) => event.stopPropagation()}>
        <header className="modal-header">
          <h2>{title}</h2>
          <button type="button" className="icon-button" aria-label="Cerrar" onClick={onClose}>
            <X size={18} />
          </button>
        </header>
        <div className="modal-body">
          {children}
        </div>
      </section>
    </div>
  );
}
