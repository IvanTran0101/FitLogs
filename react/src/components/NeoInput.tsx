import type { InputHTMLAttributes } from 'react'

type NeoInputProps = {
  label: string
  error?: string
} & InputHTMLAttributes<HTMLInputElement>

export function NeoInput({ label, error, id, className = '', ...props }: NeoInputProps) {
  const inputId = id ?? label.toLowerCase().replaceAll(' ', '-')

  return (
    <label className={`neo-field ${className}`} htmlFor={inputId}>
      <span className="neo-field-label">{label}</span>

      <input
        id={inputId}
        className={error ? 'neo-input has-error' : 'neo-input'}
        aria-invalid={error ? true : undefined}
        aria-describedby={error ? `${inputId}-error` : undefined}
        {...props}
      />

      {error ? (
        <span className="neo-field-error" id={`${inputId}-error`}>
          {error}
        </span>
      ) : null}
    </label>
  )
}