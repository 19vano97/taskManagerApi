// src/PrivateRoute.tsx
import { useEffect, type ReactNode } from "react";
import { useAuth } from "react-oidc-context";
import { Navigate, useLocation } from "react-router-dom";
import { LoaderMain } from "../components/LoaderMain";

export function PrivateRoute({ children }: { children: ReactNode }) {
  const auth = useAuth();
  const location = useLocation();

  if (auth.isLoading) return <LoaderMain />;

  if (!auth.isAuthenticated) {
    auth.signinRedirect({
      state: { returnTo: location.pathname + location.search }
    });
    return null;
  }

  return <>{children}</>;
}