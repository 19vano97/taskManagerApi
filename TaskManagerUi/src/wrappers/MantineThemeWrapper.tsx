// src/MantineThemeWrapper.tsx
import { MantineProvider, createTheme } from '@mantine/core';
import { useColorScheme } from '@mantine/hooks';
import { useState } from 'react';

export const MantineThemeWrapper = ({ children }: { children: React.ReactNode }) => {
  const preferredColorScheme = useColorScheme();
  const [colorScheme, setColorScheme] = useState<'light' | 'dark'>(preferredColorScheme || 'light');

  const toggleColorScheme = () =>
    setColorScheme((prev) => (prev === 'dark' ? 'light' : 'dark'));

  const theme = createTheme({
    primaryColor: 'blue',
  });

  return (
    <MantineProvider
      theme={theme}
      defaultColorScheme={colorScheme}
    >
      {children}
    </MantineProvider>
  );
};
