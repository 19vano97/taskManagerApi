import React from "react";
import { Navigate } from "react-router-dom";
import { useAuth } from "react-oidc-context";

const PrivateRoute: React.FC<React.PropsWithChildren> = ({ children }) => {
  const auth = useAuth();

  if (auth.isLoading) return <p>Loading...</p>;

  return auth.isAuthenticated ? children : <Navigate to="/" />;
};

export default PrivateRoute;