import { useAuth } from 'react-oidc-context';

export const useSafeAuth = () => {
  const auth = useAuth();
  if (!auth) {
    return {
      isAuthenticated: false,
      isLoading: true,
      signinRedirect: () => {},
      signoutRedirect: () => {},
      user: null,
    };
  }
  return auth;
};
