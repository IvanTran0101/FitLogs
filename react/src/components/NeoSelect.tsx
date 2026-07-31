import type { SelectHTMLAttributes } from 'react'

type NeoSelectOption = {
  label: string
  value: string
}

type NeoSelectProps = {
  label: string
  options: NeoSelectOption[]
  error?: string
} & SelectHTMLAttributes<HTMLSelectElement>

export function NeoSelect({
  label,
  options,
  error,
  id,
  className = '',
  ...props
}: NeoSelectProps) {
  const selectId = id ?? label.toLowerCase().replaceAll(' ', '-')

  return (
    <label className={`neo-field ${className}`} htmlFor={selectId}>
      <span className="neo-field-label">{label}</span>

      <select
        id={selectId}
        className={error ? 'neo-input has-error' : 'neo-input'}
        aria-invalid={error ? true : undefined}
        aria-describedby={error ? `${selectId}-error` : undefined}
        {...props}
      >
        {options.map((option) => (
          <option key={option.value} value={option.value}>
            {option.label}
          </option>
        ))}
      </select>

      {error ? (
        <span className="neo-field-error" id={`${selectId}-error`}>
          {error}
        </span>
      ) : null}
    </label>
  )
}