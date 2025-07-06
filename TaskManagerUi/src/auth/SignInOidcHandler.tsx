import { useEffect } from "react";
import { useAuth } from "react-oidc-context";
import { useNavigate } from "react-router-dom";

function SignInOidcHandler() {
  const auth = useAuth();
  const nav = useNavigate();

  useEffect(() => {
    const storedRedirectPath = sessionStorage.getItem("postLoginRedirectPath");

    if (auth.isAuthenticated) {
      nav(storedRedirectPath || "/");
      sessionStorage.removeItem("postLoginRedirectPath");
    }
  }, [auth.isAuthenticated]);

  return <p>Processing login...</p>;
}

export function initiateLogin(redirectPath: string) {
  const auth = useAuth();
  sessionStorage.setItem("postLoginRedirectPath", redirectPath);
  auth.signinRedirect();
}

export default SignInOidcHandler;