import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import App from './App.tsx'
import { oidcConfig } from './auth/authConfig.ts'
import { AuthProvider, useAuth } from 'react-oidc-context'

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

const events = {
  onAccessTokenExpiring: () => {
    console.log("🔄 access token is about to expire, attempting silent renew");
  },
  onAccessTokenExpired: () => {
    console.log("❗ access token expired; doing a silent renew instead of logout");
    const auth = useAuth();
    auth.signinSilent().catch(err => {
      console.error("Silent renew failed", err);
      // optional: show a banner, but DON'T call signoutRedirect()
    });
  },
  onSilentRenewError: (err: Error) => {
    console.error("🔴 silent renew error", err);
    // swallow it—so the user stays on the page
  },
};

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <MantineThemeWrapper>
      <ErrorBoundary FallbackComponent={ErrorFallback}>
        <BrowserRouter>
          <AuthProvider {...oidcConfig} {...events}>
            <App />
          </AuthProvider>
        </BrowserRouter>
      </ErrorBoundary>
    </MantineThemeWrapper>
  </StrictMode>,
)
