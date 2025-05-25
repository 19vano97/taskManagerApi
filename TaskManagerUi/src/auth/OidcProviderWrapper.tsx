import { AuthProvider } from 'react-oidc-context'
import { oidcConfig } from './authConfig'

export const OidcProviderWrapper = ({ children }: { children: React.ReactNode }) => {
    return (
        <AuthProvider
        {...oidcConfig}
        // onSigninCallback={(user) => {
        //     const returnUrl = '/'
        //     window.history.replaceState({}, document.title, returnUrl)
        // }}
        >
        {children}
        </AuthProvider>
    )
}