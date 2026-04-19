import { NumberField } from '@base-ui/react';
import { FormControl, FormLabel, InputBase, IconButton } from '@mui/material';
import { Add, Remove } from '@mui/icons-material';

interface NumberFieldMUIProps {
  label: string;
  value: number;
  onChange: (value: number) => void;
  min?: number;
  max?: number;
  step?: number;
  sx?: object;
}

export function NumberFieldMUI({ label, value, onChange, min, max, step, sx }: NumberFieldMUIProps) {
  return (
    <NumberField.Root
      value={value}
      onValueChange={(v) => { if (v !== null && !isNaN(v)) onChange(v); }}
      min={min}
      max={max}
      step={step}
    >
      <FormControl fullWidth size="small" sx={sx}>
        <FormLabel sx={{ fontSize: '0.75rem', mb: 0.25, color: 'text.secondary' }}>{label}</FormLabel>
        <div style={{ display: 'flex', alignItems: 'center' }}>
          <NumberField.Decrement>
            <IconButton size="small" sx={{ borderRadius: 1, p: 0.25, minWidth: 28, minHeight: 28 }}>
              <Remove fontSize="small" />
            </IconButton>
          </NumberField.Decrement>
          <NumberField.Input
            render={(props) => {
              const { color, ...inputProps } = props as any;
              return (
                <InputBase
                  {...inputProps}
                  sx={{
                    textAlign: 'center',
                    px: 0.5,
                    py: 0.25,
                    fontSize: '0.875rem',
                    minWidth: 40,
                    height: 28,
                    borderRadius: 1,
                    border: '1px solid',
                    borderColor: 'rgba(0,0,0,0.23)',
                    '&:focus-within': { borderColor: 'primary.main' },
                  }}
                />
              );
            }}
          />
          <NumberField.Increment>
            <IconButton size="small" sx={{ borderRadius: 1, p: 0.25, minWidth: 28, minHeight: 28 }}>
              <Add fontSize="small" />
            </IconButton>
          </NumberField.Increment>
        </div>
      </FormControl>
    </NumberField.Root>
  );
}