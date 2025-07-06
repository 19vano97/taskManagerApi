import React from "react";
import { Navigate, useLocation } from "react-router-dom";
import { useSafeAuth } from "../hooks/useSafeAuth";
import { LoaderMain } from "../components/LoaderMain";

const PrivateRoute: React.FC<React.PropsWithChildren> = ({ children }) => {
  const auth = useSafeAuth();
  const location = useLocation();

  if (auth.isLoading) return <LoaderMain />;

  if (!auth.isAuthenticated) {
    // Save the current path to sessionStorage before redirecting
    sessionStorage.setItem("postLoginRedirectPath", location.pathname + location.search);

    auth.signinRedirect(); 

    return <LoaderMain />;
  }

  return children;
};

export default PrivateRoute;