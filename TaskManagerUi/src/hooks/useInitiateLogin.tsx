import { useAuth } from "react-oidc-context";
import { useLocation } from "react-router-dom";

export function useInitiateLogin() {
  const auth = useAuth();
  const location = useLocation();

  return () => {
    auth.signinRedirect({ state: { returnTo: location.pathname + location.search } });
  };
}