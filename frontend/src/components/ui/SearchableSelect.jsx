import { useEffect, useId, useMemo, useState } from 'react';
import { Search } from 'lucide-react';
import { Field } from './Field.jsx';

export function SearchableSelect({
  label,
  value,
  onChange,
  options,
  placeholder = 'Seleccionar',
  searchPlaceholder = 'Buscar y seleccionar...',
}) {
  const datalistId = useId();
  const selectedOption = useMemo(() => options.find((option) => option.value === value), [options, value]);
  const [text, setText] = useState(selectedOption?.label ?? '');

  useEffect(() => {
    setText(selectedOption?.label ?? '');
  }, [selectedOption]);

  function handleChange(nextText) {
    setText(nextText);
    const exactOption = options.find((option) => option.label.toLowerCase() === nextText.trim().toLowerCase());
    onChange(exactOption?.value ?? '');
  }

  return (
    <Field label={label}>
      <div className="searchable-select">
        <Search size={16} />
        <input
          list={datalistId}
          value={text}
          onChange={(event) => handleChange(event.target.value)}
          placeholder={searchPlaceholder || placeholder}
        />
        <datalist id={datalistId}>
          {options.map((option) => (
            <option key={option.value} value={option.label}>
              {option.searchText ?? option.label}
            </option>
          ))}
        </datalist>
      </div>
    </Field>
  );
}
