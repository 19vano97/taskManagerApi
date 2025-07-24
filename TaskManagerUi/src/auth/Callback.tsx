import { useEffect } from "react";
import { useAuth } from "react-oidc-context";
import { useNavigate } from "react-router-dom";

export default function Callback() {
  const auth = useAuth();
  const navigate = useNavigate();

  useEffect(() => {
    if (!auth.isLoading && auth.isAuthenticated) {
      const returnTo = (auth.user?.state as any)?.returnTo as string;
      navigate(returnTo ?? "/", { replace: true });
    }
  }, [auth.isLoading, auth.isAuthenticated, auth.user, navigate]);

  return <p>Signing you in…</p>;
}