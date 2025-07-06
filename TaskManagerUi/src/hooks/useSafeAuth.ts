import { useAuth, type AuthContextProps } from 'react-oidc-context';

export const useSafeAuth = (): AuthContextProps => {
  const auth = useAuth();

  return (
    auth ?? {
      isAuthenticated: false,
      isLoading: true,
      signinRedirect: () => {},
      signoutRedirect: () => {},
      user: null,
      userManager: undefined,
      events: undefined,
    }
  );
};