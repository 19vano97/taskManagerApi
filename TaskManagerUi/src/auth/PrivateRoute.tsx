import React from "react";
import { Navigate } from "react-router-dom";
import { LoaderMain } from "../components/LoaderMain";
import { useSafeAuth } from "../hooks/useSafeAuth";

const PrivateRoute: React.FC<React.PropsWithChildren> = ({ children }) => {
  const auth = useSafeAuth();

  if (auth.isLoading) return <LoaderMain />;
  if (!auth.isAuthenticated) {
    return ;
  }

  return auth.isAuthenticated ? children : <Navigate to="/" />;
};

export default PrivateRoute;