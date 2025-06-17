import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import App from './App.tsx'
import { oidcConfig } from './auth/authConfig.ts'
import { AuthProvider } from 'react-oidc-context'

import { ErrorBoundary } from 'react-error-boundary';
import { MantineThemeWrapper } from './wrappers/MantineThemeWrapper.tsx'
import { BrowserRouter } from 'react-router-dom'

function ErrorFallback({ error }: { error: any }) {
  return (
    <div>
      <h2>Something went wrong!</h2>
      <pre>{error.message}</pre>
    </div>
  );
}

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <MantineThemeWrapper>
      <ErrorBoundary FallbackComponent={ErrorFallback}>
        <BrowserRouter>
          <AuthProvider {...oidcConfig}>
            <App />
          </AuthProvider>
        </BrowserRouter>
      </ErrorBoundary>
    </MantineThemeWrapper>
  </StrictMode>,
)
