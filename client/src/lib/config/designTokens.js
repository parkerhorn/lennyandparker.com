// Design tokens for consistent styling across the wedding website

export const colors = {
  // Wedding color palette
  primary: {
    50: '#faf5ff',
    100: '#f3e8ff', 
    200: '#e9d5ff',
    300: '#d8b4fe',
    400: '#c084fc',
    500: '#a855f7',
    600: '#9333ea',
    700: '#7c3aed',
    800: '#6b21a8',
    900: '#581c87'
  },
  secondary: {
    50: '#f0fdf4',
    100: '#dcfce7',
    200: '#bbf7d0', 
    300: '#86efac',
    400: '#4ade80',
    500: '#22c55e',
    600: '#16a34a',
    700: '#15803d',
    800: '#166534',
    900: '#14532d'
  },
  accent: {
    cream: '#faf8f5',
    brown: '#6d4a3f'
  }
};

export const typography = {
  fonts: {
    script: ['Homemade Apple', 'cursive'],
    serif: ['Special Elite', 'serif'], 
    sans: ['Courier Prime', 'monospace']
  },
  sizes: {
    xs: '0.75rem',    // 12px
    sm: '0.875rem',   // 14px
    base: '1rem',     // 16px
    lg: '1.125rem',   // 18px
    xl: '1.25rem',    // 20px
    '2xl': '1.5rem',  // 24px
    '3xl': '1.875rem', // 30px
    '4xl': '2.25rem',  // 36px
    '5xl': '3rem',     // 48px
    '6xl': '3.75rem'   // 60px
  }
};

export const spacing = {
  section: '5rem',     // 80px - standard section padding
  container: '4rem',   // 64px - container padding
  card: '1.5rem',      // 24px - card padding
  button: '0.75rem',   // 12px - button padding
  gap: {
    sm: '0.5rem',      // 8px
    md: '1rem',        // 16px  
    lg: '1.5rem',      // 24px
    xl: '2rem'         // 32px
  }
};

export const shadows = {
  wedding: '4px 4px 0',
  weddingHover: '2px 2px 0'
};

export const transitions = {
  fast: '0.2s ease',
  normal: '0.3s ease',
  slow: '0.5s ease'
};