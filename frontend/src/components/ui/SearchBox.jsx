import { Search } from 'lucide-react';
import { Field } from './Field.jsx';

export function SearchBox({ label = 'Buscar', value, onChange, placeholder = 'Buscar...' }) {
  return (
    <Field label={label}>
      <div className="search-box">
        <Search size={16} />
        <input value={value} onChange={(event) => onChange(event.target.value)} placeholder={placeholder} />
      </div>
    </Field>
  );
}
