import { useState } from 'react';
import { TextInput, Container } from '@mantine/core';
import type { TextInputProps } from '@mantine/core';

/**
 * Type alias for .NET TimeOnly string: "HH:mm:ss" or "HH:mm"
 */
export type TimeOnly = string;

/**
 * Normalize and validate HH:mm input into HH:mm:ss format
 */
export function normalizeTimeOnlyInput(value: string): TimeOnly | null {
  const match = /^([01]?\d|2[0-3]):([0-5]\d)$/.exec(value.trim());
  if (!match) return null;

  const [, hour, minute] = match;
  return `${hour.padStart(2, '0')}:${minute.padStart(2, '0')}:00`;
}

type TimeOnlyInputProps = Omit<TextInputProps, 'value' | 'onChange'> & {
  value: TimeOnly | '';
  onChange: (value: TimeOnly | '') => void;
};

/**
 * Input for entering time in HH:mm, returned as normalized TimeOnly (HH:mm:ss)
 */
export function TimeOnlyInput({
  value,
  onChange,
  label = 'Estimated Time (HH:mm)',
  ...props
}: TimeOnlyInputProps) {
  const [input, setInput] = useState<string>(value ?? '');
  const [error, setError] = useState<string | null>(null);

  const handleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const raw = event.currentTarget.value;
    setInput(raw);

    const normalized = normalizeTimeOnlyInput(raw);
    if (normalized) {
      setError(null);
      onChange(normalized);
    } else {
      setError('Time must be in HH:mm format');
      onChange('');
    }
  };

  return (
    <Container fluid>
      <TextInput
      label={label}
      placeholder="e.g. 01:30"
      value={input}
      onChange={handleChange}
      error={error}
      {...props}
    />
    </Container>
  );
}
